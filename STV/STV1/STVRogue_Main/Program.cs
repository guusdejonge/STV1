﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.GameLogic;
using STVRogue.Utils;

namespace STVRogue
{
    /* A dummy top-level program to run the STVRogue game */
    class Program
    {
        static Game game;
        static void Main(string[] args)
        {
            game = new Game(0, 10, 20);
            game.player.location = game.dungeon.startNode;
            int level = 1;
            Node prevNode = null;
            while (true)
            {
                
                if (game.commands.Any(c => c.text.Contains("MOVE")))
                {
                    var com = game.commands.Where(c => c.text.Contains("MOVE")).Last();
                    prevNode = com.previousNode;
                }
                if (game.player.location is Bridge)
                {
                    int lvl = game.dungeon.level(game.player.location as Bridge);
                    level = lvl + 1; 
                }
                if (game.player.location.zone.nodes.Contains(game.dungeon.exitNode))
                {
                    foreach (var n in game.player.location.zone.nodes)
                        n.alert = true;
                }

                if (game.player.location.contested)
                    fight(game.player.location);


                Console.WriteLine();
                var nodeId = getNodeId(game.player.location);
                Console.WriteLine("Possible commands:");
                Console.WriteLine("     MOVE + nodeId. ");
                if (game.player.bag.Count > 0)
                {
                    Console.WriteLine("      USE + itemId");
                    Console.WriteLine("Bagpack contains:");
                    foreach(var item in game.player.bag)
                    {
                        Console.WriteLine("       {1}, itemId: {0}", game.player.bag.IndexOf(item), item.GetType().ToString().Replace("STVRogue.GameLogic.", ""));
                    }
                }

                Console.WriteLine();
                Console.WriteLine("HP: {0}", game.player.HP);
                Console.WriteLine("Level: {0}", level);
                Console.WriteLine("Killpoint:", game.player.KillPoint);


                Console.WriteLine();

                if (prevNode!=null)
                {
                    Console.WriteLine("Previous node: nodeId {0}", getNodeId(prevNode));

                }
                Console.WriteLine("Current node:");
                Console.WriteLine("      nodeId: {0}", nodeId);

                Console.WriteLine("Next nodes:");
                foreach (var node in game.player.location.neighbors)
                    Console.WriteLine("      nodeId: {0}", getNodeId(node));


                Console.WriteLine();
                var command = Console.ReadLine();
                game.update(new Command(command.ToUpper()));
            }
        }

        static int getNodeId(Node node)
        {
            var zone = game.dungeon.zones.Where(z => z.nodes.Contains(node)).First();
            var nodeId = zone.nodes.IndexOf(node);
            return nodeId;
        }

        static void fight(Node node)
        {
            Node prevNode = null;
            if (game.commands.Any(c => c.text.Contains("MOVE")))
            {
                var com = game.commands.Where(c => c.text.Contains("MOVE")).Last();
                prevNode = com.previousNode;
            }

            Console.WriteLine("You encountered a pack of monsters!");
            while (node.contested)
            {
                Console.WriteLine("Possible commands:");
                Console.WriteLine("     ATTACK");
                if (game.player.bag.Count > 0)
                {
                    Console.WriteLine("      USE + itemId");
                    Console.WriteLine("Bagpack contains:");
                    foreach (var item in game.player.bag)
                    {
                        Console.WriteLine("       {1}, itemId: {0}", game.player.bag.IndexOf(item), item.GetType().ToString().Replace("STVRogue.GameLogic.", ""));
                    }
                }
                Console.WriteLine("     FLEE");
                Console.WriteLine();
                Console.WriteLine("HP: {0}", game.player.HP);
                Console.WriteLine();
                Console.WriteLine("Number of packs in this node: {0}", node.packs.Count());
                Console.WriteLine("Number of monsters in first pack: {0}", node.packs.First().members.Count());
                Console.WriteLine("HP of monsters:");
                for(int i = 0; i < node.packs.First().members.Count(); i++)
                {
                    Console.WriteLine("Monster {0}: {1}", i, node.packs.First().members[i].HP);
                }
                Console.WriteLine();
                var cmd = new Command(Console.ReadLine());
                cmd.previousNode = prevNode;
                node.fight(game.player, cmd);
            }
        }
    }
}
