using Holecek.FuzzyMath.Intervals;

namespace Holecek.FuzzyMath.FuzzyNumbers;

public static class FuzzyNumberArithemtic
{
    /// <summary>
    /// Adds two fuzzy numbers
    /// </summary>
    /// <param name="alphaCutsCount">The number of alpha-cuts for the results</param>
    public static FuzzyNumber Add(FuzzyNumber a, FuzzyNumber b, int alphaCutsCount)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(a, b, (alphaCutA, alphaCutB) => alphaCutA + alphaCutB, alphaCutsCount);
    }

    /// <summary>
    /// Subtracts two fuzzy numbers
    /// </summary>
    /// <param name="alphaCutsCount">The number of alpha-cuts for the results</param>
    public static FuzzyNumber Subtract(FuzzyNumber a, FuzzyNumber b, int alphaCutsCount)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(a, b, (alphaCutA, alphaCutB) => alphaCutA - alphaCutB, alphaCutsCount);
    }

    /// <summary>
    /// Multiplies two fuzzy numbers
    /// </summary>
    /// <param name="alphaCutsCount">The number of alpha-cuts for the results</param>
    public static FuzzyNumber Multiply(FuzzyNumber a, FuzzyNumber b, int alphaCutsCount)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(a, b, (alphaCutA, alphaCutB) => alphaCutA * alphaCutB, alphaCutsCount);
    }

    /// <summary>
    /// Divides two fuzzy numbers
    /// </summary>
    /// <param name="alphaCutsCount">The number of alpha-cuts for the results</param>
    /// <exception cref="DivideByZeroException">Thrown when the divisor (the second fuzzy number) contains zero.</exception>
    public static FuzzyNumber Divide(FuzzyNumber a, FuzzyNumber b, int alphaCutsCount)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(a, b, (alphaCutA, alphaCutB) => alphaCutA / alphaCutB, alphaCutsCount);
    }

    /// <summary>
    /// Negation of an interval (the sign is changed).
    /// </summary>
    public static FuzzyNumber Negation(FuzzyNumber a, int alphaCutsCount)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(a, alphaCut => IntervalArithmetic.Negation(alphaCut), alphaCutsCount);
    }

    /// <summary>
    /// For a fuzzy number A, it returns 1/A.
    /// </summary>
    /// <exception cref="DivideByZeroException">Thrown when the fuzzy number contains zero.</exception>
    public static FuzzyNumber Reciprocal(FuzzyNumber a, int alphaCutsCount)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(a, alphaCut => IntervalArithmetic.Reciprocal(alphaCut), alphaCutsCount);
    }
}
