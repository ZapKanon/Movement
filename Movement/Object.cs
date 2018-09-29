using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Movement
{
    abstract class Object
    {
        //Sets up everything that a generic object needs
        //All objects are further classified into players, npcs, pieces of terrain, collectibles, etc.

        //Fields

        //every object has an id number for sprite drawing
        public int id;
        public double screenWidthMultiplier;
        public double screenHeightMultiplier;
        public Vector2 location;

        //Constructor
        public Object(int id, Vector2 location, double screenWidthMultiplier, double screenHeightMultiplier)
        {
            this.id = id;
            this.location = location;
            this.screenWidthMultiplier = screenWidthMultiplier;
            this.screenHeightMultiplier = screenHeightMultiplier;
        }
    }
}
