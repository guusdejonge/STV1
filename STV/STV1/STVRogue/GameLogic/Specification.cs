﻿using System;
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
            int mon = G.dungeon.calculateMonstersInDungeon();


            if (historyP.Count() >= 1)
            {
                int previousPIndex = -1;
                // get last p where p was true:
                if (historyP.Contains(true))
                {
                    for(int i = 0; i < historyP.Count(); i++)
                    {
                        if (historyP[i] == true)
                            previousPIndex = i;
                    }
                }

                int previousQIndex = -2;
                // get last p where p was true:
                if (historyQ.Contains(true))
                {
                    for (int i = 0; i < historyQ.Count(); i++)
                    {
                        if (historyQ[i] == true)
                            previousQIndex = i;
                    }
                }

                if (previousPIndex == -1)
                    verdict =true;
                else
                {
                    if (previousQIndex >= previousPIndex)
                        verdict = true;
                    else
                    {
                        verdict =false;
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
        public Conditional(List<Specification> ant, Specification con)
        {
            antecedents = ant;
            consequent = con;
        }

        public override void test(Game G)
        {
            bool allAntecedentsTrue = true;
            foreach (var ant in antecedents)
            {
                ant.test(G);
                bool test = ant.getVerdict();
                allAntecedentsTrue = allAntecedentsTrue && test;
            }

            if (allAntecedentsTrue)
            {
                consequent.test(G);
                bool con = consequent.getVerdict();
                verdict = con;
            }
            else
            {
                verdict = verdict && true;
            }
        }
    }




}
