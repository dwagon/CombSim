using System;
using Pastel;

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
            Console.WriteLine(message.Pastel(ConsoleColor.Blue).PastelBg(ConsoleColor.White));
        }
    }

    public class AttackMessage
    {
        public string Attacker;
        public string AttackName;
        public int Mods;
        public string Result;
        public int Roll;
        public string Victim;

        public AttackMessage(string attacker = "", string victim = "", string attackName = "", int roll = 0,
            int mods = 0, string result = "")
        {
            Attacker = attacker;
            Victim = victim;
            AttackName = attackName;
            Roll = roll;
            Mods = mods;
            Result = result;
        }

        public new string ToString()
        {
            string rollMsg = "";
            if (Roll != 0)
            {
                rollMsg = $", rolls {Roll}";
                if (Mods != 0)
                {
                    rollMsg += $" + {Mods}";
                }

                rollMsg += $" = {Roll + Mods}";
            }

            string message =
                $"{Attacker} attacked {Victim} with {AttackName}{rollMsg}: {Result}";
            return message;
        }
    }
}