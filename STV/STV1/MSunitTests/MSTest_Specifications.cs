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
        public void MSTest_playerHP_never_negative()
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
