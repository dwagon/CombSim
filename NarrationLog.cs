using System;

namespace CombSim
{
    public enum LogLevelEnum {
        Info,
        Action
    }
    public static class NarrationLog
    {
        public static void LogMessage(string message, LogLevelEnum level = LogLevelEnum.Info )
        {
            Console.WriteLine(message);
        }
    }
}