﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace STVRogue.GameLogic
{

    /* An example of a test class written using NUnit testing framework. 
     * This one is to unit-test the class Player. The test is incomplete though, 
     * as it only contains two test cases. 
     */
    [TestFixture]
    public class NTest_Player
    {

        [Test]
        public void NTest_use_onEmptyBag()
        {
            Player P = new Player(DateTime.Now.Millisecond);
            Assert.Throws<ArgumentException>(() => P.use(new Item(DateTime.Now.Millisecond)));
        }

        [Test]
        public void NTest_use_item_in_bag()
        {
            Player P = new Player(DateTime.Now.Millisecond);
            Item x = new HealingPotion(DateTime.Now.Millisecond);
            P.bag.Add(x);
            P.use(x);
            Assert.True(x.used) ;
            Assert.False(P.bag.Contains(x));
        }
    }
}
