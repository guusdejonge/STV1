using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.GameLogic;

namespace UnitTests_STVRogue
{
    [TestClass]
    public class MSTest_Pack
    {
        [TestMethod]
        public void MSTest_empty_pack_members()
        {
            Pack P = new Pack(0);
            Assert.IsTrue(P.members.Count() == 0);
        }
        
        [TestMethod]
        public void MSTest_pack_members()
        {
            Random rnd = new Random();
            int rndInt = rnd.Next(1, 10);
            Pack P = new Pack(rndInt);
            Assert.IsTrue(P.members.Count() == rndInt);
        }

        [TestMethod]
        public void MSTest_empty_pack_startingHP()
        {
            Pack P = new Pack(0);
            Assert.IsTrue(P.startingHP == 0);
        }

        [TestMethod]
        public void MSTest_pack_startingHP()
        {
            Random rnd = new Random();
            int rndInt = rnd.Next(1, 10);
            Pack P = new Pack(rndInt);

            int totalMemberHP = 0;
            foreach(Monster m in P.members)
            {
                totalMemberHP += m.HP;
            }

            Assert.IsTrue(P.startingHP == totalMemberHP);
        }
        
    }
}
