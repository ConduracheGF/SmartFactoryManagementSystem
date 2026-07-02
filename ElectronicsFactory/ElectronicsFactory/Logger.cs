using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{
    internal static class Logger
    {
        public static void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[{DateTime.Now: HH:mm:ss}] [INFO] {message}");
            Console.ResetColor();
        }

        public static void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[{DateTime.Now: HH:mm:ss}] [WARN] {message}");
            Console.ResetColor();
        }

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

        public static void Read()
        {
            Console.ReadLine();
        }
    }
}
