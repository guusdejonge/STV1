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
    public class Program
    {
        static Game game;
        public static int level = 1;
        static void Main(string[] args)
        {
            game = new Game(3, 10, 20);

            while (true)
            {
                if (game.commands.Any(c => c.text.Contains("M")))
                {
                    var com = game.commands.Where(c => c.text.Contains("M")).Last();
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

                Console.WriteLine(" ---------------------------");
                Console.WriteLine(" TURN " + game.turn + ":");
                Console.WriteLine(" ---------------------------");

                if (game.player.location.contested)
                {
                    fight(game.player.location);
                }
                else
                {
                    Console.WriteLine(" * PLAYER");
                    Console.WriteLine("     HP: \t{0}", game.player.HP);
                    Console.WriteLine("     Level: \t{0}", level);
                    Console.WriteLine("     Killpoint: {0}", game.player.KillPoint);
                    Console.WriteLine();
                    Console.WriteLine(" * BAG");
                    if (game.player.bag.Count > 0)
                    {
                        foreach (var item in game.player.bag)
                        {
                            Console.WriteLine("     {1} (i: {0})", game.player.bag.IndexOf(item), item.GetType().ToString().Replace("STVRogue.GameLogic.", ""));
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
                        Console.WriteLine("     Previous node: \t{0} (L{1})", getNodeId(game.prevNode), game.dungeon.zones.IndexOf(game.prevNode.zone));
                    }
                    else
                    {
                        Console.WriteLine("     Previous node: \t- (L-)");
                    }
                    Console.WriteLine("     Current node: \t{0} (L{1})", getNodeId(game.player.location), game.dungeon.zones.IndexOf(game.player.location.zone));
                    Console.Write("     Connected node(s): ");

                    foreach (var node in game.player.location.neighbors)
                    {
                        if (node != game.player.location.neighbors.Last())
                        {
                            Console.Write("{0} (L{1}), ", getNodeId(node), game.dungeon.zones.IndexOf(node.zone));
                        }
                        else
                        {
                            Console.Write("{0} (L{1})", getNodeId(node), game.dungeon.zones.IndexOf(node.zone));
                        }
                    }
                    
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine(" * POSSIBLE ACTIONS");
                    Console.WriteLine("     M \'n\' \'l' \t: Move to node n of level l");
                    if (game.player.bag.Count > 0)
                    {
                        Console.WriteLine("     U \'i\' \t: Use item i");
                    }
                    Console.WriteLine("     S \t\t: Save current game state");
                    Console.WriteLine("     L \t\t: Load last saved game state");
                    Console.WriteLine();
                    Console.WriteLine(" > ENTER YOUR MOVE: ");
                    Console.Write("      ");
                    var command = "";
                    if (game.commandsLoaded.Count() > 0)
                    {
                        Console.WriteLine("       DOE MOVE: " + game.commandsLoaded.First());
                        command = game.commandsLoaded.First();
                        game.commandsLoaded.RemoveAt(0);
                    }
                    else
                    {
                        command = Console.ReadLine();
                    }
                    game.update(new Command(command.ToUpper()));
                }

                game.test();
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
            Logger.log("YOU ENCOUNTERED A PACK");
            while (node.contested)
            {
                Console.WriteLine(" * MONSTERS");
                for (int i = 0; i < node.packs.First().members.Count(); i++)
                {
                    Console.WriteLine("     Monster {0}: {1}HP", i, node.packs.First().members[i].HP);
                }
                /*
                if (game.commands.Any(c => c.text.Contains("MOVE")))
                {
                    var com = game.commands.Where(c => c.text.Contains("MOVE")).Last();
                }*/
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
                Console.WriteLine("     A \t\t: Attack pack");
                if (game.player.bag.Count > 0)
                {
                    Console.WriteLine("     U \'i\' \t: Use item i");
                }
                Console.WriteLine("     F \t\t: Flee");
                Console.WriteLine();
                Console.WriteLine(" > ENTER YOUR MOVE: ");
                Console.Write("      ");

                var cmd = new Command("");
                if (game.commandsLoaded.Count() > 0)
                {
                    Console.WriteLine("       DOE MOVE: " + game.commandsLoaded.First());
                    cmd = new Command(game.commandsLoaded.First());
                    game.commandsLoaded.RemoveAt(0);
                }
                else
                {
                    cmd = new Command(Console.ReadLine());
                }
                Console.WriteLine();
                cmd.previousNode = game.prevNode;
                node.fight(game.player, cmd);

                game.commands.Add(cmd);
                game.turn++;
                game.test();
            }
        }
    }
}
