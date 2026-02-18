using Holecek.FuzzyMath.Intervals;

namespace Holecek.FuzzyMath.FuzzyNumbers;

/// <summary>
/// The class provides methods for converting between break points and alpha-cuts representations of fuzzy numbers.
/// Break points are a non-decreasing sequence of numbers. They are sutable for presenting fuzzy numbers to the user,
/// or for gathering user input.
/// 
/// For example:
/// 1 represents a crisp number,
/// 1, 2 represents an interval,
/// 1, 2, 3 represents a triangular fuzzy number,
/// 1, 2, 3, 4 represents a trapezoidal fuzzy number.
/// 
/// More break points can be used to represent more complex fuzzy numbers. The break points then consist of the lower
/// and upper bounds of the alpha-cuts, For example, 1, 2, 3, 4, 5, 6 represents a fuzzy number with alpha-cuts [1, 6], [2, 5], [3, 4].
/// If the kernel has only a single element, the corresponding break point doesn't have to be repeated. For example, 1, 2, 3, 3, 4, 5
/// can be written simply as 1, 2, 3, 4, 5. This is analogous to the notation for triangular fuzzy numbers (1, 2, 2, 3 is in fact
/// a triangular fuzzy number because its kernel contains only a single element, and is usually written simply as 1, 2, 3).
/// </summary>
public static class BreakPointsConverter
{
    public static Interval[] ConvertToAlphaCuts(IList<double> breakPoints)
    {
        if (breakPoints.Count == 1 || breakPoints.Count == 2)
        {
            return ConvertToAlphaCuts(new double[] { breakPoints.First(), breakPoints.First(), breakPoints.Last(), breakPoints.Last() });
        }

        int alphacutsCount = (int)Math.Ceiling(breakPoints.Count / 2.0);
        var alphaCuts = new Interval[alphacutsCount];

        for (int i = 0; i < alphacutsCount; i++)
        {
            double min = breakPoints[i];
            double max = breakPoints[breakPoints.Count - i - 1];
            alphaCuts[i] = new Interval(min, max);
        }

        return alphaCuts;
    }

    public static double[] ConvertFromAlphaCuts(IList<Interval> alphaCuts)
    {
        if (alphaCuts.Count == 2 && alphaCuts[0].Min == alphaCuts[1].Min && alphaCuts[0].Max == alphaCuts[1].Max)
        {
            return alphaCuts[0].Size == 0 ?
                new double[] { alphaCuts[0].Min } :
                new double[] { alphaCuts[0].Min, alphaCuts[0].Max };
        }

        int breakPointsCount = alphaCuts.Count * 2;

        if (alphaCuts.Last().Size == 0)
        {
            // Conventions - if the kernel is only a single element, don't repeat the break point.
            // For example, a triangular fuzzy number is expressed as 1, 2, 3 instead of 1, 2, 2, 3
            breakPointsCount--;
        }

        var breakPoints = new double[breakPointsCount];
        for (int i = 0; i < alphaCuts.Count; i++)
        {
            breakPoints[i] = alphaCuts[i].Min;
            breakPoints[breakPoints.Length - i - 1] = alphaCuts[i].Max;
        }

        return breakPoints;
    }
}
