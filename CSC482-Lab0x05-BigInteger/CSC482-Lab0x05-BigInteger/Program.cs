using System;

namespace CSC482_Lab0x05_BigInteger
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test big int functions
            var bigInt = new MyBigInteger(1);
            if (bigInt.RunVerificationTests())
            {
                Console.WriteLine("BigInt verification Tests Passed!");
                bigInt.RunTimeTests();
            }
            else
            {
                Console.WriteLine("One or more BigInt verification tests failed.");
            }

            // Test fib functions with bigInt
            var sandbox = new FibSandbox();
            if (sandbox.RunVerificationTests())
            {
                Console.WriteLine("BigFibonacci Verification Tests Passed!");
                sandbox.RunTimeTests();
            }
            else
            {
                Console.WriteLine("One or more BigFibonacci verification tests failed.");
            }
        }
    }
}
