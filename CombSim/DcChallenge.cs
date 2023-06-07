using System;

namespace CombSim
{
    public class DcChallenge
    {
        private int _difficulty;
        private Action<Creature, Creature> _failed;
        private Action<Creature, Creature> _saved;
        private StatEnum _savingStat;

        public DcChallenge(StatEnum savingStat, int difficulty, Action<Creature, Creature> saved,
            Action<Creature, Creature> failed)
        {
            _savingStat = savingStat;
            _difficulty = difficulty;
            _saved = saved;
            _failed = failed;
        }

        public bool MakeSave(Creature actor, Creature cause, out int roll)
        {
            roll = 0;
            if (actor.MakeSavingThrow(_savingStat, _difficulty, out roll))
            {
                _saved(actor, cause);
                return true;
            }

            _failed(actor, cause);
            return false;
        }
    }
}