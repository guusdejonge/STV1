using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.GameLogic;
using STVRogue.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests_STVRogue
{
    [TestClass]
    public class MSTest_Rules
    {
        [TestMethod]
        public void MSTest_Rules_RNode()
        {
            string[] files = { "savedata1.txt", "savedata2.txt" };
            List<GamePlay> plays = loadSavedGamePlays(files);


            Predicate<Game> p = new Predicate<Game>(g => (g.dungeon.zones.Where(z => z.nodes.Any(n => n.getNumberOfMonsters() > n.M)).FirstOrDefault() == null));
            Specification S = new Always(p);



            foreach (GamePlay gp in plays)
            {
                gp.Replay(S);
            }

            Assert.IsTrue(S.getVerdict());
        }

        [TestMethod]
        public void MSTest_Rules_RZone()
        {
            string[] files = { "savedata1.txt", "savedata2.txt" };
            List<GamePlay> plays = loadSavedGamePlays(files);


            Predicate<Game> predicate = new Predicate<Game>(g => (g.dungeon.zones.Where(z => z.nodes.Any(n => n.packs.Any(p => p.lastZone != p.location.zone))).FirstOrDefault() == null));
            Specification S = new Always(predicate);



            foreach (GamePlay gp in plays)
            {
                gp.Replay(S);
            }

            Assert.IsTrue(S.getVerdict());
        }

        //[TestMethod]
        //public void MSTest_Rules_RAlert()
        //{
        //    string[] files = { "savedata1.txt", "savedata2.txt" };
        //    List<GamePlay> plays = loadSavedGamePlays(files);

        //    Predicate<Game> p1 = new Predicate<Game>(g => g.dungeon.zones.Where(z => z.nodes.Any(n => n.packs.Any(p => p.lastDistToPlayer < p.distToPlayer))).FirstOrDefault() == null);
        //    Predicate<Game> p2 = new Predicate<Game>(
        //        g => g.dungeon.zones.Where(
        //            z => z.nodes.Any(
        //                n => n.packs.Any(
        //                    p => p.lastDistToPlayer < p.distToPlayer))
        //             && z.nodes.Any(
        //                        n => n == g.player.location))
        //                        .FirstOrDefault().nodes.Any(n => n.alert == true));
        //    Specification S = new Unless(p1, p2);



        //    foreach (GamePlay gp in plays)
        //    {
        //        gp.Replay(S);
        //    }

        //    Assert.IsTrue(S.getVerdict());
        //}

        [TestMethod]
        public void MSTest_Rules_REndZone()
        {
            string[] files = { "savedata1.txt", "savedata2.txt" };
            List<GamePlay> plays = loadSavedGamePlays(files);


            Predicate<Game> p1 = new Predicate<Game>(g=>g.player.location.zone == g.dungeon.zones.Last());
            Predicate<Game> p2 = new Predicate<Game>(g => g.dungeon.zones.Last().nodes.All(n=>n.packs.All(p=>p.distToPlayer<p.lastDistToPlayer)));
            Specification S = new LeadsTo(p1,p2);



            foreach (GamePlay gp in plays)
            {
                gp.Replay(S);
            }

            Assert.IsTrue(S.getVerdict());
        }

        public List<GamePlay> loadSavedGamePlays(string[] files)
        {
            List<GamePlay> plays = new List<GamePlay>();
            foreach (String s in files)
            {
                GamePlay g = new GamePlay();
                g.Load(s);
                plays.Add(g);
            }
            return plays;
        }
    }
}
