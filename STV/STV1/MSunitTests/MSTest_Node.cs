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

            n.fight(p, commands);

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
            c = new Command("flee");
            c.previousNode = previous;
            commands.Add(c);
            c = new Command("item null");

            n.fight(p, commands);

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

            n.fight(player,commands);

            Assert.IsFalse(n.contested);
            Assert.IsFalse(n.packs.Contains(pack.Object));
            
        }

        [TestMethod]
        public void MSTest_nodes_fleePack()
        {
            var node = new Mock<Node>(3);

        }

    }
}
