using System;

namespace CombSim
{
    public partial class Creature
    {
        public virtual bool CanCastSpell(Spell spell)
        {
            throw new NotImplementedException();
        }

        public virtual void DoCastSpell(Spell spell)
        {
            throw new NotImplementedException();
        }

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