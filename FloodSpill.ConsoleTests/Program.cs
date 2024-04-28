using FloodSpill.Queues;
using FloodSpill.Utilities;
using System;
using System.Diagnostics;

namespace FloodSpill.ConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {
            //NoWalls_Fifo();

            SpillFlood_BigAreaWithVerticalWalls_ScanlineAndFifoPerformanceComparison();
        }
        public static void NoWalls_Fifo()
        {
            var markMatrix = new int[10, 5];
            var floodParameters = new FloodParameters(startX: 1, startY: 1);

            new FloodSpiller().SpillFlood(floodParameters, markMatrix);

            string representation = MarkMatrixVisualiser.Visualise(markMatrix);
            Console.WriteLine(representation);
        }

        public static void SpillFlood_BigAreaWithVerticalWalls_ScanlineAndFifoPerformanceComparison()
        {
            int size = 1000;
            var result = new int[size, size];

            var walkability = new bool[size, size];
            // vertical walls should be OPTIMAL for scanline because it skips opening nodes when going vertically
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    bool isWalkable = x % 2 == 0 || y == size / 2;
                    walkability[x, y] = isWalkable;
                }
            }

            var startPosition = new Position(size / 5, size / 5);

            Predicate<int, int> qualifier = (x, y) => walkability[x, y];
            var parameters = new FloodParameters(new LifoPositionQueue(), startPosition.X, startPosition.Y)
            {
                Qualifier = qualifier
            };

            var stopwatch = Stopwatch.StartNew();
            new FloodScanlineSpiller().SpillFlood(parameters, result);
            Console.WriteLine("scanline:" + stopwatch.ElapsedMilliseconds + " ms");
            stopwatch.Restart();
            new FloodSpiller().SpillFlood(parameters, result);
            Console.WriteLine("normal:" + stopwatch.ElapsedMilliseconds + " ms");
        }

    }
}
