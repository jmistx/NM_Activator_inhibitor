using System;
using HE.Logic;
using NUnit.Framework;

namespace HE.Test
{
    [TestFixture]
    public class EquationSolverShould
    {
        private const double Pi = Math.PI;
        private readonly Func<double, double> sin = Math.Sin;
        private readonly Func<double, double> exp = Math.Exp;

        [Test]
        public void SolveTestExample()
        {
            var solver = new HeatEquationSolver
            {
                StartCondition = x => sin(Pi * x),
            };

            const int n = 20000;
            const int k = 200;
            var answer1 = solver.Solve(0.001, n, k);

            Func<double, double, double> exactAnswer = (x, t) => sin(Pi * x) * exp(-Pi * Pi * t);
            var maximumOfDifference = Compute.MaximumOfDifference(answer1, exactAnswer);
            Expect.FloatsAreEqual(0, maximumOfDifference);
        }

        [Test]
        public void SolveTestExample3()
        {
            var solver = new HeatEquationSolver
            {
                LeftBoundCondition = t => 6,
                RightBoundCondition = t => 1
            };

            const int n = 20;
            const int k = 200;
            var answer1 = solver.Solve(10, n, k);

            Func<double, double, double> exactAnswer = (x, t) => 6 + (-5) * x;
            var maximumOfDifference = Compute.MaximumOfDifference(answer1, exactAnswer);
            Expect.FloatsAreEqual(0, maximumOfDifference);
        }

        [Test]
        public void SolveTestExample2WithSecondOrderOfConvergenceForSpaceStep()
        {
            var solver = new HeatEquationSolver {Function = (x, t) => 1000 * sin(Pi*x) };
            Func<double, double, double> exactAnswer = (x, t) => 1000 * sin(Pi * x) * (1 - exp(-Pi * Pi * t)) * (1 / (Pi * Pi));

            Expect.OrderOfConvergenceIs(
                orderOfConvergence: 2, 
                forFunction: param => solver.Solve(timeOfEnd: 0.7, spaceIntervals: param, timeIntervals: 2000), 
                toFunction: exactAnswer, 
                startParameter: 60);
        }

        [Test]
        public void SolveTestExampleWithSecondOrderOfConvergenceForSpaceStep()
        {
            var solver = new HeatEquationSolver { StartCondition = x => 1000 * sin(Pi * x) };
            Func<double, double, double> exactAnswer = (x, t) => 1000 * sin(Pi * x) * exp(-Pi * Pi * t);

            Expect.OrderOfConvergenceIs(
                orderOfConvergence: 2, 
                forFunction: param => solver.Solve(timeOfEnd: 0.001, spaceIntervals: param, timeIntervals: 2000), 
                toFunction: exactAnswer, 
                startParameter: 40);
        }

        [Test]
        public void SolveTestExample1WithFirstOrderOfConvergenceForTimeStep()
        {
            var solver = new HeatEquationSolver { StartCondition = x => sin(Pi * x) };
            Func<double, double, double> exactAnswer = (x, t) => sin(Pi * x) * exp(-Pi * Pi * t);

            Expect.OrderOfConvergenceIs(
                orderOfConvergence: 1, 
                forFunction: param => solver.Solve(timeOfEnd: 0.001, spaceIntervals: 200, timeIntervals: param), 
                toFunction: exactAnswer, 
                startParameter: 40);
        }

        [Test]
        public void SolveTestExample2WithFirstOrderOfConvergenceForTimeStep()
        {
            var solver = new HeatEquationSolver { Function = (x, t) => sin(Pi * x) };
            Func<double, double, double> exactAnswer = (x, t) => sin(Pi * x) * (1 - exp(-Pi * Pi * t)) * (1 / (Pi * Pi));

            Expect.OrderOfConvergenceIs(
                orderOfConvergence: 1, 
                forFunction: param => solver.Solve(timeOfEnd: 0.001, spaceIntervals: 200, timeIntervals: param), 
                toFunction: exactAnswer, 
                startParameter: 40);
        }

        [Test]
        public void SolveTrivialEquationExactly()
        {
            var solver = new HeatEquationSolver();

            const int n = 10;
            const int k = 20;
            var answer = solver.Solve(5.0, n, k);

            Assert.AreEqual(n + 1, answer.LastLayer.Length);
            foreach (double temperature in answer.LastLayer)
            {
                Assert.AreEqual(0, temperature);
            }
            Assert.AreEqual(n + 1, answer.Nodes.Length);
            Assert.AreEqual(0, answer.Nodes[0]);
            Assert.AreEqual(1, answer.Nodes[n]);
            for (int i = 0; i < n; i++)
            {
                Assert.AreEqual(answer.Nodes[i], i/(double)n);
            }
        }
    }
}