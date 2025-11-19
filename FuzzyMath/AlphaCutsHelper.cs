namespace Holecek.FuzzyMath;

internal static class AlphaCutsHelper
{

    public static void ThrowIfAlphaCutsAreInvalid(
        IList<Interval> alphaCuts,
        string errorMessageForNotEnoughAlphaCuts = "Alpha-cuts list must contain at least 2 elements (the support and the kernel).",
        string errorMessageTemplateForInvalidAlphaCut = "Invalid alpha-cut value ({0}). Each alpha-cut must be a subset of the previous one ({1}) in this case).")
    {
        if (alphaCuts == null)
        {
            throw new ArgumentNullException();
        }

        if (alphaCuts.Count < 2)
        {
            throw new ArgumentException(errorMessageForNotEnoughAlphaCuts);
        }

        for (int i = 1; i < alphaCuts.Count; i++)
        {
            if (!alphaCuts[i - 1].Contains(alphaCuts[i], tolerance: 0))
            {
                throw new ArgumentException(String.Format(errorMessageTemplateForInvalidAlphaCut, alphaCuts[i], alphaCuts[i - 1]));
            }
        }
    }

    public static int GetHighestAlphaCutIndexContainingValue(IList<Interval> alphaCuts, double value)
    {
        if (!alphaCuts.First().Contains(value))
        {
            throw new InvalidOperationException("No alpha-cut contains the value.");
        }

        for (int i = 1; i < alphaCuts.Count; i++)
        {
            if (!alphaCuts[i].Contains(value))
            {
                return i - 1;
            }
        }

        return 0;
    }
}
