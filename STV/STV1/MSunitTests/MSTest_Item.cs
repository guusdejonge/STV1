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
    public class MSTest_Item
    {
        [TestMethod]
        public void MSTest_items_useItemTwice()
        {
            Crystal c = new Crystal(DateTime.Now.Millisecond);

            Player p = new Player(DateTime.Now.Millisecond);

          

            c.use(p);
            c.use(p);

            

            Assert.IsTrue(p.accelerated);
            Assert.IsTrue(c.used);
        }

        [TestMethod]
        public void MSTest_useItemOnce()
        {
            Crystal c = new Crystal(DateTime.Now.Millisecond);

            Player p = new Player(DateTime.Now.Millisecond);

            c.use(p);

            Assert.IsTrue(p.accelerated);
            Assert.IsTrue(c.used);
        }

        [TestMethod]
        public void MSTest_useCrystalInBridge()
        {
            Crystal c = new Crystal(DateTime.Now.Millisecond);

            Player p = new Player(DateTime.Now.Millisecond);

            Bridge b = new Bridge(3);

            var d = new Dungeon(3, 3,3, 1);

            p.dungeon = d;
            p.location = b;

            c.use(p);
            c.use(p);



            Assert.IsTrue(p.accelerated);
            Assert.IsTrue(c.used);
            Assert.IsTrue(b.fromNodes.Count()==0);
        }


    }
}
