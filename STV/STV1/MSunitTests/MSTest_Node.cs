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
            Node previous = new Node(3);
            Node n = new Node(3);
            n.contested = true;

            var pack = new Pack(1);
            n.packs.Add(pack);

            Player p = new Player();

            List<Command> commands = new List<Command>();

            Command c = new Command("flee");
            commands.Add(c);
            c.previousNode = previous;

            //n.fight(p, commands);

            Assert.IsFalse(n.contested);
        }

        [TestMethod]
        public void MSTest_nodes_playerUseItems()
        {
            Node previous = new Node(3);
            Node n = new Node(3);
            n.contested = true;

            var pack = new Pack(1);
            n.packs.Add(pack);
            Player p = new Player();

            Crystal crystal = new Crystal();
            HealingPotion hp = new HealingPotion();
            p.bag.Add(crystal);
            p.bag.Add(hp);

            List<Command> commands = new List<Command>();

            Command c = new Command("item potion");
            commands.Add(c);
            c = new Command("item crystal");
            commands.Add(c);
            c = new Command("item null");
            commands.Add(c);
            c = new Command("flee");
            c.previousNode = previous;
            commands.Add(c);
            

            //n.fight(p, commands);

            Assert.IsFalse(p.bag.Contains(crystal) && p.bag.Contains(hp));
            Assert.IsTrue(p.accelerated);
        }

        [TestMethod]
        public void MSTest_nodes_playerFightWeakPack()
        {
            var pack = new Mock<Pack>(1);
            var monster = new Mock<Monster>();

            var monsters = new List<Monster>();
            var player = new Player();

            monsters.Add(monster.Object);

            pack.SetupAllProperties();
            pack.Object.members = monsters;

            monster.SetupAllProperties();
            monster.Object.HP = 4;
            monster.Object.pack = pack.Object;

            Node n = new Node(3);
            n.packs.Add(pack.Object);
            n.contested = true;

            List<Command> commands = new List<Command>();
            var c = new Command("attack");
            commands.Add(c);

            //n.fight(player,commands);

            Assert.IsFalse(n.contested);
            Assert.IsFalse(n.packs.Contains(pack.Object));
            
        }

        [TestMethod]
        public void MSTest_nodes_fleePackEmptyNode()
        {
            //NEED DUNGEON
            var node = new Node(3);
            var exitNode = new Node(3);
            var emptyNode = new Node(3);
            var dungeon = new Dungeon(1, 5, 1, DateTime.Now.Millisecond);
            var utils = new Mock<UtilsClass>();

            var pack = new Pack(1);

            var commands = new List<Command>();
            var c = new Command("attack");
            commands.Add(c);

            var player = new Player();
            player.AttackRating = 0;
            dungeon.exitNode = exitNode;
            pack.dungeon = dungeon;
            pack.location = node;
            node.packs.Add(pack);
            node.contested = true;
            node.utils = utils.Object;
            node.neighbors.Add(emptyNode);
            utils.Setup(m => m.fleeProb(pack)).Returns(1);

            //node.fight(player, commands);

            Assert.IsFalse(node.packs.Contains(pack));
        }

        [TestMethod]
        public void MSTest_nodes_fleePackFullNode()
        {
            //NEED DUNGEON
            var node = new Node(3);
            var fullNode = new Node(1);
            var pack = new Pack(1);
            var fullPack = new Pack(1);
            var previousNode = new Node(3);

            var utils = new Mock<UtilsClass>();
            var dungeon = new Dungeon(1, 1, 1, DateTime.Now.Millisecond);
            var commands = new List<Command>();
            var c = new Command("attack");
            commands.Add(c);
            c = new Command("flee");
            c.previousNode = previousNode;
            commands.Add(c);

            var player = new Player();
            player.AttackRating = 0;

            fullNode.packs.Add(fullPack);
            pack.dungeon = dungeon;
            pack.location = node;
            node.packs.Add(pack);
            node.contested = true;
            node.utils = utils.Object;
            node.neighbors.Add(fullNode);
            utils.Setup(m => m.fleeProb(pack)).Returns(1);

            //node.fight(player, commands);

            Assert.IsTrue(node.packs.Contains(pack));

        }

        //[TestMethod]
        public void MSTest_nodes_fleePackExitNode()
        {
            //NEED DUNGEON
            var node = new Node(3);
            var exitNode = new Node(1);
            var pack = new Pack(1);
            var dungeon = new Dungeon(1, 1, 1, DateTime.Now.Millisecond);
            var utils = new Mock<UtilsClass>();
            var commands = new List<Command>();

            var c = new Command("attack");
            commands.Add(c);

            var player = new Player();
            player.AttackRating = 0;

            dungeon.exitNode = exitNode;
            pack.dungeon = dungeon;
            node.packs.Add(pack);
            node.contested = true;
            node.utils = utils.Object;
            utils.Setup(m => m.fleeProb(pack)).Returns(1);
            
            //node.fight(player, commands);

            Assert.IsTrue(node.packs.Contains(pack));
        }

        [TestMethod]
        public void MSTest_nodes_fleeSecondPack()
        {
            //NEED DUNGEON
            Node node = new Node(3);
            Pack pack = new Pack(1);
            Pack secondPack = new Pack(1);
            var emptyNode = new Node(3);
            var previousNode = new Node(3);
            var dungeon = new Dungeon(1, 5, 1, DateTime.Now.Millisecond);
            var utils = new Mock<UtilsClass>();


            var commands = new List<Command>();
            var c = new Command("attack");
            commands.Add(c);
            c = new Command("attack");
            commands.Add(c);
            c = new Command("flee");
            c.previousNode = previousNode;
            commands.Add(c);

            var player = new Player();
            player.AttackRating = 0;
            pack.dungeon = dungeon;
            pack.location = node;
            node.packs.Add(pack);
            node.packs.Add(secondPack);
            node.contested = true;
            node.utils = utils.Object;
            node.neighbors.Add(emptyNode);
            utils.Setup(m => m.fleeProb(pack)).Returns(1);

            //node.fight(player, commands);

            Assert.IsTrue(node.packs.Contains(secondPack));

        }

        [TestMethod]
        public void MSTest_nodes_gameOver()
        {
            var node = new Node(3);
            var pack = new Pack(5);
            var player = new Player();

            player.HP = 4;
            player.AttackRating = 0;
            node.contested = true;
            node.packs.Add(pack);

            var commands = new List<Command>();
            var c = new Command("attack");
            commands.Add(c);

            //node.fight(player, commands);

            Assert.AreEqual(0, player.HP);
            Assert.IsFalse(node.contested);
        }

    }
}
