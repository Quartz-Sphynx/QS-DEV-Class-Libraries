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
        // 1. Find the path to the current system's core runtime library folder
        string assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

        // 2. Explicitly load CoreLib, Console, AND the missing System.Runtime assembly mapping
        var trustedAssemblies = new[]
        {
    MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Private.CoreLib.dll")),
    MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")),
    MetadataReference.CreateFromFile(typeof(Console).Assembly.Location)
};


        // 3. Parse the code with strict compilation choices
        var syntaxTree = CSharpSyntaxTree.ParseText(generatedSourceCode);
        var compilation = CSharpCompilation.Create("UserBlocksAssembly")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(trustedAssemblies)
            .AddSyntaxTrees(syntaxTree);

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[SANDBOX COMPILATION FAILED]:");
            foreach (var diagnostic in result.Diagnostics)
            {
                if (diagnostic.Severity == DiagnosticSeverity.Error)
                {
                    Console.WriteLine($"  -> {diagnostic.GetMessage()}");
                }
            }
            Console.ResetColor();
            return; // Safe return out of the loop
        }


        ms.Seek(0, SeekOrigin.Begin);

        // 4. Load into an isolated AssemblyLoadContext for easy garbage collection
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
            // 5. Clean up and unload the memory so old block runs don't leak memory
            alc.Unload();
        }
    }
}
