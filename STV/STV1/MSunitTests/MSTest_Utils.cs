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
        public void MSTest_utils_shortestPath()
        {
            var utils = new UtilsClass();

            var zone1 = new Mock<Zone>(2,2);
            zone1.SetupAllProperties();
            var zone2 = new Mock<Zone>(2, 2);
            zone2.SetupAllProperties();

            Node start = new Node(3);
            Node node1 = new Node(3);
            Node node2 = new Node(3);
            Node node3 = new Node(3);
        }

        [TestMethod]
        public void MSTest_bridge_toNodes()
        {
            Bridge b = new Bridge(3);

            Node n = new Node(3);

            b.connectToNodeOfNextZone(n);

            Assert.IsTrue(b.toNodes.Contains(n));
        }


    }
}
