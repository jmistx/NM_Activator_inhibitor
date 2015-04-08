﻿using System;
using System.Collections.Generic;

namespace HE.Logic
{
    public class ActivatorEquationSolver
    {
        private const int InitialConditionComponentsCount = 20;

        public ActivatorEquationSolver()
        {
            InittialConditionU1 = new double[InitialConditionComponentsCount];
            InittialConditionU2 = new double[InitialConditionComponentsCount];
            SnapshotTimeStep = 0.1;
        }

        public double[] InhibitorLayer { get; set; }
        public double[] ActivatorLayer { get; set; }

        public List<double[]> ActivatorTimeLine { get; set; }
        public List<double[]> InhibitorTimeLine { get; set; }
        public double SnapshotTimeStep { get; set; }
        public double ActualSnapshotTime { get; private set; }

        public double Lambda1 { get; set; }
        public double Lambda2 { get; set; }
        public double Rho { get; set; }
        public double Kappa { get; set; }
        public double Gamma { get; set; }
        public double C { get; set; }
        public double Nu { get; set; }

        private double Length
        {
            get { return 1; }
        }

        public double Time { get; set; }

        public double[] InittialConditionU2 { get; set; }

        public double[] InittialConditionU1 { get; set; }
        public double TimeStep { get; set; }
        public int N { get; set; }

        public double[] ActivatorLayerNext { get; set; }

        public double[] InhibitorLayerNext { get; set; }
        public double CurrentTime { get; set; }
        public int SnapshotSize { get; set; }
        public double LastLayerDifference { get; set; }

        public void ComputeUntilTime()
        {
            int m = (int) ((Time - CurrentTime)/TimeStep);
            int iterationsPerSnapshot = CountIterationsPerSnapshot(m);

            for (int i = 0; i < m; i++)
            {
                ConditionalSnapshot(i, iterationsPerSnapshot);
                SingleStep();
            }

            CurrentTime += m*TimeStep;
        }

        private void Swap()
        {
            double[] swapLayer = ActivatorLayer;
            ActivatorLayer = ActivatorLayerNext;
            ActivatorLayerNext = swapLayer;
            swapLayer = InhibitorLayer;
            InhibitorLayer = InhibitorLayerNext;
            InhibitorLayerNext = swapLayer;
        }

        private double diff(double[] u, int i, double h)
        {
            return (u[i - 1] - 2*u[i] + u[i + 1])/(h*h);
        }

        private double GetInitValueActivator(double x)
        {
            return GetInitialConditionValue(InittialConditionU1, x);
        }

        private double GetInitValueInhibitor(double x)
        {
            return GetInitialConditionValue(InittialConditionU2, x);
        }

        private double GetInitialConditionValue(double[] initialCondition, double x)
        {
            double y = 0;
            for (int i = 0; i < InitialConditionComponentsCount; i++)
            {
                y += initialCondition[i]*Math.Cos(Math.PI*i*x);
            }
            return y;
        }

        public void SingleStep()
        {
            double h = Length/N;
            double k = TimeStep;

            for (int i = 1; i < N; i++)
            {
                ActivatorLayerNext[i] = ActivatorLayer[i] + k*(Lambda1*diff(ActivatorLayer, i, h)
                                                               + Rho +
                                                               Kappa*ActivatorLayer[i]*ActivatorLayer[i]/
                                                               InhibitorLayer[i] -
                                                               Gamma*ActivatorLayer[i]);
                InhibitorLayerNext[i] = InhibitorLayer[i] + k*(Lambda2*diff(InhibitorLayer, i, h)
                                                               +
                                                               C*(ActivatorLayer[i]*ActivatorLayer[i]) -
                                                               Nu*InhibitorLayer[i]);
            }
            ActivatorLayerNext[0] = ActivatorLayerNext[1];
            ActivatorLayerNext[N] = ActivatorLayerNext[N - 1];
            InhibitorLayerNext[0] = InhibitorLayerNext[1];
            InhibitorLayerNext[N] = InhibitorLayerNext[N - 1];

            LastLayerDifference = ComputeDifference(ActivatorLayer, ActivatorLayerNext);

            Swap();
        }

        private double ComputeDifference(double[] activatorLayer, double[] activatorLayerNext)
        {
            double result = 0;
            for (int i = 1; i < activatorLayer.Length - 1; i++)
            {
                result = Math.Max(result, Math.Abs(activatorLayer[i] - activatorLayerNext[i]));
            }
            return result;
        }

        private void MakeSnapshot()
        {
            var activatorLayerCopy = CopyArrayToSnapshot(ActivatorLayer);
            var inhibitorLayerCopy = CopyArrayToSnapshot(InhibitorLayer);

            ActivatorTimeLine.Add(activatorLayerCopy);
            InhibitorTimeLine.Add(inhibitorLayerCopy);
            ActualSnapshotTime += SnapshotTimeStep;
        }

        private double[] CopyArrayToSnapshot(double[] layer)
        {
            var layerCopy = new double[SnapshotSize];
            var scale =  (double)layer.Length / (double)SnapshotSize;

            for (int i = 0; i < SnapshotSize; i++)
            {
                layerCopy[i] = layer[(int)(i * scale)];
            }

            return layerCopy;
        }

        public void AlignTimeStep()
        {
            double h = Length/N;
            TimeStep = h*h/2.0;
        }

        public void PrepareComputation()
        {
            int n = N;
            CurrentTime = 0;
            

            ActivatorLayer = new double[n + 1];
            InhibitorLayer = new double[n + 1];
            ActivatorLayerNext = new double[n + 1];
            InhibitorLayerNext = new double[n + 1];
            ActivatorTimeLine = new List<double[]>();
            InhibitorTimeLine = new List<double[]>();

            double h = Length/n;

            for (int i = 0; i < n + 1; i++)
            {
                double x = i*h;
                ActivatorLayer[i] = GetInitValueActivator(x);
                InhibitorLayer[i] = GetInitValueInhibitor(x);
            }

            MakeSnapshot();
            ActualSnapshotTime = 0;
        }

        public void MultipleSteps(int stepsByClickQuantity)
        {
            int iterationsPerSnapshot = CountIterationsPerSnapshot(stepsByClickQuantity);

            for (int i = 0; i < stepsByClickQuantity; i++)
            {
                ConditionalSnapshot(i, iterationsPerSnapshot);
                SingleStep();
            }
            CurrentTime += stepsByClickQuantity*TimeStep;
        }

        private int CountIterationsPerSnapshot(int layerQuantity)
        {
            var numberOfSnapshots = (CurrentTime + layerQuantity * TimeStep - ActualSnapshotTime) / SnapshotTimeStep;
            int iterationsPerSnapshot = (int)(layerQuantity / numberOfSnapshots);
            return iterationsPerSnapshot;
        }

        private void ConditionalSnapshot(int i, int iterationsPerSnapshot)
        {
            if ((i + 1)%iterationsPerSnapshot == 0)
            {
                MakeSnapshot();
            }
        }
    }
}