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
        var fuzzyNumber = new FuzzyNumber([ alphaCut1, alphaCut2, alphaCut3 ]);

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
    [DataRow(0,    1,    5)]
    [DataRow(0.5,  1.5,  4)]
    [DataRow(0.75, 1.75, 3.5)]
    [DataRow(1,    2,    3)]
    public void GetAlphaCut_ForLinearFuzzyNumbers(double alpha, double expectedAlfaCutMin, double expectedAlfaCutMax)
    {
        var fuzzyNumber = new FuzzyNumber(1, 2, 3, 5);

        Interval alphaCut = fuzzyNumber.GetAlphaCut(alpha);
        const double delta = 0.001;

        Assert.AreEqual(expectedAlfaCutMin, alphaCut.Min, delta);
        Assert.AreEqual(expectedAlfaCutMax, alphaCut.Max, delta);
    }

    [TestMethod]
    [DataRow(0,    1,    5)]
    [DataRow(0.5,  1.5,  4)]
    [DataRow(0.75, 1.75, 3.5)]
    [DataRow(1,    2,    3)]
    public void GetAlphaCut_ForPiecewiseLinearFuzzyNumbers_3AlphaCuts(double alpha, double expectedAlfaCutMin, double expectedAlfaCutMax)
    {
        var fuzzyNumber = new FuzzyNumber(
        [
            new Interval(1, 5),
            new Interval(1.5, 4),
            new Interval(2, 3),
        ]);

        Interval alphaCut = fuzzyNumber.GetAlphaCut(alpha);
        const double delta = 0.001;

        Assert.AreEqual(expectedAlfaCutMin, alphaCut.Min, delta);
        Assert.AreEqual(expectedAlfaCutMax, alphaCut.Max, delta);
    }


    [TestMethod]
    public void GetMembership_ForTriangularFuzzyNumberKernel_MustBeOne()
    {
        var fuzzyNumber = new FuzzyNumber(1, 2, 3);
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
        var fuzzyNumber = new FuzzyNumber(1, 2, 3, 4);

        const double expectedMembershipDegree = 1;
        double actualMembershipDegree = fuzzyNumber.GetMembership(pointInsideKernel);
        Assert.AreEqual(expectedMembershipDegree, actualMembershipDegree);
    }

    [TestMethod]
    [DataRow(0.9)]
    [DataRow(4.1)]
    public void GetMembership_OutsideSupport_MustBeZero(double pointOutsideSupport)
    {
        var fuzzyNumber = new FuzzyNumber(1, 2, 3, 4);

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
        var fuzzyNumber = new FuzzyNumber(1, 2, 3, 5);

        double actualMembershipDegree = fuzzyNumber.GetMembership(x);

        const double delta = 0.0001;
        Assert.AreEqual(expectedMembershipDegree, actualMembershipDegree, delta);
    }

    [TestMethod]
    public void WithAlphaCutsCount_ForSameNumberOfAlphaCuts_AlphaCutsAreNotChanged()
    {
        var originalFuzzyNumber = new FuzzyNumber(1, 2, 3, 5);
        int originalAlphaCutsCount = originalFuzzyNumber.AlphaCuts.Length;

        var modifiedFuzzyNumber = originalFuzzyNumber.WithAlphaCutsCount(originalAlphaCutsCount);

        Assert.HasCount(originalAlphaCutsCount, modifiedFuzzyNumber.AlphaCuts);
        Assert.AreEqual(originalFuzzyNumber.AlphaCuts[0], originalFuzzyNumber.AlphaCuts[0]);
        Assert.AreEqual(originalFuzzyNumber.AlphaCuts[1], originalFuzzyNumber.AlphaCuts[1]);
    }

    [TestMethod]
    public void WithAlphaCutsCount_ForDifferentNumberOfAlphaCuts_AlphaCutsAreChanged()
    {
        var originalFuzzyNumber = new FuzzyNumber(1, 2, 3, 5);

        const int newAlphaCutsCount = 3;
        Interval[] expectedModifiedAlphaCuts =
        [
            new Interval(1, 5),
            new Interval(1.5, 4),
            new Interval(2, 3)
        ];

        var modifiedFuzzyNumber = originalFuzzyNumber.WithAlphaCutsCount(newAlphaCutsCount);

        Assert.HasCount(expectedModifiedAlphaCuts.Length, modifiedFuzzyNumber.AlphaCuts);
        Assert.AreEqual(expectedModifiedAlphaCuts[0], modifiedFuzzyNumber.AlphaCuts[0]);
        Assert.AreEqual(expectedModifiedAlphaCuts[1], modifiedFuzzyNumber.AlphaCuts[1]);
        Assert.AreEqual(expectedModifiedAlphaCuts[2], modifiedFuzzyNumber.AlphaCuts[2]);
    }

    [TestMethod]
    public void WithAlphaCutsCount_ForInvalidNumberOfAlphaCuts_Throws()
    {
        var originalFuzzyNumber = new FuzzyNumber(1, 2, 3, 5);
        var newInvalidAlphaCutsCount = 1;

        Assert.Throws<ArgumentException>(() => originalFuzzyNumber.WithAlphaCutsCount(newInvalidAlphaCutsCount));
    }
}