﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HE.Logic;
using NUnit.Framework;

namespace HE.Test
{
    [TestFixture]
    class ActivatorEquationSolverShould
    {
        [Test]
        public void SolveEquation()
        {
            var solver = new ActivatorEquationSolver
            {
                Ro = 1.1,
                Kappa = 1.5,
                C = 1.2,
                Gamma = 0.1,
                Nyu = 0.1,
                Length = 1,
                Time = 1000,
                Lambda1 = 1,
                Lambda2 = 2
            };
            double alpha = (solver.Ro + solver.Kappa * solver.Nyu / solver.C ) / solver.Gamma;
            double beta = solver.C*alpha*alpha / solver.Nyu;
            var answer = solver.Solve();

            Assert.That(answer.ActivatorLayer[5], Is.Not.NaN);
            Assert.That(answer.InhibitorLayer[5], Is.Not.NaN);
            
            Assert.That(Math.Abs(answer.ActivatorLayer[5] - alpha), Is.LessThan(0.1));
            Assert.That(Math.Abs(answer.InhibitorLayer[5] - beta), Is.LessThan(0.1));
        }
    }
}