﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Item
    {
        public String id;
	    public Boolean used = false ;
        public Random rnd;
        public Item(int Seed) { rnd = new Random(Seed); }
        //public Item(String id) { this.id = id; }

        virtual public void use(Player player)
        {
            if (used) {
                //Logger.log("" + player.id + " is trying to use an expired item: "
                //              + this.GetType().Name + " " +  id 
                //              + ". Rejected.");
                return ;
            }
            used = true ;
        }
    }

    public class HealingPotion : Item
    {
        public int HPvalue;
        public HealingPotion(int Seed) : base(Seed)
        {
            rnd = new Random(Seed);
            HPvalue = (int)rnd.Next(10) + 1;
        }
        /* Create a healing potion with random HP-value */
        //public HealingPotion(String id) : base(id)
        //{
        //    HPvalue = (int) RandomGenerator.rnd.Next(10) + 1;
        //}

        override public void use(Player player)
        {
            base.use(player);
            player.HP = (int) Math.Min(player.HPbase, player.HP + HPvalue);   
        }
    }

    public class Crystal : Item
    {
        public Crystal(int Seed) : base(Seed) { rnd = new Random(Seed); }
        //public Crystal(String id) : base(id) { }
        override public void use(Player player)
        {
            base.use(player);
            player.accelerated = true;
            if (player.location is Bridge) player.dungeon.disconnect(player.location as Bridge);    
        }
    }
}
