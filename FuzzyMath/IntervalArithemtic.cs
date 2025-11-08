namespace Holecek.FuzzyMath;

public static class IntervalArithemtic
{
    /// <summary>
    /// Adds two intervals.
    /// </summary>
    public static Interval Add(Interval a, Interval b)
    {
        return new Interval(a.Min + b.Min, a.Max + b.Max);
    }

    /// <summary>
    /// Subtracts two intervals.
    /// </summary>
    public static Interval Subtract(Interval a, Interval b)
    {
        return new Interval(a.Min - b.Max, a.Max - b.Min);
    }

    /// <summary>
    /// Multiplies two intervals.
    /// </summary>
    public static Interval Multiply(Interval a, Interval b)
    {
        double c1 = a.Min * b.Min;
        double c2 = a.Min * b.Max;
        double c3 = a.Max * b.Min;
        double c4 = a.Max * b.Max;

        return new Interval(
            Math.Min(Math.Min(c1, c2), Math.Min(c3, c4)),
            Math.Max(Math.Max(c1, c2), Math.Max(c3, c4)));
    }

    /// <summary>
    /// Divides two intervals.
    /// </summary>
    /// <exception cref="DivideByZeroException">Thrown when the divisor (the second interval) contains zero.</exception>"
    public static Interval Divide(Interval a, Interval b)
    {
        if (b.Contains(0.0))
        {
            throw new DivideByZeroException("The divisor cannot contain zero.");
        }

        double c1 = a.Min / b.Min;
        double c2 = a.Min / b.Max;
        double c3 = a.Max / b.Min;
        double c4 = a.Max / b.Max;

        return new Interval(
            Math.Min(Math.Min(c1, c2), Math.Min(c3, c4)),
            Math.Max(Math.Max(c1, c2), Math.Max(c3, c4)));
    }

    /// <summary>
    /// Negation of an interval (the sign of both bounds is changed).
    /// </summary>
    public static Interval Negation(Interval interval)
    {
        return new Interval(-1.0 * interval.Max, -1.0 * interval.Min);
    }

    /// <summary>
    /// For interval x returns 1/x.
    /// </summary>
    public static Interval Reciprocal(Interval interval)
    {
        var one = new Interval(1.0, 1.0);
        return Divide(one, interval);
    }
}
