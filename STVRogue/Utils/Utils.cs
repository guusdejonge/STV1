using STVRogue.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STVRogue.GameLogic.Dungeon;

namespace STVRogue.Utils
{
    public class UtilsClass
    {
        public List<Node> shortestPath(Node u, Node v, List<Zone> zones)
        {
            var distances = new Dictionary<Node, int>();
            var path = new List<Node>();
            var currentNode = u;


            while (path.Last() != v)
            {
                var currentZone = zones.Where(x => x.nodes.Contains(currentNode)).FirstOrDefault();

                
                    if (currentZone.nodes.Contains(v))
                        path.AddRange(shortestPathInZone(currentNode, v, currentZone));
                    else
                    {
                        var partialPath = shortestPathToBridge(currentNode, currentZone);
                        currentNode = partialPath.Last();
                        path.AddRange(partialPath);
                    }
                


            }

            return path;
        }

        private List<Node> shortestPathInZone(Node u, Node v, Zone zone)
        {
            var path = new List<Node>();
            var distances = new Dictionary<Node, Tuple<int, Node>>();
            var unvisitedNodes = zone.nodes;
            var currentNode = u;

            foreach (Node node in zone.nodes)
            {
                if (node == u)
                    distances.Add(node, new Tuple<int, Node>(0, u));
                else
                    distances.Add(node, new Tuple<int, Node>(999, null));

            }

            while (unvisitedNodes.Contains(v) && distances.OrderBy(x => x.Value.Item1).First().Value.Item1 == 999)
            {
                currentNode = distances.OrderBy(x => x.Value.Item1).First().Key;

                foreach (Node node in currentNode.neighbors)
                {
                    var currentDistance = distances[node].Item1;
                    var potentialDistance = distances[currentNode].Item1;

                    if (potentialDistance < currentDistance)
                        distances[node] = new Tuple<int, Node>(potentialDistance, currentNode);
                }

                unvisitedNodes.Remove(currentNode);

            }

            var endNode = v;
            while (distances.ContainsKey(u))
            {
                var temp = endNode;
                path.Add(endNode);
                endNode = distances[temp].Item2;
                distances.Remove(endNode);
            }


            path.Reverse();
            return path;


        }

        private List<Node> shortestPathToBridge(Node u, Zone zone)
        {
            return shortestPathInZone(u, zone.nodes.Last(), zone);
        }
    }

    public class Logger
    {
        /* You can change the behavior of this logger. */
        public static void log(String s)
        {
            Console.Out.WriteLine("** " + s);
        }
    }

    public class RandomGenerator
    {
        static private Random rnd_ = null;
        static public Random rnd
        {
            get
            {
                if (rnd_ == null) rnd_ = new Random();
                return rnd_;
            }
        }


        static public void initializeWithSeed(int seed)
        {
            rnd_ = new Random(seed);
        }


    }

}
