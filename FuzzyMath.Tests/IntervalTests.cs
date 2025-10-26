using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Holecek.FuzzyMath.Tests;

[TestClass]
public sealed class IntervalTests
{
    [TestMethod]
    public void Constructor_ForValidInterval_SetsProperties_()
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

}
