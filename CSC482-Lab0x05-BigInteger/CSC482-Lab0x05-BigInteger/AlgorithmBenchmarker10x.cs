using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CSC482_Lab0x05_BigInteger
{
    class AlgStats10x
    {
        public int x = 0;
        public int digitCount = 0;
        public Dictionary<int, double> PrevTimesTable = new Dictionary<int, double>();
        public double PrevTimeMicro = 0;
        public double TimeMicro = 0;
        public double Expected10xRatio = 0;
        public double ExpectedPlus1Ratio = 0;
        public MyBigInteger AlgResult;
        public double Actual10xRatio = 0;
        public double ActualPlus1Ratio = 0;
    }

    class AlgorithmBenchmarker10x
    {
        private const double MaxSecondsPerAlgorithm = 25;
        private const double MaxMicroSecondsPerAlg = MaxSecondsPerAlgorithm * 1000000;

        private const double MaxSecondsPerIteration = 0.5;
        private const double MaxMicroSecondsPerIteration = MaxSecondsPerIteration * 1000000;
        

        private const int NMin = 1;
        private const int NMax = 900; // 92 is max before 64-bit overflow

        private readonly Random _rand = new Random();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        // To use benchmarker, simply define the delegate with the signature of your algorithm to test and also the data
        // source it will use.
        internal delegate MyBigInteger Algorithm(int x);
        // The algorithms are responsible for implementing their own doubling ratio calculator
        internal delegate void DoublingCalculator(AlgStats10x stats);

        private List<Algorithm> _algorithms = new List<Algorithm>();
        private List<DoublingCalculator> _doublingCalculators = new List<DoublingCalculator>();

        // Called from within the scope of your algorithms instantiation, simply pass the algorithm function name
        // and the doublingcalculator function name as parameters. Call RunTimeTests to run each algorithm added
        // and display statistics based on doubling calculator.
        public void AddAlgorithmToBenchmark(Algorithm algorithm, DoublingCalculator doublingCalc)
        {
            _algorithms.Add(algorithm);
            _doublingCalculators.Add(doublingCalc);
        }

        public void RunTimeTests()
        {
            Debug.Assert(_algorithms.Count == _doublingCalculators.Count);

            for (int i = 0; i < _algorithms.Count; i++)
            {
                AlgorithmTestRuntime(_algorithms[i], _doublingCalculators[i]);
            }
        }
        private void AlgorithmTestRuntime(Algorithm algorithm, DoublingCalculator doublingCalc)
        {
            PrintHeader(algorithm);

            var currentStats = new AlgStats10x();

            int power = 1;
            for (var n = NMin; n <= NMax; n++)
            {
                for (int i = 1; i <= 9; i++)
                {
                    currentStats.x = power * i;
                    currentStats.digitCount = n;

                    if (currentStats.TimeMicro > MaxMicroSecondsPerAlg)
                    {
                        PrintAlgorithmTerminationMessage(algorithm);
                        break;
                    }

                    PrintIndexColumn(currentStats.x);

                    int testCount = 1;
                    int maxTest = 1000000;
                    long tickCounter = 0;
                    while (testCount <= maxTest && TicksToMicroseconds(tickCounter) < MaxMicroSecondsPerIteration)
                    {
                        _stopwatch.Restart();
                        currentStats.AlgResult = algorithm(currentStats.x);
                        _stopwatch.Stop();
                        tickCounter += _stopwatch.ElapsedTicks;
                        testCount++;
                    }

                    double averageTimeMicro = TicksToMicroseconds(tickCounter) / testCount;

                    currentStats.PrevTimeMicro = currentStats.TimeMicro;
                    currentStats.TimeMicro = averageTimeMicro;
                    // Need to keep a dictionary of previous times for doubling calculation on this alg.
                    currentStats.PrevTimesTable.TryAdd(currentStats.x, averageTimeMicro);

                    doublingCalc(currentStats);

                    PrintData(currentStats);

                    // New Row
                    Console.WriteLine();
                }

                power *= 10;

                if (currentStats.TimeMicro > MaxMicroSecondsPerAlg)
                {
                    break;
                }

            }
        }

        // Should be abstracted out so that column names etc, can be passed and this function doesn't have
        // to be modified in source code.
        private void PrintHeader(Algorithm algorithm)
        {
            Console.WriteLine($"Starting run-time tests for {algorithm.Method.Name}...\n");
            Console.WriteLine(
                " \t\t\t           |    |              10x  Ratios       |    |               +1  Ratios       |        |                Algorithm|   |   Input|");
            Console.WriteLine(
                "X\t\t\t       Time|    |         Actual|        Expected|    |        Actual|        Expected |        |                   Result|   |    Size|");
        }

        private void PrintAlgorithmTerminationMessage(Algorithm algorithm)
        {
            Console.WriteLine($"{algorithm.Method.Name} exceeded allotted time, terminating...\n");
        }

        private void PrintIndexColumn(int x)
        {
            Console.Write($"{x,-15}");
        }

        private void PrintData(AlgStats10x stats)
        {
            var actual10xFormatted = stats.Actual10xRatio < 0
                ? "na".PadLeft(20)
                : stats.Actual10xRatio.ToString("F2").PadLeft(20);
            var expect10xFormatted = stats.Expected10xRatio < 0
                ? "na".PadLeft(16)
                : stats.Expected10xRatio.ToString("F2").PadLeft(16);

            var actualPlus1Formatted = stats.ActualPlus1Ratio < 0
                ? "na".PadLeft(20)
                : stats.ActualPlus1Ratio.ToString("F2").PadLeft(20);
            var expectPlus1Formatted = stats.ExpectedPlus1Ratio < 0
                ? "na".PadLeft(16)
                : stats.ExpectedPlus1Ratio.ToString("F2").PadLeft(16);

            Console.Write(
                $"{stats.TimeMicro,20:F2} {actual10xFormatted} {expect10xFormatted}" +
                $"{actualPlus1Formatted} {expectPlus1Formatted}" +
                $"{stats.AlgResult,30} {stats.digitCount,12:N0}");
        }

        private static double TicksToMicroseconds(long ticks)
        {
            return (double) ticks / Stopwatch.Frequency * 1000000;
        }
    }
}
