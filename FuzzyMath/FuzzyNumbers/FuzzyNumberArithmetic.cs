using Holecek.FuzzyMath.Intervals;

namespace Holecek.FuzzyMath.FuzzyNumbers;

public static class FuzzyNumberArithmetic
{
    /// <summary>
    /// Adds two fuzzy numbers
    /// </summary>
    /// <param name="alphaCutsCount">The number of alpha-cuts for the results</param>
    public static FuzzyNumber Add(FuzzyNumber a, FuzzyNumber b, int alphaCutsCount)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(a, b, (alphaCutA, alphaCutB) => alphaCutA + alphaCutB, alphaCutsCount);
    }

    public static FuzzyNumber Add(FuzzyNumber a, double b, int alphaCutsCount) => Add(a, new FuzzyNumber(b), alphaCutsCount);

    public static FuzzyNumber Add(double a, FuzzyNumber b, int alphaCutsCount) => Add(new FuzzyNumber(a), b, alphaCutsCount);

    /// <summary>
    /// Subtracts two fuzzy numbers
    /// </summary>
    /// <param name="alphaCutsCount">The number of alpha-cuts for the results</param>
    public static FuzzyNumber Subtract(FuzzyNumber a, FuzzyNumber b, int alphaCutsCount)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(a, b, (alphaCutA, alphaCutB) => alphaCutA - alphaCutB, alphaCutsCount);
    }

    public static FuzzyNumber Subtract(FuzzyNumber a, double b, int alphaCutsCount) => Subtract(a, new FuzzyNumber(b), alphaCutsCount);
    
    public static FuzzyNumber Subtract(double a, FuzzyNumber b, int alphaCutsCount) => Subtract(new FuzzyNumber(a), b, alphaCutsCount);

    /// <summary>
    /// Multiplies two fuzzy numbers
    /// </summary>
    /// <param name="alphaCutsCount">The number of alpha-cuts for the results</param>
    public static FuzzyNumber Multiply(FuzzyNumber a, FuzzyNumber b, int alphaCutsCount)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(a, b, (alphaCutA, alphaCutB) => alphaCutA * alphaCutB, alphaCutsCount);
    }

    public static FuzzyNumber Multiply(FuzzyNumber a, double b, int alphaCutsCount) => Multiply(a, new FuzzyNumber(b), alphaCutsCount);

    public static FuzzyNumber Multiply(double a, FuzzyNumber b, int alphaCutsCount) => Multiply(new FuzzyNumber(a), b, alphaCutsCount);

    /// <summary>
    /// Divides two fuzzy numbers
    /// </summary>
    /// <param name="alphaCutsCount">The number of alpha-cuts for the results</param>
    /// <exception cref="DivideByZeroException">Thrown when the divisor (the second fuzzy number) contains zero.</exception>
    public static FuzzyNumber Divide(FuzzyNumber a, FuzzyNumber b, int alphaCutsCount)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(a, b, (alphaCutA, alphaCutB) => alphaCutA / alphaCutB, alphaCutsCount);
    }

    public static FuzzyNumber Divide(FuzzyNumber a, double b, int alphaCutsCount) => Divide(a, new FuzzyNumber(b), alphaCutsCount);

    public static FuzzyNumber Divide(double a, FuzzyNumber b, int alphaCutsCount) => Divide(new FuzzyNumber(a), b, alphaCutsCount);

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
