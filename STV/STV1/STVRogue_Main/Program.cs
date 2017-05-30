﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.GameLogic;
namespace STVRogue
{
    /* A dummy top-level program to run the STVRogue game */
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(3, 10, 10);
            game.player.location = new Node(0); //M tijdelijk 0
            while (true)
            {
                Console.ReadKey();
                game.update(new Command(""));
            }
        }
    }
}
