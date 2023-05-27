using System;
using System.Text.RegularExpressions;

namespace CombSim
{
    public static class Dice
    {
        public static int Roll(string dice, bool max = false)
        {
            var rnd = new Random();
            var result = 0;
            var rx = new Regex(@"(?<rolls>\d*)d(?<sides>\d+)(?<sign>[\+-])?(?<mod>\d+)?",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var match = rx.Match(dice);

            if (!match.Success)
                throw new Exception("Dice.Roll: Couldn't parse " + dice);

            var rolls = match.Groups["rolls"].Length > 0 ? int.Parse(match.Groups["rolls"].Value) : 1;

            for (var roll = 0; roll < rolls; roll++)
                if (max)
                    result += int.Parse(match.Groups["sides"].Value);
                else
                    result += rnd.Next(1, 1 + int.Parse(match.Groups["sides"].Value)); // End is exclusive, hence +1

            if (match.Groups["sign"].Length > 0)
            {
                if (match.Groups["sign"].Value == "-")
                    result -= int.Parse(match.Groups["mod"].Value);
                else
                    result += int.Parse(match.Groups["mod"].Value);
            }

            return result;
        }

        public static int RollD20(bool hasAdvantage = false, bool hasDisadvantage = false, string reason = "")
        {
            if (hasAdvantage && hasDisadvantage)
            {
                hasAdvantage = false;
                hasDisadvantage = false;
            }

            int roll;
            var d1 = Roll("d20");
            var d2 = Roll("d20");

            if (hasAdvantage)
            {
                roll = Math.Max(d1, d2);
                Console.WriteLine($"// With advantage {roll} = {d1}, {d2}");
            }
            else if (hasDisadvantage)
            {
                roll = Math.Min(d1, d2);
                Console.WriteLine($"// With disadvantage {roll} = {d1}, {d2}");
            }
            else
            {
                roll = d1;
            }

            return roll;
        }
    }
}