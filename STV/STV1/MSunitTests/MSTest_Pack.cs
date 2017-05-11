using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.GameLogic;
using Moq;

namespace UnitTests_STVRogue
{
    [TestClass]
    public class MSTest_Pack
    {
        [TestMethod]
        public void MSTest_empty_pack_members()
        {
            Pack P = new Pack(0);
            Assert.IsTrue(P.GetMembers().Count() == 0);
        }
        
        [TestMethod]
        public void MSTest_pack_members()
        {
            Random rnd = new Random();
            int rndInt = rnd.Next(1, 10);
            Pack P = new Pack(rndInt);
            Assert.IsTrue(P.GetMembers().Count() == rndInt);
        }

        [TestMethod]
        public void MSTest_empty_pack_startingHP()
        {
            Pack P = new Pack(0);
            Assert.IsTrue(P.GetStartingHP() == 0);
        }

        [TestMethod]
        public void MSTest_pack_startingHP()
        {
            Random rnd = new Random();
            int rndInt = rnd.Next(1, 10);
            Pack P = new Pack(rndInt);

            int totalMemberHP = 0;
            foreach(Monster m in P.GetMembers())
            {
                totalMemberHP += m.HP;
            }

            Assert.IsTrue(P.GetStartingHP() == totalMemberHP);
        }

        [TestMethod]
        public void MSTest_pack_attack_kill()
        {
            //ARRANGE
            Pack pack = new Pack(3);

            var player = new Mock<Player>();
          
            

            player.SetupAllProperties();
            player.Object.HP = 2;

            pack.Attack(player.Object);

            Assert.IsTrue(player.Object.HP == 0);
        }

        [TestMethod]
        public void MSTest_pack_attack_dont_kill()
        {
            //ARRANGE
            Pack pack = new Pack(3);

            var player = new Mock<Player>();

            player.SetupAllProperties();
            player.Object.HP = 10;

            pack.Attack(player.Object);

            Assert.IsTrue(player.Object.HP > 0);
        }
    }
}
