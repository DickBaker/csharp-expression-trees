using System;
using System.Linq.Expressions;
using Xunit;

namespace TitanicExplorer.Scripting.Tests;

public class UnitTest1
{
    [Fact]
    public void IsPrimeTest()
    {
        ParameterExpression value = Expression.Parameter(typeof(int), "value");

        Expression result = ScriptingEngine.IsPrime(value);

        var expr = Expression.Lambda<Func<int, bool>>(result, value);

        Func<int, bool> func = expr.Compile();

        Assert.False(func(0));
        Assert.False(func(1));
        Assert.True(func(2));
        Assert.True(func(3));
        Assert.False(func(4));
        Assert.True(func(5));
        Assert.False(func(9));
        Assert.False(func(144));
        Assert.True(func(1531));
    }

    [Fact]
    public void IsPrimeCSTest()
    {
        Assert.False(ScriptingEngine.IsPrimeCS(0));
        Assert.False(ScriptingEngine.IsPrimeCS(1));
        Assert.True(ScriptingEngine.IsPrimeCS(2));
        Assert.True(ScriptingEngine.IsPrimeCS(3));
        Assert.False(ScriptingEngine.IsPrimeCS(4));
        Assert.True(ScriptingEngine.IsPrimeCS(5));
        Assert.False(ScriptingEngine.IsPrimeCS(9));
        Assert.False(ScriptingEngine.IsPrimeCS(144));
        Assert.True(ScriptingEngine.IsPrimeCS(1531));
    }

    [Fact]
    public void Test()
    {
        static bool func(int value)
        {
            if (value <= 1) { return false; }

            if (value <= 3) { return true; }

            if (value % 2 == 0) { return false; }

            var i = 3;
            var boundary = (int)Math.Floor(Math.Sqrt(value));
            while (i <= boundary)
            {
                if (value % i == 0) { return false; }

                i += 2;
            }

            return true;
        }

        Assert.True(func(19));
    }

    [Fact]
    public void Factorial()
    {
        ParameterExpression value = Expression.Parameter(typeof(int));

        Expression result = ScriptingEngine.Factorial(value);

        var expr = Expression.Lambda<Func<int, int>>(result, value);

        Func<int, int> func = expr.Compile();

        Assert.Equal(6, func(3));
        Assert.Equal(120, func(5));
    }
}