// https://www.dndbeyond.com/monsters/zombie

using System;
using System.Linq;

namespace CombSim.Monsters
{
    public class Zombie : Monster
    {
        public Zombie(string name, string team = "Zombies") : base(name, team)
        {
            Stats.Add(StatEnum.Strength, new Stat(13));
            Stats.Add(StatEnum.Dexterity, new Stat(6));
            Stats.Add(StatEnum.Constitution, new Stat(16));
            Stats.Add(StatEnum.Intelligence, new Stat(3));
            Stats.Add(StatEnum.Wisdom, new Stat(6));
            Stats.Add(StatEnum.Charisma, new Stat(5));
            Repr = "z";
            ArmourClass = 8;
            HitDice = "3d8+9";
            Immune.Add(DamageTypeEnums.Poison);
            AddAction(new ZombieSlam());
        }

        protected override void FallenUnconscious()
        {
            var lastDamage = DamageReceived.Last();
            if(lastDamage.type != DamageTypeEnums.Radiant && MakeSavingThrow(StatEnum.Constitution, 5+lastDamage.hits))
            {
                HitPoints = 1;
                Console.WriteLine($"// {Name} uses Undead Fortitude to not fall unconscious");
            }
            else
            {
                base.FallenUnconscious();
            }
            
        }

        private class ZombieSlam : MeleeAttack
        {
            public ZombieSlam() : base("Slam", new DamageRoll("1d6", DamageTypeEnums.Bludgeoning)) {} 
        }
    }
}