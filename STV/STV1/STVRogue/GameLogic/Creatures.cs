using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Creature
    {
        public String id;
        public String name;
        public int HP;
        public List<Item> bag = new List<Item>();

        public int AttackRating = 1;
        public Node location;
        public Creature() { }
        virtual public void Attack(Creature foe)
        {
            int diff = foe.HP - AttackRating;
            foe.HP = (int)Math.Max(0, foe.HP - AttackRating);
            String killMsg = foe.HP == 0 ? ", KILLING it" : "";
            Logger.log("Creature " + id + " attacks " + foe.id + killMsg + ".");
        }
        virtual public void moveTo(Node node)
        {
            if (this is Monster && location is Bridge)
            {
                Bridge b = (Bridge)location;
                if(b.fromNodes.Contains(location) && !b.toNodes.Contains(node))
                {
                    this.location = node;
                }
                else if(b.toNodes.Contains(location) && !b.fromNodes.Contains(node))
                {
                    this.location = node;
                }
            }
            else if(this is Player && node is Bridge)
            {
                foreach (var n in node.zone.nodes)
                {
                    n.alert = false;
                }
                this.location = node;
            }
            else
            {
                this.location = node;
            }

            if (this is Player)
            {
                foreach (Item i in node.items)
                {
                    bag.Add(i);
                }
                node.items.Clear();
            }

            if (this is Player && node.packs.Count > 0)
            {
                node.contested = true;

                node.alert = true;
            }
           
            
        }
    }


    public class Monster : Creature
    {
        public Pack pack;

        /* Create a monster with a random HP */
        //public Monster(String id)
        //{
        //    this.id = id; name = "Orc";
        //    HP = 1 + RandomGenerator.rnd.Next(6);
        //}

        public Monster()
        {
            name = "Orc";
            HP = 1 + RandomGenerator.rnd.Next(6);
        }

    }

    public class Player : Creature
    {
        public Dungeon dungeon;
        public int HPbase = 100;
        public Boolean accelerated = false;
        public int KillPoint = 0;
        public Player()
        {
            id = "player";
            AttackRating = 5;
            HP = HPbase;
        }

        public void use(Item item)
        {
            if (!bag.Contains(item) || item.used) throw new ArgumentException();
            item.use(this);
            bag.Remove(item);
        }

        override public void Attack(Creature foe)
        {
            if (!(foe is Monster)) throw new ArgumentException();
            Monster foe_ = foe as Monster;
            if (!accelerated)
            {
                base.Attack(foe);
                if (foe_.HP == 0)
                {
                    foe_.pack.members.Remove(foe_);
                    KillPoint++;
                }
            }
            else
            {
                var remove = new List<Monster>();

                foreach (Monster target in foe_.pack.members)
                {
                    base.Attack(target);
                    if (target.HP == 0)
                    {
                        remove.Add(foe_);
                        KillPoint++;
                    }
                }

                foreach (Monster m in remove)
                {
                    foe_.pack.members.Remove(m);
                }
                accelerated = false;
            }
        }
    }
}
