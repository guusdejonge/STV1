using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STVRogue.GameLogic
{
    public class Specification
    {
        protected bool verdict = true;
        public bool getVerdict() { return verdict; }
        public virtual void test(Game G) { }
    }

    public class onNegativeHP_Spec : Specification
    {
        public override void test(Game G)
        {
            verdict = verdict && G.player.HP >= 0;
        }
    }
}
