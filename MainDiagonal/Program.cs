using System;
using System.Collections.Generic;
using System.Text;

namespace MainDiagonal
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            
            Random r = new Random();

            Int32 mSize = GetIntFromUser("Enter matrix size", 1, 10);
            
            Int32[,] matrix = new Int32[mSize, mSize];

            Console.WriteLine();
            Console.WriteLine("Main diagonal:");
            for (Int32 i = 0; i < mSize; i++)
            {
                for (Int32 j = 0; j < mSize; j++)
                {
                    matrix[i, j] = r.Next(10,99);
                    if (i == j) ColorfulWriteLine(new Dictionary<object, ConsoleColor?>()
                    {
                        {matrix[i, j].ToString().PadLeft(2+i*3), ConsoleColor.DarkGreen},
                    });
                }
            }
            
            Console.WriteLine();
            Console.WriteLine("Full matrix:");
            for (Int32 i = 0; i < mSize; i++)
            {
                for (Int32 j = 0; j < mSize; j++)
                {
                    Console.Write($"{matrix[i, j]} ");
                }
                Console.WriteLine();
            }

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
        
        static Int32 GetIntFromUser(String textRequest, Int32 minValue, Int32 maxValue)
        {
            ColorfulWriteLine(new Dictionary<object, ConsoleColor?>()
            {
                {textRequest, null},
                {" (from ", null},
                {minValue, ConsoleColor.DarkCyan},
                {" to ", null},
                {maxValue, ConsoleColor.DarkCyan},
                {")", null},
            });
            var userNumber = Console.ReadLine();
            if (Int32.TryParse(userNumber, out var number) && number >= minValue && number <= maxValue)
            {
                return number;
            }
            else
            {
                Console.WriteLine("Invalid input. Try again");
                return GetIntFromUser(textRequest, minValue, maxValue);
            }
        }
    }
}