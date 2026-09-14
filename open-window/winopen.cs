using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace OpenWindow
{
    public class WinOpen
    {
        /// <summary>
        /// Automatically identifies the parent project, logs to console, and renders a generic webview frame.
        /// </summary>
        public static async Task LaunchAsync(string htmlContent, string windowTitle = "Utility Window", int width = 800, int height = 600)
        {
            // 1. Automatically grab the project name from the calling application assembly
            AssemblyName callingAssemblyName = Assembly.GetCallingAssembly().GetName();
            string projectName = callingAssemblyName.Name ?? "UnknownProject";

            // 2. Output the initial load sequence and trigger the greeting loop
            Console.WriteLine("==================================================");
            Console.WriteLine($"[OPEN-WINDOW]: Initializing background engine...");
            Console.WriteLine($">>> Hello {projectName}!"); 
            Console.WriteLine("==================================================");

            // 3. Optional ReadLine check to pause deployment if you want a confirmation step
            Console.Write("\nPress [Enter] to spin up the web interface view layer...");
            Console.ReadLine();

            // 4. Fire up the dynamic UI frame
            Console.WriteLine("[OPEN-WINDOW]: Launching borderless application frame...");
            using var playwright = await Playwright.CreateAsync();
            
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                Args = new[] { 
                    $"--app=data:text/html,<html><head><title>{windowTitle}</title></head><body>Loading...</body></html>", 
                    $"--window-size={width},{height}",
                    "--disable-extensions"
                }
            });

            var page = await browser.NewPageAsync();
            await page.SetContentAsync(htmlContent);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n[STATUS]: Interface operational for {projectName}.");
            Console.ResetColor();

            Console.WriteLine("Press any key in this terminal to terminate the window context.");
            Console.ReadKey();
        }
    }
}
