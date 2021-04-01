using System;

namespace Kasumi
{
    public class Globals
    {
        // Incorporates the bot's configuration and exit code for bootstrapping.
        public static int ExitCode { get; set; }
        public static string Token { get; set; }
        public static string Prefix { get; set; }
        public static DateTime StartTime { get; set; }
        public static string OsuKey { get; set; }
        public static string AiKey { get; set; }
        public static bool Dev { get; set; }
    }
}
