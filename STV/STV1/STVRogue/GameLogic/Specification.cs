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

    public class Always : Specification
    {
        private Predicate<Game> p;
        public Always(Predicate<Game> p) { this.p = p; }
        public override void test(Game G) { verdict = verdict && p(G); }
    }

    public class Unless : Specification
    {
        private Predicate<Game> p;
        private Predicate<Game> q;
        public Unless(Predicate<Game> p, Predicate<Game> q) { this.p = p; this.q = q; }
        // to keep track all past values of p && !q:
        List<bool> history = new List<bool>();
        public override void test(Game G)
        {
            bool newVerdict;
            if (history.Count() >= 1)
            {
                // check if p && !q holds on the previos state:
                bool previous = history.Last();
                // calculate whether the previous and current state
                // satisfies the unless property:
                newVerdict = !previous || (previous && (p(G) || q(G)));
            }
            else newVerdict = true;
            // update accumulated verdict:
            verdict = verdict && newVerdict;
            // push p && !q to the history:
            int mon = G.dungeon.calculateMonstersInDungeon();
            history.Add(p(G) && !q(G));
        }
    }

    public class LeadsTo : Specification
    {
        private Predicate<Game> p;
        private Predicate<Game> q;
        public LeadsTo(Predicate<Game> p, Predicate<Game> q) { this.p = p; this.q = q; }
        // to keep track all past values of p && !q:
        List<bool> historyP = new List<bool>();
        List<bool> historyQ = new List<bool>();
        public override void test(Game G)
        {
            if (historyP.Count() >= 1)
            {
                // check get last p where p was true:
                int previousPIndex = historyP.IndexOf(historyP.Where(p=>p ==true).Last());
                // calculate whether the previous and current state
                // satisfies the unless property:
                int previousQIndex = historyQ.IndexOf(historyQ.Where(q => q == true).Last());

                if (previousPIndex == -1)
                    verdict = true;
                else
                {
                    if (previousQIndex > previousPIndex)
                        verdict = true;
                    else
                    {
                        verdict = false;
                    }
                }

            }

            historyP.Add(p(G));
            historyQ.Add(q(G));
        }
    }

    public class Conditional : Specification
    {
        private List<Specification> antecedents;
        private Specification consequent;
        Conditional(List<Specification> ant, Specification con)
        {
            antecedents = ant;
            consequent = con;
        }

        public override void test(Game G)
        {
            int falseCount = 0;
            foreach(var ant in antecedents)
            {
                ant.test(G);
                bool test = ant.getVerdict();
                if (!test)
                    falseCount++;
            }
            if (falseCount == 1)
                verdict = verdict && true;
            else if (falseCount == 0)
            {
                consequent.test(G);
                bool con = consequent.getVerdict();
                verdict = verdict && con;
            }
            else
            {
                verdict = false;
            }
        }
    }




}
