using Holecek.FuzzyMath.FuzzyNumbers;
using Holecek.FuzzyMath.Intervals;

namespace Holecek.FuzzyMath.Tests.FuzzyNumbers;

[TestClass]
public class BreakPointsConverterTests
{
    [TestMethod]
    public void ConvertFromAlphaCuts_CrispNumbers()
    {
        // Fuzzy numbers are required to contain at least 2 alpha-cuts. If they represent a crisp number,
        // they can be expressed simply just by 1 break point (5 instead of 5, 5, 5, 5).
        var alphaCuts = new Interval[]
        {
            new Interval(5, 5),
            new Interval(5, 5)
        };
        var expectedBreakPoints = new double[] { 5 };

        var breakPoints = BreakPointsConverter.ConvertFromAlphaCuts(alphaCuts);

        CollectionAssert.AreEqual(expectedBreakPoints, breakPoints);
    }

    [TestMethod]
    public void ConvertFromAlphaCuts_Intervals()
    {
        // Fuzzy numbers are required to contain at least 2 alpha-cuts. If they represent an interval,
        // they can be expressed simply just by 2 break points (5, 7 instead of 5, 5, 7, 7).
        var alphaCuts = new Interval[]
        {
            new Interval(5, 7),
            new Interval(5, 7)
        };
        var expectedBreakPoints = new double[] { 5, 7 };

        var breakPoints = BreakPointsConverter.ConvertFromAlphaCuts(alphaCuts);

        CollectionAssert.AreEqual(expectedBreakPoints, breakPoints);
    }

    [TestMethod]
    public void ConvertFromAlphaCuts_TriangularFuzzyNumbers()
    {
        // For triangular fuzzy numbers, 3 break points are expected.
        var alphaCuts = new Interval[]
        {
            new Interval(5, 9),
            new Interval(7, 7)
        };
        var expectedBreakPoints = new double[] { 5, 7, 9 };

        var breakPoints = BreakPointsConverter.ConvertFromAlphaCuts(alphaCuts);

        CollectionAssert.AreEqual(expectedBreakPoints, breakPoints);
    }

    [TestMethod]
    public void ConvertFromAlphaCuts_TrapezoidalFuzzyNumbers()
    {
        var alphaCuts = new Interval[]
        {
            new Interval(5, 10),
            new Interval(7, 9)
        };
        var expectedBreakPoints = new double[] { 5, 7, 9, 10 };

        var breakPoints = BreakPointsConverter.ConvertFromAlphaCuts(alphaCuts);

        CollectionAssert.AreEqual(expectedBreakPoints, breakPoints);
    }

    [TestMethod]
    public void ConvertFromAlphaCuts_EvenNumberOfBreakPoints()
    {
        var alphaCuts = new Interval[]
        {
            new Interval(1, 10),
            new Interval(4, 9),
            new Interval(5, 7)
        };
        var expectedBreakPoints = new double[] { 1, 4, 5, 7, 9, 10 };

        var breakPoints = BreakPointsConverter.ConvertFromAlphaCuts(alphaCuts);

        CollectionAssert.AreEqual(expectedBreakPoints, breakPoints);
    }

    [TestMethod]
    public void ConvertFromAlphaCuts_OddNumberOfBreakPoints()
    {
        // By convention, if the kernel (the last alpha-cut) contains only a single element, the break points can be
        // written in a simpler form: just (1, 4, 6, 9, 10) instead of (1, 4, 6, 6, 9, 10).
        var alphaCuts = new Interval[]
        {
            new Interval(1, 10),
            new Interval(4, 9),
            new Interval(6, 6)
        };
        var expectedBreakPoints = new double[] { 1, 4, 6, 9, 10 };

        var breakPoints = BreakPointsConverter.ConvertFromAlphaCuts(alphaCuts);

        CollectionAssert.AreEqual(expectedBreakPoints, breakPoints);
    }
}
