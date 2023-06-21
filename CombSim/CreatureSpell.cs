using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CombSimTest")]

namespace CombSim
{
    public partial class Creature
    {
        internal Spell ConcentratingOnSpell; // What spell the creature is concentrating on if any

        public virtual bool CanCastSpell(Spell spell)
        {
            throw new NotImplementedException();
        }

        public Spell ConcentratingOn()
        {
            return ConcentratingOnSpell;
        }

        public void ConcentrateOn(Spell spell)
        {
            ConcentratingOnSpell = spell;
            OnTakingDamage += MakeConcentrationCheck;
            Console.WriteLine($"// {Name} ConcentrateOn {spell.Name()}");
        }

        public void MakeConcentrationCheck(object sender, OnTakingDamageEventArgs e)
        {
            Console.WriteLine($"// {Name} MakeConcentrationCheck sender={sender} dmg={e.Damage.hits}");
            if (ConcentratingOnSpell == null)
                return;
            var diff = Math.Max(10, e.Damage.hits / 2);
            if (MakeSavingThrow(StatEnum.Constitution, diff, out var roll))
            {
                Console.WriteLine($"// {Name} succeeded Concentration check ({roll} vs DC {diff})");
                return;
            }

            Console.WriteLine(
                $"// {Name} failed Concentration check ({roll} vs DC {diff}) - {ConcentratingOnSpell.Name()} ending");
            ConcentratingOnSpell?.EndConcentration(this);
            ConcentratingOnSpell = null;
        }

        public virtual void DoCastSpell(Spell spell)
        {
            throw new NotImplementedException();
        }

        // Add spell to a character so that they can cast it
        protected void AddSpell(Spell spell)
        {
            _spells[spell.Name()] = spell;
            AddAction(spell);
        }

        public int SpellAttackModifier()
        {
            return ProficiencyBonus + SpellModifier();
        }

        public int SpellModifier()
        {
            return Stats[SpellCastingAbility].Bonus();
        }

        public int SpellSaveDc()
        {
            return 8 + SpellAttackModifier();
        }

        public Spell GetSpell(string name)
        {
            if (_spells.ContainsKey(name))
            {
                return _spells[name];
            }

            return null;
        }
    }
}