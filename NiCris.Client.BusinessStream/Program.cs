using NiCris.Client.BusinessStream.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NiCris.Client.BusinessStream
{
    class Program
    {
        static void Main(string[] args)
        {
            FirstMethod();
            SecondMethod();

            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        [ExceptionAspect]
        [LogAspect]
        static void FirstMethod()
        {
            throw new DivideByZeroException("An Error Occured...");
        }

        [LogAspect]
        [TimingAspect]
        static void SecondMethod()
        {
            Thread.Sleep(1000);
        }
    }
}
