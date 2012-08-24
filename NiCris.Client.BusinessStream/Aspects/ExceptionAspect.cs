using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostSharp.Aspects;

namespace NiCris.Client.BusinessStream.Aspects
{
    [Serializable]
    public class ExceptionAspect : OnExceptionAspect
    {
        public override void OnException(MethodExecutionArgs args)
        {
            Console.WriteLine(String.Format("Exception in :[{0}] , Message:[{1}]", args.Method, args.Exception.Message));
            args.FlowBehavior = FlowBehavior.Continue;

            base.OnException(args);
        }
    }
}
