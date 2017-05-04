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
        UtilsClass utils = new UtilsClass();

        public Node startNode;
        public Node exitNode;
        public uint difficultyLevel;
        /* a constant multiplier that determines the maximum number of monster-packs per node: */
        public uint M;

        private Random rnd = new Random();
        private int connectivity = 4;

        List<Zone> zones = new List<Zone>();


        /* To create a new dungeon with the specified difficult level and capacity multiplier */
        public Dungeon(uint level, uint nodeCapacityMultiplier)
        {
            Logger.log("Creating a dungeon of difficulty level " + level + ", node capacity multiplier " + nodeCapacityMultiplier + ".");
            difficultyLevel = level;
            M = nodeCapacityMultiplier;

            List<Zone> zones = new List<Zone>();
            zones.Add(new Zone());          //de eerste zone

            for (int zone = 1; zone < level + 1; zone++)  //de opeenvolgende zones  
            {
                zones.Add(new Zone());

                Bridge newBridge = new Bridge(zone);                //geef de zone mee als het level van de bridge
                Node exitNode = zones[zone - 1].nodes.Last();       //de laatste node van de vorige zone
                Node startNode = zones[zone].nodes.First();         //de eerste node van de nieuwe zone

                foreach (Node neighbor in exitNode.neighbors)       //voor elke neighbor van de  laatste node
                {
                    newBridge.connectToNodeOfSameZone(neighbor);
                }

                foreach (Node neighbor in startNode.neighbors)      //voor elke neighbor van de eerste node
                {
                    newBridge.connectToNodeOfNextZone(neighbor);
                }

                zones[zone - 1].nodes[zones[zone - 1].nodes.Count()] = newBridge;   //replace exitNode van de vorige zone voor de bridge
                zones[zone].nodes[0] = newBridge;                                   //replace startNode van de nieuwe zone voor de bridge
            }
        }

        /* Return a shortest path between node u and node v */
        public List<Node> shortestPath(Node u, Node v)
        {
            return utils.shortestPath(u, v, zones);
        }

        /* To disconnect a bridge from the rest of the zone the bridge is in. */
        public void disconnect(Bridge b)
        {
            Logger.log("Disconnecting the bridge " + b.id + " from its zone.");
            var fromNodes = b.fromNodes;
            foreach (var node in fromNodes)
                b.disconnect(node);
            startNode = b;
        }

        /* To calculate the level of the given node. */
        public uint level(Node d)
        {
            if (d is Bridge)
            {
                var b = (Bridge)d;
                return (uint)b.level;
            }
            else
                return 0;
        }
    }

    public class Zone
    {
        public List<Node> nodes = new List<Node>();
        Random rnd = new Random();

        public Zone()
        {
            nodes.Add(new Node());                                  //de startnode
            int totalConnections = 0;                               //het totaal aantal connecties tot nu toe

            int amountOfNodes = rnd.Next(1, 9);                     //1 tot 9 nieuwe nodes toevoegen
            for (int node = 1; node < amountOfNodes + 1; node++)    //voor elke opvolgende node
            {
                nodes.Add(new Node());

                int amountOfConnections = rnd.Next(1, 4);                               //connect hem met 1 tot 4 van de vorige nodes
                while ((totalConnections + amountOfConnections) / (node + 1) > 3)       //bereken average connectivity
                {
                    amountOfConnections -= 1;                                           //verlaag aantal nieuwe nodes als deze boven de drie uitkomt
                }
                totalConnections += amountOfConnections;

                for (int connection = 0; connection < amountOfConnections; connection++)
                {
                    int randomPreviousNode = rnd.Next(nodes.Count - 1);                 //kies random een van de vorige nodes
                    if (!nodes[node].neighbors.Contains(nodes[randomPreviousNode]))     //controleer of hij deze al als neighbor heeft
                    {
                        nodes[node].connect(nodes[randomPreviousNode]);                 //zo niet: connect hiermee
                    }
                }
            }
        }
    }

    public class Node
    {
        public String id;
        public List<Node> neighbors = new List<Node>();
        public List<Pack> packs = new List<Pack>();
        public List<Item> items = new List<Item>();
        public bool contested;

        public Node() { }
        public Node(String id) { this.id = id; }

        /* To connect this node to another node. */
        public void connect(Node nd)
        {
            neighbors.Add(nd); nd.neighbors.Add(this);
        }

        /* To disconnect this node from the given node. */
        public void disconnect(Node nd)
        {
            neighbors.Remove(nd); nd.neighbors.Remove(this);
        }


        /* Execute a fight between the player and the packs in this node.
         * Such a fight can take multiple rounds as describe in the Project Document.
         * A fight terminates when either the node has no more monster-pack, or when
         * the player's HP is reduced to 0. 
         */
        public void fight(Player player, List<Command> commands)
        {
            while (contested)
            {
                var pack = packs.First();
                var command = commands.First().text.ToLower().Split(' ');
                switch (command[0])
                {
                    case "flee":
                        player.moveTo(commands.First().previousNode);
                        contested = false;
                        break;
                    case "attack":
                        player.Attack(pack.members.First());
                        
                        break;
                    case "item":
                        if (command[1] == "potion")
                        {
                            var item = player.bag.Where(q => q.GetType() == typeof(HealingPotion)).First();
                            player.use(item);
                        }
                        else if(command[1] == "crystal")
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
            }
        }
    }

    public class Bridge : Node
    {
        public List<Node> fromNodes = new List<Node>();
        public List<Node> toNodes = new List<Node>();
        public int level;

        public Bridge() { }
        public Bridge(String id) : base(id) { }

        public Bridge(int lev)
        {
            level = lev;
        }

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


