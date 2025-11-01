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
        var singleAlphaCut = new Interval[] { new Interval(-1, 6) };

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

    [TestMethod]
    public void GetMembership_ForTriangularFuzzyNumberKernel_MustBeOne()
    {
        FuzzyNumber fuzzyNumber = new FuzzyNumber(1, 2, 3);
        const double pointInsideKernel = 2;

        const double expectedMembershipDegree = 1;
        double actualMembershipDegree = fuzzyNumber.GetMembership(pointInsideKernel);
        Assert.AreEqual(expectedMembershipDegree, actualMembershipDegree);
    }

    [TestMethod]
    [DataRow(2)]
    [DataRow(2.5)]
    [DataRow(3)]
    public void GetMembership_ForTrapeziodalFuzzyNumberKernel_MustBeOne(double pointInsideKernel)
    {
        FuzzyNumber fuzzyNumber = new FuzzyNumber(1, 2, 3, 4);

        const double expectedMembershipDegree = 1;
        double actualMembershipDegree = fuzzyNumber.GetMembership(pointInsideKernel);
        Assert.AreEqual(expectedMembershipDegree, actualMembershipDegree);
    }


    [TestMethod]
    [DataRow(0.9)]
    [DataRow(4.1)]
    public void GetMembership_OutsideSupport_MustBeZero(double pointOutsideSupport)
    {
        FuzzyNumber fuzzyNumber = new FuzzyNumber(1, 2, 3, 4);

        const double expectedMembershipDegree = 0;
        double actualMembershipDegree = fuzzyNumber.GetMembership(pointOutsideSupport);
        Assert.AreEqual(expectedMembershipDegree, actualMembershipDegree);
    }

    [TestMethod]
    [DataRow(1, 0)]
    [DataRow(1.25, 0.25)]
    [DataRow(1.75, 0.75)]
    [DataRow(2, 1)]
    [DataRow(4, 0.5)]
    [DataRow(4.5, 0.25)]
    public void GetMembership_InsideSupport(double x, double expectedMembershipDegree)
    {
        FuzzyNumber fuzzyNumber = new FuzzyNumber(1, 2, 3, 5);

        double actualMembershipDegree = fuzzyNumber.GetMembership(x);

        const double delta = 0.0001;
        Assert.AreEqual(expectedMembershipDegree, actualMembershipDegree, delta);
    }
}