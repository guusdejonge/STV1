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
        public virtual Single fleeProb(Pack pack)
        {
            var totalPackHp = pack.members.Sum(m => m.HP);
            var fleeProbability = (1 - (totalPackHp / pack.startingHP)) * 0.5f;
            return fleeProbability;
        }

        public virtual int rnd(int min, int max)
        {
            Random rnd = new Random();
            int random = rnd.Next(min, max);
            return random;
        }

        public List<Node> shortestPath(Node u, Node v, List<Zone> zones)
        {
            var distances = new Dictionary<Node, int>();
            var path = new List<Node>();
            var currentNode = u;


            while (path.LastOrDefault() != v)
            {
                var currentZone = zones.Where(x => x.nodes.Contains(currentNode)).FirstOrDefault();

                if (currentNode is Bridge)
                {
                    var b = currentNode as Bridge;
                    currentNode = b.toNodes.First();
                    currentZone = zones.Where(x => x.nodes.Contains(currentNode)).FirstOrDefault();
                    currentNode = b;
                }

                
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

            if(u is Bridge)
            {
                //distances.Add(u, new Tuple<int, Node>(0, u));
                //unvisitedNodes.Add(u);
                zone.nodes.Add(u);
            }

            foreach (Node node in zone.nodes)
            {
                if (node == u)
                    distances.Add(node, new Tuple<int, Node>(0, u));
                else
                    distances.Add(node, new Tuple<int, Node>(999, null));

            }

            var b = unvisitedNodes.FirstOrDefault(q => q is Bridge) as Bridge;

            if (b != null && b!=u)
            {
                foreach (var node in b.toNodes)
                    unvisitedNodes.Remove(node);
            }
            

            

            var unvis = unvisitedNodes.Contains(v);
            var check = distances.OrderBy(x => x.Value.Item1).First().Value.Item1;
            while (unvis && (check == 999 || check == 0))
            {
                currentNode = distances.OrderBy(x => x.Value.Item1).First(q=>unvisitedNodes.Contains(q.Key)).Key;
              
                
                foreach (Node node in currentNode.neighbors)
                {
                    if(currentNode is Bridge)
                    {
                        var n = currentNode as Bridge;
                        if (n == u)
                        {
                            if (n.fromNodes.Contains(node))
                                continue;
                        }

                        if (n != u)
                        {
                            if (n.toNodes.Contains(node))
                                continue;
                        }
                        
                    }
                    var currentDistance = distances[node].Item1;
                    var potentialDistance = (distances[currentNode].Item1)+1;

                    if (potentialDistance < currentDistance)
                        distances[node] = new Tuple<int, Node>(potentialDistance, currentNode);
                }

                unvisitedNodes.Remove(currentNode);
                unvis = unvisitedNodes.Contains(v);
            }

            Node temp = v;
            Node remove = null;
            while (distances.ContainsKey(u))
            {
                path.Add(temp);
                remove = temp;
                temp = distances[temp].Item2;
                distances.Remove(remove);
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
