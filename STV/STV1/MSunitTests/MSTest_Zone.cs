﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        [TestMethod]
        public void MSTest_zone_averageconnectivity()
        {
            var u = new Mock<UtilsClass>();

            Zone z = new Zone(3, 100);
            z.utils = u.Object;
            u.Setup(m => m.rnd(1, Math.Min(4, z.nodes.Count()))).Returns(Math.Min(4, z.nodes.Count));

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

            for (int i = 0; i < 10; i++)
            {
                int multiplier = 3;
                Zone z = new Zone(multiplier, 100);
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
            Zone z = new Zone(10, 0);

            z.CreateItems();

            bool itemcreated = false;

            foreach(Node n in z.nodes)
            {
                if(n.items.Count()>0)
                {
                    itemcreated = true;
                }
            }

            Assert.IsTrue(itemcreated);
        }

        /*
        [TestMethod]
        public void MSTest_zone_averageconnectivity()
        {
            bool test = true;

            for (int i = 0; i < 10; i++)
            {
                int totalconnections = 0;

                Zone z = new Zone(5, 100);

                foreach(Node n in z.nodes)
                {
                    totalconnections += n.neighbors.Count();
                }

                if(totalconnections * 0.5 / z.nodes.Count() > 3)    //die *0.5 is omdat 1 connection bij 2 nodes in de neighbor lijst staat
                {
                    test = false;
                }
            }

            Assert.IsTrue(test);
        }
        */
    }
}