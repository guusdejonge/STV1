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
    public class MSTest_Bridge
    {
        [TestMethod]
        public void MSTest_bridge_fromNodes()
        {
            Bridge b = new Bridge(3);

            Node n = new Node(3);

            b.connectToNodeOfSameZone(n);

            Assert.IsTrue(b.fromNodes.Contains(n));
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
