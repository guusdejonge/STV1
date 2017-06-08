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
        public int turn;
        public Specification specification; //Null if no specification is beind tested

        /* This creates a player and a random dungeon of the given difficulty level and node-capacity
         * The player is positioned at the dungeon's starting-node.
         * The constructor also randomly seeds monster-packs and items into the dungeon. The total
         * number of monsters are as specified. Monster-packs should be seeded as such that
         * the nodes' capacity are not violated. Furthermore the seeding of the monsters
         * and items should meet the balance requirements stated in the Project Document.
         */
        public Game(int difficultyLevel, int nodeCapcityMultiplier, int numberOfMonsters)
        {
            turn = 1;
            L = difficultyLevel;
            M = nodeCapcityMultiplier;
            N = numberOfMonsters;
            S = DateTime.Now.Millisecond;

            Logger.log("CREATING A DUNGEON OF DIFFICULTY LEVEL " + difficultyLevel + ", NODE CAPACITY MULTIPLIER "
                       + nodeCapcityMultiplier + ", AND " + numberOfMonsters + " MONSTERS.");
            player = new Player(S);
            prevNode = null;
            commands = new List<Command>();
            dungeon = new Dungeon(difficultyLevel, nodeCapcityMultiplier, numberOfMonsters, S);
            player.location = dungeon.startNode;

            specification = null;
        }

        public void test()
        {
            if (specification != null)
            {
                specification.test(this);
            }
        }

        public void saveGamePlay()    //the game will be saved in STVRogue_Main\bin\Debug as savedata.txt
        {
            commandsLoaded.Clear();
            GamePlay G = new GamePlay(L, M, N, S, commands);
            G.Save("savedata.txt");
            Logger.log("SAVING GamePlay TO STVROGUE_MAIN/BIN/DEBUG/SAVEDATA.TXT");

        }

        public void loadGamePlay()    //the game will be saved in STVRogue_Main\bin\Debug as savedata.txt
        {
            turn = 1;
            GamePlay G = new GamePlay();
            G.Load("savedata.txt");
            Logger.log("LOADING GamePlay FROM STVROGUE_MAIN/BIN/DEBUG/SAVEDATA.TXT");
            

            player = new Player(G.S);
            prevNode = null;
            dungeon = new Dungeon(G.L, G.M, G.N, G.S);
            player.location = dungeon.startNode;

            commands.Clear();
            commandsLoaded.Clear();
            foreach(Command c in G.Commands)
            {
                commandsLoaded.Add(c.text);
            }

            //specification = null;
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
                case "M":
                    userCommand.previousNode = player.location;
                    var node = Convert.ToInt32(split[1]);
                    var zone = dungeon.zones[Convert.ToInt32(split[2])];
                    prevNode = player.location;
                    player.moveTo(zone.nodes[node]);
                    Logger.log("YOU MOVED TO NODE " + node);
                    break;
                case "U":
                    var itemId = Convert.ToInt32(split[1]);
                    var item = player.bag[itemId];
                    player.use(item);
                    Logger.log("YOU USED ITEM " + itemId);
                    break;
                case "S":
                    saveGamePlay();
                    Logger.log("GAME SAVED");
                    break;
                case "L":
                    loadGamePlay();
                    break;
            }
           
            if(userCommand.text != "SAVE" && userCommand.text != "LOAD")
            {
                commands.Add(userCommand);
                turn++;
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

            foreach(Bridge b in bridges)
            {
                if (b.M / 2 > b.packs.Count())
                {
                    var nodes = b.zone.nodes.Where(n => n != b);
                    var packSum = nodes.Sum(n => n.packs.Count());

                    if (packSum > 0)
                    {
                        List<Tuple<Pack, int>> packDistance = new List<Tuple<Pack, int>>();
                        foreach (Node node in nodes)
                        {
                            var pack = node.packs.FirstOrDefault();
                            if (pack != null)
                            {
                                var dist = node.utils.shortestPath(node, b, dungeon.zones).Count();
                                packDistance.Add(new Tuple<Pack, int>(pack, dist));
                            }
                        }

                        var closestPack = packDistance.OrderBy(t => t.Item2).First().Item1;
                        closestPack.moveTowards(b);
                    }

                }
            }

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

            test();
        }
    }

    public class GameCreationException : Exception
    {
        //public GameCreationException() {}
        public GameCreationException(String explanation) : base(explanation) { }
    }
}
