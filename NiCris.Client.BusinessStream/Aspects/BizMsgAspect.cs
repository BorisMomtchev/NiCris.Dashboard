using System;
using System.Reflection;
using PostSharp.Aspects;
using System.Diagnostics;

namespace NiCris.Client.BusinessStream.Aspects
{
    [Serializable]
    public sealed class BizMsgAspect : OnMethodBoundaryAspect
    {
        // Assigned and serialized at build time, then deserialized and read at runtime.
        private string MethodName { get; set; }
        private ParameterInfo[] ParameterInfo { get; set; }

        // Requires
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; }

        // Optional
        public string Description { get; set; }
        public string AppId { get; set; }
        public string ServiceId { get; set; }
        public string StyleId { get; set; }
        public string Roles { get; set; }

        // C~tor
        public BizMsgAspect(string name, string user)
        {
            this.Name = name;
            this.Date = DateTime.Now;
            this.User = user;
        }

        // This method is executed at build-time, inside PostSharp.
        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            // Computes the field value at build-time so that reflection is not necessary at runtime.
            this.MethodName = method.DeclaringType.FullName + "." + method.Name;
            this.ParameterInfo = method.GetParameters();
        }

        // This method is executed at runtime inside your application, before target methods
        public override void OnEntry(MethodExecutionArgs args)
        {
            Trace.WriteLine(MethodName + " - OnEntry");
            Trace.WriteLine(string.Format("Name: {0}, Date: {1}, User: {2}", Name, Date.ToString(), User));
            Trace.WriteLine(string.Format("MethodName: {0}", MethodName));
            Trace.WriteLine(string.Format("ParameterInfo[] Length: {0}", ParameterInfo.Length));

            // Get the values of ParameterInfo[] params with Reflection?
            // NOTE: simple reflective techniques cannot achieve what you desire with full generality
            // at least not without hooking into the debugger/profiling API: 
            // http://stackoverflow.com/questions/1867482/c-sharp-getting-value-of-parms-using-reflection

            Trace.WriteLine(string.Format("Entering {0}.{1}", args.Method.DeclaringType.Name, args.Method.Name));
            
            Trace.WriteLine(args.Method.GetParameters()[0].Name + " = " + args.Arguments.GetArgument(0));
            Trace.WriteLine(args.Method.GetParameters()[1].Name + " = " +
                                (args.Arguments.GetArgument(1) as SomeObject).XYZ + ", " + 
                                (args.Arguments.GetArgument(1) as SomeObject).ABC
                            );

            /*
            for (int x = 0; x < args.Arguments.Count; x++)
                Trace.WriteLine(args.Method.GetParameters()[x].Name + " = " + args.Arguments.GetArgument(x));
            */
        }

        // This method is executed at runtime inside your application, when target methods exit with success
        public override void OnSuccess(MethodExecutionArgs args)
        {
            Trace.WriteLine(MethodName + " - OnSuccess");
        }

        // This method is executed at runtime inside your application, when target methods exit with an exception
        public override void OnException(MethodExecutionArgs args)
        {
            Trace.WriteLine(MethodName + " - OnException\n" + args.Exception.Message);
        }
    }
}
