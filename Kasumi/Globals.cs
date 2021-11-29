using System;
using Kasumi.Telemetry;

namespace Kasumi
{
    public class Globals
    {
        public static string Token { get; set; }
        
        public static string Prefix { get; set; }
        
        public static DateTime StartTime { get; set; }

        public static bool Dev { get; set; }

        public static TelemetryClient TelemetryClient { get; set; }
    }
}
