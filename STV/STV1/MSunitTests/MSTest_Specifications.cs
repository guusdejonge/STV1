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
    public class MSTest_Specifications
    {
        [TestMethod]
        public void MSTest_Specifications_never_negative()
        {
            string[] files = {"savedata1.txt", "savedata2.txt"};
            List<GamePlay> plays = loadSavedGamePlays(files);
            Specification S = new onNegativeHP_Spec();

            foreach (GamePlay gp in plays)
            {
                gp.Replay(S);  
            }

            Assert.IsTrue(S.getVerdict());
        }

        [TestMethod]
        public void MSTest_Specifications_Always()
        {
            string[] files = { "savedata1.txt", "savedata2.txt" };

            List<GamePlay> plays = loadSavedGamePlays(files);
            Specification Always = new Always(new Predicate<Game>(g => g.player.HP >= 0));

            foreach (GamePlay gp in plays)
            {
                gp.Replay(Always);
            }

            Assert.IsTrue(Always.getVerdict());
        }
        
        [TestMethod]
        public void MSTest_Specifications_Unless()
        {
            string[] files = { "savedata1.txt", "savedata2.txt" };
            List<GamePlay> plays = loadSavedGamePlays(files);
            
            for(int M = 5; M < 30; M = M + 5)
            {
                foreach (GamePlay gp in plays)
                {
                    Specification Unless = new Unless(new Predicate<Game>((g => g.dungeon.calculateMonstersInDungeon() == M)), new Predicate<Game>(g => g.dungeon.calculateMonstersInDungeon() < M));
                    gp.Replay(Unless);
                    Assert.IsTrue(Unless.getVerdict());
                }
            }
        }

        [TestMethod]
        public void MSTest_Specifications_LeadsTo()
        {
            string[] files = { "savedata1.txt", "savedata2.txt" };
            List<GamePlay> plays = loadSavedGamePlays(files);

            for (int M = 20; M < 21; M = M + 5)
            {
                foreach (GamePlay gp in plays)
                {
                    Specification LeadsTo = new LeadsTo(new Predicate<Game>((g => g.dungeon.calculateMonstersInDungeon() == M)), new Predicate<Game>(g => g.dungeon.calculateMonstersInDungeon() < M));
                    gp.Replay(LeadsTo);
                    Assert.IsTrue(LeadsTo.getVerdict());
                }
            }
        }

        public List<GamePlay> loadSavedGamePlays(string[] files)
        {
            List<GamePlay> plays = new List<GamePlay>();
            foreach(String s in files)
            {
                GamePlay g = new GamePlay();
                g.Load(s);
                plays.Add(g);
            }
            return plays;
        }
    }
}
