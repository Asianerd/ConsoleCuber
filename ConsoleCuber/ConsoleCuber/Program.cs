using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleCuber
{
    class Program
    {
        public static Random random = new Random();
        public static State state;
        public static bool playing = true;

        static void Main(string[] args)
        {
            state = State.Viewing;
            Console.ForegroundColor = ConsoleColor.White;
            Dialogue.initializeColors();

            if (!File.Exists(Scramble.filePath)) { Scramble.Save(); }
            Scramble.Load();

            while (playing)
            {
                Console.Clear();
                Scramble.Save();
                Dialogue.ColoredPrint("Please press only one button at a time!", ConsoleColor.Magenta);
                ConsoleKey input = Dialogue.FetchKey("View records / Play    [ v / p ] : ").Key;
                Console.WriteLine("");

                switch (input)
                {
                    case ConsoleKey.Escape:
                        playing = false;
                        break;
                    case ConsoleKey.P:
                        state = State.Playing;
                        var scramble = new Scramble(null);
                        Console.Clear();
                        Console.WriteLine("");
                        Dialogue.TimedDialogue(scramble.turnSetStringCollection.Select(n => $"$col$a$ext${n}").ToArray(), 50);
                        Dialogue.FetchKey("\nPress any key to start!\n");
                        scramble.Start();
                        Dialogue.FetchKey("\nPress any button to end\n");
                        Scramble bestScramble = Scramble.scrambleHistory.Count()>1?Scramble.topScramble:null;
                        scramble.End();
                        Console.WriteLine($"Time : {scramble.timeSeconds}");
                        if (bestScramble != null)
                        {
                            if (scramble.time < bestScramble.time)
                            {
                                Dialogue.TimedDialogue(new string[]
                                {
                                "",
                                "$col$a\tNew record!",
                                $"$col$dPrevious best : {bestScramble.timeReadable}",
                                $"$col$e     New best : {scramble.timeReadable}",
                                "",
                                $"$col$eDifference of {Scramble.TimeToReadable(bestScramble.timeSeconds - scramble.timeSeconds)}"
                                }, 50);
                            }
                        }
                        break;
                    case ConsoleKey.C:
                        Dialogue.TimedDialogue(new string[] {
                            $"$col$dConsoleCuber was develop entirely by Asianerd.",
                            $"$col$b    Github  : https://github.com/Asianerd"
                        }, 0);

                        if(Dialogue.FetchKey("Visit website? [y/n]").Key == ConsoleKey.Y)
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "https://github.com/Asianerd",
                                UseShellExecute = true
                            });
                            break;
                        }
                        break;
                    default:
                        state = State.Viewing;
                        Scramble.ViewTop();
                        Console.Write("\n\n");
                        Scramble.ViewHistory();
                        Console.Write("\n\n");
                        Scramble.PrintStatistics();
                        break;
                }
                state = State.None;
                if (playing)
                {
                    Console.ReadKey();
                }
            }
        }

        public enum State
        {
            None,
            Playing,
            Viewing
        }
    }
}