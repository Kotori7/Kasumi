using System;

namespace Kasumi
{
    public class Globals
    {
        // Incorporates the bot's configuration and exit code for bootstrapping.
        public static string Name = "Kasumi"; // probably not used lol
        public static int ExitCode { get; set; }
        public static string Token { get; set; }
        public static string Prefix { get; set; }
        public static int Happiness { get; set; } // may use at somne point idk
        public static DateTime StartTime { get; set; }
        public static string OsuKey { get; set; }
        public static string AIKey { get; set; }
    }
}
