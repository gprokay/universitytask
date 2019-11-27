using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UniversityTask.Core;

namespace UniversityTask.Bench
{
    public class Program
    {
        private PointGrid grid;

        public int PointCount = 75;

        public int MaxCoord = 75;

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(0);
            grid = new PointGrid(GetPoints(random, PointCount, 0, MaxCoord).ToArray(), random.Next(0, PointCount), MaxCoord);
        }

        [Benchmark]
        public void BackTrack()
        {
            grid.TryFindSmallestBoundingBox_BackTrack(out _, out _, out _);
        }

        [Benchmark]
        public void NaiveSearch()
        {
            grid.TryFindSmallestBoundingBox_Naive(out _, out _, out _);
        }

        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Program>();
        }

        private HashSet<Point> GetPoints(Random random, int count, int minCoord, int maxCoord)
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
