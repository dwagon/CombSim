using System;

namespace CombSim
{
    public enum LogLevelEnum
    {
        Dice = 1,
        Info = 2,
        Action = 3
    }

    public static class NarrationLog
    {
        public static void LogMessage(string message, LogLevelEnum level = LogLevelEnum.Info)
        {
            Console.WriteLine(message);
        }
    }
}