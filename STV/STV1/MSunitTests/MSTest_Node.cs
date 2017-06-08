using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.GameLogic;
using Moq;
using STVRogue;
using STVRogue.Utils;

namespace UnitTests_STVRogue
{
    [TestClass]
    public class MSTest_Node
    {
        [TestMethod]
        public void MSTest_nodes_connectNode()
        {
            Node n = new Node(3);
            Node m = new Node(3);

            n.connect(m);

            Assert.IsTrue(n.neighbors.Contains(m));
            Assert.IsTrue(m.neighbors.Contains(n));
        }

        [TestMethod]
        public void MSTest_nodes_disconnectNode()
        {
            Node n = new Node(3);
            Node m = new Node(3);

            n.connect(m);

            m.disconnect(n);

            Assert.IsFalse(n.neighbors.Contains(m));
            Assert.IsFalse(m.neighbors.Contains(n));
        }

        [TestMethod]
        public void MSTest_nodes_playerFleeFight()
        {
            Game g = new Game(3, 10, 10); 
            Node firstNeighbor = g.player.location.neighbors.First();


            var pack = new Pack(1, DateTime.Now.Millisecond);
            pack.location = firstNeighbor;
            pack.dungeon = g.dungeon;

  
            


            g.update(new Command("M " + g.dungeon.zones[0].nodes.IndexOf(firstNeighbor) + " " + g.dungeon.zones.IndexOf(firstNeighbor.zone)));

            if (firstNeighbor.packs.Count() <= 0)
            {
                firstNeighbor.packs.Add(pack);
                pack.location = firstNeighbor;
            }
            firstNeighbor.contested = true;
            Command flee = new Command("F");
            flee.previousNode = g.prevNode;
            firstNeighbor.fight(g.player, flee);

            Assert.IsFalse(firstNeighbor.contested);
        }

        [TestMethod]
        public void MSTest_nodes_playerUseItems()
        {
            Game g = new Game(3, 10, 10);
            g.player.bag.Add(new Crystal(DateTime.Now.Millisecond));
            g.player.bag.Add(new HealingPotion(DateTime.Now.Millisecond));
            g.player.HP = 10;

            g.update(new Command("U 0"));
            g.update(new Command("U 0"));

            Assert.IsTrue(g.player.bag.Count == 0);
            Assert.IsTrue(g.player.HP > 10);
            Assert.IsTrue(g.player.accelerated);
        }

        [TestMethod]
        public void MSTest_nodes_playerFightWeakPack()
        {
            Game g = new Game(3, 10, 10);
            Node firstNeighbor = g.player.location.neighbors.First();

            if (firstNeighbor.packs.Count() != 0)
            {
                firstNeighbor.packs.Clear();
            }

            Pack p = new Pack(1, DateTime.Now.Millisecond);
            p.members[0].HP = 1;

            p.dungeon = g.dungeon;

            g.player.AttackRating = 10;
            g.update(new Command("M " + g.dungeon.zones[0].nodes.IndexOf(firstNeighbor) + " " + g.dungeon.zones.IndexOf(firstNeighbor.zone)));
            firstNeighbor.packs.Clear();
            if (firstNeighbor.packs.Count() <= 0)
            {
                firstNeighbor.packs.Add(p);
                p.location = firstNeighbor;
            }
            firstNeighbor.fight(g.player, new Command("A"));
            
            Assert.IsFalse(firstNeighbor.contested);
            Assert.IsFalse(firstNeighbor.packs.Contains(p));
        }

        [TestMethod]
        public void MSTest_nodes_fleePackEmptyNode()
        {
            Game g = new Game(3, 10, 10);
            Node firstNeighbor = g.player.location.neighbors.First();

            Pack p = new Pack(1, DateTime.Now.Millisecond);
            p.members[0].HP = 100;
            p.dungeon = g.dungeon;
            p.location = firstNeighbor;

            g.update(new Command("M " + g.dungeon.zones[0].nodes.IndexOf(firstNeighbor) + " " + g.dungeon.zones.IndexOf(firstNeighbor.zone)));

            if (firstNeighbor.packs.Count() > 0)
            {
                firstNeighbor.packs.Clear();
            }

            firstNeighbor.packs.Add(p);
            p.location = firstNeighbor;
            firstNeighbor.contested = true;
            var utils = new Mock<UtilsClass>(DateTime.Now.Millisecond);
            utils.Setup(m => m.fleeProb(p)).Returns(2);
            firstNeighbor.utils = utils.Object;
            g.player.AttackRating = 0;
            foreach (var n in firstNeighbor.neighbors)
                n.packs.Clear();
            firstNeighbor.fight(g.player, new Command("A"));

            Assert.IsFalse(firstNeighbor.packs.Contains(p));
        }

        [TestMethod]
        public void MSTest_nodes_fleePackFullNode()
        {
            //NEED DUNGEON
            var node = new Node(3);
            var fullNode = new Node(1);
            var pack = new Pack(1, DateTime.Now.Millisecond);
            var fullPack = new Pack(1, DateTime.Now.Millisecond);
            var previousNode = new Node(3);

            var utils = new Mock<UtilsClass>(DateTime.Now.Millisecond);
            var dungeon = new Dungeon(1, 1, 1, DateTime.Now.Millisecond);
            var commands = new List<Command>();
            var c = new Command("A");
            commands.Add(c);
            c = new Command("F");
            c.previousNode = previousNode;
            commands.Add(c);

            var player = new Player(DateTime.Now.Millisecond);
            player.AttackRating = 0;

            fullNode.packs.Add(fullPack);
            pack.dungeon = dungeon;
            pack.location = node;
            node.packs.Add(pack);
            node.contested = true;
            node.utils = utils.Object;
            node.neighbors.Add(fullNode);
            utils.Setup(m => m.fleeProb(pack)).Returns(2);

            //node.fight(player, commands);

            Assert.IsTrue(node.packs.Contains(pack));

        }

        //[TestMethod]
        public void MSTest_nodes_fleePackExitNode()
        {
            //NEED DUNGEON
            var node = new Node(3);
            var exitNode = new Node(1);
            var pack = new Pack(1, DateTime.Now.Millisecond);
            var dungeon = new Dungeon(1, 1, 1, DateTime.Now.Millisecond);
            var utils = new Mock<UtilsClass>();
            var commands = new List<Command>();

            var c = new Command("attack");
            commands.Add(c);

            var player = new Player(DateTime.Now.Millisecond);
            player.AttackRating = 0;

            dungeon.exitNode = exitNode;
            pack.dungeon = dungeon;
            node.packs.Add(pack);
            node.contested = true;
            node.utils = utils.Object;
            utils.Setup(m => m.fleeProb(pack)).Returns(2);
            
            //node.fight(player, commands);

            Assert.IsTrue(node.packs.Contains(pack));
        }

        [TestMethod]
        public void MSTest_nodes_fleeSecondPack()
        {
            //NEED DUNGEON
            Node node = new Node(3);
            Pack pack = new Pack(1, DateTime.Now.Millisecond);
            Pack secondPack = new Pack(1, DateTime.Now.Millisecond);
            var emptyNode = new Node(3);
            var previousNode = new Node(3);
            var dungeon = new Dungeon(1, 5, 1, DateTime.Now.Millisecond);
            var utils = new Mock<UtilsClass>(DateTime.Now.Millisecond);


            var commands = new List<Command>();
            var c = new Command("attack");
            commands.Add(c);
            c = new Command("attack");
            commands.Add(c);
            c = new Command("flee");
            c.previousNode = previousNode;
            commands.Add(c);

            var player = new Player(DateTime.Now.Millisecond);
            player.AttackRating = 0;
            pack.dungeon = dungeon;
            pack.location = node;
            node.packs.Add(pack);
            node.packs.Add(secondPack);
            node.contested = true;
            node.utils = utils.Object;
            node.neighbors.Add(emptyNode);
            utils.Setup(m => m.fleeProb(pack)).Returns(2);

            //node.fight(player, commands);

            Assert.IsTrue(node.packs.Contains(secondPack));

        }

        [TestMethod]
        public void MSTest_nodes_gameOver()
        {
            Game g = new Game(3, 10, 10);
            Node firstNeighbor = g.player.location.neighbors.First();

            var pack = new Pack(10, DateTime.Now.Millisecond);
            pack.dungeon = g.dungeon;
            pack.location = firstNeighbor;

            g.update(new Command("M " + g.dungeon.zones[0].nodes.IndexOf(firstNeighbor) + " " + g.dungeon.zones.IndexOf(firstNeighbor.zone)));

            firstNeighbor.packs.Clear();
            firstNeighbor.packs.Add(pack);
            pack.location = firstNeighbor;
            firstNeighbor.contested = true;
            
            g.player.HP = 1;
            g.player.AttackRating = 0;
            firstNeighbor.fight(g.player, new Command("A"));

            Assert.AreEqual(0, g.player.HP);
            Assert.IsFalse(firstNeighbor.contested);
        }
    }
}
