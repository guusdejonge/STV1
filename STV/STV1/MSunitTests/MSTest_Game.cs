using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.GameLogic;
using Moq;
using STVRogue;

namespace UnitTests_STVRogue
{
    [TestClass]
    public class MSTest_Game
    {
        [TestMethod]
        public void MSTest_create_game()
        {
            Game g = new Game(3, 10, 10);
            Assert.IsTrue(g != null);
        }

        [TestMethod]
        public void MSTest_game_command()
        {
            Game g = new Game(3, 10, 10);

            Command c = new Command("attack");

            var test = g.update(c);

            Assert.IsTrue(test);
        }

        [TestMethod]
        [ExpectedException(typeof(GameCreationException))]
        public void MSTest_game_created_exception()
        {
            Game g = new Game(3, 3, 1000);
        }
    }
}
