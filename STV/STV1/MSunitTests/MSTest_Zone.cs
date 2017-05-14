using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests_STVRogue
{
    [TestClass]
    public class MSTest_Zone
    {
        [TestMethod]
        public void MSTest_zone_zero_monsters()
        {
            Zone z = new Zone(10, 0);
            int amountOfMonsters = 0;
            foreach(Node n in z.nodes)
            {
                foreach(Pack p in n.packs)
                {
                    amountOfMonsters += p.members.Count();
                }
            }
            Assert.IsTrue(amountOfMonsters == 0);
        }

        [TestMethod]
        public void MSTest_zone_multiple_monsters()
        {
            Zone z = new Zone(10, 10);
            z.CreatePacks();

            int amountOfMonsters = 0;
            foreach (Node n in z.nodes)
            {
                foreach (Pack p in n.packs)
                {
                    amountOfMonsters += p.members.Count();
                }
            }
            Assert.IsTrue(amountOfMonsters == 10);
        }
    }
}
