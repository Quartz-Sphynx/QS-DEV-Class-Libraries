using System;
using System.Reflection;
using Photino.NET;

namespace OpenWindow
{
    public class WinOpen
    {
        /// <summary>
        /// Automatically identifies the parent project, logs to console, and renders a true native webview window.
        /// </summary>
        public static void Launch(string htmlContent, string windowTitle = "Utility Window", int width = 800, int height = 600)
        {
            // 1. Always targets your root project executable (tea_rex)
            AssemblyName entryAssemblyName = Assembly.GetEntryAssembly()?.GetName() ?? Assembly.GetCallingAssembly().GetName();
            string projectName = entryAssemblyName.Name ?? "UnknownProject";

            // 2. Output the initial load sequence
            Console.WriteLine("==================================================");
            Console.WriteLine($"[OPEN-WINDOW]: Initializing Photino UI engine...");
            Console.WriteLine($">>> Hello {projectName}!");
            Console.WriteLine("==================================================");

            Console.Write("\nPress [Enter] to spin up the native interface view layer...");
            Console.ReadLine();

            Console.WriteLine("[OPEN-WINDOW]: Launching application window...");

            // 3. Hook into the window initialization cycle
            // This ensures the native OS WebView component parses and loads the layout 
            // string immediately upon window creation, preventing a blank/black frame.
            var window = new PhotinoWindow()
                .SetTitle(windowTitle)
                .SetSize(width, height)
                .Center();

            // Bind your HTML layout data safely into the wrapper context
            window.LoadRawString(htmlContent);

            // Render and block until closed
            window.WaitForClose();

            Console.WriteLine("\n[OPEN-WINDOW]: Window context terminated by user.");
        }
    }
}
