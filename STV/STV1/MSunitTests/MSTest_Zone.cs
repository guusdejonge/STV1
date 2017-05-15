using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using STVRogue.GameLogic;
using STVRogue.Utils;
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
            Zone z = new Zone(10, 0, null);
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
            Zone z = new Zone(10, 10, null);
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

        [TestMethod]
        public void MSTest_zone_averageconnectivity()
        {
            var u = new Mock<UtilsClass>();
            u.Setup(m => m.rnd(1, 4)).Returns(4);

            Zone z = new Zone(3, 0, u.Object);
            
            int totalconnections = 0;

            foreach (Node n in z.nodes)
            {
                totalconnections += n.neighbors.Count();
            }

            Assert.IsTrue(totalconnections * 0.5 / z.nodes.Count() < 3);    //die *0.5 is omdat 1 connection bij 2 nodes in de neighbor lijst staat
        }
        
        [TestMethod]
        public void MSTest_zone_multiplier()
        {
            bool test = true;

            for (int i = 0; i < 100; i++)
            {
                int multiplier = 3;
                Zone z = new Zone(multiplier, 10, null);
                z.CreatePacks();

                foreach(Node n in z.nodes)
                {
                    foreach (Pack p in n.packs)
                    {
                        if (p.members.Count() > multiplier)
                        {
                            test = false;
                        }
                    }
                }
            }

            Assert.IsTrue(test);
        }

        [TestMethod]
        public void MSTest_zone_createitem()
        {
            Zone z = new Zone(10, 0, null);

            z.CreateItems();

            bool itemcreated = false;

            foreach(Node n in z.nodes)
            {
                if(n.items.Count() > 0)
                {
                    itemcreated = true;
                }
            }

            Assert.IsTrue(itemcreated);
        }
    }
}
