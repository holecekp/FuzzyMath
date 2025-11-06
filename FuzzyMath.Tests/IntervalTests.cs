using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Holecek.FuzzyMath.Tests;

[TestClass]
public sealed class IntervalTests
{
    [TestMethod]
    public void Constructor_ForValidInterval_SetsProperties()
    {
        var interval = new Interval(1, 2);

        Assert.AreEqual(1, interval.Min);
        Assert.AreEqual(2, interval.Max);
    }

    [TestMethod]
    public void Constructor_ForInvalidValidInterval_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            _ = new Interval(1, 0.9);
        });
    }

    [TestMethod]
    public void Size_ReturnsCorrectValue()
    {
        var interval = new Interval(1, 2);
        double expectedSize = 1;

        double actualSize = interval.Size;
        Assert.AreEqual(expectedSize, actualSize);
    }

    [TestMethod]
    public void Center_ReturnsCorrectValue()
    {
        var interval = new Interval(1, 2);
        const double expectedCenter = 1.5;

        double actualCenter = interval.Center;

        Assert.AreEqual(expectedCenter, actualCenter);
    }

    [TestMethod]
    [DataRow(-1, 2)]
    [DataRow(0, 1)]
    [DataRow(2, 2)]
    public void RestrictTo_ForIntervalsInsideUniverse_DoesntChangeInterval(double inputMin, double inputMax)
    {
        var input = new Interval(inputMin, inputMax);
        var universe = new Interval(-1, 2);

        var actualRsult = input.RestrictTo(universe);
        Assert.AreEqual(input, actualRsult);
    }

    [TestMethod]
    [DataRow(-9, 9, -1, 2)]
    [DataRow(-2, 2, -1, 2)]
    [DataRow(-1, 3, -1, 2)]
    [DataRow(-2, 1, -1, 1)]
    [DataRow(0, 3, 0, 2)]
    public void RestrictTo_ForIntervalsNotFullyInsideUniverse_RestrictsInterval(double inputMin, double inputMax, double expectedResultMin, double expectedResultMax)
    {
        var input = new Interval(inputMin, inputMax);
        var universe = new Interval(-1, 2);
        var expectedResult = new Interval(expectedResultMin, expectedResultMax);

        var actualRsult = input.RestrictTo(universe);
        Assert.AreEqual(expectedResult, actualRsult);
    }


    [TestMethod]
    [DataRow(-9, -8, -1, -1)]
    [DataRow(8, 9, 2, 2)]
    public void RestrictTo_ForIntervalsFullyOutsideUniverse_ReturnsNearestValidInterval(double inputMin, double inputMax, double expectedResultMin, double expectedResultMax)
    {
        var input = new Interval(inputMin, inputMax);
        var universe = new Interval(-1, 2);
        var expectedResult = new Interval(expectedResultMin, expectedResultMax);

        var actualRsult = input.RestrictTo(universe);
        Assert.AreEqual(expectedResult, actualRsult);
    }
}
