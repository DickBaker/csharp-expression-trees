using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using AgileObjects.ReadableExpressions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TitanicExplorer.Data;
using static TitanicExplorer.Data.Passenger;

namespace TitanicExplorer.Pages;

public class IndexModel : PageModel
{
    public IndexModel(ILogger<IndexModel> logger)
    {
        Logger = logger;

        var sampleDataPath = Path.GetTempFileName();

        System.IO.File.WriteAllText(sampleDataPath, DataFiles.passengers);

        Passengers = Passenger.LoadFromFile(sampleDataPath);
    }
    public ILogger<IndexModel> Logger { get; }

    public IEnumerable<Passenger> Passengers
    {
        get; private set;
    }

    public void OnGet()
    { }

    public string? Query { get; set; }

    public void OnPost()
    {
        var survived = Request.Form["survived"] != "" ? ParseSurvived(Request.Form["survived"]!) : null;
        var pClass = ParseNullInt(Request.Form["pClass"]!);
        SexValue? sex = Request.Form["sex"] != "" ? ParseSex(Request.Form["sex"]!) : null;
        var age = ParseNullDecimal(Request.Form["age"]!);
        var minimumFare = ParseNullDecimal(Request.Form["minimumFare"]!);
        Query = Request.Form["query"]!;

        Passengers = FilterPassengers(survived, pClass, sex, age, minimumFare);
    }

    IEnumerable<Passenger> FilterPassengers(bool? survived, int? pClass, SexValue? sex, decimal? age, decimal? minimumFare)
    {
        Expression? currentExpression = null;

        if (!string.IsNullOrEmpty(Query))
        {
            Expression<Func<Passenger, bool>> expr = DynamicExpressionParser.ParseLambda<Passenger, bool>(new ParsingConfig(), true, Query);

            Func<Passenger, bool> func = expr.Compile();

            return Passengers.Where(func);
        }

        ParameterExpression passengerParameter = Expression.Parameter(typeof(Passenger));

        if (survived != null)
        {
            currentExpression = CreateExpression<bool>(survived.Value, null, "Survived", passengerParameter);
        }

        if (pClass != null)
        {
            currentExpression = CreateExpression<int>(pClass.Value, currentExpression, "PClass", passengerParameter);
        }

        if (sex != null)
        {
            currentExpression = CreateExpression<SexValue>(sex.Value, currentExpression, "Sex", passengerParameter);
        }

        if (age != null)
        {
            currentExpression = CreateExpression<decimal>(age.Value, currentExpression, "Age", passengerParameter);
        }

        if (minimumFare != null)
        {
            currentExpression = CreateExpression<decimal>(minimumFare.Value, currentExpression, "Fare", passengerParameter, ">");
        }

        if (currentExpression != null)
        {
            var expr = Expression.Lambda<Func<Passenger, bool>>(currentExpression, false, new List<ParameterExpression> { passengerParameter });
            Func<Passenger, bool> func = expr.Compile();

            Query = expr.ToReadableString();

            Passengers = Passengers.Where(func);
        }

        return Passengers;
    }

    /// <summary>
    /// Aggregates an expression with a property and an operator
    /// </summary>
    /// <typeparam name="T">The type of the parameter</typeparam>
    /// <param name="value">The constant value to use in the expression</param>
    /// <param name="currentExpression">The expression to aggregate with, if any</param>
    /// <param name="propertyName">The name of the property to call on the objectParameter</param>
    /// <param name="objectParameter">The parameter for the object for evaluation</param>
    /// <param name="operatorType">A string of the operator to use</param>
    /// <returns></returns>
    static Expression CreateExpression<T>(T value, Expression? currentExpression, string propertyName, ParameterExpression objectParameter, string operatorType = "=")
    {
        ConstantExpression valueToTest = Expression.Constant(value);

        MemberExpression propertyToCall = Expression.Property(objectParameter, propertyName);
        Expression operatorExpression = operatorType switch
        {
            ">" => Expression.GreaterThan(propertyToCall, valueToTest),
            "<" => Expression.LessThan(propertyToCall, valueToTest),
            ">=" => Expression.GreaterThanOrEqual(propertyToCall, valueToTest),
            "<=" => Expression.LessThanOrEqual(propertyToCall, valueToTest),
            _ => Expression.Equal(propertyToCall, valueToTest),
        };

        return currentExpression == null
            ? operatorExpression
            : Expression.And(currentExpression, operatorExpression);
    }

    public decimal? ParseNullDecimal(string value) =>
        decimal.TryParse(value, out var result)
            ? result
            : null;

    public int? ParseNullInt(string value) =>
        int.TryParse(value, out var result)
            ? result
            : null;

    public SexValue? ParseSex(string value) =>
        value == "male"
            ? SexValue.Male
            : SexValue.Female;

    public bool? ParseSurvived(string value) =>
        value == "Survived";
}