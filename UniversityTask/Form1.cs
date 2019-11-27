using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UniversityTask.Core;

namespace UniversityTask
{
    public partial class MainForm : Form
    {
        private const int PointCount = 50;
        private const int MaxCoord = 40;

        private readonly Random random;

        private readonly Pen grayThinPen = new Pen(Color.LightGray, 1);
        private readonly Pen blackThinPen = new Pen(Color.Black, 1);
        private readonly Pen blackThickPen = new Pen(Color.Black, 2);

        private readonly Brush blackBrush = new SolidBrush(Color.Black);
        private readonly Brush blueBrush = new SolidBrush(Color.LightBlue);
        private readonly Brush whiteBrush = new SolidBrush(Color.White);

        private Rectangle? result;
        private PointGrid grid;
        private int naiveCount;
        private int count;

        private MainForm(Random random)
        {
            this.random = random;
            InitializeComponent();
            SetupGrid();
            GetResult();
        }

        public MainForm() : this(new Random()) { }

        public MainForm(int seed) : this(new Random(seed)) { }

        private void SetupGrid()
        {
            var points = GetPoints(PointCount, 0, MaxCoord).ToArray();
            var qIndex = random.Next(0, PointCount);

            grid = new PointGrid(points, qIndex, MaxCoord);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ReCalculate();
        }

        private void ReCalculate()
        {
            SetupGrid();
            GetResult();

            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var size = Math.Min(ClientSize.Width, ClientSize.Height) - 10;

            var step = size / MaxCoord;

            size = step * MaxCoord;

            var offsetX = (ClientSize.Width - size) / 2;
            var offsetY = (ClientSize.Height - size) / 2;


            e.Graphics.Clear(BackColor);
            e.Graphics.FillRectangle(whiteBrush, offsetX, offsetY, step * MaxCoord, step * MaxCoord);

            for (int i = 0; i <= MaxCoord; ++i)
            {
                e.Graphics.DrawLine(grayThinPen, offsetX + step * i, offsetY, offsetX + step * i, offsetY + step * MaxCoord);
                e.Graphics.DrawLine(grayThinPen, offsetX, offsetY + step * i, offsetX + step * MaxCoord, offsetY + step * i);
            }

            for (int i = 0; i < grid.Points.Length; ++i)
            {
                var p = grid.Points[i];
                var x = p.X * step + offsetX - 5;
                var y = p.Y * step + offsetY - 5;

                if (i == grid.QIndex)
                {
                    e.Graphics.FillEllipse(blueBrush, x, y, 10, 10);
                    e.Graphics.DrawEllipse(blackThinPen, x, y, 10, 10);
                }
                else
                {
                    e.Graphics.FillEllipse(blackBrush, x, y, 10, 10);
                }
            }

            if (result.HasValue)
            {
                e.Graphics.DrawRectangle(blackThickPen, new Rectangle(result.Value.X * step + offsetX, result.Value.Y * step + offsetY, result.Value.Width * step, result.Value.Height * step));
            }

            e.Graphics.DrawString($"{naiveCount} / {count}", Font, blackBrush, 5, 5);
        }

        private void GetResult()
        {
            result = null;
            if (grid.TryFindSmallestBoundingBox_BackTrack(out var p1, out var p2, out var count))
            {
                result = new Rectangle(p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);
            }

            if (grid.TryFindSmallestBoundingBox_Naive(out _, out _, out var naiveCount))
            {
            }

            if (naiveCount > this.naiveCount)
            {
                this.naiveCount = naiveCount;
            }


            if (count > this.count)
            {
                this.count = count;
            }
        }

        private HashSet<Point> GetPoints(int count, int minCoord, int maxCoord)
        {
            var points = new HashSet<Point>(count);

            while (points.Count < count)
            {
                var p = new Point(random.Next(minCoord, maxCoord + 1), random.Next(minCoord, maxCoord + 1));
                points.Add(p);
            }

            return points;
        }
    }
}
