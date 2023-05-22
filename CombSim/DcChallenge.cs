using System;

namespace CombSim
{
    public class DcChallenge
    {
        protected int Difficulty;
        protected Action<Creature, Creature> Failed;
        protected Action<Creature, Creature> Saved;
        protected StatEnum SavingStat;

        public DcChallenge(StatEnum savingStat, int difficulty, Action<Creature, Creature> saved,
            Action<Creature, Creature> failed)
        {
            SavingStat = savingStat;
            Difficulty = difficulty;
            Saved = saved;
            Failed = failed;
        }

        public bool MakeSave(Creature actor, Creature cause, out int roll)
        {
            roll = 0;
            if (actor.MakeSavingThrow(SavingStat, Difficulty, out roll))
            {
                Saved(actor, cause);
                return true;
            }

            Failed(actor, cause);
            return false;
        }
    }
}