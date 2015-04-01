using System.Collections.Generic;
using System.Data;
using System.Windows.Input;
using HE.Logic;
using Microsoft.TeamFoundation.MVVM;
using OxyPlot;

namespace HE.Gui
{
    internal class ActivatorViewModel : ViewModelBase
    {
        public ActivatorViewModel()
        {
            CalculateCommand = new RelayCommand(Caluclate);
            InitialCondition = new List<InitialHarmonic>();
            EquationSolver = new ActivatorEquationSolver();

            for (int i = 0; i < 20; i++)
            {
                InitialCondition.Add(new InitialHarmonic(i));
            }

            SetTestParameters();
        }

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
        public DataView LastActivatorLayerView { get; set; }
        public DataView LastInhibitorLayerView { get; set; }

        public List<InitialHarmonic> InitialCondition { get; set; }

        public ICommand CalculateCommand { get; set; }

        public ActivatorEquationSolver EquationSolver { get; set; }

        private void SetTestParameters()
        {
            Rho = 1.1;
            Kappa = 1.5;
            C = 1.2;
            Gamma = 0.1;
            Nu = 0.1;
            Lambda1 = 1;
            Lambda2 = 2;
            RaisePropertyChanged(null);
        }

        private void Caluclate()
        {
            EquationSolver.Rho = Rho;
            EquationSolver.Kappa = Kappa;
            EquationSolver.C = C;
            EquationSolver.Gamma = Gamma;
            EquationSolver.Nu = Nu;
            EquationSolver.Lambda1 = Lambda1;
            EquationSolver.Lambda2 = Lambda2;
            EquationSolver.Time = EndMomentT;
            
            ActivatorEquationSolveAnswer answer = EquationSolver.Solve();

            LastActivatorLayerView = Populate(answer.ActivatorLayer);
            LastInhibitorLayerView = Populate(answer.InhibitorLayer);
            RaisePropertyChanged(null);
        }

        private DataView Populate(double[] answer)
        {
            return BindingHelper.GetBindableArray(answer);
        }

        private DataView Populate(EquationSolveAnswer answer)
        {
            var result = new double[2, answer.Nodes.Length];
            for (int i = 0; i < answer.Nodes.Length; i++)
            {
                result[0, i] = answer.Nodes[i];
                result[1, i] = answer.LastLayer[i];
            }
            return BindingHelper.GetBindable2DArray(result);
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