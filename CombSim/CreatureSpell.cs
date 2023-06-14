using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CombSimTest")]

namespace CombSim
{
    public partial class Creature
    {
        internal Spell _concentration; // What spell the creature is concentrating on if any

        public virtual bool CanCastSpell(Spell spell)
        {
            throw new NotImplementedException();
        }

        public Spell ConcentratingOn()
        {
            return _concentration;
        }

        public void ConcentrateOn(Spell spell)
        {
            _concentration = spell;
        }

        public void MakeConcentrationCheck(object sender, OnTakingDamageEventArgs e)
        {
            var diff = Math.Max(10, e.Damage.hits / 2);
            if (MakeSavingThrow(StatEnum.Constitution, diff, out var roll))
            {
                Console.WriteLine($"Succeeded Concentration check ({roll} vs DC {diff})");
                return;
            }

            Console.WriteLine($"Failed Concentration check ({roll} vs DC {diff}) - {_concentration.Name()} ending");
            _concentration?.EndConcentration();
            _concentration = null;
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