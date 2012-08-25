using NiCris.Client.BusinessStream.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace NiCris.Client.BusinessStream
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            FirstMethod();
            SecondMethod();

            Console.WriteLine("\nPress any key to continue with BizMsgAspect");
            Console.ReadKey();

            var someObj = new SomeObject(314, "pi");
            ThirdMethod(123, someObj);

            // Console.WriteLine("\nPress any key to exit");
            // Console.ReadKey();
        }

        [LogAspect]
        [ExceptionAspect]
        static void FirstMethod()
        {
            throw new DivideByZeroException("An Error Occured...");
        }

        [LogAspect]
        [TimingAspect]
        [SimpleTraceAttribute]
        static void SecondMethod()
        {
            Thread.Sleep(100);
            SecondMethodEx();
        }
        static void SecondMethodEx()
        {
            Thread.Sleep(200);
        }


        [TimingAspect]
        [ExceptionAspect]
        [BizMsgAspect("BizMsg_Name", "SomeUser", AppId = "AppID:100")]
        static void ThirdMethod(int someInt, SomeObject someObj)
        {
            Thread.Sleep(1000);
            throw new Exception("Test Exception Msg.");
        }

    }

    public class SomeObject
    {
        public int XYZ { get; private set; }
        public string ABC { get; private set; }

        public SomeObject(int xyz, string abc)
        {
            this.XYZ = xyz;
            this.ABC = abc;
        }
    }
}
