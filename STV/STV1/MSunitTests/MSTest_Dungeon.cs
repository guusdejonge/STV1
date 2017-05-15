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
    public class MSTest_Dungeon
    {
        [TestMethod]
        public void MSTest_dungeon_two_levels()
        {
            Dungeon d = new Dungeon(2, 10, 10);
            Assert.IsTrue(d.zones.Count() == 3);
        }

        [TestMethod]
        public void MSTest_dungeon_multiple_levels()
        {
            Dungeon d = new Dungeon(10, 10, 10);
            Assert.IsTrue(d.zones.Count() == 11);
        }

        [TestMethod]
        public void MSTest_dungeon_monsters_distribution_no_rest()
        {
            Dungeon d = new Dungeon(2, 10, 18);
            bool z1 = d.zones[0].monstersInZone == 3;
            bool z2 = d.zones[1].monstersInZone == 6;
            bool z3 = d.zones[2].monstersInZone == 9;
            Assert.IsTrue(z1 && z2 && z3);
        }

        [TestMethod]
        public void MSTest_dungeon_monsters_distribution_rest()
        {
            Dungeon d = new Dungeon(2, 10, 17);
            bool z1 = d.zones[0].monstersInZone == 2;
            bool z2 = d.zones[1].monstersInZone == 5;
            bool z3 = d.zones[2].monstersInZone == 10;
            Assert.IsTrue(z1 && z2 && z3);
        }
    }
}
