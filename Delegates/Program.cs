using System.Linq.Expressions;

namespace Delegates;

static class Program
{
    static void Main()
    {
        ParameterExpression xExpression = Expression.Parameter(typeof(int), "x");
        ConstantExpression constantExpression = Expression.Constant(12);
        BinaryExpression greaterThan = Expression.GreaterThan(xExpression, constantExpression);

        ConstantExpression constant4Expression = Expression.Constant(4);
        BinaryExpression lessThan = Expression.LessThan(xExpression, constant4Expression);

        BinaryExpression or = Expression.Or(greaterThan, lessThan);

        var expr = Expression.Lambda<Func<int, bool>>(or, false, new List<ParameterExpression> { xExpression, });
        Func<int, bool> func = expr.Compile();

        Console.WriteLine(func(2));
    }
}