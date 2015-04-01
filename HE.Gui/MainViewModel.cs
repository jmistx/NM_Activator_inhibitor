using System;
using System.Data;
using System.Windows.Input;
using ExampleLibrary;
using ExpressionEvaluator;
using HE.Logic;
using Microsoft.TeamFoundation.MVVM;
using OxyPlot;
using OxyPlot.Axes;

namespace HE.Gui
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            CalculateCommand = new RelayCommand(Calculate);
            PopulateFirstExampleCommand = new RelayCommand(PopulateFirstExample);
            PopulateSecondExampleCommand = new RelayCommand(PopulateSecondExample);
            RightBoundary = 1;
            EndTime = 0.7;
            NumberOfTimeIntervals = 100;
            NumberOfSpaceIntervals = 10;
            LeftBoundaryCondition = "0.0";
            RightBoundaryCondition = "0.0";
            InitialCondition = "0.0";
            Function = "0.0";
            InitMatrixModel();
        }

        public string Function { get; set; }

        public ICommand CalculateCommand { get; set; }
        public ICommand PopulateSecondExampleCommand { get; set; }
        public ICommand PopulateFirstExampleCommand { get; set; }

        public double LeftBoundary { get; set; }
        public double RightBoundary { get; set; }
        public double EndTime { get; set; }
        public int NumberOfSpaceIntervals { get; set; }
        public int NumberOfTimeIntervals { get; set; }
        public string LeftBoundaryCondition { get; set; }
        public string RightBoundaryCondition { get; set; }
        public string InitialCondition { get; set; }

        public DataView LastLayer { get; set; }

        public PlotModel MatrixModel { get; set; }

        private void InitMatrixModel()
        {
            MatrixModel = new PlotModel();
            PlotModel plotModel1 = MatrixModel;
            var linearAxis1 = new LinearAxis();
            linearAxis1.EndPosition = 0;
            linearAxis1.StartPosition = 1;
            plotModel1.Axes.Add(linearAxis1);
            var linearAxis2 = new LinearAxis();
            linearAxis2.Position = AxisPosition.Bottom;
            plotModel1.Axes.Add(linearAxis2);
            var matrixSeries1 = new MatrixSeries();
            matrixSeries1.ShowDiagonal = true;
            matrixSeries1.Matrix = new Double[3, 3];
            matrixSeries1.Matrix[0, 0] = 1;
            matrixSeries1.Matrix[0, 1] = 0;
            matrixSeries1.Matrix[0, 2] = 0;
            matrixSeries1.Matrix[1, 0] = 0;
            matrixSeries1.Matrix[1, 1] = 2;
            matrixSeries1.Matrix[1, 2] = 0;
            matrixSeries1.Matrix[2, 0] = 0;
            matrixSeries1.Matrix[2, 1] = 0;
            matrixSeries1.Matrix[2, 2] = 3;
            plotModel1.Series.Add(matrixSeries1);
        }

        private void PopulateSecondExample()
        {
            LeftBoundary = 0;
            RightBoundary = 1;

            EndTime = 0.7;

            NumberOfSpaceIntervals = 10;
            NumberOfTimeIntervals = 100;

            LeftBoundaryCondition = "0.0";
            RightBoundaryCondition = "0.0";

            InitialCondition = "0.0";
            Function = "m.Sin(m.PI * p.x)";

            RaisePropertyChanged(null);
        }

        private void PopulateFirstExample()
        {
            LeftBoundary = 0;
            RightBoundary = 1;

            EndTime = 0.7;

            NumberOfSpaceIntervals = 10;
            NumberOfTimeIntervals = 100;

            LeftBoundaryCondition = "0.0";
            RightBoundaryCondition = "0.0";

            InitialCondition = "m.Sin(m.PI * p.x)";
            Function = "0.0";

            RaisePropertyChanged(null);
        }

        private void Calculate()
        {
            var solver = new HeatEquationSolver
            {
                LeftBoundary = LeftBoundary,
                RightBoundary = RightBoundary,
                LeftBoundCondition = Parser.ParseTimeArgMethod(LeftBoundaryCondition),
                RightBoundCondition = Parser.ParseTimeArgMethod(RightBoundaryCondition),
                StartCondition = Parser.ParsePositionArgMethod(InitialCondition),
                Function = Parser.ParseTwoArgsMethod(Function)
            };
            EquationSolveAnswer answer = solver.Solve(EndTime, NumberOfSpaceIntervals, NumberOfTimeIntervals);
            LastLayer = Populate(answer);
            RaisePropertyChanged(null);
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


    public class TwoArgs
    {
        public double x { get; set; }
        public double t { get; set; }
    }

    public class TimeArg
    {
        public double t { get; set; }
    }

    public class PositionArg
    {
        public double x { get; set; }
    }

    public class Parser
    {
        public static Func<double, double> ParsePositionArgMethod(string textExpression)
        {
            var typeRegistry = new TypeRegistry();
            var param = new PositionArg();
            typeRegistry.RegisterType("m", typeof (Math));
            typeRegistry.RegisterSymbol("p", param);

            var expression = new CompiledExpression<double>(textExpression)
            {
                TypeRegistry = typeRegistry
            };

            Func<double, double> f = x =>
            {
                param.x = x;
                return expression.Eval();
            };
            return f;
        }

        public static Func<double, double> ParseTimeArgMethod(string textExpression)
        {
            var typeRegistry = new TypeRegistry();
            var param = new TimeArg();
            typeRegistry.RegisterType("m", typeof (Math));
            typeRegistry.RegisterSymbol("p", param);

            var expression = new CompiledExpression<double>(textExpression)
            {
                TypeRegistry = typeRegistry
            };

            Func<double, double> f = t =>
            {
                param.t = t;
                return expression.Eval();
            };
            return f;
        }

        public static Func<double, double, double> ParseTwoArgsMethod(string textExpression)
        {
            var typeRegistry = new TypeRegistry();
            var param = new TwoArgs();
            typeRegistry.RegisterType("m", typeof (Math));
            typeRegistry.RegisterSymbol("p", param);

            var expression = new CompiledExpression<double>(textExpression)
            {
                TypeRegistry = typeRegistry
            };

            Func<double, double, double> f = (x, t) =>
            {
                param.x = x;
                param.t = t;
                return expression.Eval();
            };
            return f;
        }
    }
}