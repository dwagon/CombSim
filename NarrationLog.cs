using System;

namespace CombSim
{
    public enum LogLevelEnum
    {
        Info = 1,
        Action = 2
    }

    public static class NarrationLog
    {
        public static void LogMessage(string message, LogLevelEnum level = LogLevelEnum.Info)
        {
            Console.WriteLine(message);
        }
    }
}