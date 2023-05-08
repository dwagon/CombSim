using System;
using System.Text.RegularExpressions;

namespace CombSim
{
    public static class Dice
    {
        public static int Roll(string dice)
        {
            var rnd = new Random();
            var result = 0;
            var rx = new Regex(@"(?<rolls>\d*)d(?<sides>\d+)(?<sign>[\+-])?(?<mod>\d+)?",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var match = rx.Match(dice);

            if (!match.Success)
                throw new Exception("Dice.Roll: Couldn't parse " + dice);

            var rolls = match.Groups["rolls"].Length>0 ?Int32.Parse(match.Groups["rolls"].Value):1;

            for (var roll = 0; roll < rolls; roll++)
            {
                result += rnd.Next(1, Int32.Parse(match.Groups["sides"].Value));
            }

            if (match.Groups["sign"].Length>0)
            {
                if (match.Groups["sign"].Value == "-")
                    result -= Int32.Parse(match.Groups["mod"].Value);
                else
                    result += Int32.Parse(match.Groups["mod"].Value);
            }

            return result;
        }

        public static int RollD20(bool hasAdvantage=false, bool hasDisadvantage=false)
        {
            if (hasAdvantage && hasDisadvantage)
            {
                hasAdvantage = false;
                hasDisadvantage = false;
            }

            var roll = Roll("d20");
            if (hasAdvantage) roll = Math.Max(Roll("d20"), Roll("d20"));
            if (hasDisadvantage) roll = Math.Min(Roll("d20"), Roll("d20"));

            return roll;
        }
    }
}