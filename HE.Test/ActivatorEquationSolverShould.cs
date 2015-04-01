using System;
using HE.Logic;
using NUnit.Framework;

namespace HE.Test
{
    [TestFixture]
    internal class ActivatorEquationSolverShould
    {
        [Test]
        public void SolveEquation()
        {
            var solver = new ActivatorEquationSolver
            {
                Rho = 1.1,
                Kappa = 1.5,
                C = 1.2,
                Gamma = 0.1,
                Nu = 0.1,
                Time = 1000,
                Lambda1 = 1,
                Lambda2 = 2,
                TimeStep = 0.1,
                N = 100
            };
            solver.InittialConditionU1[0] = 1;
            solver.InittialConditionU2[0] = 0.5;
            double alpha = (solver.Rho + solver.Kappa*solver.Nu/solver.C)/solver.Gamma;
            double beta = solver.C*alpha*alpha/solver.Nu;

            solver.ComputeUntilTime();

            Assert.That(solver.ActivatorLayer[5], Is.Not.NaN);
            Assert.That(solver.InhibitorLayer[5], Is.Not.NaN);

            Assert.That(Math.Abs(solver.ActivatorLayer[5] - alpha), Is.LessThan(0.1));
            Assert.That(Math.Abs(solver.InhibitorLayer[5] - beta), Is.LessThan(0.1));
        }
    }
}