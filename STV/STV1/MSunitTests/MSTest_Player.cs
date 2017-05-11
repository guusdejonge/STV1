using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
//using STVRogue.GameLogic;

namespace STVRogue.GameLogic
{

    /* An example of a test class written using VisualStudio's own testing
     * framework. 
     * This one is to unit-test the class Player. The test is incomplete though, 
     * as it only contains two test cases. 
     */
    [TestClass]
    public class MSTest_Player
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MSTest_use_onEmptyBag()
        {
            Player P = new Player();
            P.use(new Item());
        }

        [TestMethod]
        public void MSTest_use_item_in_bag()
        {
            Player P = new Player();
            Item x = new HealingPotion("pot1");
            P.bag.Add(x);
            P.use(x);
            Assert.IsFalse(P.bag.Contains(x));
        }

        [TestMethod]
        public void MSTest_attack_one_monster_low_HP()
        {
            //ARRANGE
            var pack = new Mock<Pack>(1);
            var monster = new Mock<Monster>();

            var monsters = new List<Monster>();
            var player = new Player();

            monsters.Add(monster.Object);

            pack.SetupAllProperties();
            pack.Object.members = monsters;

            monster.SetupAllProperties();
            monster.Object.HP = 4;
            monster.Object.pack = pack.Object;

            //ACT
            player.Attack(monster.Object);

            //ASSERT
            Assert.AreEqual(1, player.KillPoint);
        }

        [TestMethod]
        public void MSTEST_attack_one_monster_high_HP()
        {
            //ARRANGE
            var pack = new Mock<Pack>(1);
            var monster = new Mock<Monster>();

            var monsters = new List<Monster>();
            var player = new Player();

            monsters.Add(monster.Object);

            pack.SetupAllProperties();
            pack.Object.members = monsters;

            monster.SetupAllProperties();
            monster.Object.HP = 10;
            monster.Object.pack = pack.Object;

            //ACT
            player.Attack(monster.Object);

            //ASSERT
            Assert.AreEqual(0, player.KillPoint);
        }
    }
}
