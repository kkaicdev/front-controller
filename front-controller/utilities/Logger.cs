using System;
using System.Runtime.InteropServices;

namespace FrontController
{
    public static class Logger
    {
        private static readonly object _lock = new object();

        public static void Log(string prefix, string message)
        {
            lock (_lock)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine($"[{timestamp}] {prefix} {message}");
            }
        }

        public static void Info(string message) => Log("[*]", message);
        public static void Success(string message) => Log("[+]", message);
        public static void Error(string message) => Log("[!]", message);
    }
}