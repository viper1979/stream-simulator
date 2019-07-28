using System;

namespace StreamSimulator.Core.Extensions
{
    public static class StringExtensions
    {
        public static void Dump(this string message)
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " | " + message);
        }
    }
}
