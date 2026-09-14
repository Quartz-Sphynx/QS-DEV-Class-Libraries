using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public class BlockSandbox
{
    public static void ExecuteSafeCode(string generatedSourceCode)
    {
        // 1. Only reference essential, safe assemblies (No System.IO, System.Net, etc.)
        var trustedAssemblies = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location), // System.Private.CoreLib
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location)  // System.Console
        };

        // 2. Parse the code with strict compilation choices
        var syntaxTree = CSharpSyntaxTree.ParseText(generatedSourceCode);
        var compilation = CSharpCompilation.Create("UserBlocksAssembly")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(trustedAssemblies)
            .AddSyntaxTrees(syntaxTree);

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            // Handle compilation errors (e.g., block validation failed)
            return;
        }

        ms.Seek(0, SeekOrigin.Begin);

        // 3. Load into an isolated AssemblyLoadContext for easy garbage collection
        var alc = new AssemblyLoadContext("UserCodeContext", isCollectible: true);
        try
        {
            Assembly assembly = alc.LoadFromStream(ms);
            
            // Look up the entry method you generated (e.g., UserScript.Run)
            var type = assembly.GetType("UserScript");
            var method = type?.GetMethod("Run");
            
            // Execute the block code
            method?.Invoke(null, null);
        }
        finally
        {
            // 4. Clean up and unload the memory so old block runs don't leak memory
            alc.Unload();
        }
    }
}
