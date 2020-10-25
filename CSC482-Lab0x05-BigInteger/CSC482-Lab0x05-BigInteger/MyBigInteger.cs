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
        public MyBigInteger Times(MyBigInteger valB)
        {
            // Got some guidance from (https://www.geeksforgeeks.org/multiply-large-numbers-represented-as-strings/)
            // I want to spend some time coming up with a new solution before attempting the fast multiplication algorithm

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

            int thisIndx = 0;
            for (int i = GetLength() - 1; i >= 0; i--)
            {
                int carry = 0;
                int digA = AsciiToDigit(this.GetDigitChar(i));

                int valBindx = 0;
                for (int j = GetLength() - 1; j >= 0; j--)
                {
                    int digB = AsciiToDigit(valB.GetDigitChar(j));

                    int sum = digA * digB + multiplicationSum[thisIndx + valBindx] + carry;

                    carry = sum / 10;

                    multiplicationSum[thisIndx + valBindx] = sum % 10;

                    // Multiply each digit in b by the current digit of this (a)
                    valBindx++;
                }

                if (carry > 0)
                {
                    multiplicationSum[thisIndx + valBindx] += carry;
                }

                thisIndx++;
            }
            // Count leading 0 in unused section of result
            int zeroCount = 0;
            while (multiplicationSum[multiplicationSum.Length - zeroCount - 1] == 0)
            {
                zeroCount++;
            }

            for (int i = multiplicationSum.Length - zeroCount - 1; i >= 0; i--)
            {
                result.Append(DigitToAscii(multiplicationSum[i]));
            }

            return new MyBigInteger(result.ToString());
        }

        public int CompareTo(MyBigInteger other)
        {
            if (this._value.Equals(other._value) && this._negative == other._negative) return 0;

            // Need to think about implementing other comparisons if we ever need to sort etc...
            else return -1;
        }

        public override string ToString()
        {
            return this._value.ToString();
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

        private List<int> B10StringToB32BigInt(string value)
        {
            int b10Multiplier = 1;
            int currentB32Digit = 0;
            List<int> b32Value = new List<int>();

            for (int i = value.Length - 1; i >= 0; i--, b10Multiplier *= 10)
            {
                
            }

            return b32Value;
        }
    }
}
