using System;
using System.Collections.Generic;

namespace CSC482_Lab0x05_BigInteger
{
    class FibSandbox
    {
        public MyBigInteger FibLoop(int x)
        {
            
            if (x < 2) return new MyBigInteger(x.ToString());

            MyBigInteger fib1 = new MyBigInteger("0");
            MyBigInteger fib2 = new MyBigInteger("1");

            // Iterate from 2->x and calculate each proceeding term of the fib
            // sequence until we have calculated the final xth term
            for (int i = 2; i <= x; i++)
            {
                MyBigInteger nextFib = fib1 + fib2;
                fib1 = fib2;
                fib2 = nextFib;
            }

            // Return xth fibonacci.
            return fib2;
        }

        public void FibLoopDoublingCalc(AlgStats10x algStats)
        {
            // Only calculate doubling ratios for power of 10 values of n and if n/10 exists in prev times table.
            if (algStats.x % 10 == 0 && algStats.PrevTimesTable.TryGetValue(algStats.x / 10, out double oneTenthTime))
            {
                algStats.Actual10xRatio = (algStats.TimeMicro / oneTenthTime);
                algStats.Expected10xRatio = Math.Pow(algStats.x, 2) / Math.Pow((double)algStats.x / 10, 2);
                algStats.ActualPlus1Ratio = algStats.Actual10xRatio;
                algStats.ExpectedPlus1Ratio =
                    Math.Pow(10, 2 * algStats.digitCount) / Math.Pow(10, 2 * (algStats.digitCount - 1));
            }
            else
            {
                algStats.Actual10xRatio = -1;
                algStats.Expected10xRatio = -1;
                algStats.ActualPlus1Ratio = -1;
                algStats.ExpectedPlus1Ratio = -1;
            }
        }

        public MyBigInteger FibMatrix(int x)
        {
            var multiplier = new MyBigInteger[2, 2] {{new MyBigInteger("1"), new MyBigInteger("1")}, {new MyBigInteger("1"), new MyBigInteger("0")}};
            var result = CalculatePowerMatrix(multiplier, x);

            // The way I implemented calculate power matrix results in the fibonacci value
            // stored in the bottom right corner of matrix rather than top left.
            return result[1, 1];
        }

        public void FibMatrixDoublingCalc(AlgStats10x algStats)
        {
            // Only calculate doubling ratios for power of 10 values of n and if n/10 exists in prev times table.
            if (algStats.x % 10 == 0 && algStats.PrevTimesTable.TryGetValue(algStats.x / 10, out double oneTenthTime))
            {
                algStats.Actual10xRatio = (algStats.TimeMicro / oneTenthTime);
                algStats.Expected10xRatio = Math.Log2(algStats.x) * Math.Pow(algStats.x, 2) / (Math.Log2((double)algStats.x / 10) * Math.Pow(((double)algStats.x / 10),2));
                algStats.ActualPlus1Ratio = algStats.Actual10xRatio;
                algStats.ExpectedPlus1Ratio =
                    Math.Pow(10, 2 * algStats.digitCount) / Math.Pow(10, 2 * (algStats.digitCount - 1));
            }
            else
            {
                algStats.Actual10xRatio = -1;
                algStats.Expected10xRatio = -1;
                algStats.ActualPlus1Ratio = -1;
                algStats.ExpectedPlus1Ratio = -1;
            }
        }

        // Similar to the fast calculate power int
        private MyBigInteger[,] CalculatePowerMatrix(MyBigInteger[,] m, long y)
        {
            //Calculates m^y
            if (y == 0) return new MyBigInteger[2, 2]
                {{new MyBigInteger("1"), new MyBigInteger("1")}, {new MyBigInteger("1"), new MyBigInteger("0")}};

            var doublePower = m;
            var result = new MyBigInteger[2, 2]
                {{new MyBigInteger("1"), new MyBigInteger("1")}, {new MyBigInteger("1"), new MyBigInteger("0")}};

            // while there are digits left to process
            while (y > 0)
            {
                // if current low-order digit is 1
                if ((y & 1) > 0)
                {
                    // multiply current matrix by the value of the doublePower matrix
                    result = MultiplyMatrices(result, doublePower);
                }

                // move next digit to low order position
                y >>= 1;

                // just like with ints, the power can be doubled for each 'bit position' in the 
                // y value. this will cut down on the number of matrix multiplications required.
                //13 ->  b:     1            1           0 <- skip 1
                //x^13 = M^8 * (1)  * M^4 * (1) * M^2 * (0) * M * (1)
                doublePower = MultiplyMatrices(doublePower, doublePower);
            }

            return result;
        }

        private MyBigInteger[,] MultiplyMatrices(MyBigInteger[,] A, MyBigInteger[,] B)
        {
            var result = new MyBigInteger[2,2]
                {{new MyBigInteger("0"), new MyBigInteger("0")}, {new MyBigInteger("0"), new MyBigInteger("0")}};
            // iterate through A's rows
            for (int i = 0; i < A.GetLength(0); i++)
            {
                // iterate B's columns
                for (int j = 0; j < B.GetLength(1); j++)
                {
                    // iterate B's rows
                    for (int k = 0; k < B.GetLength(0); k++)
                    {
                        // sum product of each element in A's row and B's column
                        result[i,j] += A[i,k] * B[k,j];
                    }
                }
            }

            return result;
        }

        public void RunTimeTests()
        {
            var benchmarker = new AlgorithmBenchmarker10x();
            benchmarker.AddAlgorithmToBenchmark(FibLoop, FibLoopDoublingCalc);
            benchmarker.AddAlgorithmToBenchmark(FibMatrix, FibMatrixDoublingCalc);

            benchmarker.RunTimeTests();
        }

        public bool RunVerificationTests()
        {
            var twentyFifthFib = new MyBigInteger("75025");

            if (FibLoop(25) != twentyFifthFib) return false;
            if (FibMatrix(25) != twentyFifthFib) return false;

            return true;
        }
    }
}
