using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Battleships
{
    static class SRandom
    {
        static readonly Random R = new Random();
        static readonly object L = new object();

        public static UInt16 Next(int min, int max)
        {
            lock (L) return (UInt16) R.Next(min, max);
        }
    }

    internal static class Program
    {
        private static Boolean _playerOneWon;
        private static Boolean _playerTwoWon;
        
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            
            Boolean readyPlayerOne = true;

            UInt16[,] playerOneShips = CreateBattleshipField(ZeroTwoDimMatrix(10, 10));
            UInt16[,] playerTwoShips = CreateBattleshipField(ZeroTwoDimMatrix(10, 10));

            UInt16[,] playerOneTargets = ZeroTwoDimMatrix(10, 10);
            UInt16[,] playerTwoTargets = ZeroTwoDimMatrix(10, 10);

            //Turns while not win
            while (!_playerOneWon && !_playerTwoWon)
            {
                Console.Clear();
                ColorfulWrite(new Dictionary<object, ConsoleColor?>()
                {
                    {"Ready player ", null},
                    {readyPlayerOne ? "one" : "two", ConsoleColor.DarkCyan},
                });
                Console.WriteLine();
                
                CheckReady();

                if (readyPlayerOne)
                    // Make shot(-s)
                    Shot(ref playerOneTargets, ref playerTwoShips, playerOneShips);
                else
                    // Make shot(-s)
                    Shot(ref playerTwoTargets, ref playerOneShips, playerTwoShips);

                // End move
                readyPlayerOne = !readyPlayerOne;
                // Check wining conditions
                _playerOneWon = IsWin(playerTwoShips);
                _playerTwoWon = IsWin(playerOneShips);
            }

            ColorfulWrite(new Dictionary<object, ConsoleColor?>()
            {
                {"Congratulations, Player ", null},
                {_playerOneWon ? "One" : "Two", ConsoleColor.DarkCyan},
                {"! You won!", null},
            });
            Console.WriteLine();

            // PrintFields(playerOneShips, playerOneTargets);
            // Console.WriteLine();
            // PrintFields(playerTwoShips, playerTwoTargets);

            // User-friendly app finish
            Console.WriteLine();
            ColorfulWrite(new Dictionary<object, ConsoleColor?>()
            {
                {"Press ", null},
                {"<Enter>", ConsoleColor.DarkCyan},
                {" to close application...", null},
            });
            Console.WriteLine();
            Console.Read();
        }

        static void CheckReady()
        {
            // In case user not ready ask again
            if (!GetBoolFromUser("Are you ready?"))
                CheckReady();
        }

        static void ColorfulWrite(Dictionary<object, ConsoleColor?> dataDictionary)
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
        }

        static Boolean GetBoolFromUser(String textRequest)
        {
            Console.WriteLine(textRequest);
            ColorfulWrite(new Dictionary<object, ConsoleColor?>()
            {
                {"Enter ", null},
                {"Y", ConsoleColor.DarkCyan},
                {" if yes or ", null},
                {"N", ConsoleColor.DarkCyan},
                {" if no", null},
            });
            Console.WriteLine();
            switch (@Console.ReadLine().ToLower())
            {
                case "yes":
                case "да":
                case "д":
                case "y":
                    return true;
                case "no":
                case "нет":
                case "н":
                case "n":
                    return false;
                default:
                    Console.WriteLine("Invalid input. Try again");
                    return GetBoolFromUser(textRequest);
            }
        }

        static UInt16[,] ZeroTwoDimMatrix(UInt16 dim0, UInt16 dim1)
        {
            UInt16[,] matrix = new UInt16[dim0, dim1];

            for (int i = 0; i < matrix.GetUpperBound(0); i++)
            for (int j = 0; j < matrix.GetUpperBound(1); j++)
                matrix[i, j] = 0;

            return matrix;
        }

        static UInt16[,] CreateBattleshipField(UInt16[,] field)
        {
            // Add 1 ship with size 4
            field = PlaceShip(4, field);

            // Add 2 ships with size 3
            field = PlaceShip(3, field);
            field = PlaceShip(3, field);

            // Add 3 ships with size 2
            field = PlaceShip(2, field);
            field = PlaceShip(2, field);
            field = PlaceShip(2, field);

            // Add 4 ships with size 1
            field = PlaceShip(1, field);
            field = PlaceShip(1, field);
            field = PlaceShip(1, field);
            field = PlaceShip(1, field);

            return field;
        }

        static UInt16[,] PlaceShip(UInt16 shipSize, UInt16[,] field)
        {
            UInt16 isVert = SRandom.Next(0, 1); // `0` -- horizontal; `1` -- vertical
            UInt16 cStart = (0 == isVert) ? SRandom.Next(0, 10 - shipSize) : SRandom.Next(0, 9);
            UInt16 rStart = (1 == isVert) ? SRandom.Next(0, 10 - shipSize) : SRandom.Next(0, 9);

            // In case start position is busy restart ship align
            if (1 == field[rStart, cStart])
                return PlaceShip(shipSize, field);

            // In case start row is not first one check previous row on start col
            if (0 != rStart)
                // In case start col on previous row is busy restart ship align
                if (1 == field[rStart - 1, cStart])
                    return PlaceShip(shipSize, field);

            // In case start col is not first one check previous col on start row
            if (0 != cStart)
                // In case start row on previous col is busy restart ship align
                if (1 == field[rStart, cStart - 1])
                    return PlaceShip(shipSize, field);

            if (0 == isVert) // Horizontal
            {
                for (int j = cStart; j < cStart + shipSize; j++)
                {
                    // In case row is not first one check previous row
                    if (0 != rStart)
                        // In case ship's col on previous row is busy restart ship align
                        if (1 == field[rStart - 1, j])
                            return PlaceShip(shipSize, field);

                    // In case row is not last one check next row
                    if (9 != rStart)
                        // In case ship's col on next row is busy restart ship align
                        if (1 == field[rStart + 1, j])
                            return PlaceShip(shipSize, field);
                }

                // In case ships ends not on last col
                if (10 != cStart + shipSize)
                    // In case next col on ship's row is busy restart ship align
                    if (1 == field[rStart, cStart + shipSize])
                        return PlaceShip(shipSize, field);

                // Place ship
                for (int j = cStart; j < cStart + shipSize; j++)
                    field[rStart, j] = 1;
            }
            else // Vertical
            {
                for (int i = rStart; i < rStart + shipSize; i++)
                {
                    // In case col is not first one check previous col
                    if (0 != cStart)
                        // In case ship's row on previous col is busy restart ship align
                        if (1 == field[i, cStart - 1])
                            return PlaceShip(shipSize, field);

                    // In case col is not last one check next col
                    if (9 != cStart)
                        // In case ship's row on next col is busy restart ship align
                        if (1 == field[i, cStart + 1])
                            return PlaceShip(shipSize, field);
                }

                // In case ships ends not on last row
                if (10 != rStart + shipSize)
                    // In case next row on ship's col is busy restart ship align
                    if (1 == field[rStart + shipSize, cStart])
                        return PlaceShip(shipSize, field);

                // Place ship
                for (int i = rStart; i < rStart + shipSize; i++)
                    field[i, cStart] = 2;
            }

            return field;
        }

        static Boolean IsWin(UInt16[,] otherPlayerShips)
        {
            for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                // In case any ship/part of ship is alive return false
                if (1 == otherPlayerShips[i, j])
                    return false;
            return true;
        }

        static void PrintFields(UInt16[,] ships, UInt16[,] targets)
        {
            Dictionary<object, ConsoleColor?> letters = new Dictionary<object, ConsoleColor?>()
            {
                {PadBoth("#", 3), ConsoleColor.DarkCyan},
                {PadBoth("A", 3), ConsoleColor.DarkCyan},
                {PadBoth("B", 3), ConsoleColor.DarkCyan},
                {PadBoth("C", 3), ConsoleColor.DarkCyan},
                {PadBoth("D", 3), ConsoleColor.DarkCyan},
                {PadBoth("E", 3), ConsoleColor.DarkCyan},
                {PadBoth("F", 3), ConsoleColor.DarkCyan},
                {PadBoth("G", 3), ConsoleColor.DarkCyan},
                {PadBoth("H", 3), ConsoleColor.DarkCyan},
                {PadBoth("I", 3), ConsoleColor.DarkCyan},
                {PadBoth("J", 3), ConsoleColor.DarkCyan},
            };
            
            // Clear Console
            Console.Clear();
            
            // Print legend
            Console.WriteLine("Legend:");
            ColorfulWrite(new Dictionary<object, ConsoleColor?>()
            {
                {PadBoth("", 3), null},
                {" -- Unknown", null},
            }); Console.WriteLine();
            ColorfulWrite(new Dictionary<object, ConsoleColor?>()
            {
                {PadBoth("~", 3), null},
                {" -- Clear water", null},
            }); Console.WriteLine();
            ColorfulWrite(new Dictionary<object, ConsoleColor?>()
            {
                {PadBoth("[ ]", 3), ConsoleColor.DarkGreen},
                {" -- Player ships", null},
            }); Console.WriteLine();
            ColorfulWrite(new Dictionary<object, ConsoleColor?>()
            {
                {PadBoth("[X]", 3), ConsoleColor.DarkRed},
                {" -- Hits/Destroyed ships", null},
            }); Console.WriteLine();
            ColorfulWrite(new Dictionary<object, ConsoleColor?>()
            {
                {PadBoth("O", 3), null},
                {" -- Miss", null},
            }); Console.WriteLine();
            
            Console.WriteLine();

            // Print letters for ships
            ColorfulWrite(letters);

            // Print separator
            Console.Write(PadBoth("", 6));

            // Print letters for targets
            ColorfulWrite(letters);

            // End line
            Console.WriteLine();

            // For each string
            for (UInt32 i = 0; i < 10; i++)
            {
                // Print number for ships
                ColorfulWrite(new Dictionary<object, ConsoleColor?>()
                {
                    {PadBoth($"{i + 1}", 3), ConsoleColor.DarkCyan},
                });

                // Print ships line
                for (UInt32 j = 0; j < 10; j++)
                {
                    /*  ~  -- 0 -- sea
                     * [ ] -- 1 -- ships
                     * [X] -- 2 -- hit
                     *  O  -- 3 -- miss
                     */
                    switch (ships[i, j])
                    {
                        case 0:
                            ColorfulWrite(new Dictionary<object, ConsoleColor?>()
                            {
                                {PadBoth("~", 3), null},
                            });
                            break;
                        case 1:
                            ColorfulWrite(new Dictionary<object, ConsoleColor?>()
                            {
                                {PadBoth("[ ]", 3), ConsoleColor.DarkGreen},
                            });
                            break;
                        case 2:
                            ColorfulWrite(new Dictionary<object, ConsoleColor?>()
                            {
                                {PadBoth("[X]", 3), ConsoleColor.DarkRed},
                            });
                            break;
                        case 3:
                            ColorfulWrite(new Dictionary<object, ConsoleColor?>()
                            {
                                {PadBoth("O", 3), null},
                            });
                            break;
                        default:
                            throw new Exception("Undefined filed filler");
                    }
                }

                // Print separator
                Console.Write(PadBoth("", 6));

                // Print number for targets
                ColorfulWrite(new Dictionary<object, ConsoleColor?>()
                {
                    {PadBoth($"{i + 1}", 3), ConsoleColor.DarkCyan},
                });

                // Print targets line
                for (UInt32 j = 0; j < 10; j++)
                {
                    /*     -- 0 -- unknown
                     * [X] -- 2 -- killed/hit
                     *  O  -- 3 -- miss
                     */
                    switch (targets[i, j])
                    {
                        case 0:
                            ColorfulWrite(new Dictionary<object, ConsoleColor?>()
                            {
                                {PadBoth("~", 3), null},
                            });
                            break;
                        case 2:
                            ColorfulWrite(new Dictionary<object, ConsoleColor?>()
                            {
                                {PadBoth("[X]", 3), ConsoleColor.DarkRed},
                            });
                            break;
                        case 3:
                            ColorfulWrite(new Dictionary<object, ConsoleColor?>()
                            {
                                {PadBoth("O", 3), null},
                            });
                            break;
                        default:
                            throw new Exception("Undefined filed filler");
                    }
                }

                // End line
                Console.WriteLine();
            }
        }

        static String PadBoth(String source, Int32 length)
        {
            Int32 spaces = length - source.Length;
            Int32 padLeft = spaces / 2 + source.Length;
            return source.PadLeft(padLeft).PadRight(length);
        }

        static void Shot(ref UInt16[,] myTargets, ref UInt16[,] otherPlayerShips, UInt16[,] myShips)
        {
            // Print user's ships and targets screens
            PrintFields(myShips, myTargets);
            
            // Get shot coordinates
            UInt16[] shot = GetShotCoordinates();

            // In case not first shot on position
            if (0 != myTargets[shot[0], shot[1]])
            {
                Console.WriteLine("You have already fired at this position");
                Thread.Sleep(2000);
                Shot(ref myTargets, ref otherPlayerShips, myShips);
            }

            if (1 == otherPlayerShips[shot[0], shot[1]]) // In case of hit
            {
                myTargets[shot[0], shot[1]] = 2;
                otherPlayerShips[shot[0], shot[1]] = 2;
                
                // Check for wining
                if (IsWin(otherPlayerShips))
                    return;

                // Shot one more time
                Shot(ref myTargets, ref otherPlayerShips, myShips);
            }
            else if (0 == otherPlayerShips[shot[0], shot[1]]) // In case of miss
            {
                myTargets[shot[0], shot[1]] = 3;
                otherPlayerShips[shot[0], shot[1]] = 3;
                
                ColorfulWrite(new Dictionary<object, ConsoleColor?>()
                {
                    {"You ", null},
                    {"miss", ConsoleColor.DarkCyan},
                });
                Console.WriteLine();
                Thread.Sleep(2000);
            }
        }

        static UInt16[] GetShotCoordinates()
        {
            UInt16[] coordinates = new UInt16[2];

            ColorfulWrite(new Dictionary<object, ConsoleColor?>()
            {
                {"Enter shot coordinates (ex. ", null},
                {"A7", ConsoleColor.DarkCyan},
                {")", null},
            });
            Console.WriteLine();

            Char[] userInput = Console.ReadLine().ToLower().ToCharArray();

            // Read letter
            switch (userInput[0])
            {
                case 'a':
                    coordinates[1] = 0;
                    break;
                case 'b':
                    coordinates[1] = 1;
                    break;
                case 'c':
                    coordinates[1] = 2;
                    break;
                case 'd':
                    coordinates[1] = 3;
                    break;
                case 'e':
                    coordinates[1] = 4;
                    break;
                case 'f':
                    coordinates[1] = 5;
                    break;
                case 'g':
                    coordinates[1] = 6;
                    break;
                case 'h':
                    coordinates[1] = 7;
                    break;
                case 'i':
                    coordinates[1] = 8;
                    break;
                case 'j':
                    coordinates[1] = 9;
                    break;
                default:
                    Console.WriteLine("Invalid input. Try again");
                    return GetShotCoordinates();
            }

            // Read number
            switch (userInput.Length)
            {
                case 2:
                    switch (userInput[1])
                    {
                        case '1':
                            coordinates[0] = 0;
                            break;
                        case '2':
                            coordinates[0] = 1;
                            break;
                        case '3':
                            coordinates[0] = 2;
                            break;
                        case '4':
                            coordinates[0] = 3;
                            break;
                        case '5':
                            coordinates[0] = 4;
                            break;
                        case '6':
                            coordinates[0] = 5;
                            break;
                        case '7':
                            coordinates[0] = 6;
                            break;
                        case '8':
                            coordinates[0] = 7;
                            break;
                        case '9':
                            coordinates[0] = 8;
                            break;
                        default:
                            Console.WriteLine("Invalid input. Try again");
                            return GetShotCoordinates();
                    }

                    break;
                case 3 when '1' == userInput[1] && '0' == userInput[2]:
                    coordinates[0] = 9;
                    break;
                default:
                    Console.WriteLine("Invalid input. Try again");
                    return GetShotCoordinates();
            }

            return coordinates;
        }
    }
}