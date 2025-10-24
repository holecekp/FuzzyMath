using System;

namespace Holecek.FuzzyMath;

/// <summary>
/// A closed interval
/// </summary>
public struct Interval
{
    /// <summary>
    /// Minimum value of the interval (infimum).
    /// </summary>
    public double Min { get; private set; }

    /// <summary>
    /// Maximum value of the interval (supremum).
    /// </summary>
    public double Max { get; private set; }

    public double Size => Math.Abs(Max - Min);

    public double Center => (Max + Min) / 2.0;

    /// <summary>
    /// Creates a closed interval [min, max].
    /// </summary>
    public Interval(double min, double max)
    {
        if (max < min)
        {
            throw new ArgumentException("The max parameter cannot have a lower value than the min parameter. ");
        }

        Min = min;
        Max = max;
    }

    /// <summary>
    /// Verifies if the number <paramref name="x"/> lies inside this interval.
    /// </summary>
    /// <param name="x">A real number</param>
    /// <param name="tolerance">An optional tolerance that will be used for the comparison</param>
    public bool Contains(double x, double tolerance = 0)
    {
        return (x + tolerance >= Min) && (x - tolerance <= Max);
    }

    /// <summary>
    /// Verifies if the number <paramref name="interval"/> lies inside this interval.
    /// </summary>
    /// <param name="interval">An interval</param>
    /// <param name="tolerance">An optional tolerance that will be used for the comparison</param>
    public bool Contains(Interval interval, double tolerance)
    {
        return (interval.Min + tolerance >= Min) && (interval.Max - tolerance <= Max);
    }

    public override string ToString()
    {
        return $"[{Min}, {Max}]";
    }
}
