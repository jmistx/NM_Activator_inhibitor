using System;
using HE.Logic;
using NUnit.Framework;

namespace HE.Test
{
    public class Expect
    {
        public static void FloatsAreEqual(double expected, double actual)
        {
            if (Math.Abs(actual - expected) > 0.00001)
            {
                Assert.Fail("expected: " + expected + ", actual:" + actual);
            }
        }

        public static void OrderOfConvergenceIs(int orderOfConvergence, Func<int, EquationSolveAnswer> forFunction, Func<double, double, double> toFunction, int startParameter)
        {
            var n1 = startParameter;
            var answer1 = forFunction(n1);

            var n2 = startParameter * 10;
            var answer2 = forFunction(n2);

            var maximumOfDifference1 = Compute.MaximumOfDifference(answer1, toFunction);
            var exponent1 = GetExponent(maximumOfDifference1);

            var maximumOfDifference2 = Compute.MaximumOfDifference(answer2, toFunction);
            var exponent2 = GetExponent(maximumOfDifference2);

            Assert.AreEqual(orderOfConvergence, Math.Abs(exponent2 - exponent1));
        }

        private static double GetExponent(double maximumOfDifference1)
        {
            return Math.Floor(Math.Abs(Math.Log10(Math.Abs(maximumOfDifference1))));
        }
    }

    class Compute
    {
        public static double MaximumOfDifference(EquationSolveAnswer answer, Func<double, double, double> exactAnswer)
        {
            double maximumDifference = 0;
            for (int i = 0; i < answer.LastLayer.Length; i++)
            {
                var difference = Math.Abs(answer.LastLayer[i] - exactAnswer(answer.Nodes[i], answer.TimeOfEnd));
                if (difference > maximumDifference)
                {
                    maximumDifference = difference;
                }
            }
            return maximumDifference;
        }
    }
}