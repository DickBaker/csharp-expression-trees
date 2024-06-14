using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace TitanicExplorer.Scripting;

public static class ScriptingEngine
{
    public static Expression ExpressionFromString<T, T1>(string value) =>
        DynamicExpressionParser.ParseLambda<T, T1>(new ParsingConfig(), true, value);

    /// <summary>
    /// C# high-level code to find if specified number is prime
    /// </summary>
    /// <param name="number">input parameter for testing if prime</param>
    /// <returns>true if number is prime, false otherwise</returns>
    /// <remarks>
    /// 1.  introduced at [4.10; 1:27] and in making-expression-trees-work-for-you-slides.pdf #25/37, but unfortunately absent in repo
    ///     - so featured here and unit-tested in TitanicExplorer.Scripting.Tests.UnitTest1.IsPrimeCSTest
    /// 2.  unfortunately all the juicy "Working with Roslyn" code is also absent [sigh]
    /// 3.  the short-circuit code tweaked slightly [yeah small-beer I know, but it goaded me], and ditto in IsPrime below
    /// </remarks>
    public static bool IsPrimeCS(int number)
    {
        if (number <= 1) return false;
        if (number <= 3) return true;
        if (number % 2 == 0) return false;

        var boundary = (int)Math.Floor(Math.Sqrt(number));
        for (var i = 3; i <= boundary; i += 2)
        {
            if (number % i == 0) { return false; }
        }
        return true;
    }

    public static Expression IsPrime(ParameterExpression value)
    {
        LabelTarget label = Expression.Label();

        ParameterExpression result = Expression.Parameter(typeof(bool), "result");

        LabelTarget returnLabel = Expression.Label(typeof(bool));

        BinaryExpression valueLessThanEqualToOne = Expression.LessThanOrEqual(value, Expression.Constant(1));
        BinaryExpression valueLessThanEqualToThree = Expression.LessThanOrEqual(value, Expression.Constant(3));
        BinaryExpression valueModTwoZero = Expression.Equal(Expression.Modulo(value, Expression.Constant(2)), Expression.Constant(0));

        System.Reflection.MethodInfo sqRt = typeof(Math).GetMethod("Sqrt")!;
        System.Reflection.MethodInfo floor = typeof(Math).GetMethod("Floor", [typeof(double)])!;

        MethodCallExpression valueSqRt = Expression.Call(null, sqRt, Expression.Convert(value, typeof(double)));

        UnaryExpression evalFunction = Expression.Convert(Expression.Call(null, floor, valueSqRt), typeof(int));

        ParameterExpression boundary = Expression.Variable(typeof(int), "boundary");

        ParameterExpression i = Expression.Variable(typeof(int), "i");

        Expression modBlock = Expression.IfThen(
            Expression.Equal(Expression.Modulo(value, i), Expression.Constant(0)),
            Expression.Return(returnLabel, Expression.Constant(false))
        );

        Expression incrementI = Expression.AddAssign(i, Expression.Constant(2));

        BlockExpression block = Expression.Block(
            [result, i, boundary],
                Expression.IfThen(
                        valueLessThanEqualToOne,
                        Expression.Return(returnLabel, Expression.Constant(false))
                ),
                Expression.IfThen(
                        valueLessThanEqualToThree,
                        Expression.Return(returnLabel, Expression.Constant(true))
                ),
                Expression.IfThen(
                        valueModTwoZero,
                        Expression.Return(returnLabel, Expression.Constant(false))
                ),

                Expression.Assign(i, Expression.Constant(3)),
                Expression.Assign(boundary, evalFunction),
                Expression.Loop(
                    Expression.IfThenElse
                    (
                        Expression.LessThanOrEqual(i, boundary),
                        Expression.Block(modBlock, incrementI),
                        Expression.Break(label)
                    ),
                    label
                ),
                Expression.Return(returnLabel, Expression.Constant(true)),
                Expression.Label(returnLabel, Expression.Constant(true))
                );

        return block;
    }

    /// <summary>
    /// From Microsoft Docs: https://docs.microsoft.com/en-us/dotnet/csharp/expression-trees-building
    /// </summary>
    /// <returns>Returns a factorial expression</returns>
    public static Expression Factorial(ParameterExpression value)
    {
        ParameterExpression result = Expression.Variable(typeof(int), "result");

        // Creating a label that represents the return value
        LabelTarget label = Expression.Label(typeof(int));

        BinaryExpression initializeResult = Expression.Assign(result, Expression.Constant(1));

        // This is the inner block that performs the multiplication,
        // and decrements the value of 'n'
        BlockExpression block = Expression.Block(
            Expression.Assign(result,
                Expression.Multiply(result, value)),
            Expression.PostDecrementAssign(value)
        );

        // Creating a method body.
        BlockExpression body = Expression.Block(
            [result],
            initializeResult,
            Expression.Loop(
                Expression.IfThenElse(
                    Expression.GreaterThan(value, Expression.Constant(1)),
                    block,
                    Expression.Break(label, result)
                ),
                label
            )
        );

        return body;
    }
}