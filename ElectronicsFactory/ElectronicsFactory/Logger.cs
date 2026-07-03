using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{
    /// <summary>
    /// Static utility class responsible for all console output in the application.
    /// Centralizes logging so that message formatting and coloring stays consistent
    /// </summary>
    internal static class Logger
    {
        // Logs a general informational message in white text, prefixed with a timestamp.
        public static void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[{DateTime.Now: HH:mm:ss}] [INFO] {message}");
            Console.ResetColor();
        }

        // Logs a warning message in yellow text, with a timestamp and used for issues (invalid state or non-critical business rule)
        public static void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[{DateTime.Now: HH:mm:ss}] [WARN] {message}");
            Console.ResetColor();
        }

        // Logs an error message in red text, with a timestamp and used for failed operations or critical business rule
        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now: HH:mm:ss}] [ERROR] {message}");
            Console.ResetColor();
        }

        // Clears the console screen. Used before redrawing menus
        public static void Clear()
        {
            Console.Clear();
        }

        // Resets the console foreground color to its default value
        public static void ResetColor()
        {
            Console.ResetColor();
        }
    }
}
