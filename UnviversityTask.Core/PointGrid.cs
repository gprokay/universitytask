using System;
using System.Drawing;
using System.Linq;

namespace UniversityTask.Core
{
    public class PointGrid
    {
        private int maxCoord;

        public Point[] Points { get; }
        public int QIndex { get; }

        public PointGrid(Point[] points, int qIndex, int maxCoord)
        {
            Points = points;
            QIndex = qIndex;
            this.maxCoord = maxCoord;
        }

        public bool TryFindSmallestBoundingBox_Naive(out Point p1, out Point p2, out int stepCount)
        {
            var q = Points[QIndex];
            p1 = default;
            p2 = default;
            stepCount = 0;

            var rect1 = new Rectangle(0, 0, q.X, q.Y);
            var rect2 = new Rectangle(q.X + 1, q.Y + 1, maxCoord - q.X, maxCoord - q.Y);
            var p1Count = rect1.Width * rect1.Height;
            var p2Count = rect2.Width * rect2.Height;

            for (int i = 0; i < p1Count; ++i)
            {
                for (int j = 0; j < p2Count; ++j)
                {
                    stepCount++;
                    p1 = GetPoint(rect1, i);
                    p2 = GetPoint(rect2, j);
                    if (IsGoodSolution(p1, p2, q))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool TryFindSmallestBoundingBox_BackTrack(out Point p1, out Point p2, out int stepCount)
        {
            p1 = default;
            p2 = default;
            stepCount = 0;

            var q = Points[QIndex];

            var rect1 = new Rectangle(0, 0, q.X, q.Y);
            var rect2 = new Rectangle(q.X + 1, q.Y + 1, maxCoord - q.X, maxCoord - q.Y);
            var rects = new[] { rect1, rect2 };
            var results = new int[2];

            if (q.X == 0 || q.Y == 0 || q.X == maxCoord || q.Y == maxCoord)
            {
                return false;
            }

            var level = 0;

            var p1Count = rect1.Width * rect1.Height;
            var p2Count = rect2.Width * rect2.Height;
            var levelCount = new[] { p1Count, p2Count };
            var isGoodSolution = false;

            while (results[0] < p1Count && !(isGoodSolution = IsGoodSolution(rects, results)))
            {
                stepCount++;
                if (!IsGoodForLevel(level, results, rects))
                {
                    if (results[level] < levelCount[level] - 1)
                    {
                        results[level]++;
                        continue;
                    }
                    else
                    {
                        if (level > 0)
                        {
                            results[level] = 0;
                            level--;
                            results[level]++;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if (level < 1)
                    {
                        level++;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (isGoodSolution)
            {
                p1 = GetPoint(rects[0], results[0]);
                p2 = GetPoint(rects[1], results[1]);
            }

            return results[0] < p1Count && isGoodSolution;
        }

        private bool IsGoodForLevel(int level, int[] result, Rectangle[] rects)
        {
            if (level == 0)
            {
                var r = GetPoint(rects[level], result[level]);
                var q = Points[QIndex];
                var isBad = Points.Any(p => p.X > r.X && p.Y > r.Y && p.X < q.X && p.Y < q.Y);
                return !isBad;
            }
            else if (level == 1)
            {
                return IsGoodSolution(rects, result);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(level));
            }
        }

        private bool IsGoodSolution(Rectangle[] rects, int[] results)
        {
            var p1 = GetPoint(rects[0], results[0]);
            var p2 = GetPoint(rects[1], results[1]);
            var q = Points[QIndex];
            return IsGoodSolution(p1, p2, q);
        }

        private bool IsGoodSolution(Point p1, Point p2, Point q)
        {
            var hasPointInside = Points.Any(p => p.X > p1.X && p.Y > p1.Y && p.X < p2.X && p.Y < p2.Y && p != q);
            var topCheck = Points.Count(p => p.Y == p1.Y && p.X >= p1.X && p.X <= p2.X) == 1;
            var bottomCheck = Points.Count(p => p.Y == p2.Y && p.X >= p1.X && p.X <= p2.X) == 1;
            var leftCheck = Points.Count(p => p.X == p1.X && p.Y >= p1.Y && p.Y <= p2.Y) == 1;
            var rightCheck = Points.Count(p => p.X == p2.X && p.Y >= p1.Y && p.Y <= p2.Y) == 1;
            return !hasPointInside && topCheck && bottomCheck && leftCheck && rightCheck;
        }

        private static Point GetPoint(Rectangle rect, int index)
        {
            var p = rect.Width > 0
                ? new Point(index % rect.Width + rect.X, index / rect.Width + rect.Y)
                : new Point(0, 0);
            return p;
        }
    }
}
