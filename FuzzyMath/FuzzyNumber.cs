using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
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
