using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Holecek.FuzzyMath.Tests;

[TestClass]
public class FuzzyNumberTests
{
    [TestMethod]
    public void Constructor_ForCrispNumber_SetsProperties()
    {
        const double crispNumber = 3;
        var fuzzyNumber = new FuzzyNumber(crispNumber);

        Assert.HasCount(2, fuzzyNumber.AlphaCuts);
        Assert.AreEqual(new Interval(crispNumber, crispNumber), fuzzyNumber.AlphaCuts[0]);
        Assert.AreEqual(new Interval(crispNumber, crispNumber), fuzzyNumber.AlphaCuts[1]);
    }

    [TestMethod]
    public void Constructor_ForInterval_SetsProperties()
    {
        const double min = 2;
        const double max = 3;
        var fuzzyNumber = new FuzzyNumber(min, max);

        Assert.HasCount(2, fuzzyNumber.AlphaCuts);
        Assert.AreEqual(new Interval(min, max), fuzzyNumber.AlphaCuts[0]);
        Assert.AreEqual(new Interval(min, max), fuzzyNumber.AlphaCuts[1]);
    }

    [TestMethod]
    public void Constructor_ForValidTriangularFuzzyNumber_SetsProperties()
    {
        const double a = -1;
        const double b = 2;
        const double c = 3;
        var fuzzyNumber = new FuzzyNumber(a, b, c);

        Assert.HasCount(2, fuzzyNumber.AlphaCuts);
        Assert.AreEqual(new Interval(a, c), fuzzyNumber.AlphaCuts[0]);
        Assert.AreEqual(new Interval(b, b), fuzzyNumber.AlphaCuts[1]);
    }

    [TestMethod]
    public void Constructor_ForInvalidValidTriangularFuzzyNumber_Throws()
    {
        const double a = 1;
        const double b = 3;
        const double c = 2;

        Assert.Throws<ArgumentException>(() => new FuzzyNumber(a, b, c));
    }

    [TestMethod]
    public void Constructor_ForValidTrapezoidalFuzzyNumber_SetsProperties()
    {
        const double a = -1;
        const double b = 2;
        const double c = 3;
        const double d = 6;
        var fuzzyNumber = new FuzzyNumber(a, b, c, d);

        Assert.HasCount(2, fuzzyNumber.AlphaCuts);
        Assert.AreEqual(new Interval(a, d), fuzzyNumber.AlphaCuts[0]);
        Assert.AreEqual(new Interval(b, c), fuzzyNumber.AlphaCuts[1]);
    }

    [TestMethod]
    public void Constructor_ForInvalidValidTrapezoidalFuzzyNumber_Throws()
    {
        const double a = -1;
        const double b = -2;
        const double c = 3;
        const double d = 6;

        Assert.Throws<ArgumentException>(() => new FuzzyNumber(a, b, c, d));
    }

    [TestMethod]
    public void Constructor_ForValidPiecewiseLinearFuzzyNumber_SetsProperties()
    {
        var alphaCut1 = new Interval(-1, 6);
        var alphaCut2 = new Interval(0, 4);
        var alphaCut3 = new Interval(2, 3);
        var fuzzyNumber = new FuzzyNumber(new Interval[] { alphaCut1, alphaCut2, alphaCut3 });

        Assert.HasCount(3, fuzzyNumber.AlphaCuts);
        Assert.AreEqual(alphaCut1, fuzzyNumber.AlphaCuts[0]);
        Assert.AreEqual(alphaCut2, fuzzyNumber.AlphaCuts[1]);
        Assert.AreEqual(alphaCut3, fuzzyNumber.AlphaCuts[2]);
    }

    [TestMethod]
    public void Constructor_ForPiecewiseLinearFuzzyNumberWithInvalidAlphaCuts_Throw()
    {
        var alphaCut1 = new Interval(-1, 6);
        var alphaCut2 = new Interval(0, 7);
        var alphaCut3 = new Interval(2, 3);
        var alphaCuts = new Interval[] { alphaCut1, alphaCut2, alphaCut3 };

        Assert.Throws<ArgumentException>(() => new FuzzyNumber(alphaCuts));
    }

    [TestMethod]
    public void Constructor_ForPiecewiseLinearFuzzyNumberWithNotEnoughAlphaCuts_Throw()
    {
        var singleAlphaCut = new Interval[] { new Interval(-1, 6)};

        Assert.Throws<ArgumentException>(() => new FuzzyNumber(singleAlphaCut));
    }

    [TestMethod]
    public void Support()
    {
        const double a = -1;
        const double b = 2;
        const double c = 3;
        const double d = 6;
        var fuzzyNumber = new FuzzyNumber(a, b, c, d);

        Assert.AreEqual(new Interval(a, d), fuzzyNumber.Support);
    }

    [TestMethod]
    public void Kernel()
    {
        const double a = -1;
        const double b = 2;
        const double c = 3;
        const double d = 6;
        var fuzzyNumber = new FuzzyNumber(a, b, c, d);

        Assert.AreEqual(new Interval(b, c), fuzzyNumber.Kernel);
    }
}