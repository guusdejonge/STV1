using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Dungeon
    {
        UtilsClass utils = new UtilsClass();

        public Node startNode;
        public Node exitNode;
        public int L;
        /* a constant multiplier that determines the maximum number of monster-packs per node: */
        public int M;
        public int N;

        private Random rnd = new Random();
        public List<Zone> zones = new List<Zone>();

        /* To create a new dungeon with the specified difficult level and capacity multiplier */
        public Dungeon(int Level, int nodeCapacityMultiplier, int numberOfMonsters)
        {
            Logger.log("Creating a dungeon of difficulty level " + Level + ", node capacity multiplier " + nodeCapacityMultiplier + ".");
            L = Level;
            M = nodeCapacityMultiplier;
            N = numberOfMonsters;

            int monstersLeft = N;
            int monstersDone = MonstersInZone(0);
            zones.Add(new Zone(M, monstersDone, null));                             //de eerste zone
            monstersLeft -= monstersDone;

            for (int zone = 1; zone < L; zone++)  //de opeenvolgende zones  
            {
                zones.Add(new Zone(M, MonstersInZone(zone), null));
                CreateBridge(zones[zone - 1], zones[zone]);            //de zone geeft het level van de bridge aan
                monstersLeft -= MonstersInZone(zone);
            }

            zones.Add(new Zone(M, monstersLeft, null));                   //de laatste zone

            startNode = zones[0].nodes[0];
            exitNode = zones.Last().nodes.Last();

            GeneratePacks();
            GenerateItems();
        }

        public void CreateBridge(Zone zoneFrom, Zone zoneTo)
        {
            Bridge newBridge = new Bridge(N);   //de index van de nieuwe zone is het level van de bridge
            newBridge.level = zones.IndexOf(zoneTo);

            Node exitNode = zoneFrom.nodes.Last();                  //de laatste node van de vorige zone
            Node startNode = zoneTo.nodes.First();                  //de eerste node van de nieuwe zone

            List<Node> NeighborsExitNode = new List<Node>();

            foreach (Node neighbor in exitNode.neighbors)           //voor elke neighbor van de laatste node van de vorige zone
            {
                newBridge.connectToNodeOfSameZone(neighbor);
                NeighborsExitNode.Add(neighbor);
            }

            List<Node> NeighborsStartNode = new List<Node>();

            foreach (Node neighbor in startNode.neighbors)          //voor elke neighbor van de eerste node van de nieuwe zone
            {
                newBridge.connectToNodeOfNextZone(neighbor);
                NeighborsStartNode.Add(neighbor);
            }

            foreach (Node n in NeighborsExitNode)
            {
                n.disconnect(exitNode);
            }

            zoneFrom.nodes.Remove(exitNode);
            zoneFrom.nodes.Add(newBridge);

            foreach(Node n in NeighborsStartNode)
            {
                n.disconnect(startNode);
            }

            zoneTo.nodes.RemoveAt(0);                           //verwijder de eerste node van de nieuwe zone
        }

        public void GeneratePacks()
        {
            foreach (Zone z in zones)    //ga alle zones langs
            {
                z.CreatePacks();
            }

            foreach (Zone z in zones)
            {
                foreach (Node n in z.nodes)
                {
                    foreach (Pack p in n.packs)
                    {
                        p.dungeon = this;
                    }
                }
            }
        }

        public void GenerateItems()
        {
            foreach (Zone z in zones)    //ga alle zones langs
            {
                z.CreateItems();
            }
        }

        public int MonstersInZone(int zone)                 //geldt voor alle zones behalve de laatste
        {
            return (int)Math.Floor((double)((2 * (zone + 1) * N) / ((L + 2) * (L + 1))));
        }

        /* Return a shortest path between node u and node v */
        public List<Node> shortestPath(Node u, Node v)
        {
            return utils.shortestPath(u, v, zones);
        }

        /* To disconnect a bridge from the rest of the zone the bridge is in. */
        public virtual void disconnect(Bridge b)
        {
            Logger.log("Disconnecting the bridge " + b.id + " from its zone.");
            var fromNodes = b.fromNodes;
            foreach (var node in fromNodes)
                b.disconnect(node);
            startNode = b;
        }

        /* To calculate the level of the given node. */
        public int level(Node d)
        {
            if (d is Bridge)
            {
                var b = (Bridge)d;
                return (int)b.level;
            }
            else
                return 0;
        }
    }

    public class Zone
    {
        public List<Node> nodes = new List<Node>();
        Random rnd = new Random();
        public int M;
        public int monstersInZone;
        public int amountOfNodes;
        public UtilsClass utils;

        public Zone(int M2, int monstersInZone2, UtilsClass u)
        {
            if(u != null)
            {
                utils = u;
            }
            else
            {
                utils = new UtilsClass();
            }

            nodes.Add(new Node(M));                                 //de startnode
            int totalConnections = 0;                               //het totaal aantal connecties in de zone
            this.M = M2;
            this.monstersInZone = monstersInZone2;

            int minAmountOfNodes = monstersInZone / M + 3;          //min + 3 nodes

            amountOfNodes = rnd.Next(minAmountOfNodes, minAmountOfNodes + 10);

            for (int node = 1; node < amountOfNodes + 1; node++)    //voor elke opvolgende node
            {
                nodes.Add(new Node(M));

                int amountOfConnections = utils.rnd(1, 4);      //connect hem met 1 tot 4 (of minder als er minder nodes zijn) van de vorige nodes
                amountOfConnections = Math.Min(4, nodes.Count() - 1);

                while ((totalConnections + amountOfConnections) / (node + 4) > 3)       //voorkom dat de average connectivity hierdoor hoger dan 3 zou worden
                {
                    amountOfConnections = 1;
                }
                totalConnections += amountOfConnections;

                for (int connection = 0; connection < amountOfConnections; connection++)
                {
                    int randomPreviousNode = rnd.Next(0, nodes.Count - 1);                 //kies random een van de vorige nodes
                    while (nodes[node].neighbors.Contains(nodes[randomPreviousNode]))  //controleer of hij deze al als neighbor heeft
                    {
                        randomPreviousNode = rnd.Next(0, nodes.Count - 1);
                    }
                    nodes[node].connect(nodes[randomPreviousNode]);                     //zo niet: connect hiermee
                }
            }
        }

        public void CreatePacks()
        {
            int maxRandomNode = 0;

            if (nodes.Any(q => q.GetType() == typeof(Bridge)))  //kijk of je maken heb met de laatse zone 
            {
                maxRandomNode = nodes.Count() - 2;              //in de exit node mag nu geen pack geplaatst worden
            }
            else                                                //zo niet: dan te maken met normale zone
            {
                maxRandomNode = nodes.Count() - 1;
            }

            int minAmountOfPacks = (int)Math.Ceiling((double)(monstersInZone / M));
            int maxAmountOfPacks = monstersInZone;
            int Packs = rnd.Next(minAmountOfPacks, maxAmountOfPacks);
            int[] monstersInPack = new int[Packs];              //een array waarin komt te staan hoeveel monsters in elke pack komt

            for (int i = 0; i < Packs; i++)
            {
                monstersInPack[i] = monstersInZone / Packs;     //het aantal dat er sowieso in komt. bijv: 8 monsters, 3 packs, dus sowieso 2 per pack
            }

            if(Packs > 0)
            {
                int rest = monstersInZone % Packs;                  //nu nog de rest eerlijk verdelen: bijv: nog 2 over om te verdelen. de eerste en tweede pack nog +1.
                for (int i = 0; i < Packs; i++)
                {
                    if (rest > 0)
                    {
                        monstersInPack[i]++;
                        rest--;
                    }
                }
            }
            

            foreach (int i in monstersInPack)
            {
                int randomNode = rnd.Next(0, maxRandomNode);        //kies willekeurige node

                int currentMonstersInThisNode = calculateMonstersInNode(nodes[randomNode]);

                while (currentMonstersInThisNode + i > M)            //als M overschreven worden, kies nieuwe node totdat dit niet meer het geval is
                {
                    randomNode = rnd.Next(0, maxRandomNode);
                    currentMonstersInThisNode = calculateMonstersInNode(nodes[randomNode]);
                }

                Pack newPack = new Pack(i);
                newPack.location = nodes[randomNode];             //zet deze node als de pack zn location
                nodes[randomNode].packs.Add(newPack);             //voeg pack toe aan de node
            }
        }

        public void CreateItems()
        {
            int randomAmountOfItems = rnd.Next(1, nodes.Count());

            for (int i = 0; i < randomAmountOfItems; i++)
            {
                int item = rnd.Next(1, 2);
                int randomNode = rnd.Next(0, nodes.Count() - 1);

                if (item == 1)
                {
                    nodes[randomNode].items.Add(new HealingPotion());
                }
                else
                {
                    nodes[randomNode].items.Add(new Crystal());
                }
            }
        }

        public int calculateMonstersInNode(Node n)
        {
            int currentMonstersInThisNode = 0;

            foreach (Pack p in n.packs)          //tel huidig aantal monsters in die node
            {
                currentMonstersInThisNode += p.members.Count;
            }

            return currentMonstersInThisNode;
        }
    }

    public class Node
    {
        public int M;
        public String id;
        public List<Node> neighbors = new List<Node>();
        public List<Pack> packs = new List<Pack>();
        public List<Item> items = new List<Item>();
        public bool contested;
        public bool fled;
        public UtilsClass utils = new UtilsClass();

        public Node(int M) { this.M = M; }
        //public Node(int M, String id) { this.M = M; this.id = id; }

        /* To connect this node to another node. */
        public void connect(Node nd)
        {
            neighbors.Add(nd); nd.neighbors.Add(this);
        }

        /* To disconnect this node from the given node. */
        public virtual void disconnect(Node nd)
        {
            neighbors.Remove(nd); nd.neighbors.Remove(this);
        }


        /* Execute a fight between the player and the packs in this node.
         * Such a fight can take multiple rounds as describe in the Project Document.
         * A fight terminates when either the node has no more monster-pack, or when
         * the player's HP is reduced to 0. 
         */
        public virtual bool fight(Player player, List<Command> commands)
        {
            while (contested)
            {
                var pack = packs.First();

                //Player's turn
                var command = commands.First().text.ToLower().Split(' ');
                switch (command[0])
                {
                    case "flee":
                        player.moveTo(commands.First().previousNode);
                        contested = false;
                        break;
                    case "attack":
                        player.Attack(pack.members.First());
                        if (pack.members.Count() == 0)
                        {
                            packs.Remove(pack);
                            Logger.log("One pack defeated.");

                            if (packs.Count() == 0)
                            {
                                contested = false;
                                Logger.log("All packs defeated");
                            }
                        }

                        break;
                    case "item":
                        if (command[1] == "potion")
                        {
                            var item = player.bag.Where(q => q.GetType() == typeof(HealingPotion)).First();
                            player.use(item);
                        }
                        else if (command[1] == "crystal")
                        {
                            var item = player.bag.Where(q => q.GetType() == typeof(Crystal)).First();
                            player.use(item);
                        }
                        else
                        {
                            Logger.log("Item does not exist");
                        }

                        break;
                }

                commands.Remove(commands.First());

                //Pack's turn
                if (packs.Count() != 0)
                {
                    Random random = new Random();
                    var fleeProb = utils.fleeProb(pack);
                    if (random.NextDouble() < fleeProb && !fled)
                    {
                        Node node = null;
                        Node neighborCheck = neighbors.FirstOrDefault(q => q.packs.Sum(p => p.members.Count()) < q.M);
                        Node exitCheck = neighbors.FirstOrDefault(q => q!= pack.dungeon.exitNode);
                        if ((node = neighbors.FirstOrDefault(q => q.packs.Sum(p => p.members.Count()) < q.M && q != pack.dungeon.exitNode)) != null)
                        {
                            pack.move(node);
                            fled = true;
                            if (packs.Count == 0)
                                contested = false;

                        }
                    }
                    else
                    {
                        pack.Attack(player);
                        if (player.HP == 0)
                        {
                            Logger.log("GAME OVER");
                            contested = false;
                            break;
                        }
                    }
                }
            }

            return true;
        }

        
    }

    public class Bridge : Node
    {
        public List<Node> fromNodes = new List<Node>();
        public List<Node> toNodes = new List<Node>();
        public int level;

        public Bridge(int N) : base(N) { }
        //public Bridge(int N, String id) : base(N, id) { }

        /* Use this to connect the bridge to a node from the same zone. */
        public void connectToNodeOfSameZone(Node nd)
        {
            base.connect(nd);
            fromNodes.Add(nd);
        }

        /* Use this to connect the bridge to a node from the next zone. */
        public void connectToNodeOfNextZone(Node nd)
        {
            base.connect(nd);
            toNodes.Add(nd);
        }

    }
}