using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace ConsoleCuber
{
    class RichPresence
    {
        public static RichPresence Instance = null;
        Discord.Discord userDiscord;
        Activity activity;
        public ActivityManager activityManager;
        Result updateResult;

        public RichPresence()
        {
            userDiscord = new Discord.Discord(811708034870542356, (UInt64)CreateFlags.NoRequireDiscord);
            activityManager = userDiscord.GetActivityManager();
            activity = new Activity
            {
                State = "",
                Details = "",
                Timestamps =
                {
                    Start = DateTimeOffset.Now.ToUnixTimeSeconds()
                },
                Instance = true
            };
        }

        public static void Initialize()
        {
            if (Instance == null)
            {
                Instance = new RichPresence();
            }
        }

        public static void UpdatePresence(Scramble scramble = null)
        {
            if (Program.state == Program.State.Playing)
            {
                if (scramble.startTime > 0d)
                {
                    Instance.activity.State = $"Solving";
                }
                else
                {
                    Instance.activity.State = $"Scrambling";
                }
            }
            else
            {
                Instance.activity.State = Program.state.ToString();
            }
            Instance.activity.Details = $"Average : {Scramble.AverageTime()}";

            switch (Program.state)
            {
                case Program.State.Playing:
                    Instance.activity.Details = scramble.turnSetString;
                    Instance.activity.Assets.LargeImage = "scramblediconbig";
                    break;
                default:
                    Instance.activity.Assets.LargeImage = "idlebig";
                    break;
            }
            Instance.activity.Assets.LargeText = Program.state.ToString();

            Instance.activity.Assets.SmallImage = "iconbig";
            Instance.activity.Assets.SmallText = "ConsoleCuber by Asianerd";

            for (int i = 0; i < 10; i++) // Updates ten times for good measure
            {
                Instance.activityManager.UpdateActivity(Instance.activity, result =>
                    {
                        Instance.updateResult = result;
                    }
                );
                Instance.userDiscord.RunCallbacks();
            }
        }

        public static void AnnounceState()
        {
            if (Instance.updateResult != Result.Ok)
            {
                Dialogue.ColoredPrint($"Error encountered when calling back Discord RPC! {Instance.updateResult}",ConsoleColor.Red);
            }
            else
            {
                Dialogue.ColoredPrint($"No problems with Discord RPC; All systems go!", ConsoleColor.Green);
            }
        }
    }
}
