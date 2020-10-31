using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CSC482_Lab0x05_BigInteger
{
    class MyBigInteger : IComparable<MyBigInteger>
    {
        private StringBuilder _value;
        private bool _negative = false;

        public MyBigInteger(string bigInteger)
        {
            _value = new StringBuilder(bigInteger);
        }

        public MyBigInteger(int numDigits)
        {
            _value = new StringBuilder();
            Random rand = new Random();
            for (int i = 0; i < numDigits; i++)
            {
                _value.Append(rand.Next('0', '9'));
            }
        }

        public int GetLength()
        {
            return _value.Length;
        }

        private char GetDigitChar(int index)
        {
            return _value[index];
        }

        private void PadToWidth(int width)
        {
            int length = this.GetLength();
            if (length < width)
            {
                for (int i = 0; i < width - length; i++)
                {
                    _value.Insert(0, "0");
                }
            }
        }

        public void RunTimeTests()
        {
            var benchmarker = new AlgorithmBenchmarker();
            
            benchmarker.AddAlgorithmToBenchmark(Plus, PlusDoublingCalc);
            benchmarker.AddAlgorithmToBenchmark(Times, TimesDoublingCalc);

            benchmarker.RunTimeTests();
        }

        public bool RunVerificationTests()
        {
          var sumTestExpected = new MyBigInteger("1000000000000000000000111");
          var productTestExpected = new MyBigInteger("111000000000000000000000000");
          var testA = new MyBigInteger("1000000000000000000000000");
          var testB = new MyBigInteger("111");
          if (testA + testB != sumTestExpected) return false;
          if (testA * testB != productTestExpected) return false;

          return true;
        }

        public MyBigInteger Plus(MyBigInteger valA, MyBigInteger valB)
        {
            return valA + valB;
        }

        public MyBigInteger Plus(MyBigInteger valB)
        {
            int carry = 0;
            StringBuilder result = new StringBuilder();

            // Pad shorter number
            valB.PadToWidth(Math.Max(this.GetLength(), valB.GetLength()));
            this.PadToWidth(Math.Max(this.GetLength(), valB.GetLength()));

            for (int i = GetLength() - 1; i >= 0; i--)
            {
                int digA = AsciiToDigit(this.GetDigitChar(i));
                int digB = AsciiToDigit(valB.GetDigitChar(i));

                int sum = digA + digB + carry;
                int digC = sum;
                if (sum >= 10)
                {
                    carry = 1;
                    digC = sum - 10;
                }
                else
                {
                    carry = 0;
                }

                result.Insert(0, DigitToAscii(digC));
            }

            if (carry != 0) result.Insert(0, DigitToAscii(carry));
            return new MyBigInteger(result.ToString());
        }

        public void PlusDoublingCalc(AlgStats algStats)
        {
            // Only calculate doubling ratios for even values of n and if n/2 exists in prev times table.
            if (algStats.n % 2 == 0 && algStats.PrevTimesTable.TryGetValue(algStats.n / 2, out double halfTime))
            {
                algStats.ActualDoublingRatio = (algStats.TimeMicro / halfTime);
                algStats.ExpectedDoublingRatio = algStats.n / ((double)algStats.n / 2);
            }
            else
            {
                algStats.ActualDoublingRatio = -1;
                algStats.ExpectedDoublingRatio = -1;
            }
        }

        public MyBigInteger Times(MyBigInteger valB)
        {
            // Got some guidance from (https://www.geeksforgeeks.org/multiply-large-numbers-represented-as-strings/)
            // Faster than using MyBigInteger Plus() method. Keeps a running result in an array rather than creating
            // temporary big integer or string to hold result for use in Plus().

            // Handle exceptional cases that can avoid entering the loops.
            if(this.GetLength() == 0 || valB.GetLength() == 0) return new MyBigInteger("0");
            if (this.GetLength() == 1 && this.GetDigitChar(0) == '1') return new MyBigInteger(valB.ToString());
            if (valB.GetLength() == 1 && valB.GetDigitChar(0) == '1') return new MyBigInteger(this.ToString());
            if (this.GetLength() == 1 && this.GetDigitChar(0) == '0') return new MyBigInteger("0");
            if (valB.GetLength() == 1 && valB.GetDigitChar(0) == '0') return new MyBigInteger("0");

            StringBuilder result = new StringBuilder(2 * this.GetLength());

            // Pad shorter number 
            valB.PadToWidth(Math.Max(this.GetLength(), valB.GetLength()));
            this.PadToWidth(Math.Max(this.GetLength(), valB.GetLength()));

            // max width of result is 2x size of longest value, values are padded so just 2x this.Length
            int[] multiplicationSum = new int[this.GetLength() * 2];

            // Used to keep track of the correct offset since each iteration of the sum pass will
            // need to be shifted by a power of 10 so correct digits can be added.
            int power10Offset = 0;
            for (int i = GetLength() - 1; i >= 0; i--)
            {
                int carry = 0;
                int digA = AsciiToDigit(this.GetDigitChar(i));

                int currentSumDigitIdx = 0;
                for (int j = GetLength() - 1; j >= 0; j--)
                {
                    int digB = AsciiToDigit(valB.GetDigitChar(j));

                    // Keeping running total sum in multiplicationSum Array, so need to add previous value stored in
                    // multSum[thisindx + valbIndx] to current sum. Power10Offset keeps track of starting index.
                    int sum = digA * digB + multiplicationSum[power10Offset + currentSumDigitIdx] + carry;

                    carry = sum / 10;

                    multiplicationSum[power10Offset + currentSumDigitIdx] = sum % 10;

                    // Multiply and add each digit in b by the current digit of this (a)
                    currentSumDigitIdx++;
                }

                if (carry > 0)
                {
                    multiplicationSum[power10Offset + currentSumDigitIdx] += carry;
                }

                // Shift next addition by 1 digit (power of 10).
                power10Offset++;
            }
            // Count leading 0 in unused section of result
            int zeroCount = 0;
            while (multiplicationSum[multiplicationSum.Length - zeroCount - 1] == 0)
            {
                zeroCount++;
            }

            // Only build new result string such that leading zeroes are ignored.
            for (int i = multiplicationSum.Length - zeroCount - 1; i >= 0; i--)
            {
                result.Append(DigitToAscii(multiplicationSum[i]));
            }

            return new MyBigInteger(result.ToString());
        }

        public MyBigInteger Times(MyBigInteger valA, MyBigInteger valB)
        {
            return valA * valB;
        }

        public void TimesDoublingCalc(AlgStats algStats)
        {
            // Only calculate doubling ratios for even values of n and if n/2 exists in prev times table.
            if (algStats.n % 2 == 0 && algStats.PrevTimesTable.TryGetValue(algStats.n / 2, out double halfTime))
            {
                algStats.ActualDoublingRatio = (algStats.TimeMicro / halfTime);
                algStats.ExpectedDoublingRatio = Math.Pow(algStats.n, 2) / (Math.Pow((double)algStats.n / 2, 2));
            }
            else
            {
                algStats.ActualDoublingRatio = -1;
                algStats.ExpectedDoublingRatio = -1;
            }
        }

        public int CompareTo(MyBigInteger other)
        {
            if (this._value.Equals(other._value) && this._negative == other._negative) return 0;

            // Need to think about implementing other comparisons if we ever need to sort etc...
            else return -1;
        }

        public override string ToString()
        {
            if (_value.Length < 12) return _value.ToString();
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(_value, 0, 5);
                sb.Append("...");
                sb.Append(_value, _value.Length - 5, 5);

                return sb.ToString();
            }
        }

        public static MyBigInteger operator +(MyBigInteger a, MyBigInteger b)
        {
            return a.Plus(b);
        }

        public static MyBigInteger operator *(MyBigInteger a, MyBigInteger b)
        {
            return a.Times(b);
        }

        public static bool operator ==(MyBigInteger a, MyBigInteger b)
        {
            return a.CompareTo(b) == 0 ? true : false;
        }

        public static bool operator !=(MyBigInteger a, MyBigInteger b)
        {
            return a.CompareTo(b) != 0 ? true : false;
        }

        private int AsciiToDigit(char c)
        {
            return c - '0';
        }

        private char DigitToAscii(int d)
        {
            return (char)(d + '0');
        }
    }
}
