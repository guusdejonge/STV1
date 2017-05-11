using STVRogue.GameLogic;
using System;
namespace STVRogue
{
    public class Command
    {

        public string text;
        public Node previousNode;
        public Command(string text) { this.text = text; }
        override public string ToString() { return "no-action"; }

    }
}
