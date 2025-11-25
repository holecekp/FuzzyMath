using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Holecek.FuzzyMath
{
    /// <summary>
    /// A piece-wise linear fuzzy number
    /// </summary>
    public class FuzzyNumber
    {
        public Interval[] AlphaCuts { get; protected set; }
        public Interval Support => AlphaCuts.First();
        public Interval Kernel => AlphaCuts.Last();

        /// <summary>
        /// A trapezoidal fuzzy number (supportMin, kernelMin, kernelMax, supportMax)
        /// </summary>
        public FuzzyNumber(double supportMin, double kernelMin, double kernelMax, double supportMax)
        {
            var support = new Interval(supportMin, supportMax);
            var kernel = new Interval(kernelMin, kernelMax);
            AlphaCuts = new Interval[] { support, kernel };

            AlphaCutsHelper.ThrowIfAlphaCutsAreInvalid(
                AlphaCuts,
                errorMessageTemplateForInvalidAlphaCut: "The constructor parameters don't define a valid fuzzy number. Kernel {0} must be a subset of the support {1}");
        }

        /// <summary>
        /// A triangular fuzzy number (supportMin, kernel, supportMax)
        /// </summary>
        public FuzzyNumber(double supportMin, double kernel, double supportMax)
            : this(supportMin, kernel, kernel, supportMax) { }

        /// <summary>
        /// A fuzzy number representing a closed interval [supportMin, supportMax]
        /// </summary>
        public FuzzyNumber(double supportMin, double supportMax)
            : this(supportMin, supportMin, supportMax, supportMax) { }

        /// <summary>
        /// A fuzzy number representing a crisp value x
        /// </summary>
        public FuzzyNumber(double x)
            : this(x, x) { }

        /// <summary>
        /// A piece-wise linear fuzzy number. The first alpha-cut is the support, the last one is the kernel.
        /// </summary>
        public FuzzyNumber(IList<Interval> alphaCuts)
        {
            AlphaCutsHelper.ThrowIfAlphaCutsAreInvalid(alphaCuts);

            var alphaCutsClone = new Interval[alphaCuts.Count];
            alphaCuts.CopyTo(alphaCutsClone, 0);

            AlphaCuts = alphaCutsClone;
        }

        /// <summary>
        /// Returns an alpha-cut
        /// </summary>
        /// <param name="alpha">Value between 0 and 1</param>
        public Interval GetAlphaCut(double alpha)
        {
            if (alpha == 0)
            {
                return Support;
            }

            if (alpha == 1)
            {
                return Kernel;
            }

            if (alpha < 0 || alpha > 1 || double.IsNaN(alpha))
            {
                throw new ArgumentOutOfRangeException(nameof(alpha), "Alpha must a number be between 0 a 1.");
            }

            double alphaCutsSteps = 1.0 / (double)(AlphaCuts.Length - 1);

            int lowerAlphaCutIndex = (int)Math.Floor(alpha / alphaCutsSteps);
            int higherAlphaCutIndex = lowerAlphaCutIndex + 1;

            double lowerAlpha = (double)lowerAlphaCutIndex * alphaCutsSteps;
            double ratio = (alpha - lowerAlpha) / alphaCutsSteps;

            Interval lowerAlphaCut = AlphaCuts[lowerAlphaCutIndex];
            Interval higherAlphaCut = AlphaCuts[higherAlphaCutIndex];

            double resultMin = lowerAlphaCut.Min + ratio * (higherAlphaCut.Min - lowerAlphaCut.Min);
            double resultMax = lowerAlphaCut.Max - ratio * (lowerAlphaCut.Max - higherAlphaCut.Max);

            return new Interval(resultMin, resultMax);
        }

        /// <summary>
        /// Returns the membership degree for a given element x.
        /// </summary>
        /// <returns>Membership degree of the element x (a number between 0 and 1)</returns>
        public double GetMembership(double x)
        {
            if (!Support.Contains(x))
            {
                return 0;
            }

            if (Kernel.Contains(x))
            {
                return 1;
            }

            int lowerAlphaCutIndex = AlphaCutsHelper.GetHighestAlphaCutIndexContainingValue(AlphaCuts, x);
            int higherAlphaCutIndex = lowerAlphaCutIndex + 1;

            Interval lowerAlphaCut = AlphaCuts[lowerAlphaCutIndex];
            Interval higherAlphaCut = AlphaCuts[higherAlphaCutIndex];

            double alphaStep = 1 / (double)(AlphaCuts.Length - 1);
            double lowerAlpha = lowerAlphaCutIndex * alphaStep;

            double ratio;
            if (x < higherAlphaCut.Min)
            {
                ratio = (x - lowerAlphaCut.Min) / (higherAlphaCut.Min - lowerAlphaCut.Min);
            }
            else
            {
                ratio = (lowerAlphaCut.Max - x) / (lowerAlphaCut.Max - higherAlphaCut.Max);
            }

            return lowerAlpha + ratio * alphaStep;
        }

        /// <summary>
        /// Creates a copy of this fuzzy number with a different number of alpha-cuts. The minimum number of alpha-cuts is 2.
        /// </summary>
        /// <remarks>
        /// This is especially useful if you need to perform operations on multiple fuzzy numbers, which requires all of them to have the same
        /// number of alpha-cuts.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">The exception is thrown if the new nuber of alpha-cuts is less than 2.</exception>
        public FuzzyNumber WithAlphaCutsCount(int newAlphaCutsCount)
        {
            if (newAlphaCutsCount < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(newAlphaCutsCount), "The number of alpha-cuts must be at least 2.");
            }

            if (newAlphaCutsCount == AlphaCuts.Length)
            {
                return new FuzzyNumber(AlphaCuts);
            }

            var alphaCuts = new Interval[newAlphaCutsCount];
            for (int i = 0; i < newAlphaCutsCount; i++)
            {
                double alpha = AlphaCutsHelper.GetAlphaForAlphaCutIndex(i, newAlphaCutsCount);
                alphaCuts[i] = GetAlphaCut(alpha);
            }

            return new FuzzyNumber(alphaCuts);
        }

        public bool IsEqualTo(FuzzyNumber other, double tolerance)
        {
            if (other == null)
            {
                return false;
            }

            for (int i = 0; i < AlphaCuts.Length; i++)
            {
                double alpha = AlphaCutsHelper.GetAlphaForAlphaCutIndex(i, AlphaCuts.Length);
                Interval otherFuzzyNumberAlphaCut = other.GetAlphaCut(alpha);
                if (!AlphaCuts[i].IsEqualTo(otherFuzzyNumberAlphaCut, tolerance))
                {
                    return false;
                }
            }

            // If the number of alpha-cuts is different, check the remaining alpha-cuts of the other fuzzy number
            // Skip the first and the last alpha-cuts, because the support and kernel has already been cheched.
            if (AlphaCuts.Length != other.AlphaCuts.Length)
            {
                for (int i = 1; i < other.AlphaCuts.Length - 1; i++)
                {
                    double alpha = AlphaCutsHelper.GetAlphaForAlphaCutIndex(i, other.AlphaCuts.Length);
                    Interval thisFuzzyNumberAlphaCut = GetAlphaCut(alpha);
                    if (!other.AlphaCuts[i].IsEqualTo(thisFuzzyNumberAlphaCut, tolerance))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Creates a new fuzzy number by defining a function that generates its alpha-cuts.
        /// </summary>
        /// <param name="getAlphaCutFunction">A function that returns an alpha-cut for the given alpha value (from 0 to 1).
        /// If the returned alpha-cut is not a subinterval of the previous one (which can occur due to the limited
        /// accuracy of floating-point operations), the alpha-cut will be automatically adjusted to satisfy this condition.</param>
        /// <param name="alphaCutsCount">The number of alpha-cuts for the resulting fuzzy number.</param>
        public static FuzzyNumber FromAlphaCutFunction(Func<double, Interval> getAlphaCutFunction, int alphaCutsCount)
        {
            var alphaCuts = new Interval[alphaCutsCount];

            for (int i = 0; i < alphaCutsCount; i++)
            {
                double alpha = AlphaCutsHelper.GetAlphaForAlphaCutIndex(i, alphaCutsCount);
                alphaCuts[i] = getAlphaCutFunction(alpha);
            }

            AlphaCutsHelper.AdjustsAlphaCutsInListToSatisfySubintervalCondition(alphaCuts);
            return new FuzzyNumber(alphaCuts);
        }

        /// <summary>
        /// Applies a unary operation to a fuzzy number. A new fuzzy number is created by applying the given operation to alpha-cuts
        /// of the original fuzzy number.
        /// </summary>
        /// <param name="input">The original fuzzy number.</param>
        /// <param name="alphaCutsUnaryOperation">A unary operation that, given an alpha-cut (a specific interval) of the original fuzzy number,
        /// returns the modified alpha-cut.</param>
        /// <param name="alphaCutsCount">The number of alpha-cuts for the resulting fuzzy number.</param>
        public static FuzzyNumber FromFuzzyNumberOperation(FuzzyNumber input, Func<Interval, Interval> alphaCutsUnaryOperation, int alphaCutsCount)
        {
            return FromAlphaCutFunction(
                alpha => alphaCutsUnaryOperation(input.GetAlphaCut(alpha)),
                alphaCutsCount);
        }

        /// <summary>
        /// Applies a binary operation on two fuzzy numbers. A new fuzzy number is created by applying the given operation to alpha-cuts
        /// of the original fuzzy numbers.
        /// </summary>
        /// <param name="firstInput">The first input fuzzy number</param>
        /// <param name="secondInput">The second input fuzzy number</param>
        /// <param name="alphaCutsBinaryOperation">A binary operation that, given two corresponding alpha-cuts from the first and second input fuzzy
        /// numbers, returns the alpha-cut for the resulting fuzzy number.</param>
        /// <param name="alphaCutsCount">The number of alpha-cuts for the resulting fuzzy number.</param>
        public static FuzzyNumber FromFuzzyNumberOperation(
            FuzzyNumber firstInput,
            FuzzyNumber secondInput,
            Func<Interval, Interval, Interval> alphaCutsBinaryOperation,
            int alphaCutsCount)
        {
            return FromAlphaCutFunction(
                alpha => alphaCutsBinaryOperation(firstInput.GetAlphaCut(alpha), secondInput.GetAlphaCut(alpha)),
                alphaCutsCount);
        }
    }
}
