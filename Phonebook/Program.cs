using System;
using System.Collections.Generic;
using System.Text;

namespace Phonebook
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            
            String[,] phonebook = new String[5, 2];

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"Enter contact #{i+1} name");
                phonebook[i, 0] = Console.ReadLine();
                Console.WriteLine($"Enter contact #{i+1} email/cellphone number");
                phonebook[i, 1] = Console.ReadLine();
            }

            Console.WriteLine();
            Console.WriteLine("Your phonebook:");
            for (int i = 0; i < 5; i++)
            {
                Console.Write($"#{i}:\t{phonebook[i, 0]}\t\t{phonebook[i, 1]}");
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