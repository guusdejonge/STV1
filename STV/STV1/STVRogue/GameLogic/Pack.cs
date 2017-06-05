using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Pack
    {
        //String id;
        public List<Monster> members = new List<Monster>() ;
        public int startingHP = 0 ;
        public Node location;
        public Dungeon dungeon;
        public List<Node> path;

        /*public Pack(String id, int n)
        {
            this.id = id;
            for (int i = 0; i < n; i++)
            {
                Monster m = new Monster("" + id + "_" + i);
                members.Add(m);
                startingHP += m.HP;
            }
        }*/

        public Pack(int n, int Seed)
        {
            for (int i = 0; i < n; i++)
            {
                Monster m = new Monster(Seed);
                members.Add(m);
                m.pack = this;
                startingHP += m.HP;
            }
            path = new List<Node>();
        }

        public void Attack(Player p)
            {
            foreach (Monster m in members) {
               m.Attack(p);
               if (p.HP == 0) break ;
            }
        }

        /* Move the pack to an adjacent node. */
        public void move(Node u)
        {
            if (!location.neighbors.Contains(u)) throw new ArgumentException();
            int capacity = (int)(u.M * (dungeon.level(u) + 1));
            // count monsters already in the node:
            foreach (Pack Q in location.packs)
            {
                capacity = capacity - Q.members.Count;
            }
            // capacity now expresses how much space the node has left
            if (members.Count > capacity)
            {
               // Logger.log("Pack " + id + " is trying to move to a full node " + u.id + ", but this would cause the node to exceed its capacity. Rejected.");
                return;
            }
            location.packs.Remove(this);
            location = u;
            u.packs.Add(this);
            //Logger.log("Pack " + id + " moves to an already full node " + u.id + ". Rejected.");
        }

        /* Move the pack one node further along a shortest path to u. */
        public void moveTowards(Node u)
        {
            path = dungeon.shortestPath(location,u) ;
            move(path[1]) ;
        }

        public virtual int GetStartingHP()
        {
            return startingHP;
        }

        public virtual List<Monster> GetMembers()
        {
            return members;
        }
    }
}
