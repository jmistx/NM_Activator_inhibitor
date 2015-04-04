using System;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Series;

namespace ExampleLibrary
{
    /// <summary>
    ///     Provides a series that visualizes the structure of a matrix.
    /// </summary>
    public class MatrixSeries : XYAxisSeries
    {
        /// <summary>
        ///     The image
        /// </summary>
        private OxyImage image;

        /// <summary>
        ///     The matrix
        /// </summary>
        private double[,] matrix;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MatrixSeries" /> class.
        /// </summary>
        public MatrixSeries()
        {
            ShowDiagonal = false;
            MinimumGridLineDistance = 4;
            GridColor = OxyColors.LightGray;
            BorderColor = OxyColors.Gray;
            NotZeroColor = OxyColors.Black;
            TresholdValue = 1;
            MaxTime = 5;
            MaxCoordinate = 1;
            Interpolate = false;
            TrackerFormatString = "{0}\r\n[{1},{2}] = {3}";
        }

        public bool Interpolate { get; set; }

        public double MaxCoordinate { get; set; }

        public double MaxTime { get; set; }

        /// <summary>
        ///     Gets or sets the matrix.
        /// </summary>
        public double[,] Matrix
        {
            get { return matrix; }

            set
            {
                image = null;
                matrix = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to show the diagonal.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the diagonal should be shown; otherwise, <c>false</c>.
        /// </value>
        public bool ShowDiagonal { get; set; }

        /// <summary>
        ///     Gets or sets the minimum grid line distance.
        /// </summary>
        public double MinimumGridLineDistance { get; set; }

        /// <summary>
        ///     Gets or sets the color of the grid.
        /// </summary>
        /// <value>
        ///     The color of the grid.
        /// </value>
        public OxyColor GridColor { get; set; }

        /// <summary>
        ///     Gets or sets the color of the border around the matrix.
        /// </summary>
        /// <value>
        ///     The color of the border.
        /// </value>
        public OxyColor BorderColor { get; set; }

        /// <summary>
        ///     Gets or sets the color of the not zero elements of the matrix.
        /// </summary>
        public OxyColor NotZeroColor { get; set; }

        /// <summary>
        ///     Gets or sets the zero tolerance (inclusive).
        /// </summary>
        /// <value>
        ///     The zero tolerance.
        /// </value>
        public double TresholdValue { get; set; }

        public List<double[]> Timeline1 { get; set; }
        public List<double[]> Timeline2 { get; set; }

        /// <summary>
        ///     Gets the point on the series that is nearest the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="interpolate">Interpolate the series if this flag is set to <c>true</c>.</param>
        /// <returns>
        ///     A TrackerHitResult for the current hit.
        /// </returns>
        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            DataPoint dp = InverseTransform(point);
            var i = (int) dp.Y;
            var j = (int) dp.X;

            if (i >= 0 && i < matrix.GetLength(0) && j >= 0 && j < matrix.GetLength(1))
            {
                double value = matrix[i, j];
                string text = StringHelper.Format(
                    ActualCulture,
                    TrackerFormatString,
                    null,
                    Title,
                    i,
                    j,
                    value);
                return new TrackerHitResult(this, dp, point, null, -1, text);
            }

            return null;
        }

        /// <summary>
        ///     Renders the series on the specified render context.
        /// </summary>
        /// <param name="rc">The rendering context.</param>
        /// <param name="model">The model.</param>
        public override void Render(IRenderContext rc, PlotModel model)
        {
            if (Matrix == null)
            {
                return;
            }

            int m = Matrix.GetLength(0);
            int n = Matrix.GetLength(1);

            ScreenPoint p0 = Transform(0, MaxTime);
            ScreenPoint p1 = Transform(MaxCoordinate, 0);

            if (image == null)
            {
                var pixels = new OxyColor[m, n];
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        pixels[i, j] = OxyColor.FromAColor((byte) ((Matrix[i, n - j - 1]/TresholdValue)*255),
                            OxyColors.Black);
                    }
                }

                image = OxyImage.Create(pixels, ImageFormat.Png);
            }


            OxyRect clip = GetClippingRect();
            drawBorder(rc, clip, new DataPoint(0, MaxTime), new DataPoint(MaxCoordinate, 0));
            double x0 = p0.X;
            double y0 = p0.Y;
            double w = Math.Abs(p0.X - p1.X);
            double h = Math.Abs(p0.Y - p1.Y);
            rc.DrawClippedImage(clip, image, x0, y0, w, h, 1, Interpolate);
        }

        private void drawBorder(IRenderContext rc, OxyRect clip, DataPoint leftUp, DataPoint rightDown)
        {
            var borderPoints = new List<ScreenPoint>
            {
                Transform(leftUp.X, leftUp.Y),
                Transform(leftUp.X, rightDown.Y),
                Transform(rightDown.X, rightDown.Y),
                Transform(rightDown.X, leftUp.Y),
                Transform(leftUp.X, leftUp.Y),
            };

            rc.DrawClippedLineSegments(clip, borderPoints, OxyColors.DarkRed, 2, null, LineJoin.Miter, true);
        }

        /// <summary>
        ///     Updates the max/minimum values.
        /// </summary>
        protected override void UpdateMaxMin()
        {
            base.UpdateMaxMin();
            if (Matrix == null)
            {
                return;
            }

            MinX = 0;
            MaxX = MaxCoordinate;
            MinY = 0;
            MaxY = MaxTime;
        }
    }
}