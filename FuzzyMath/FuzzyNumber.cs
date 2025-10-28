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

        /// <summary>
        /// A trapezoidal fuzzy number (supportMin, kernelMin, kernelMax, supportMax)
        /// </summary>
        public FuzzyNumber(double supportMin, double kernelMin, double kernelMax, double supportMax)
        {
            var support = new Interval(supportMin, supportMax);
            var kernel = new Interval(kernelMin, kernelMax);
            AlphaCuts = new Interval[] { support, kernel };
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

        private static void ThrowIfAlphaCutsAreInvalid(IList<Interval> alphaCuts)
        {
            if (alphaCuts == null)
            {
                throw new ArgumentNullException(nameof(alphaCuts));
            }

            if (alphaCuts.Count < 2)
            {
                throw new ArgumentException("Alpha-cuts list must contain at least 2 elements (the support and the kernel).", nameof(alphaCuts));
            }

            for (int i = 1; i < alphaCuts.Count; i++)
            {
                if (!alphaCuts[i - 1].Contains(alphaCuts[i], tolerance: 0))
                {
                    throw new ArgumentException($"Invalid alpha-cut value ({alphaCuts[i]}). Each alpha-cut must be a subset of the previous one ({alphaCuts[i - 1]} in this case).", nameof(alphaCuts));
                }
            }
        }
    }
}
