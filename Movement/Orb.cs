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
    enum OrbFaction
    {
        Chival,
        Pincer,
        Aspect,
        Special
    }

    enum OrbName
    {
        FramePlatformer,
        PassionVector,
        SplitChopper,
        NighThunder
    }
    class Orb
    {
        private OrbName name;
        private string description;
        private bool owned;

        //Constructor
        public Orb(OrbName name)
        {
            this.name = name;
            GetDescription();
        }

        //Properties

        public OrbName Name { get; }

        public OrbFaction Faction { get; }

        public bool Owned { get; set; }

        public string Description { get; }

        //Methods
        //Returns coordiantes to pull orb image from sheet in draw calls
        public Vector2 GetImageLoc()
        {
            int imageX = (((int)name) % 10) * 400;
            int imageY = (((int)name) % 10) * 400;

            Vector2 imageLoc = new Vector2(imageX, imageY);

            return imageLoc;
        }

        public void GetDescription()
        {
            switch (name)
            {
                case OrbName.FramePlatformer:
                    description = "FramePlatformer Description";
                    break;

                case OrbName.PassionVector:
                    description = "PassionVector Description";
                    break;

                case OrbName.SplitChopper:
                    description = "SplitChopper Description";
                    break;

                case OrbName.NighThunder:
                    description = "NighThunder Description";
                    break;
            }
        }
    }
}
