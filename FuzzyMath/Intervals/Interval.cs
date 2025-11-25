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
            throw new ArgumentException("The max parameter cannot have a lower value than the min parameter.");
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
    public bool Contains(Interval interval, double tolerance = 0)
    {
        return (interval.Min + tolerance >= Min) && (interval.Max - tolerance <= Max);
    }

    /// <summary>
    /// Verifies if this interval is equal to the <paramref name="other"/> interval.
    /// </summary>
    /// <param name="other">An other interval</param>
    /// <param name="tolerance">An optional tolerance that will be used for the comparison</param>
    public bool IsEqualTo(Interval other, double tolerance = 0)
    {
        return Math.Abs(Min - other.Min) <= tolerance && Math.Abs(Max - other.Max) <= tolerance;
    }

    /// <summary>
    /// Returns the interval modifed so that it would lie inside the <paramref name="universe"/> interval.
    /// If the interval lies completely outside the universe, the nearest valid interval within the <paramref name="universe"/>
    /// is returned.
    /// </summary>
    public Interval RestrictTo(Interval universe)
    {
        double restrictedMin = Math.Min(Math.Max(Min, universe.Min), universe.Max);
        double restrictedMax = Math.Max(Math.Min(Max, universe.Max), universe.Min);
        return new Interval(restrictedMin, restrictedMax);
    }

    public override string ToString() => $"[{Min}, {Max}]";

    public void Deconstruct(out double min, out double max)
    {
        min = Min;
        max = Max;
    }

    public static Interval operator +(Interval a, Interval b) => IntervalArithmetic.Add(a, b);
    public static Interval operator +(Interval a, double b) => IntervalArithmetic.Add(a, b);
    public static Interval operator +(double a, Interval b) => IntervalArithmetic.Add(a, b);

    public static Interval operator -(Interval a, Interval b) => IntervalArithmetic.Subtract(a, b);
    public static Interval operator -(Interval a, double b) => IntervalArithmetic.Subtract(a, b);
    public static Interval operator -(double a, Interval b) => IntervalArithmetic.Subtract(a, b);

    public static Interval operator *(Interval a, Interval b) => IntervalArithmetic.Multiply(a, b);
    public static Interval operator *(Interval a, double b) => IntervalArithmetic.Multiply(a, b);
    public static Interval operator *(double a, Interval b) => IntervalArithmetic.Multiply(a, b);

    public static Interval operator /(Interval a, Interval b) => IntervalArithmetic.Divide(a, b);
    public static Interval operator /(Interval a, double b) => IntervalArithmetic.Divide(a, b);
    public static Interval operator /(double a, Interval b) => IntervalArithmetic.Divide(a, b);
}
