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

        /* This creates a player and a random dungeon of the given difficulty level and node-capacity
         * The player is positioned at the dungeon's starting-node.
         * The constructor also randomly seeds monster-packs and items into the dungeon. The total
         * number of monsters are as specified. Monster-packs should be seeded as such that
         * the nodes' capacity are not violated. Furthermore the seeding of the monsters
         * and items should meet the balance requirements stated in the Project Document.
         */
        public Game(int difficultyLevel, int nodeCapcityMultiplier, int numberOfMonsters) 
        {
            int Seed = DateTime.Now.Millisecond;

            Logger.log("Creating a game of difficulty level "+ difficultyLevel + ", node capacity multiplier "
                       + nodeCapcityMultiplier + ", and " + numberOfMonsters + " monsters.");
            player = new Player();
            dungeon = new Dungeon(difficultyLevel, nodeCapcityMultiplier, numberOfMonsters, Seed);
        }

        public void saveGame(int difficultyLevel, int nodeCapcityMultiplier, int numberOfMonsters, int Seed)    //the game will be saved in STVRogue_Main\bin\Debug as savedata.txt
        {
            List<String> saveLines = new List<String>();

            saveLines.Add(difficultyLevel.ToString());
            saveLines.Add(nodeCapcityMultiplier.ToString());
            saveLines.Add(numberOfMonsters.ToString());
            saveLines.Add(Seed.ToString());
            
            //save actions

            System.IO.File.WriteAllLines(AppDomain.CurrentDomain.BaseDirectory + "savedata.txt", saveLines.ToArray());
        }

        public void loadGame()    //the game will be saved in STVRogue_Main\bin\Debug as savedata.txt
        {
            string[] readLines = System.IO.File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "savedata.txt");
            dungeon = new Dungeon(Int32.Parse(readLines[0]), Int32.Parse(readLines[1]), Int32.Parse(readLines[2]), Int32.Parse(readLines[3]));

            foreach (string s in readLines)
            {
                //perform actions
            }
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
            Logger.log("Player does " +  userCommand);
            return true;
        }
    }
    
    public class GameCreationException: Exception {
       //public GameCreationException() {}
       public GameCreationException(String explanation) : base(explanation){}  
    }
}
