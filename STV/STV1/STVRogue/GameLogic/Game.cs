using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Game
    {
        public Player player;
        public Dungeon dungeon;
        public List<Command> commands;
        public List<String> commandsLoaded = new List<String>();
        public int L;
        public int M;
        public int N;
        public int S;
        public Node prevNode;

        /* This creates a player and a random dungeon of the given difficulty level and node-capacity
         * The player is positioned at the dungeon's starting-node.
         * The constructor also randomly seeds monster-packs and items into the dungeon. The total
         * number of monsters are as specified. Monster-packs should be seeded as such that
         * the nodes' capacity are not violated. Furthermore the seeding of the monsters
         * and items should meet the balance requirements stated in the Project Document.
         */
        public Game(int difficultyLevel, int nodeCapcityMultiplier, int numberOfMonsters)
        {
            L = difficultyLevel;
            M = nodeCapcityMultiplier;
            N = numberOfMonsters;
            S = DateTime.Now.Millisecond;

            Logger.log("Creating a game of difficulty level " + difficultyLevel + ", node capacity multiplier "
                       + nodeCapcityMultiplier + ", and " + numberOfMonsters + " monsters.");
            player = new Player();
            prevNode = null;
            dungeon = new Dungeon(difficultyLevel, nodeCapcityMultiplier, numberOfMonsters, S);
            player.location = dungeon.startNode;

            commands = new List<Command>();
        }

        public void saveGame()    //the game will be saved in STVRogue_Main\bin\Debug as savedata.txt
        {
            List<String> saveLines = new List<String>();

            saveLines.Add(L.ToString());
            saveLines.Add(M.ToString());
            saveLines.Add(N.ToString());
            saveLines.Add(S.ToString());

            foreach (Command c in commands)
            {
                saveLines.Add(c.text);
            }

            System.IO.File.WriteAllLines(AppDomain.CurrentDomain.BaseDirectory + "savedata.txt", saveLines.ToArray());
        }

        public void loadGame()    //the game will be saved in STVRogue_Main\bin\Debug as savedata.txt
        { 
            string[] readLines = System.IO.File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "savedata.txt");

            L = Int32.Parse(readLines[0]);
            M = Int32.Parse(readLines[1]);
            N = Int32.Parse(readLines[2]);
            S = Int32.Parse(readLines[3]);

            Logger.log("Loading a game of difficulty level " + L + ", node capacity multiplier "
                      + M + ", and " + N + " monsters.");
            player = new Player();
            prevNode = null;
            dungeon = new Dungeon(L, M, N, S);
            player.location = dungeon.startNode;
            commands = new List<Command>();

            commandsLoaded = new List<String>();
            for (int i = 4; i < readLines.Length; i++)
            {
                commandsLoaded.Add(readLines[i]);
            }
        }

        private string GetCurrentDirectory()
        {
            throw new NotImplementedException();
        }

        /*
         * A single update turn to the game. 
         */
        public Boolean update(Command userCommand)
        {
            var split = userCommand.text.Split(' ');
            switch (split[0])
            {
                case "MOVE":
                    userCommand.previousNode = player.location;
                    var node = Convert.ToInt32(split[1]);
                    var zone = dungeon.zones.Where(z => z.nodes.Contains(player.location)).First();
                    player.moveTo(zone.nodes[node]);
                    break;
                case "USE":
                    var itemId = Convert.ToInt32(split[1]);
                    var item = player.bag[itemId];
                    player.use(item);
                    break;
                case "SAVE":
                    saveGame();
                    break;
                case "LOAD":
                    loadGame();
                    break;
            }
           
            if(userCommand.text != "SAVE" && userCommand.text != "LOAD")
            {
                commands.Add(userCommand);
            }
            
            movePacks();

            return true;
        }

        private void movePacks()
        {
            var currentZone = dungeon.zones.Where(z => z.nodes.Contains(player.location)).First();

            List<Bridge> bridges = new List<Bridge>();
            foreach (Zone z in dungeon.zones)
            {

                var bridge = z.nodes.Where(n => n.GetType() == typeof(Bridge)).FirstOrDefault() as Bridge;
                if (bridge != null)
                    bridges.Add(bridge);
            }

            //foreach(Bridge b in bridges)
            //{
            //    if(b.M/2 > b.packs.Count())
            //    {
            //        var nodes = b.zone.nodes.Where(n=>n!=b);
            //        var packSum = nodes.Sum(n => n.packs.Count());

            //        if (packSum > 0)
            //        {
            //            List<Tuple<Pack, int>> packDistance = new List<Tuple<Pack, int>>();
            //            foreach(Node node in nodes)
            //            {
            //                var pack = node.packs.FirstOrDefault();
            //                if (pack != null)
            //                {
            //                    var dist = node.utils.shortestPath(node, b, dungeon.zones).Count();
            //                    packDistance.Add(new Tuple<Pack, int>(pack, dist));
            //                }
            //            }

            //            var closestPack = packDistance.OrderBy(t => t.Item2).First().Item1;
            //            closestPack.moveTowards(b);
            //        }

            //    }
            //}

            List<Tuple<Pack, Node>> movePacks = new List<Tuple<Pack, Node>>();

            //alert has gone off for this zone
            if (currentZone.nodes.Any(n => n.alert == true))
            {

                foreach (var node in currentZone.nodes)
                {
                    List<Node> path = new List<Node>();
                    if (player.location == node)
                        continue;

                    if (node.packs.Count() > 0)
                        path = node.utils.shortestPath(node, player.location, dungeon.zones);

                    var neighbors = node.neighbors;
                    foreach (var pack in node.packs)
                    {
                        var random = dungeon.rnd.NextDouble();
                        if (random < 0.5)
                        {
                            movePacks.Add(new Tuple<Pack, Node>(pack, path[1]));
                        }
                    }

                }


            }

            //random movement of packs
            else
            {
                List<string> moves = new List<string>();
                foreach (var node in currentZone.nodes)
                {
                    var neighbors = node.neighbors;
                    foreach (var pack in node.packs)
                    {
                        var random = dungeon.rnd.NextDouble();
                        if (random < 0.5)
                        {
                            var index = dungeon.rnd.Next(neighbors.Count());
                            var randomZone = dungeon.zones.Where(z => z.nodes.Contains(neighbors[index])).First();
                            if (randomZone == currentZone)
                            {
                                movePacks.Add(new Tuple<Pack, Node>(pack, neighbors[index]));
                            }
                        }
                    }
                }
            }

            foreach (var tuple in movePacks)
            {
                tuple.Item1.move(tuple.Item2);
            }
        }
    }

    public class GameCreationException : Exception
    {
        //public GameCreationException() {}
        public GameCreationException(String explanation) : base(explanation) { }
    }
}
