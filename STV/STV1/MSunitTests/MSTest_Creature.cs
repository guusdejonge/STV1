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
    public class MSTest_Creature
    {
        [TestMethod]
        public void MSTest_moveToNodeWithPack()
        {
            Player p = new Player();

            //Node n = new Node(3);
            var packs = new List<Pack>();
            Pack pack = new Pack(3);
            packs.Add(pack);

            var n = new Mock<Node>(3);

            n.SetupAllProperties();
            n.Object.packs = packs; ;

            n.Setup(m => m.fight(p, null)).Returns(true);


            p.moveTo(n.Object);


            Assert.AreEqual(p.location, n.Object);
            Assert.IsTrue(n.Object.contested);

        }

        [TestMethod]
        public void MSTest_moveToNodeWithoutPack()
        {
            Player p = new Player();

            Node n = new Node(3);

            p.moveTo(n);

            Assert.AreEqual(p.location, n);
            Assert.IsFalse(n.contested);

        }




    }
}
