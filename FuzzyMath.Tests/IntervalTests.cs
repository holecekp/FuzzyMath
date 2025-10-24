using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Holecek.FuzzyMath.Tests;

[TestClass]
public sealed class IntervalTests
{
    [TestMethod]
    public void ValidInterval()
    {
        var interval = new Interval(1, 2);

        Assert.AreEqual(1, interval.Min);
        Assert.AreEqual(2, interval.Max);
    }

    [TestMethod]
    public void InvalidInterval()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            _ = new Interval(1, 0.9);
        });
    }
}
