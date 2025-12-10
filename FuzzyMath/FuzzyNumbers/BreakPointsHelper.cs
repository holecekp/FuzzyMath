using Holecek.FuzzyMath.Intervals;

namespace Holecek.FuzzyMath.FuzzyNumbers;

internal static class BreakPointsHelper
{
    internal static Interval[] BreakPointsToAlphaCuts(IList<double> breakPoints)
    {
        if (breakPoints.Count == 1 || breakPoints.Count == 2)
        {
            return BreakPointsToAlphaCuts(new double[] { breakPoints.First(), breakPoints.First(), breakPoints.Last(), breakPoints.Last() });
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

    internal static double[] AlphaCutsToBreakPoints(IList<Interval> alphaCuts)
    {
        int breakPointsCount = alphaCuts.Count * 2;
        var breakPoints = new double[breakPointsCount];

        for (int i = 0; i < alphaCuts.Count; i++)
        {
            breakPoints[i] = alphaCuts[i].Min;
            breakPoints[breakPoints.Length - i - 1] = alphaCuts[i].Max;
        }

        return breakPoints;
    }
}
