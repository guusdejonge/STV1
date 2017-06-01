using System;
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
        public static int turn = 1;
        public static int level = 1;
        static void Main(string[] args)
        {
            game = new Game(3, 10, 20);
            
            

            var zone = game.dungeon.zones.First();

            foreach(var node in game.dungeon.zones.First().nodes)
            {
                Console.WriteLine("Node: {0}. Packs: {1}, Monsters: {2}", zone.nodes.IndexOf(node), node.packs.Count(), zone.calculateMonstersInNode(node));
            }

            while (true)
            {
                if (game.commands.Any(c => c.text.Contains("MOVE")))
                {
                    var com = game.commands.Where(c => c.text.Contains("MOVE")).Last();
                    game.prevNode = com.previousNode;
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

                var nodeId = getNodeId(game.player.location);

                Console.WriteLine();
                Console.WriteLine(" ---------------------------");
                Console.WriteLine(" TURN " + turn + ":"); turn++;
                Console.WriteLine(" ---------------------------");

                if (game.player.location.contested)
                {
                    fight(game.player.location);
                }
                else
                {
                    Console.WriteLine(" * PLAYER");
                    Console.WriteLine("     HP: {0}", game.player.HP);
                    Console.WriteLine("     Level: {0}", level);
                    Console.WriteLine("     Killpoint: {0}", game.player.KillPoint);
                    Console.WriteLine();
                    Console.WriteLine(" * BAG");
                    if (game.player.bag.Count > 0)
                    {
                        foreach (var item in game.player.bag)
                        {
                            Console.WriteLine("     {1} (I: {0})", game.player.bag.IndexOf(item), item.GetType().ToString().Replace("STVRogue.GameLogic.", ""));
                        }
                    }
                    else
                    {
                        Console.WriteLine("     -");
                    }
                    Console.WriteLine();
                    Console.WriteLine(" * LOCATION");
                    if (game.prevNode != null)
                    {
                        Console.WriteLine("     Previous node: {0}", nodeId);
                    }
                    Console.WriteLine("     Current node: {0}", nodeId);
                    Console.Write("     Neighbouring node: ");
                    foreach (var node in game.player.location.neighbors)
                    {
                        if (node != game.player.location.neighbors.Last())
                        {
                            Console.Write("{0}, ", getNodeId(node));
                        }
                        else
                        {
                            Console.Write("{0}", getNodeId(node));
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine(" * POSSIBLE ACTIONS");
                    Console.WriteLine("     \"Move N\"\t: Move to node N");
                    if (game.player.bag.Count > 0)
                    {
                        Console.WriteLine("     \"Use I\"\t: Use item I");
                    }
                    Console.WriteLine("     \"Save\"\t: Save current game state");
                    Console.WriteLine("     \"Load\"\t: Load last saved game state");
                    Console.WriteLine();
                    Console.WriteLine(" > ENTER YOUR MOVE: ");
                    Console.Write("      ");
                    var command = "";
                    if (game.commandsLoaded.Count() > 0)
                    {
                        command = game.commandsLoaded.First();
                        game.commandsLoaded.RemoveAt(0);
                    }
                    else
                    {
                        command = Console.ReadLine();
                    }
                    game.update(new Command(command.ToUpper()));
                }
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
            Console.WriteLine(" **********************");
            Console.WriteLine(" YOU ENCOUTERED A PACK");
            Console.WriteLine(" ***********************");
            while (node.contested)
            {
                Console.WriteLine(" * MONSTERS");
                for (int i = 0; i < node.packs.First().members.Count(); i++)
                {
                    Console.WriteLine("     Monster {0}: {1}HP", i, node.packs.First().members[i].HP);
                }

                Node prevNode = null;
                if (game.commands.Any(c => c.text.Contains("MOVE")))
                {
                    var com = game.commands.Where(c => c.text.Contains("MOVE")).Last();
                    prevNode = com.previousNode;
                }
                Console.WriteLine();
                Console.WriteLine(" * PLAYER");
                Console.WriteLine("     HP: {0}", game.player.HP);
                Console.WriteLine("     Level: {0}", level);
                Console.WriteLine("     Killpoint: {0}", game.player.KillPoint);
                Console.WriteLine();
                Console.WriteLine(" * BAG");
                if (game.player.bag.Count > 0)
                {
                    foreach (var item in game.player.bag)
                    {
                        Console.WriteLine("     {1} (I: {0})", game.player.bag.IndexOf(item), item.GetType().ToString().Replace("STVRogue.GameLogic.", ""));
                    }
                }
                else
                {
                    Console.WriteLine("     -");
                }
                Console.WriteLine();
                Console.WriteLine(" * POSSIBLE ACTIONS");
                Console.WriteLine("     \"Attack\"");
                if (game.player.bag.Count > 0)
                {
                    Console.WriteLine("     \"Use I\"\t: Use item I");
                }
                Console.WriteLine("     \"Flee\"");
                Console.WriteLine();
                Console.WriteLine(" > ENTER YOUR MOVE: ");
                Console.Write("      ");

                var cmd = new Command("");
                if (game.commandsLoaded.Count() > 0)
                {
                    cmd = new Command(game.commandsLoaded.First());
                    game.commandsLoaded.RemoveAt(0);
                }
                else
                {
                    cmd = new Command(Console.ReadLine());
                }
                Console.WriteLine();
                Console.Write(" ***********************");
                Console.WriteLine();
                cmd.previousNode = game.prevNode;
                node.fight(game.player, cmd);

                game.commands.Add(cmd);

            }
        }
    }
}
