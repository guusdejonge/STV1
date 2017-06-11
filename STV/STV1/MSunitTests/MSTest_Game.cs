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
            Game g = new Game(10, 3, 10000);
        }


        //[TestMethod]
        //public void MSTest_game_alert()
        //{
        //    for (int t = 0; t < 1000; t++)
        //    {



        //        Game g = new Game(3, 10, 10);
        //        Node firstNeighbor = g.player.location.neighbors.First();


        //        Pack p = new Pack(1, DateTime.Now.Millisecond);
        //        p.members[0].HP = 100;
        //        firstNeighbor.packs.Add(p);
        //        p.location = firstNeighbor;
        //        p.dungeon = g.dungeon;

        //        g.player.AttackRating = 10;
        //        g.update(new Command("M " + g.dungeon.zones[0].nodes.IndexOf(firstNeighbor) + " " + g.dungeon.zones.IndexOf(firstNeighbor.zone)));
        //        firstNeighbor.packs.Clear();
        //        if (firstNeighbor.packs.Count() <= 0)
        //        {
        //            firstNeighbor.packs.Add(p);
        //            p.location = firstNeighbor;
        //        }
        //        firstNeighbor.fight(g.player, new Command("A"));
        //        g.movePacks();
        //        Assert.IsTrue(firstNeighbor.alert);
        //    }
        //}

    }
}
