using Holecek.FuzzyMath.Intervals;

namespace Holecek.FuzzyMath.Tests.Intervals;

[TestClass]
public class IntervalArithmeticTests
{
    [TestMethod]
    public void Add()
    {
        // From Example 4.7.2 at https://web.mit.edu/hyperbook/Patrikalakis-Maekawa-Cho/node45.html
        var a = new Interval(2, 4);
        var b = new Interval(3, 5);
        var expectedResult = new Interval(5, 9);

        var actualResult = a + b;

        Assert.AreEqual(expectedResult, actualResult);
    }

    [TestMethod]
    public void Subtract()
    {
        // From Example 4.7.2 at https://web.mit.edu/hyperbook/Patrikalakis-Maekawa-Cho/node45.html
        var a = new Interval(2, 4);
        var b = new Interval(3, 5);
        var expectedResult = new Interval(-3, 1);

        var actualResult = a - b;

        Assert.AreEqual(expectedResult, actualResult);
    }

    [TestMethod]
    public void Multiply()
    {
        // From Example 4.7.2 at https://web.mit.edu/hyperbook/Patrikalakis-Maekawa-Cho/node45.html
        var a = new Interval(2, 4);
        var b = new Interval(3, 5);
        var expectedResult = new Interval(6, 20);

        var actualResult = a * b;

        Assert.AreEqual(expectedResult, actualResult);
    }

    [TestMethod]
    public void Divide_ForDivisorNotContainingZero_ReturnsResult()
    {
        // From Example 4.7.2 at https://web.mit.edu/hyperbook/Patrikalakis-Maekawa-Cho/node45.html
        var a = new Interval(2, 4);
        var b = new Interval(3, 5);
        var expectedResult = new Interval(2.0 / 5.0, 4.0 / 3.0);

        var actualResult = a / b;

        Assert.AreEqual(expectedResult, actualResult);
    }

    [TestMethod]
    public void Divide_ForDivisorContainingZero_Throws()
    {
        var a = new Interval(2, 4);
        var b = new Interval(-1, 2);

        Assert.Throws<DivideByZeroException>(() => a / b);
    }

    [TestMethod]
    public void CombinationOfMultipleOperations()
    {
        // Left side of the Example 4.7.3 at https://web.mit.edu/hyperbook/Patrikalakis-Maekawa-Cho/node45.html
        var a = new Interval(1, 2);
        var expectedResult = new Interval(-2, 2);

        var actualResult = a * (a - a);

        Assert.AreEqual(expectedResult, actualResult);
    }

    [TestMethod]
    public void CombinationOfMultipleOperations2()
    {
        // Right side of the Example 4.7.3 at https://web.mit.edu/hyperbook/Patrikalakis-Maekawa-Cho/node45.html
        var a = new Interval(1, 2);
        var expectedResult = new Interval(-3, 3);

        var actualResult = a * a - a * a;

        Assert.AreEqual(expectedResult, actualResult);
    }
}
