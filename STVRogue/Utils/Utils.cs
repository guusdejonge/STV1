using STVRogue.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STVRogue.Utils
{
    public class UtilsClass
    {
        public List<Node> shortestPath(Node u, Node v)
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
