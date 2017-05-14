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
        public void MSTest_create_Dungeon()
        {
            Dungeon d = new Dungeon(2, 10, 10);
            Assert.IsTrue(d != null);
        }
    }
}
