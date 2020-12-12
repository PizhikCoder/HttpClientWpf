using System;
using System.Diagnostics;

namespace Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Проверка разрядности операционной системы");
            if (Environment.Is64BitOperatingSystem)
            {
                Console.WriteLine("Запускается клиент...");
                Process.Start(Environment.CurrentDirectory + "client name");
                Console.WriteLine("Клиент запущен");
            }
            else
            {
                Console.WriteLine("Запускается клиент...");
                Process.Start(Environment.CurrentDirectory + "client name");
                Console.WriteLine("Клиент запущен");
            }
            Environment.Exit(0);
        }
    }
}
