using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.GameLogic;
using Moq;
using STVRogue.Utils;

namespace UnitTests_STVRogue
{
    [TestClass]
    public class MSTest_Utils
    {
        [TestMethod]
        public void MSTest_utils_shortestPathInZone()
        {
            var utils = new UtilsClass(DateTime.Now.Millisecond);

            var zone1 = new Mock<Zone>(1, 1, null, DateTime.Now.Millisecond);
            zone1.SetupAllProperties();

            var zones = new List<Zone>();

            Node start = new Node(1, zone1.Object, 0);
            Node node1 = new Node(2, zone1.Object, 1);
            Node node2 = new Node(3, zone1.Object, 2);
            Node node3 = new Node(4, zone1.Object, 3);
            Node node4 = new Node(5, zone1.Object, 4);

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

            zones.Add(zone1.Object);

            var result = utils.shortestPath(start, node4,zones);
            var expected = new List<Node>() { start, node3, node4 };

            Assert.AreEqual(expected[0], result[0]);
            Assert.AreEqual(expected[1], result[1]);
            Assert.AreEqual(expected[2], result[2]);

        }

        [TestMethod]
        public void MSTest_utils_shortestPathBetweenZones()
        {
            var utils = new UtilsClass(DateTime.Now.Millisecond);

            var zone1 = new Mock<Zone>(1, 1, null, DateTime.Now.Millisecond);
            zone1.SetupAllProperties();
            var zone2 = new Mock<Zone>(2, 2, null, DateTime.Now.Millisecond);
            zone2.SetupAllProperties();

            var zones = new List<Zone>();

            Node start = new Node(1, zone1.Object, 0);
            Node node1 = new Node(2, zone1.Object, 1);
            Node node2 = new Node(3, zone1.Object, 2);
            Node node3 = new Node(4, zone1.Object, 3);
            Node node4 = new Node(5, zone1.Object, 4);

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

            Bridge brug = new Bridge(6, zone1.Object, 0);
            zone1.Object.nodes.Add(brug);
            brug.connectToNodeOfSameZone(node1);
            brug.connectToNodeOfSameZone(node2);

            Node node5 = new Node(7, zone2.Object, 5);
            Node node6 = new Node(8, zone2.Object, 6);

            brug.connectToNodeOfNextZone(node5);
            brug.connectToNodeOfNextZone(node6);

            zone2.Object.nodes.Clear();
            zone2.Object.nodes.Add(node5);
            zone2.Object.nodes.Add(node6);


            zones.Add(zone1.Object);
            zones.Add(zone2.Object);

            var result = utils.shortestPath(start, node6, zones).Distinct().ToList();
            var expected = new List<Node>() { start, node1, brug,node6 };

            Assert.AreEqual(expected[0], result[0]);
            Assert.AreEqual(expected[1], result[1]);
            Assert.AreEqual(expected[2], result[2]);
            Assert.AreEqual(expected[3], result[3]);
        }
    }
}
