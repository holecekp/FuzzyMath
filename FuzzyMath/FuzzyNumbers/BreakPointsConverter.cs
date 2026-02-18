using Holecek.FuzzyMath.Intervals;

namespace Holecek.FuzzyMath.FuzzyNumbers;

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
