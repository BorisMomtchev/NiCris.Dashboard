using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostSharp.Aspects;

namespace NiCris.Client.BusinessStream.Aspects
{
    // Simple trace aspect
    // Does not implemement performance best practices.
    [Serializable]
    public sealed class SimpleTraceAttribute : OnMethodBoundaryAspect
    {
        // Before target method
        public override void OnEntry(MethodExecutionArgs args)
        {
            System.Diagnostics.Trace.WriteLine(string.Format(
                "Entering {0}.{1}.",
                args.Method.DeclaringType.FullName,
                args.Method.Name));
            System.Diagnostics.Trace.Indent();
        }

        // After target method exits with success
        public override void OnExit(MethodExecutionArgs args)
        {
            System.Diagnostics.Trace.Unindent();
            System.Diagnostics.Trace.WriteLine(
                string.Format(
                "Leaving {0}.{1}.",
                args.Method.DeclaringType.FullName,
                args.Method.Name));
        }

        // After target method exits with exception
        public override void OnException(MethodExecutionArgs args)
        {
            System.Diagnostics.Trace.Unindent();
            System.Diagnostics.Trace.WriteLine(string.Format(
                "Leaving {0}.{1} with exception {2}.",
                args.Method.DeclaringType.FullName,
                args.Method.Name,
                args.Exception.GetType().Name));
        }
    }
}
