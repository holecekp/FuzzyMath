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

            ThrowIfAlphaCutsAreInvalid(
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
            ThrowIfAlphaCutsAreInvalid(alphaCuts);

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

            int lowerAlphaCutIndex = GetHighestAlphaCutIndexContainingValue(x);
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

        public static FuzzyNumber FromAlphaCutFunction(int alphaCutCount, Func<int, double, Interval> getAlphaCutFunction)
        {
            double alphaCutsStep = 1.0 / (double)(alphaCutCount - 1);
            var alphaCuts = new Interval[alphaCutCount];

            for (int i = 0; i < alphaCutCount; i++)
            {
                double alpha = i * alphaCutsStep;
                var alphaCut = getAlphaCutFunction(i, alpha);

                // Each alpha-cut must be a subinterval of the previous one. If this condition doesn't hold,
                // modify the alpha-cut so that the condition wouldn't be broken. This can happen because
                // of limited accuracy of floating point numbers operations.
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

        public static FuzzyNumber FromFuzzyNumberOperation(FuzzyNumber fuzzyNumber, Func<Interval, Interval> alphaCutsUnaryOperation)
        {
            return FromAlphaCutFunction(
                fuzzyNumber.AlphaCuts.Length,
                (alphaCutIndex, alpha) => alphaCutsUnaryOperation(fuzzyNumber.AlphaCuts[alphaCutIndex]));
        }

        public static FuzzyNumber FromFuzzyNumberOperation(
            FuzzyNumber fuzzyNumber,
            FuzzyNumber fuzzyNumber2,
            Func<Interval, Interval, Interval> alphaCutsBinaryOperation,
            int fallbackAlphaCutsCount = 60)
        {
            if (fuzzyNumber.AlphaCuts.Length != fuzzyNumber2.AlphaCuts.Length)
            {
                fuzzyNumber = fuzzyNumber.WithAlphaCutsCount(fallbackAlphaCutsCount);
                fuzzyNumber2 = fuzzyNumber2.WithAlphaCutsCount(fallbackAlphaCutsCount);
            }

            return FromAlphaCutFunction(
                fuzzyNumber.AlphaCuts.Length,
                (alphaCutIndex, alpha) => alphaCutsBinaryOperation(fuzzyNumber.AlphaCuts[alphaCutIndex], fuzzyNumber2.AlphaCuts[alphaCutIndex]));
        }

        private static void ThrowIfAlphaCutsAreInvalid(
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

        private int GetHighestAlphaCutIndexContainingValue(double value)
        {
            if (!Support.Contains(value))
            {
                throw new InvalidOperationException("No alpha-cut contains the value.");
            }

            for (int i = 1; i < AlphaCuts.Length; i++)
            {
                if (!AlphaCuts[i].Contains(value))
                {
                    return i - 1;
                }
            }

            return 0;
        }
    }
}
