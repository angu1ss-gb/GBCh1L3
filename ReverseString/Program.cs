using System;
using System.Collections.Generic;
using System.Text;

namespace ReverseString
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            
            Console.WriteLine("Enter string");
            Char[] userInput = Console.ReadLine().ToCharArray();
            Array.Reverse(userInput);
            
            Console.WriteLine();
            Console.WriteLine(new String(userInput));
            
            // User-friendly app finish
            Console.WriteLine();
            ColorfulWriteLine(new Dictionary<object, ConsoleColor?>()
            {
                {"Press ", null},
                {"<Enter>", ConsoleColor.DarkCyan},
                {" to close application...", null},
            });
            Console.Read();
        }
        
        static void ColorfulWriteLine(Dictionary<object, ConsoleColor?> dataDictionary)
        {
            foreach (KeyValuePair<object, ConsoleColor?> entry in dataDictionary)
            {
                if (null == entry.Value)
                {
                    Console.Write(entry.Key);
                }
                else
                {
                    Console.ForegroundColor = (ConsoleColor) entry.Value;
                    Console.Write(entry.Key);
                    Console.ResetColor();
                }
            }
            Console.WriteLine();
        }
    }
}