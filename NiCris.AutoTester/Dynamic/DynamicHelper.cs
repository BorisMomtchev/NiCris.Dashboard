using System;
using System.Reflection;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;

namespace NiCris.AutoTester.Dynamic
{
    public static class DynamicHelper
    {
        public static Assembly CompiledAssembly { get; private set; }

        public static void CompileAssembly(string[] code)
        {
            CompilerParameters compilerParams = new CompilerParameters();
            string outputDirectory = Directory.GetCurrentDirectory();

            compilerParams.GenerateInMemory = true;
            compilerParams.TreatWarningsAsErrors = false;
            compilerParams.GenerateExecutable = false;
            compilerParams.CompilerOptions = "/optimize";
            compilerParams.OutputAssembly = "DynamicLibrary.dll";

            string[] references = { "System.dll" };
            compilerParams.ReferencedAssemblies.AddRange(references);

            //Console.WriteLine("Compile assembly from source...");
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerResults compile = provider.CompileAssemblyFromSource(compilerParams, code);

            if (compile.Errors.HasErrors)
            {
                string text = "Compile error: ";
                foreach (CompilerError ce in compile.Errors)
                    text += "rn" + ce.ToString();
                throw new Exception(text);
            }

            CompiledAssembly = compile.CompiledAssembly;
        }

        public static void ReflectAssembly()
        {
            Console.WriteLine("Reflect the new assembly...\n");
            Console.WriteLine("Assembly Info:");

            foreach (Module m in CompiledAssembly.GetModules())
            {
                Console.WriteLine("Module: {0}", m);

                foreach (Type t in m.GetTypes())
                {
                    Console.WriteLine("Type: {0}", t.Name);

                    foreach (MethodInfo mi in t.GetMethods())
                        Console.WriteLine("Method: {0}", mi.Name);
                }
            }
        }

        public static object ExecuteMethod(string method, string initialValue, int index)
        {
            Module module = CompiledAssembly.GetModules()[0];
            Type mt = null;
            MethodInfo methInfo = null;

            if (module != null)
                mt = module.GetType("DynamicLibrary.Rules");

            if (mt != null)
                methInfo = mt.GetMethod(method);

            if (methInfo != null)
                return methInfo.Invoke(null, new object[] { initialValue, index });
            return null;
        }
    }
}
