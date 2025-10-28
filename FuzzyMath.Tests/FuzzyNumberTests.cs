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
}
