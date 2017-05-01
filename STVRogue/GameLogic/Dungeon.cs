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


            for (int zone = 0; zone < level + 1; zone++)
            {
                //string naam = zone.ToString();
                int amountOfNodes = rnd.Next(0, 10);
                zones.Add(new Zone());
                for (int node = 1; node < amountOfNodes; node++) //start overslaan (dus bij 1 beginnen) want die connect niet met eerdere nodes
                {
                    int amountOfConnections = rnd.Next(1, connectivity);
                    for (int connection = 0; connection < amountOfConnections; connection++)
                    {

                    }

                }
            }

            throw new NotImplementedException();
        }

        /* Return a shortest path between node u and node v */
        public List<Node> shortestpath(Node u, Node v)
        {
            return utils.shortestPath(u, v, zones);
        }

        public class Zone
        {
            public List<Node> nodes = new List<Node>();
        }
        
       
        
        /* To disconnect a bridge from the rest of the zone the bridge is in. */
        public void disconnect(Bridge b) {
            Logger.log("Disconnecting the bridge " + b.id + " from its zone.");
            throw new NotImplementedException();
        }

        /* To calculate the level of the given node. */
        public uint level(Node d) { throw new NotImplementedException(); }
    } 

    public class Node
    {
        public String id;
        public List<Node> neighbors = new List<Node>();
        public List<Pack> packs = new List<Pack>();
        public List<Item> items = new List<Item>();

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
        public void fight(Player player)
        {
            throw new NotImplementedException();
        }
    }

    public class Bridge : Node
    {
        List<Node> fromNodes = new List<Node>();
        List<Node> toNodes = new List<Node>();
        public Bridge(String id) : base(id) {  }
        
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
