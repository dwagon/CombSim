using CombSim;
using NUnit.Framework;

namespace CombSimTest
{
    [TestFixture]
    public class ActionTest
    {
        [Test]
        public void TestMisc()
        {
            var test = new TestAction();

            Assert.AreEqual("Testing", test.Name());
        }

        [Test]
        public void TestPerformAction()
        {
            var test = new TestAction();
            var critter = new Critter();
            test.PerformAction(critter);
            Assert.True(critter.HasCondition(ConditionEnum.Invisible));
        }
    }

    public class TestAction : Action
    {
        public TestAction() : base("Testing", ActionCategory.Action)
        {
        }

        protected override bool PreAction(Creature actor)
        {
            return true;
        }

        protected override void DoAction(Creature actor)
        {
        }

        protected override void PostAction(Creature actor)
        {
            actor.AddCondition(ConditionEnum.Invisible);
        }
    }

    public class Critter : Creature
    {
        public Critter() : base("Critter")
        {
        }
    }
}