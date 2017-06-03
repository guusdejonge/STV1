using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
            Dungeon d = new Dungeon(2, 10, 10, DateTime.Now.Millisecond);
            Assert.IsTrue(d.zones.Count() == 3);
        }

        [TestMethod]
        public void MSTest_dungeon_multiple_levels()
        {
            Dungeon d = new Dungeon(10, 10, 10, DateTime.Now.Millisecond);
            Assert.IsTrue(d.zones.Count() == 11);
        }

        [TestMethod]
        public void MSTest_dungeon_monsters_distribution_no_rest()
        {
            Dungeon d = new Dungeon(2, 10, 18, DateTime.Now.Millisecond);
            bool z1 = d.zones[0].monstersInZone == 3;
            bool z2 = d.zones[1].monstersInZone == 6;
            bool z3 = d.zones[2].monstersInZone == 9;
            Assert.IsTrue(z1 && z2 && z3);
        }

        [TestMethod]
        public void MSTest_dungeon_monsters_distribution_rest()
        {
            Dungeon d = new Dungeon(2, 10, 17, DateTime.Now.Millisecond);
            bool z1 = d.zones[0].monstersInZone == 2;
            bool z2 = d.zones[1].monstersInZone == 5;
            bool z3 = d.zones[2].monstersInZone == 10;
            Assert.IsTrue(z1 && z2 && z3);
        }

        [TestMethod]
        public void MSTest_dungeon_disconnect()
        {
            var zone1 = new Mock<Zone>(1, 1, null, DateTime.Now.Millisecond);
            zone1.SetupAllProperties();
            var zone2 = new Mock<Zone>(2, 2, null, DateTime.Now.Millisecond);
            zone2.SetupAllProperties();

            Dungeon dungeon = new Dungeon(2, 2, 2, DateTime.Now.Millisecond);
            dungeon.zones.Clear();

            var zones = new List<Zone>();

            Node start = new Node(1);
            Node node1 = new Node(2);
            Node node2 = new Node(3);
            Node node3 = new Node(4);
            Node node4 = new Node(5);

            start.connect(node1);
            start.connect(node2);
            start.connect(node3);
            node3.connect(node4);

            zone1.Object.nodes.Clear();
            zone1.Object.nodes.Add(start);
            zone1.Object.nodes.Add(node1);
            zone1.Object.nodes.Add(node2);
            zone1.Object.nodes.Add(node3);
            zone1.Object.nodes.Add(node4);

            Bridge brug = new Bridge(6);
            zone1.Object.nodes.Add(brug);
            brug.connectToNodeOfSameZone(node1);
            brug.connectToNodeOfSameZone(node2);

            Node node5 = new Node(7);
            Node node6 = new Node(8);

            brug.connectToNodeOfNextZone(node5);
            brug.connectToNodeOfNextZone(node6);

            zone2.Object.nodes.Clear();
            zone2.Object.nodes.Add(node5);
            zone2.Object.nodes.Add(node6);


            zones.Add(zone1.Object);
            zones.Add(zone2.Object);

            dungeon.zones = zones;

            dungeon.disconnect(brug);

            Assert.IsFalse(brug.neighbors.Contains(node1));
            Assert.IsFalse(brug.neighbors.Contains(node2));
        }

        [TestMethod]
        public void MSTest_dungeon_level()
        {
            Dungeon dungeon = new Dungeon(2, 2, 2, DateTime.Now.Millisecond);

            var brug = dungeon.zones.First().nodes.First(q => q is Bridge);
            var node = dungeon.zones.First().nodes.First();

            var result1 = dungeon.level(brug);
            var result2 = dungeon.level(node);

            Assert.AreEqual(1,result1);
            Assert.AreEqual(0,result2);
        }
    }
}
