using System;
using System.Diagnostics;

namespace Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(DateTime.Now.ToShortTimeString() + " Нажмите любую кнопку, чтобы начать...");
            Console.ReadKey();
            Console.WriteLine(DateTime.Now.ToShortTimeString() + " Проверка разрядности операционной системы");
            if (Environment.Is64BitOperatingSystem)
            {
                Console.WriteLine(DateTime.Now.ToShortTimeString() + " Запускается x64 клиент...");
                Process.Start(Environment.CurrentDirectory + "\\x64\\HTTP_WPF_Client-Project.exe");
                Console.WriteLine(DateTime.Now.ToShortTimeString() + " Клиент запущен");
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToShortTimeString() + " Запускается x86 клиент...");
                Process.Start(Environment.CurrentDirectory + "\\x86\\HTTP_WPF_Client-Project.exe");
                Console.WriteLine(DateTime.Now.ToShortTimeString() +  " Клиент запущен");
            }
            Console.WriteLine(DateTime.Now.ToShortTimeString() + " Нажмите любую кнопку, чтобы закрыть лаунчер...");
            Console.ReadKey();
        }
    }
}
