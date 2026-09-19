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
            // 1. Grab the root project execution identifier
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

            // 3. Initialize a clean window layout instance
            var window = new PhotinoWindow();

            // 🟢 STEP A: Supply the content layout IMMEDIATELY to satisfy startup parameters
            window.StartString = htmlContent;

            // 🟢 STEP B: Configure dimensions and tracking options AFTER setting StartString
            window.SetTitle(windowTitle)
                  .SetSize(width, height)
                  .Center()
                  .SetDevToolsEnabled(true); // Right-click -> Inspect element is now active if you need it

            // 4. Render and hold execution loop until closed
            window.WaitForClose();

            Console.WriteLine("\n[OPEN-WINDOW]: Window context terminated by user.");
        }
    }
}
