using System;
using System.Diagnostics;

namespace SimpleWorkflow
{
    public static class StringExtensions
    {
        public static void WriteDebug(this string s)
        {
            Console.WriteLine(s);
        }
    }
}