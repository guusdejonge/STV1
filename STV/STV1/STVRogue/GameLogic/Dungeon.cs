﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Dungeon
    {
        UtilsClass utils;

        public Node startNode;
        public Node exitNode;
        public int L;
        /* a constant multiplier that determines the maximum number of monster-packs per node: */
        public int M;
        public int N;
        public static int S;

        public Random rnd;
        public List<Zone> zones = new List<Zone>();

        /* To create a new dungeon with the specified difficult level and capacity multiplier */
        public Dungeon(int Level, int nodeCapacityMultiplier, int numberOfMonsters, int Seed)
        {     
            L = Level;
            M = nodeCapacityMultiplier;
            N = numberOfMonsters;
            rnd = new Random(Seed);
            //RandomGenerator.initializeWithSeed(Seed);
            utils = new UtilsClass(S);

            int monstersLeft = N;
            int monstersDone = MonstersInZone(0);
            zones.Add(new Zone(M, monstersDone, null, rnd.Next()));                             //de eerste zone
            monstersLeft -= monstersDone;

            for (int zone = 1; zone < L; zone++)  //de opeenvolgende zones  
            {
                zones.Add(new Zone(M, MonstersInZone(zone), null, rnd.Next()));
                CreateBridge(zones[zone - 1], zones[zone]);            //de zone geeft het level van de bridge aan
                monstersLeft -= MonstersInZone(zone);
            }

            zones.Add(new Zone(M, monstersLeft, null, rnd.Next()));                   //de laatste zone

            startNode = zones[0].nodes[0];
            exitNode = zones.Last().nodes.Last();

            GeneratePacks();
            GenerateItems();

            Console.WriteLine("DEBUG INFORMATION:");
            foreach (var node in zones.First().nodes)
            {
                Console.WriteLine("Node: {0}. Packs: {1}, Monsters: {2}", zones.First().nodes.IndexOf(node), node.packs.Count(), zones.First().calculateMonstersInNode(node));
            }
            Console.WriteLine();
        }

        public void CreateBridge(Zone zoneFrom, Zone zoneTo)
        {
            Bridge newBridge = new Bridge(M, rnd.Next(), zoneFrom, zones.IndexOf(zoneTo));   //de index van de nieuwe zone is het level van de bridge
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

            foreach (Node n in NeighborsStartNode)
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
            Logger.log("DISCONNECTED BRIDGE " + b.id + " FROM ITS ZONE");
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
        public Random rnd;
        public int M;
        public int monstersInZone;
        public int amountOfNodes;
        public UtilsClass utils;


        public Zone(int M2, int monstersInZone2, UtilsClass u, int Seed)
        {
            rnd = new Random(Seed);

            if (u != null)
            {
                utils = u;
            }
            else
            {
                utils = new UtilsClass(Seed);
            }

            Node n = new Node(M, rnd.Next(), this, nodes.Count());
            nodes.Add(n);                      //de startnode
            int totalConnections = 0;                               //het totaal aantal connecties in de zone
            this.M = M2;
            this.monstersInZone = monstersInZone2;

            int minAmountOfNodes = monstersInZone / M + 5;          //min + 10 nodes

            amountOfNodes = rnd.Next(minAmountOfNodes, minAmountOfNodes + 5);

            for (int node = 1; node < amountOfNodes + 1; node++)    //voor elke opvolgende node
            {
                Node m = new Node(M, rnd.Next(), this, nodes.Count());
                nodes.Add(m);
                m.zone = this;

                int amountOfConnections = utils.rnd(1, 5);      //connect hem met 1 tot 4 (of minder als er minder nodes zijn) van de vorige nodes
                amountOfConnections = Math.Min(4, nodes.Count() - 1);

                while (((totalConnections + amountOfConnections) / (node + 1) + 4) > 3 && amountOfConnections > 1)       //voorkom dat de average connectivity hierdoor hoger dan 3 zou worden
                {
                    amountOfConnections -= 1;
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

            if (!nodes.Any(q => q.GetType() == typeof(Bridge)))  //kijk of je maken heb met de laatse zone 
            {
                maxRandomNode = nodes.Count() - 2;              //in de exit node mag nu geen pack geplaatst worden
            }
            else                                                //zo niet: dan te maken met normale zone
            {
                maxRandomNode = nodes.Count() - 1;
            }

            int minAmountOfPacks = (int)(monstersInZone / M) + 1;
            int maxAmountOfPacks = monstersInZone;
            int Packs = rnd.Next(minAmountOfPacks, maxAmountOfPacks + 1);
            int[] monstersInPack = new int[Packs];              //een array waarin komt te staan hoeveel monsters in elke pack komt

            for (int i = 0; i < Packs; i++)
            {
                monstersInPack[i] = monstersInZone / Packs;     //het aantal dat er sowieso in komt. bijv: 8 monsters, 3 packs, dus sowieso 2 per pack
            }

            if (Packs > 0)
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

            List<Node> AvailableNodes = new List<Node>();
            AvailableNodes = nodes.Take(maxRandomNode + 1).ToList();

            foreach (int i in monstersInPack)
            {
                int randomNode = rnd.Next(0, maxRandomNode);        //kies willekeurige node

                int currentMonstersInThisNode = calculateMonstersInNode(AvailableNodes[randomNode]);

                while (currentMonstersInThisNode + i > M)            //als M overschreden worden, kies nieuwe node totdat dit niet meer het geval is
                {
                    AvailableNodes.Remove(AvailableNodes[randomNode]);
                    maxRandomNode--;
                    if (maxRandomNode < 0)
                    {
                        throw new GameCreationException("Combination of low multiplier and high amount of monsters");
                    }
                    randomNode = rnd.Next(0, maxRandomNode);
                    currentMonstersInThisNode = calculateMonstersInNode(AvailableNodes[randomNode]);
                }

                Pack newPack = new Pack(i, rnd.Next());
                int k = nodes.FindIndex(q => q == AvailableNodes[randomNode]);

                newPack.location = nodes[k];             //zet deze node als de pack zn location
                nodes[k].packs.Add(newPack);             //voeg pack toe aan de node
            }
        }

        public void CreateItems()
        {
            int randomAmountOfItems = rnd.Next(1, nodes.Count() + 1);

            for (int i = 0; i < randomAmountOfItems; i++)
            {
                int item = utils.rnd(1, 3);
                int randomNode = rnd.Next(0, nodes.Count());
                if (item == 1)
                {
                    nodes[randomNode].items.Add(new HealingPotion(rnd.Next()));
                }
                else
                {
                    nodes[randomNode].items.Add(new Crystal(rnd.Next()));
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
        public int id;
        public List<Node> neighbors = new List<Node>();
        public List<Pack> packs = new List<Pack>();
        public List<Item> items = new List<Item>();
        public bool contested;
        public bool fled;
        public bool alert;
        public UtilsClass utils;
        public int Seed;
        public Zone zone;

        public Node(int M, Zone z, int id) { this.M = M; Seed = DateTime.Now.Millisecond; this.zone = z; this.id = id; utils = new UtilsClass(Seed); }
        public Node(int M, int S, Zone z ,int id) { this.M = M; Seed = S; this.zone = z; this.id =id; utils = new UtilsClass(Seed); }
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
        public virtual bool fight(Player player, Command command)
        {

            var pack = packs.First();

            //Player's turn
            var cmd = command.text.ToLower().Split(' ');
            switch (cmd[0])
            {
                case "f":
                    player.moveTo(command.previousNode);
                    contested = false;
                    Logger.log("YOU FLED");
                    return true;
                case "a":
                    player.Attack(pack.members.First());
                    if (pack.members.Count() == 0)
                    {
                        Logger.log("YOU KILLED THE PACK");

                        packs.Remove(pack);

                        if (packs.Count() == 0)
                        {
                            contested = false;
                        }
                        else
                        {
                            Logger.log("YOU ENCOUNTERED ANOTHER PACK");
                        }
                    }
                    else
                    {
                        Logger.log("YOU ATTACKED THE PACK");
                    }

                    break;
                case "use":
                    var item = player.bag[int.Parse(cmd[1])];
                    player.use(item);
                    break;
            }

            //Pack's turn
            if (packs.Count() != 0)
            {
                Random random = new Random(Seed);
                var fleeProb = utils.fleeProb(pack);
                if (random.NextDouble() < fleeProb && !fled)
                {
                    Node node = null;
                    Node neighborCheck = neighbors.FirstOrDefault(q => q.packs.Sum(p => p.members.Count()) < q.M);
                    Node exitCheck = neighbors.FirstOrDefault(q => q != pack.dungeon.exitNode);
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

        public Bridge(int M, Zone z, int id) : base(M, z, id) { }
        public Bridge(int M, int S, Zone z, int id) : base(M, S, z, id) { }
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