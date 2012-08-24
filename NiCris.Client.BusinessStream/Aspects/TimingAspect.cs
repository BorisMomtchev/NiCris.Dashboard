using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostSharp.Extensibility;
using System.Diagnostics;

namespace NiCris.Client.BusinessStream.Aspects
{
    [Serializable]
    [MulticastAttributeUsage(MulticastTargets.Method)]
    public class TimingAspect : PostSharp.Aspects.OnMethodBoundaryAspect
    {
        [NonSerialized]
        Stopwatch _stopWatch;

        public override void OnEntry(PostSharp.Aspects.MethodExecutionArgs args)
        {
            _stopWatch = Stopwatch.StartNew();
            base.OnEntry(args);
        }

        public override void OnExit(PostSharp.Aspects.MethodExecutionArgs args)
        {
            Console.WriteLine(string.Format("[{0}] took {1} ms to execute",
                                            new StackTrace().GetFrame(1).GetMethod().Name,
                                            _stopWatch.ElapsedMilliseconds));
            base.OnExit(args);
        }
    }
}
