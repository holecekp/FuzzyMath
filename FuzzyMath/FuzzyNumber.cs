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
                double alpha = i / (double)(newAlphaCutsCount - 1);
                alphaCuts[i] = GetAlphaCut(alpha);
            }

            return new FuzzyNumber(alphaCuts);
        }

        /// <summary>
        /// Creates a new fuzzy number by defining a function that generates its alpha-cuts.
        /// </summary>
        /// <param name="alphaCutCount">The number of alpha-cuts for the resulting fuzzy number.</param>
        /// <param name="getAlphaCutFunction">A function that returns an alpha-cut for a given index  and alpha value.
        /// The function receives two parameters: the index of the alpha-cut (from 0 to alphaCutCount - 1) and the corresponding
        /// alpha value (from 0 to 1). If the returned alpha-cut is not a subinterval of the previous one (which can occur due to the limited
        /// accuracy of floating-point operations), the alpha-cut will be automatically adjusted to satisfy this condition.</param>
        public static FuzzyNumber FromAlphaCutFunction(int alphaCutCount, Func<int, double, Interval> getAlphaCutFunction)
        {
            double alphaCutsStep = 1.0 / (double)(alphaCutCount - 1);
            var alphaCuts = new Interval[alphaCutCount];

            for (int i = 0; i < alphaCutCount; i++)
            {
                double alpha = i * alphaCutsStep;
                var alphaCut = getAlphaCutFunction(i, alpha);

                // Each alpha-cut must be a subinterval of the previous one. If this condition doesn't hold,
                // trim the alpha-cut so that the condition wouldn't be broken.
                if (i > 0)
                {
                    var previousAlphaCut = alphaCuts[i - 1];
                    if (!previousAlphaCut.Contains(alphaCut, tolerance: 0))
                    {
                        alphaCut = alphaCut.RestrictTo(previousAlphaCut);
                    }
                }

                alphaCuts[i] = alphaCut;
            }

            return new FuzzyNumber(alphaCuts);
        }

        /// <summary>
        /// Applies a unary operation to a fuzzy number. A new fuzzy number is created by applying the given operation to each alpha-cut
        /// of the original fuzzy number.
        /// </summary>
        /// <remarks>
        /// The resulting fuzzy number will have the same number of alpha-cuts as the original fuzzy number.
        /// </remarks>
        /// <param name="input">The original fuzzy number.</param>
        /// <param name="alphaCutsUnaryOperation">A unary operation that, given an alpha-cut (a specific interval) of the original fuzzy number,
        /// returns the modified alpha-cut.</param>
        public static FuzzyNumber FromFuzzyNumberOperation(FuzzyNumber input, Func<Interval, Interval> alphaCutsUnaryOperation)
        {
            return FromAlphaCutFunction(
                input.AlphaCuts.Length,
                (alphaCutIndex, alpha) => alphaCutsUnaryOperation(input.AlphaCuts[alphaCutIndex]));
        }

        /// <summary>
        /// Applies a binary operation on two fuzzy numbers. A new fuzzy number is created by applying the given operation to each alpha-cut
        /// of the original fuzzy numbers.
        /// </summary>
        /// <remarks>
        /// The two input fuzzy numbers are not required to have the same number of alpha-cuts. If they do, the resulting fuzzy number will
        /// also have the same number of alpha-cuts. If they have a different number of alpha-cuts, the value of <paramref name="fallbackAlphaCutsCount"/>
        /// will be used as the number of alpha-cuts for the resulting fuzzy number.
        /// </remarks>
        /// <param name="firstInput">The first input fuzzy number</param>
        /// <param name="secondInput">The second input fuzzy number</param>
        /// <param name="alphaCutsBinaryOperation">A binary operation that, given two corresponding alpha-cuts from the first and second input fuzzy
        /// numbers, returns the alpha-cut for the resulting fuzzy number.</param>
        /// <param name="fallbackAlphaCutsCount">The number of alpha-cuts for the resulting fuzzy number when the first and second input fuzzy numbers
        /// have a different number of alpha-cuts.</param>
        public static FuzzyNumber FromFuzzyNumberOperation(
            FuzzyNumber firstInput,
            FuzzyNumber secondInput,
            Func<Interval, Interval, Interval> alphaCutsBinaryOperation,
            int fallbackAlphaCutsCount = 60)
        {
            if (firstInput.AlphaCuts.Length != secondInput.AlphaCuts.Length)
            {
                firstInput = firstInput.WithAlphaCutsCount(fallbackAlphaCutsCount);
                secondInput = secondInput.WithAlphaCutsCount(fallbackAlphaCutsCount);
            }

            return FromAlphaCutFunction(
                firstInput.AlphaCuts.Length,
                (alphaCutIndex, alpha) => alphaCutsBinaryOperation(firstInput.AlphaCuts[alphaCutIndex], secondInput.AlphaCuts[alphaCutIndex]));
        }
    }
}
