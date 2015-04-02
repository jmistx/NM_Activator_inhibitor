using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            CalculateCommand = new RelayCommand(ComputeUntilTime);
            PopulateFirstExampleCommand = new RelayCommand(PopulateFirstExample);
            SingleStepCommand = new RelayCommand(SingleStep);
            SetTimeStepCommand = new RelayCommand(SetTimeStep);
            PrepareComputationCommand = new RelayCommand(PrepareComputation);
            InitialCondition = new List<InitialHarmonic>();
            EquationSolver = new ActivatorEquationSolver();


            for (int i = 0; i < 20; i++)
            {
                InitialCondition.Add(new InitialHarmonic(i));
            }

            SetTestParameters();
        }

        public PlotModel MatrixModel { get; set; }

        public double Lambda1
        {
            get { return EquationSolver.Lambda1; }
            set { EquationSolver.Lambda1 = value; }
        }

        public double Lambda2
        {
            get { return EquationSolver.Lambda2; }
            set { EquationSolver.Lambda2 = value; }
        }

        public double Rho
        {
            get { return EquationSolver.Rho; }
            set { EquationSolver.Rho = value; }
        }

        public double Kappa
        {
            get { return EquationSolver.Kappa; }
            set { EquationSolver.Kappa = value; }
        }

        public double Gamma
        {
            get { return EquationSolver.Gamma; }
            set { EquationSolver.Gamma = value; }
        }

        public double Nu
        {
            get { return EquationSolver.Nu; }
            set { EquationSolver.Nu = value; }
        }

        public double C
        {
            get { return EquationSolver.C; }
            set { EquationSolver.C = value; }
        }

        public double TimeStep
        {
            get { return EquationSolver.TimeStep; }
            set { EquationSolver.TimeStep = value; }
        }

        public double EndMomentT
        {
            get { return EquationSolver.Time; }
            set { EquationSolver.Time = value; }
        }

        public int IntervalsX { get; set; }

        public double[] LastActivatorLayer { get; set; }

        public double[] LastInhibitorLayer { get; set; }
        public DataView LastActivatorLayerView { get; set; }
        public DataView LastInhibitorLayerView { get; set; }

        public List<InitialHarmonic> InitialCondition { get; set; }

        public ICommand CalculateCommand { get; set; }

        public ActivatorEquationSolver EquationSolver { get; set; }

        public ICommand PopulateFirstExampleCommand { get; set; }

        public ICommand SingleStepCommand { get; set; }

        public int StepsByClickQuantity { get; set; }

        public ICommand SetTimeStepCommand { get; set; }

        public double CurrentTime
        {
            get { return EquationSolver.CurrentTime; }
        }

        public ICommand PrepareComputationCommand { get; set; }

        public DataView FirstActivatorLayerView { get; set; }

        public DataView FirstInhibitorLayerView { get; set; }

        private void PrepareComputation()
        {
            EquationSolver.N = IntervalsX;
            EquationSolver.InittialConditionU1 = InitialCondition.Select(s => s.ActivatorValue).ToArray();
            EquationSolver.InittialConditionU2 = InitialCondition.Select(s => s.InhibitorValue).ToArray();
            EquationSolver.PrepareComputation();
            FirstActivatorLayerView = Populate(EquationSolver.ActivatorLayer);
            FirstInhibitorLayerView = Populate(EquationSolver.InhibitorLayer);
            RaisePropertyChanged(null);
        }

        private void SetTimeStep()
        {
            EquationSolver.AlignTimeStep();
            RaisePropertyChanged(null);
        }

        private void SetTestParameters()
        {
            Rho = 1.1;
            Kappa = 1.5;
            C = 1.2;
            Gamma = 0.1;
            Nu = 0.1;
            Lambda1 = 1;
            Lambda2 = 2;
            TimeStep = 0.00005;
            EndMomentT = 10;
            IntervalsX = 100;
            StepsByClickQuantity = 10;

            foreach (InitialHarmonic harmonic in InitialCondition)
            {
                harmonic.ActivatorValue = 0;
                harmonic.InhibitorValue = 0;
            }

            InitialCondition[0].ActivatorValue = 1;
            InitialCondition[0].InhibitorValue = 0.5;

            RaisePropertyChanged(null);
        }

        private void ComputeUntilTime()
        {
            EquationSolver.ComputeUntilTime();
            PopulateAnswer();
        }

        private void PopulateAnswer()
        {
            LastActivatorLayerView = Populate(EquationSolver.ActivatorLayer);
            LastInhibitorLayerView = Populate(EquationSolver.InhibitorLayer);
            RaisePropertyChanged(null);
        }

        private DataView Populate(double[] answer)
        {
            return BindingHelper.GetBindableArray(answer);
        }

        private void SingleStep()
        {
            EquationSolver.MultipleSteps(StepsByClickQuantity);

            PopulateAnswer();
        }

        private void PopulateFirstExample()
        {
            SetTestParameters();
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
            ActivatorValue = 0;
            InhibitorValue = 0;
        }

        public double InhibitorValue { get; set; }

        public double ActivatorValue { get; set; }

        public int Index { get; set; }
    }
}