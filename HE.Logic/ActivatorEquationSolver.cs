using System;

namespace HE.Logic
{
    public class ActivatorEquationSolveAnswer
    {
        public double[] ActivatorLayer { get; set; }
        public double[] InhibitorLayer { get; set; }
    }

    public class ActivatorEquationSolver
    {
        private const int InitialConditionComponentsCount = 20;

        public ActivatorEquationSolver()
        {
            InittialConditionU1 = new double[InitialConditionComponentsCount];
            InittialConditionU2 = new double[InitialConditionComponentsCount];
            InittialConditionU1[0] = 1;
            InittialConditionU2[0] = 0.5;
        }

        public double Lambda1 { get; set; }
        public double Lambda2 { get; set; }
        public double Ro { get; set; }
        public double Kappa { get; set; }
        public double Gamma { get; set; }
        public double C { get; set; }
        public double Nyu { get; set; }
        public double Length { get; set; }
        public double Time { get; set; }

        public double[] InittialConditionU2 { get; set; }

        public double[] InittialConditionU1 { get; set; }

        public ActivatorEquationSolveAnswer Solve()
        {
            int n = 100;
            double m = 100000;

            var activatorLayer = new double[n + 1];
            var inhibitorLayer = new double[n + 1];
            var activatorLayerNext = new double[n + 1];
            var inhibitorLayerNext = new double[n + 1];

            double h = Length/n;
            double k = Time/m;

            for (int i = 0; i < n + 1; i++)
            {
                double x = i * h;
                activatorLayer[i] = GetInitValueActivator(x);
                inhibitorLayer[i] = GetInitValueInhibitor(x);
            }

            for (int t = 0; t < m + 1; t++)
            {
                for (int i = 1; i < n; i++)
                {
                    activatorLayerNext[i] = activatorLayer[i] + k * (Lambda1 * diff(activatorLayer, i, h) 
                                             + Ro +
                                             Kappa * activatorLayer[i] * activatorLayer[i] / inhibitorLayer[i] -
                                             Gamma * activatorLayer[i]);
                    inhibitorLayerNext[i] = inhibitorLayer[i] + k * (Lambda2 * diff(inhibitorLayer, i, h)
                    +
                                             C * (activatorLayer[i] * activatorLayer[i]) -
                                             Nyu * inhibitorLayer[i]);
                }
                activatorLayerNext[0] = activatorLayerNext[1];
                activatorLayerNext[n] = activatorLayerNext[n-1];
                inhibitorLayerNext[0] = inhibitorLayerNext[1];
                inhibitorLayerNext[n] = inhibitorLayerNext[n - 1];

                Swap(ref activatorLayer, ref activatorLayerNext);
                Swap(ref inhibitorLayer, ref inhibitorLayerNext);
            }


            return new ActivatorEquationSolveAnswer
            {
                ActivatorLayer = activatorLayer,
                InhibitorLayer = inhibitorLayer
            };
        }

        private void Swap(ref double[] activatorLayer, ref double[] activatorLayerNext)
        {
            var swapLayer = activatorLayer;
            activatorLayer = activatorLayerNext;
            activatorLayerNext = swapLayer;
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

        private double GetInitialConditionValue(double[] inittialCondition, double x)
        {
            double y = 0;
            for (int i = 0; i < InitialConditionComponentsCount; i++)
            {
                y += inittialCondition[i]*Math.Cos(Math.PI*i*x);
            }
            return y;
        }
    }
}