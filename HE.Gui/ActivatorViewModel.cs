using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ExampleLibrary;
using HE.Logic;
using Microsoft.TeamFoundation.MVVM;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace HE.Gui
{
    internal class ActivatorViewModel : ViewModelBase
    {
        public ActivatorViewModel()
        {
            CalculateCommand = new RelayCommand(ComputeUntilTime);
            PopulateFirstExampleCommand = new RelayCommand(PopulateFirstExample);
            ApplyLagSizeCommand = new RelayCommand(ApplyLagSize);
            SingleStepCommand = new RelayCommand(SingleStep);
            SetTimeStepCommand = new RelayCommand(SetTimeStep);
            PrepareComputationCommand = new RelayCommand(PrepareComputation);
            //InitialCondition = new List<InitialHarmonic>();
            InitialCondition = new ObservableCollection<InitialHarmonic>();
            EquationSolver = new ActivatorEquationSolver();
            ApplyTresholdsCommand = new RelayCommand(RecreatePlot);


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

        //public List<InitialHarmonic> InitialCondition { get; set; }
        public ObservableCollection<InitialHarmonic> InitialCondition { get; set; }

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

        public double SnapshotTimeStep { get; set; }

        public double ActivatorTreshold { get; set; }

        public double InhibitorTreshold { get; set; }

        public ICommand ApplyTresholdsCommand { get; set; }

        public bool InterpolatePlot { get; set; }

        public bool FixedTimeScale { get; set; }

        public double TimeScaleValue { get; set; }

        public int SnapshotSize { get; set; }

        public int LagSize { get; set; }

        public double LastActivatorLayerDifferenceWithLag { get; set; }
        public double LastInhibitorLayerDifferenceWithLag { get; set; }

        public ICommand ApplyLagSizeCommand { get; set; }

        public double LastActivatorLayerDifference { get { return EquationSolver.LastActivatorLayerDifference; } }
        public double LastInhibitorLayerDifference { get { return EquationSolver.LastInhibitorLayerDifference; } }


        private void PrepareComputation()
        {
            EquationSolver.N = IntervalsX;
            EquationSolver.InittialConditionU1 = InitialCondition.Select(s => s.ActivatorValue).ToArray();
            EquationSolver.InittialConditionU2 = InitialCondition.Select(s => s.InhibitorValue).ToArray();
            EquationSolver.SnapshotSize = SnapshotSize;
            EquationSolver.SnapshotTimeStep = SnapshotTimeStep;
            
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
            SnapshotTimeStep = 0.1;
            ActivatorTreshold = 13;
            InhibitorTreshold = 1000;
            SnapshotSize = 100;
            TimeScaleValue = 5;
            FixedTimeScale = false;

            foreach (InitialHarmonic harmonic in InitialCondition)
            {
                harmonic.ActivatorValue = 0;
                harmonic.InhibitorValue = 0;
            }

            InitialCondition[0].ActivatorValue = 1;
            InitialCondition[0].InhibitorValue = 0.5;
            CollectionViewSource.GetDefaultView(InitialCondition).Refresh();

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
            LastActivatorLayerDifferenceWithLag = EquationSolver.ActivatorDifferenceWithLag;
            LastInhibitorLayerDifferenceWithLag = EquationSolver.InhibitorDifferenceWithLag;
            RecreatePlot();
        }

        private void ApplyLagSize()
        {
            EquationSolver.LagSize = LagSize;
            LastActivatorLayerDifferenceWithLag = -1;
            LastInhibitorLayerDifferenceWithLag = -1;
            RaisePropertyChanged(null);
        }

        private void RecreatePlot()
        {
            MatrixModel = new PlotModel();

            var linearAxis1 = new LinearAxis();
            MatrixModel.Axes.Add(linearAxis1);

            var linearAxis2 = new LinearAxis {Position = AxisPosition.Bottom};
            MatrixModel.Axes.Add(linearAxis2);

            var matrixSeries1 = new MatrixSeries();
            matrixSeries1.TresholdValue1 = ActivatorTreshold;
            matrixSeries1.TresholdValue2 = InhibitorTreshold;

            matrixSeries1.Timeline1 = EquationSolver.ActivatorTimeLine;
            matrixSeries1.Timeline2 = EquationSolver.InhibitorTimeLine;
            matrixSeries1.MaxTime =  EquationSolver.ActualSnapshotTime;
            if (FixedTimeScale)
            {
                linearAxis1.Maximum = TimeScaleValue;
            }
            
            matrixSeries1.Interpolate = InterpolatePlot;

            MatrixModel.Series.Add(matrixSeries1);
            RaisePropertyChanged(null);
        }

        private DataView Populate(double[] answer)
        {
            return BindingHelper.ArrayToDataView(answer);
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