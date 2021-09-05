using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCuber
{
    public class Scramble
    {
        public static List<Scramble> scrambleHistory = new List<Scramble>();
        public static string filePath = "Collection.xml";
        public static Scramble topScramble
        {
            get
            {
                return TopScrambles()[0];
            }
        }

        public List<Turns> turnSet;
        public string turnSetString;
        public List<string> turnSetStringCollection = new List<string>();
        public double time; // In milliseconds
        public double timeSeconds;
        public string timeReadable { get { return TimeToReadable(timeSeconds); } }
        public double startTime;
        public double endTime;
        public bool active = true;

        #region Constructors
        public Scramble(List<Turns> _turnSet = null)
        {
            if (_turnSet == null)
            {
                turnSet = GenerateTurnSet();
            }
            foreach (Turns x in turnSet)
            {
                string final;
                if (x.ToString().EndsWith('c'))
                {
                    final = $"{x.ToString().Replace('c', '\'')} ";
                }
                else
                {
                    final = $"{x}  ";
                }
                turnSetString += final;
                turnSetStringCollection.Add(final);
            }

            startTime = Timer.Now();
        }

        public Scramble()
        {
            // Empty constructor for saving and loading
        }
        #endregion

        #region Turn-related
        public static List<Turns> GenerateTurnSet(int _lenght = 10)
        {
            List<Turns> availableTurns = Enum.GetValues(typeof(Turns)).Cast<Turns>().ToList();
            List<Turns> final = new List<Turns>();

            while (final.Count < 10)
            {
                Turns _candidate = (Turns)Program.random.Next(0, availableTurns.Count);
                if (!turnRedundancy(final.TakeLast(3).ToList(), _candidate))
                {
                    final.Add(_candidate);
                }
            }

            return final;
        }

        static bool turnRedundancy(List<Turns> collection, Turns candidate)
        {
            foreach (Turns x in collection)
            {
                if (x.ToString().ElementAt(0) == candidate.ToString().ElementAt(0))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Time-related
        public void Start()
        {
            startTime = Timer.Now();
        }

        public void End()
        {
            endTime = Timer.Now();
            time = endTime - startTime;
            timeSeconds = time / 1000;

            active = false;
            scrambleHistory.Add(this);
        }
        #endregion


        #region Data-related
        public static void ViewHistory(bool sorted = false)
        {
            foreach (var i in scrambleHistory.Select((item, index) => new { index, item }))
            {
                Dialogue.TimedDialogue(new string[] { $"$col$f$ext${i.index + 1}. ", $"$col$b{i.item.timeReadable}\t{i.item.turnSetString}" }, 10);
            }
        }

        public static void ViewTop()
        {
            ConsoleColor[] _topFiveColor = { ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Gray, ConsoleColor.Gray };
            foreach (var i in TopScrambles().Select((item, index) => new { index, item }))
            {
                Dialogue.TimedDialogue(new string[] {
                    $"$col${Dialogue.hexChars[(int)_topFiveColor[i.index]]}{i.index + 1}. {i.item.timeReadable}\t{i.item.turnSetString}", "$ext$"
                }, 50);
            }
        }

        public static List<Scramble> TopScrambles()
        {
            List<Scramble> final = new List<Scramble>(scrambleHistory);
            return final.OrderBy(n => n.time).Take(5).ToList();
        }

        public static string TimeToReadable(double timeSeconds)
        {
            int minutes, seconds, milliseconds;
            string final = "";

            minutes = (int)(timeSeconds / 60);
            seconds = (int)(timeSeconds - (minutes * 60));
            milliseconds = (int)((timeSeconds - Math.Truncate(timeSeconds)) * 1000);

            final += $"{minutes}:{seconds.ToString().PadLeft(2, '0')}.{milliseconds:000}";

            return final;
        }

        
        public static void Load()
        {
            scrambleHistory = (List<Scramble>)SaveLoad.Load(filePath, typeof(List<Scramble>));
        }

        public static void Save()
        {
            SaveLoad.Save(filePath, scrambleHistory);
        }


        public static void PrintStatistics()
        {
            /* Stats to display
             * > Overall Average 
             * > Last 10 Average
             * > Best
             * > Cubes solved
             * > Turns executed
             * > Time cubing
             * 
             * > Hardest turnset
             * > Easiest turnset
             * > Turn count
             */

            if(scrambleHistory.Count < 1)
            {
                Dialogue.ColoredPrint("There is no data to display!", ConsoleColor.Cyan);
                return;
            }

            List<Turns> _allTurns = scrambleHistory.SelectMany(n => n.turnSet).ToList();

            Dialogue.TimedDialogue(new string[] {
                $"$col$b\tOverall Average : {TimeToReadable(scrambleHistory.Select(n => n.timeSeconds).Sum() / scrambleHistory.Count)}",
                $"$col$a\tLast 10 Average : {TimeToReadable(scrambleHistory.TakeLast(10).ToList().Select(n => n.timeSeconds).Sum() / scrambleHistory.TakeLast(10).Count())}",
                $"$col$e\t           Best : {topScramble.timeReadable}",
                $"$col$d\t   Cubes solved : {scrambleHistory.Count}",
                $"$col$d\t Turns executed : {scrambleHistory.Select(n => n.turnSet.Count).Sum()}",
                $"$col$d\t    Time cubing : {TimeToReadable(scrambleHistory.Select(n => n.timeSeconds).Sum())}",
                "",
                $"$col$b\tEasiest turnset : {topScramble.turnSetString}",
                $"$col$b\tHardest turnset : {scrambleHistory.OrderBy(n => n.time).Reverse().ToList()[0].turnSetString}",
                $"$col$b\t     Turn count : {string.Join(' ', Enum.GetValues(typeof(Turns)).Cast<Turns>().Select(n => $"{n}:{_allTurns.Where(i => i == n).Count()}"))}"}, 50);
        }
        #endregion

        public enum Turns
        {
            F,
            B,
            U,
            D,
            R,
            L,
            Fc,
            Bc,
            Uc,
            Dc,
            Rc,
            Lc
        }
    }
}
