namespace CombSim
{
    public class Character : Creature
    {
        protected int Level;

        public Character(string name, string team = "Characters") : base(name, team)
        {
        }

        protected override void FallenUnconscious()
        {
            NarrationLog.LogMessage($"{Name} has fallen unconscious");
            Conditions.SetCondition(ConditionEnum.Unconscious);
            Conditions.RemoveCondition(ConditionEnum.Ok);
            HitPoints = 0;
        }
    }
}