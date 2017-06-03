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
            Player P = new Player(DateTime.Now.Millisecond);
            P.use(new Item(DateTime.Now.Millisecond));
        }

        [TestMethod]
        public void MSTest_use_item_in_bag()
        {
            Player P = new Player(DateTime.Now.Millisecond);
            Item x = new HealingPotion(DateTime.Now.Millisecond);
            P.bag.Add(x);
            P.use(x);
            Assert.IsFalse(P.bag.Contains(x));
        }

        [TestMethod]
        public void MSTest_attack_one_monster_low_HP()
        {
            //ARRANGE
            var pack = new Mock<Pack>(1, DateTime.Now.Millisecond);
            var monster = new Mock<Monster>(DateTime.Now.Millisecond);

            var monsters = new List<Monster>();
            var player = new Player(DateTime.Now.Millisecond);

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
            var pack = new Mock<Pack>(1, DateTime.Now.Millisecond);
            var monster = new Mock<Monster>(DateTime.Now.Millisecond);

            var monsters = new List<Monster>();
            var player = new Player(DateTime.Now.Millisecond);

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

        [TestMethod]
        public void MSTEST_attack_accelerated()
        {
            //ARRANGE
            var pack = new Mock<Pack>(2, DateTime.Now.Millisecond);
            var monster = new Mock<Monster>(DateTime.Now.Millisecond);
            var monster2 = new Mock<Monster>((DateTime.Now.Millisecond));

            var monsters = new List<Monster>() { };
            var player = new Player(DateTime.Now.Millisecond);

            player.accelerated = true;

            monsters.Add(monster.Object);
            monsters.Add(monster2.Object);
            
            pack.SetupAllProperties();
            pack.Object.members = monsters;

            monster.SetupAllProperties();
            monster.Object.HP = 3;
            monster.Object.pack = pack.Object;

            monster2.SetupAllProperties();
            monster2.Object.HP = 3;
            monster2.Object.pack = pack.Object;

            //ACT
            player.Attack(monster.Object);

            //ASSERT
            Assert.AreEqual(2, player.KillPoint);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MSTest_attack_noMonster()
        {
            var player = new Player(DateTime.Now.Millisecond);

            var obj = new Mock<Player>(DateTime.Now.Millisecond);

            player.Attack(obj.Object);
        }

    }
}
