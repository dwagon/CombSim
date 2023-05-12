using System;

namespace CombSim
{
    public class Character : Creature
    {
        protected int Level { get;  set; }
        private int _deathSavesGood;
        private int _deathSavesBad;

        protected Character(string name, string team = "Characters") : base(name, team)
        {
            _deathSavesGood = 0;
            _deathSavesBad = 0;
        }

        protected override void FallenUnconscious()
        {
            NarrationLog.LogMessage($"{Name} has fallen unconscious");
            Conditions.SetCondition(ConditionEnum.Unconscious);
            Conditions.RemoveCondition(ConditionEnum.Ok);
            HitPoints = 0;
            _deathSavesBad = 0;
            _deathSavesGood = 0;
        }

        protected override void StartTurn()
        {
            base.StartTurn();

            if (Conditions.HasCondition(ConditionEnum.Unconscious) && !Conditions.HasCondition(ConditionEnum.Stable))
            {
                int roll = Dice.RollD20();
                
                if (roll == 1) _deathSavesBad += 2;
                else if (roll <= 10) _deathSavesBad++;
                else if (roll == 20)
                {
                    HitPoints = 1;
                    Conditions.RemoveCondition(ConditionEnum.Unconscious);
                    Conditions.SetCondition(ConditionEnum.Ok);
                }
                else _deathSavesGood++;
                
                if (_deathSavesBad >= 3) Died();
                if (_deathSavesGood >= 3) Conditions.SetCondition(ConditionEnum.Stable);
            }
        }
    }
}