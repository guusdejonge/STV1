using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STVRogue.GameLogic
{
    public class GamePlay
    {
        public int L;
        public int M;
        public int N;
        public int S;
        public List<Command> Commands = new List<Command>();

        public GamePlay() { }

        public GamePlay(int difficultyLevel, int nodeCapcityMultiplier, int numberOfMonsters, int seed, List<Command> com)
        {
            L = difficultyLevel;
            M = nodeCapcityMultiplier;
            N = numberOfMonsters;
            S = seed;
            Commands.Clear();
            foreach(Command c in com)
            {
                Commands.Add(c);
            }
        }

        public void Save(string fileName)
        {
            List<String> saveLines = new List<String>();

            saveLines.Add(L.ToString());
            saveLines.Add(M.ToString());
            saveLines.Add(N.ToString());
            saveLines.Add(S.ToString());

            foreach (Command c in Commands)
            {
                saveLines.Add(c.text);
            }

            System.IO.File.WriteAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName), saveLines.ToArray());
        }

        public void Load(string fileName)
        {
            string[] readLines = System.IO.File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName));

            L = Int32.Parse(readLines[0]);
            M = Int32.Parse(readLines[1]);
            N = Int32.Parse(readLines[2]);
            S = Int32.Parse(readLines[3]);

            Commands.Clear();
            
            for (int i = 4; i < readLines.Length; i++)
            {
                Commands.Add(new Command(readLines[i]));
            }

            foreach(Command c in Commands)
            {
                Console.WriteLine("JA");
            }
        }

        public void Replay(Specification s)
        {
            int level = 1;
            Game g = new Game(L, M, N);
          
            Save("savedata.txt");
            g.specification = s;
            g.loadGamePlay();

            bool go = true;

            while (go)
            {
  
                if (g.player.location is Bridge)
                {
                    int lvl = g.dungeon.level(g.player.location as Bridge);
                    level = lvl + 1;
                }
                if (g.player.location.zone.nodes.Contains(g.dungeon.exitNode))
                {
                    foreach (var n in g.player.location.zone.nodes)
                        n.alert = true;
                }

                if (g.player.location.contested)
                {
                    fight(g.player.location, g);
                }
                else
                {
                    var command = "";
                    if (g.commandsLoaded.Count() > 0)
                    {
                        command = g.commandsLoaded.First();
                        g.commandsLoaded.RemoveAt(0);
                    }
                    else
                    {
                        go = false;
                        break;
                    }
                    g.update(new Command(command.ToUpper()));
                }
                g.test();
            }
        }

        static void fight(Node node, Game g)
        {
            while (node.contested)
            {
                var cmd = new Command("");
                if (g.commandsLoaded.Count() > 0)
                {
                    cmd = new Command(g.commandsLoaded.First());
                    g.commandsLoaded.RemoveAt(0);
                }
                else
                {
                    cmd = new Command(Console.ReadLine());
                }
                cmd.previousNode = g.prevNode;
                node.fight(g.player, cmd);
                g.commands.Add(cmd);
                g.turn++;
                g.test();
            }
            int mon = g.dungeon.calculateMonstersInDungeon();
        }
    }
}
