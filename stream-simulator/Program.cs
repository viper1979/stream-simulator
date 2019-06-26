using System;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace StreamSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            var strSchema = string.Empty;
            using (var sr = File.OpenText(@"Schemas\simple.schema.json"))
            {
                strSchema = sr.ReadToEnd();
            }

            var schema = JsonSchema.FromSampleJson(strSchema);

            var settings = new CSharpGeneratorSettings
            {
                ClassStyle = CSharpClassStyle.Inpc,
                GenerateDataAnnotations = false,
                Namespace = "Mockups",
                PropertySetterAccessModifier = "public",
            };

            var classGenerator = new CSharpGenerator(schema);
            var csharpCode = classGenerator.GenerateFile();

            Console.WriteLine(csharpCode);

            ;
            return;

            /***/

            var compilation = CSharpCompilation.Create("Mockups")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(
                    MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location))
                    //MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "System.Runtime.dll")))
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(csharpCode));

            /***/

            // Debug output. In case your environment is different it may show some messages.
            foreach (var compilerMessage in compilation.GetDiagnostics())
                Console.WriteLine(compilerMessage);

            using (var memoryStream = new MemoryStream())
            {
                var emitResult = compilation.Emit(memoryStream);
                if (emitResult.Success)
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    var context = AssemblyLoadContext.Default;
                    var assembly = context.LoadFromStream(memoryStream);

                    assembly.GetType("ClassName").GetMethod("MethodName").Invoke(null, null);
                }
            }

            ;
        }
    }
}
