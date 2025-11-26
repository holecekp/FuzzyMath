using Holecek.FuzzyMath.FuzzyNumbers;
using Holecek.FuzzyMath.Intervals;

namespace Holecek.FuzzyMath.Tests.FuzzyNumbers;

[TestClass]
public class FuzzyNumberArithmeticTests
{
    [TestMethod]
    public void Add()
    {
        const int AlphaCutsCount = 2;
        var input1 = new FuzzyNumber(1, 2, 3);
        var input2 = new FuzzyNumber(3, 4, 5);

        var result = FuzzyNumberArithmetic.Add(input1, input2, AlphaCutsCount);

        Assert.HasCount(2, result.AlphaCuts);
        Assert.AreEqual(new Interval(4, 8), result.AlphaCuts[0]);
        Assert.AreEqual(new Interval(6, 6), result.AlphaCuts[1]);
    }

    [TestMethod]
    public void Subtract()
    {
        const int AlphaCutsCount = 2;
        var input1 = new FuzzyNumber(1, 2, 3);
        var input2 = new FuzzyNumber(3, 4, 5);

        var result = FuzzyNumberArithmetic.Subtract(input1, input2, AlphaCutsCount);

        Assert.HasCount(2, result.AlphaCuts);
        Assert.AreEqual(new Interval(-4, 0), result.AlphaCuts[0]);
        Assert.AreEqual(new Interval(-2, -2), result.AlphaCuts[1]);
    }

    [TestMethod]
    public void Multiply()
    {
        const int AlphaCutsCount = 2;
        var input1 = new FuzzyNumber(1, 2, 3);
        var input2 = new FuzzyNumber(3, 4, 5);

        var result = FuzzyNumberArithmetic.Multiply(input1, input2, AlphaCutsCount);

        Assert.HasCount(2, result.AlphaCuts);
        Assert.AreEqual(new Interval(3, 15), result.AlphaCuts[0]);
        Assert.AreEqual(new Interval(8, 8), result.AlphaCuts[1]);
    }

    [TestMethod]
    public void Divide()
    {
        const int AlphaCutsCount = 2;
        var input1 = new FuzzyNumber(1, 2, 3);
        var input2 = new FuzzyNumber(3, 4, 5);

        var result = FuzzyNumberArithmetic.Divide(input1, input2, AlphaCutsCount);

        Assert.HasCount(2, result.AlphaCuts);
        Assert.AreEqual(new Interval(1.0/5.0, 1), result.AlphaCuts[0]);
        Assert.AreEqual(new Interval(0.5, 0.5), result.AlphaCuts[1]);
    }

    [TestMethod]
    public void Negation()
    {
        const int AlphaCutsCount = 2;
        var input = new FuzzyNumber(1, 2, 3);

        var result = FuzzyNumberArithmetic.Negation(input, AlphaCutsCount);

        Assert.HasCount(2, result.AlphaCuts);
        Assert.AreEqual(new Interval(-3, -1), result.AlphaCuts[0]);
        Assert.AreEqual(new Interval(-2, -2), result.AlphaCuts[1]);
    }

    [TestMethod]
    public void Reciprocal()
    {
        const int AlphaCutsCount = 2;
        var input = new FuzzyNumber(1, 2, 3);

        var result = FuzzyNumberArithmetic.Reciprocal(input, AlphaCutsCount);

        Assert.HasCount(2, result.AlphaCuts);
        Assert.AreEqual(new Interval(1.0 / 3.0, 1), result.AlphaCuts[0]);
        Assert.AreEqual(new Interval(0.5, 0.5), result.AlphaCuts[1]);
    }
}
