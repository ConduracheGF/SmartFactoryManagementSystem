using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{
    
    // Static utility class responsible for all console output in the application.
    internal static class Logger
    {
        // Logs a general informational message in white text, prefixed with a timestamp.
        public static void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[{DateTime.Now: HH:mm:ss}] [INFO] {message}");
            Console.ResetColor();
        }

        // Logs a warning message in yellow text, with a timestamp and used for issues 
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

        
        public static void Clear()
        {
            Console.Clear();
        }

        
        public static void ResetColor()
        {
            Console.ResetColor();
        }
    }
}
