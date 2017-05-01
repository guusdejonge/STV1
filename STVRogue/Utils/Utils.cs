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
            var distances = new Dictionary<Node , int>();
            var path = new List<Node>();
            Node currentNode = u;
            Zone currentZone = null;
            var unvisitedNodes = new List<Node>();
            //var unvisitedZones = new List<Zone>();

            foreach(Zone zone in zones)
            {
                if (zone.nodes.Contains(u))
                {
                    //unvisitedZones = zones;
                    //unvisitedZones.Remove(zone);
                    currentZone = zone;

                    //unvisitedNodes = zone.nodes;
                    //foreach (Node node in zone.nodes)
                    //    distances.Add(node, 999);
                }

            }

            while (true)
            {
                if (currentZone.nodes.Contains(v))
                {
                    path.AddRange(shortestPathInZone(u, v, currentZone));
                }

                else
                {
                    path.AddRange(shortestPathToBridge(u, currentZone));
                }

                foreach (Node node in currentNode.neighbors)
                {
                    distances[node] = 1;
                    unvisitedNodes.Remove(node);
                }

                unvisitedNodes.Remove(currentNode);

            }
            



            if(u.neighbors.Contains(v))
            {
                path.Add(u);
                return path;
            }

            else
            {
                foreach (Node node in u.neighbors)
                {

                }
            }

            

            throw new NotImplementedException();
        }

        private List<Node> shortestPathInZone(Node u, Node v, Zone zone)
        {
            //unvisitedNodes = zone.nodes;
            //foreach (Node node in zone.nodes)
            //    distances.Add(node, 999);

            throw new NotImplementedException();
        }

        private List<Node> shortestPathToBridge(Node u, Zone zone)
        {
            throw new NotImplementedException();
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
        static private Random rnd_ = null ; 
        static public Random rnd { 
            get { if (rnd_==null) rnd_ = new Random();
                  return rnd_ ; }
        }


        static public void initializeWithSeed(int seed)
        {
            rnd_ = new Random(seed);
        }

          
    }

}
