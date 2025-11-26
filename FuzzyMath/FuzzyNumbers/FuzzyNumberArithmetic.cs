using Holecek.FuzzyMath.Intervals;

namespace Holecek.FuzzyMath.FuzzyNumbers;

public static class FuzzyNumberArithmetic
{
    /// <summary>
    /// Adds two fuzzy numbers
    /// </summary>
    public static FuzzyNumber Add(FuzzyNumber a, FuzzyNumber b) => ApplyOperation(a, b, IntervalArithmetic.Add);

    public static FuzzyNumber Add(FuzzyNumber a, double b) => ApplyOperation(a, b, IntervalArithmetic.Add);

    public static FuzzyNumber Add(double a, FuzzyNumber b) => ApplyOperation(a, b, IntervalArithmetic.Add);

    public static FuzzyNumber Add(FuzzyNumber a, FuzzyNumber b, int alphaCutsCount) => ApplyOperation(a, b, IntervalArithmetic.Add, alphaCutsCount);

    public static FuzzyNumber Add(FuzzyNumber a, double b, int alphaCutsCount) => ApplyOperation(a, b, IntervalArithmetic.Add, alphaCutsCount);

    public static FuzzyNumber Add(double a, FuzzyNumber b, int alphaCutsCount) => ApplyOperation(a, b, IntervalArithmetic.Add, alphaCutsCount);

    /// <summary>
    /// Subtracts two fuzzy numbers
    /// </summary>
    public static FuzzyNumber Subtract(FuzzyNumber a, FuzzyNumber b) => ApplyOperation(a, b, IntervalArithmetic.Subtract);

    public static FuzzyNumber Subtract(FuzzyNumber a, double b) => ApplyOperation(a, b, IntervalArithmetic.Subtract);

    public static FuzzyNumber Subtract(double a, FuzzyNumber b) => ApplyOperation(a, b, IntervalArithmetic.Subtract);

    public static FuzzyNumber Subtract(FuzzyNumber a, FuzzyNumber b, int alphaCutsCount) => ApplyOperation(a, b, IntervalArithmetic.Subtract, alphaCutsCount);

    public static FuzzyNumber Subtract(FuzzyNumber a, double b, int alphaCutsCount) => ApplyOperation(a, b, IntervalArithmetic.Subtract, alphaCutsCount);

    public static FuzzyNumber Subtract(double a, FuzzyNumber b, int alphaCutsCount) => ApplyOperation(a, b, IntervalArithmetic.Subtract, alphaCutsCount);

    /// <summary>
    /// Multiplies two fuzzy numbers
    /// </summary>
    public static FuzzyNumber Multiply(FuzzyNumber a, FuzzyNumber b) => ApplyOperation(a, b, IntervalArithmetic.Multiply);

    public static FuzzyNumber Multiply(FuzzyNumber a, double b) => ApplyOperation(a, b, IntervalArithmetic.Multiply);

    public static FuzzyNumber Multiply(double a, FuzzyNumber b) => ApplyOperation(a, b, IntervalArithmetic.Multiply);

    public static FuzzyNumber Multiply(FuzzyNumber a, FuzzyNumber b, int alphaCutsCount) => ApplyOperation(a, b, IntervalArithmetic.Multiply, alphaCutsCount);

    public static FuzzyNumber Multiply(FuzzyNumber a, double b, int alphaCutsCount) => ApplyOperation(a, b, IntervalArithmetic.Multiply, alphaCutsCount);

    public static FuzzyNumber Multiply(double a, FuzzyNumber b, int alphaCutsCount) => ApplyOperation(a, b, IntervalArithmetic.Multiply, alphaCutsCount);

    /// <summary>
    /// Divides two fuzzy numbers
    /// </summary>
    /// <exception cref="DivideByZeroException">Thrown when the divisor (the second fuzzy number) contains zero.</exception>
    public static FuzzyNumber Divide(FuzzyNumber a, FuzzyNumber b) => ApplyOperation(a, b, IntervalArithmetic.Divide);

    public static FuzzyNumber Divide(FuzzyNumber a, double b) => ApplyOperation(a, b, IntervalArithmetic.Divide);

    public static FuzzyNumber Divide(double a, FuzzyNumber b) => ApplyOperation(a, b, IntervalArithmetic.Divide);

    public static FuzzyNumber Divide(FuzzyNumber a, FuzzyNumber b, int alphaCutsCount) => ApplyOperation(a, b, IntervalArithmetic.Divide, alphaCutsCount);

    public static FuzzyNumber Divide(FuzzyNumber a, double b, int alphaCutsCount) => ApplyOperation(a, b, IntervalArithmetic.Divide, alphaCutsCount);

    public static FuzzyNumber Divide(double a, FuzzyNumber b, int alphaCutsCount) => ApplyOperation(a, b, IntervalArithmetic.Divide, alphaCutsCount);

    /// <summary>
    /// Negation of an interval (the sign is changed).
    /// </summary>
    public static FuzzyNumber Negation(FuzzyNumber a) => ApplyOperation(a, IntervalArithmetic.Negation);

    public static FuzzyNumber Negation(FuzzyNumber a, int alphaCutsCount) => ApplyOperation(a, IntervalArithmetic.Negation, alphaCutsCount);

    /// <summary>
    /// For a fuzzy number A, it returns 1/A.
    /// </summary>
    /// <exception cref="DivideByZeroException">Thrown when the fuzzy number contains zero.</exception>
    public static FuzzyNumber Reciprocal(FuzzyNumber a) => ApplyOperation(a, IntervalArithmetic.Reciprocal);

    public static FuzzyNumber Reciprocal(FuzzyNumber a, int alphaCutsCount) => ApplyOperation(a, IntervalArithmetic.Reciprocal, alphaCutsCount);

    // The following are helper methods to cover all combinations of FuzzyNumber and double inputs with or without the specification
    // of the number alpha-cuts for the result.
    private static FuzzyNumber ApplyOperation(FuzzyNumber a, Func<Interval, Interval> alphaCutsUnaryOperation)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(a, alphaCutsUnaryOperation);
    }

    private static FuzzyNumber ApplyOperation(FuzzyNumber a, FuzzyNumber b, Func<Interval, Interval, Interval> alphaCutsBinaryOperation)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(a, b, alphaCutsBinaryOperation);
    }

    private static FuzzyNumber ApplyOperation(double a, FuzzyNumber b, Func<Interval, Interval, Interval> alphaCutsBinaryOperation)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(new FuzzyNumber(a), b, alphaCutsBinaryOperation, b.AlphaCuts.Length);
    }

    private static FuzzyNumber ApplyOperation(FuzzyNumber a, double b, Func<Interval, Interval, Interval> alphaCutsBinaryOperation)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(a, new FuzzyNumber(b), alphaCutsBinaryOperation, a.AlphaCuts.Length);
    }

    private static FuzzyNumber ApplyOperation(FuzzyNumber a, Func<Interval, Interval> alphaCutsUnaryOperation, int alphaCutsCount)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(a, alphaCutsUnaryOperation, alphaCutsCount);
    }

    private static FuzzyNumber ApplyOperation(FuzzyNumber a, FuzzyNumber b, Func<Interval, Interval, Interval> alphaCutsBinaryOperation, int alphaCutsCount)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(a, b, alphaCutsBinaryOperation, alphaCutsCount);
    }

    private static FuzzyNumber ApplyOperation(double a, FuzzyNumber b, Func<Interval, Interval, Interval> alphaCutsBinaryOperation, int alphaCutsCount)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(new FuzzyNumber(a), b, alphaCutsBinaryOperation, alphaCutsCount);
    }

    private static FuzzyNumber ApplyOperation(FuzzyNumber a, double b, Func<Interval, Interval, Interval> alphaCutsBinaryOperation, int alphaCutsCount)
    {
        return FuzzyNumber.FromFuzzyNumberOperation(a, new FuzzyNumber(b), alphaCutsBinaryOperation, alphaCutsCount);
    }
}
