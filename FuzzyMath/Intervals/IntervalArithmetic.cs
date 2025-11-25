namespace Holecek.FuzzyMath;

public static class IntervalArithmetic
{
    /// <summary>
    /// Adds two intervals.
    /// </summary>
    public static Interval Add(Interval a, Interval b)
    {
        return new Interval(a.Min + b.Min, a.Max + b.Max);
    }

    public static Interval Add(Interval a, double b) => Add(a, DoubleToInterval(b));

    public static Interval Add(double a, Interval b) => Add(DoubleToInterval(a), b);

    /// <summary>
    /// Subtracts two intervals.
    /// </summary>
    public static Interval Subtract(Interval a, Interval b)
    {
        return new Interval(a.Min - b.Max, a.Max - b.Min);
    }

    public static Interval Subtract(Interval a, double b) => Subtract(a, DoubleToInterval(b));

    public static Interval Subtract(double a, Interval b) => Subtract(DoubleToInterval(a), b);

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

    public static Interval Multiply(Interval a, double b) => Multiply(a, DoubleToInterval(b));

    public static Interval Multiply(double a, Interval b) => Multiply(DoubleToInterval(a), b);

    /// <summary>
    /// Divides two intervals.
    /// </summary>
    /// <exception cref="DivideByZeroException">Thrown when the divisor (the second interval) contains zero.</exception>
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

    public static Interval Divide(Interval a, double b) => Divide(a, DoubleToInterval(b));

    public static Interval Divide(double a, Interval b) => Divide(DoubleToInterval(a), b);

    /// <summary>
    /// Negation of an interval (the sign of both bounds is changed).
    /// </summary>
    public static Interval Negation(Interval interval)
    {
        return new Interval(-1.0 * interval.Max, -1.0 * interval.Min);
    }

    /// <summary>
    /// For interval x, it returns 1/x.
    /// </summary>
    /// <exception cref="DivideByZeroException">Thrown when the interval contains zero.</exception>
    public static Interval Reciprocal(Interval interval)
    {
        var one = new Interval(1.0, 1.0);
        return Divide(one, interval);
    }

    private static Interval DoubleToInterval(double number) => new Interval(number, number);
}
