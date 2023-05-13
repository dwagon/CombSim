namespace CombSim.Spells
{
    public class RayOfFrost : ToHitSpell
    {
        public RayOfFrost() : base("Ray of Frost", 0, ActionCategory.Action)
        {
            _reach = 60 / 5;
            _dmgRoll = new DamageRoll("1d8", DamageTypeEnums.Cold);
        }

        protected override void SideEffect(Creature target)
        {
            
        }
        
        public override int GetHeuristic(Creature actor)
        {
            return 3;
        }
    }
}