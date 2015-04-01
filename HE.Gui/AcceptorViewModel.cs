using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.TeamFoundation.MVVM;
using OxyPlot;

namespace HE.Gui
{
    internal class AcceptorViewModel : ViewModelBase
    {
        public PlotModel MatrixModel { get; set; }

        public double Lambda1 { get; set; }

        public double Lambda2 { get; set; }

        public double Rho { get; set; }

        public double Kappa { get; set; }

        public double Gamma { get; set; }

        public double Nu { get; set; }

        public double C { get; set; }

        public double TimeStep { get; set; }

        public double IntervalsX { get; set; }

        public double EndMomentT { get; set; }

        public double[] LastActivatorLayer { get; set; }

        public double[] LastInhibitorLayer { get; set; }

        public List<InitialHarmonic> InitialCondition { get; set; }

        public ICommand CalculateCommand { get; set; }

        public AcceptorViewModel()
        {
            InitialCondition = new List<InitialHarmonic>();
            CalculateCommand = new RelayCommand(Caluclate);

            for (int i = 0; i < 20; i++)
            {
                InitialCondition.Add(new InitialHarmonic(i));
            }
        }

        private void Caluclate()
        {
        }
    }

    internal class InitialHarmonic
    {
        public InitialHarmonic(int i)
        {
            Index = i;
            Value = 0;
        }

        public double Value { get; set; }

        public int Index { get; set; }
    }
}