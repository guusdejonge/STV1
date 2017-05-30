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
    public class MSTest_Pack
    {
        [TestMethod]
        public void MSTest_empty_pack_members()
        {
            Pack P = new Pack(0);
            Assert.IsTrue(P.GetMembers().Count() == 0);
        }
        
        [TestMethod]
        public void MSTest_pack_members()
        {
            Random rnd = new Random();
            int rndInt = rnd.Next(1, 10);
            Pack P = new Pack(rndInt);
            Assert.IsTrue(P.GetMembers().Count() == rndInt);
        }

        [TestMethod]
        public void MSTest_empty_pack_startingHP()
        {
            Pack P = new Pack(0);
            Assert.IsTrue(P.GetStartingHP() == 0);
        }

        [TestMethod]
        public void MSTest_pack_startingHP()
        {
            Random rnd = new Random();
            int rndInt = rnd.Next(1, 10);
            Pack P = new Pack(rndInt);

            int totalMemberHP = 0;
            foreach(Monster m in P.GetMembers())
            {
                totalMemberHP += m.HP;
            }

            Assert.IsTrue(P.GetStartingHP() == totalMemberHP);
        }

        [TestMethod]
        public void MSTest_pack_attack_kill()
        {
            Pack pack = new Pack(3);

            var player = new Mock<Player>();

            player.SetupAllProperties();
            player.Object.HP = 2;

            pack.Attack(player.Object);

            Assert.IsTrue(player.Object.HP == 0);
        }

        [TestMethod]
        public void MSTest_pack_attack_dont_kill()
        {
            Pack pack = new Pack(3);

            var player = new Mock<Player>();

            player.SetupAllProperties();
            player.Object.HP = 10;

            pack.Attack(player.Object);

            Assert.IsTrue(player.Object.HP > 0);
        }

        [TestMethod]
        public void MSTest_pack_move()
        {
            Pack p = new Pack(3);

            Dungeon d = new Dungeon(2, 10, 0, DateTime.Now.Millisecond);

            Node start = d.startNode;
            Node end = start.neighbors[0];

            start.packs.Add(p);
            p.dungeon = d;
            p.location = start;

            p.move(end);

            Assert.IsTrue(p.location == end);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MSTest_pack_moveToNoNeighbor()
        {
            Pack p = new Pack(3);

            Dungeon d = new Dungeon(2, 10, 0, DateTime.Now.Millisecond);

            Node start = d.startNode;
            Node end = new Node(3);
            start.packs.Add(p);
            p.dungeon = d;
            p.location = start;

            p.move(end);

        }

        [TestMethod]
        public void MSTest_pack_moveFullNode()
        {
            Pack p = new Pack(3);

            Dungeon d = new Dungeon(2, 1, 0, DateTime.Now.Millisecond);

            Node start = d.startNode;
            Node end = start.neighbors[0];

            start.packs.Add(p);
            p.dungeon = d;
            p.location = start;

            p.move(end);

            Assert.IsTrue(p.location == start);
        }


        [TestMethod]
        public void MSTest_pack_movetowards_same_zone()
        {
            Pack p = new Pack(3);

            Dungeon d = new Dungeon(2, 10, 0, DateTime.Now.Millisecond);

            Node start = d.startNode;
            Node end = d.zones[0].nodes.Last();     //first node of last zone as ending point

            start.packs.Add(p);
            p.dungeon = d;
            p.location = start;

            p.moveTowards(end);

            Assert.IsTrue(p.location == p.path[1]);
        }
    }
}
