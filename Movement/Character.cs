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
    enum CharacterState
    {
        stand,
        dash,
        run,
        skidLag,
        jump,
        fall,
        landLag,
        shift,
        slide,
    }

    abstract class Character : Object
    {
        //Fields
        protected int dashTime;
        protected int runTime;
        protected int lagTime;
        protected int jumpTime;
        protected int fallTime;
        protected int shiftTime;
        protected int slideTime;

        protected int shiftX;
        protected int shiftY;
        protected bool didShift;

        //TODO: Change these to protected, make properties for use in Game1
        public int floorHeight;

        public int direction;
        public double momentum;
        public bool reachedApex;

        public CharacterState characterState;

        //Constructor
        public Character(int id, Vector2 location, double screenWidthMultiplier, double screenHeightMultiplier) : base(id, location, screenWidthMultiplier, screenHeightMultiplier)
        {
            floorHeight = (int)(563 * screenHeightMultiplier); //floorHeight is temporary, will be replaced by collisions
            direction = 1;
        }

        //Methods
        public void UpdateCharacter()
        {
            //Player falls if they aren't jumping and they aren't touching the floor
            if (location.Y < floorHeight && characterState != CharacterState.jump && characterState != CharacterState.shift)
            {
                characterState = CharacterState.fall;
            }
            else if (location.Y > floorHeight)
            {
                location.Y = floorHeight;
            }
            switch (characterState)
            {              

                //STAND STATE
                case (CharacterState.stand):
                    //Reset runTime, fallTime, momentum, and refresh shift
                    fallTime = 0;
                    runTime = 0;
                    momentum = 0;
                    shiftTime = 0;
                    didShift = false;

                    StandState();
                    break;

                //DASH STATE
                case (CharacterState.dash):
                    DashState();
                    break;

                //RUN STATE
                case (CharacterState.run):        
                    RunState();                                 
                    break;

                //SKIDLAG STATE
                case (CharacterState.skidLag):
                    SkidLagState();
                    break;

                //JUMP STATE
                case (CharacterState.jump):
                    JumpState();
                    break;

                //FALL STATE
                case (CharacterState.fall):
                    //Reset jumpTime and shiftTime
                    jumpTime = 0;
                    shiftTime = 0;
                    //Reset reachedApex;
                    reachedApex = false;

                    FallState();
                    break;

                //LANDLAG STATE
                case (CharacterState.landLag):
                    LandLagState();
                    break;

                //SHIFT STATE
                case (CharacterState.shift):
                    ShiftState();
                    break;
                //SLIDE STATE
                case (CharacterState.slide):
                    SlideState();
                    break;
            }
        }

        //State Methods
        //Called by universal UpdateCharacter method, overwritten by child classes
        //Player reads controller inputs while NPCS do not, etc.
        public abstract void StandState();

        public abstract void DashState();

        public abstract void RunState();

        public abstract void SkidLagState();

        public abstract void JumpState();

        public abstract void FallState();

        public abstract void ShiftState();

        public abstract void SlideState();

        public abstract void LandLagState();

        //Controls player behavior during dash
        public void Dashing()
        {
            //Player is dashing right
            if (direction == 1)
            {
                location.X += (float)(18 * (screenWidthMultiplier));
                momentum = 1.6;
            }
            //Player is dashing left
            if (direction == 0)
            {
                location.X -= (float)(18 * (screenWidthMultiplier));
                momentum = -1.6;
            }
            //Increment dashTime
            dashTime++;

        }

        //Controls acceleration of player during run
        public void Running()
        {
            //"Initial dash" is faster than sustained run? Not currently
            //Capped at 10
            if (direction == 0)
            {
                if (runTime < 10)
                {
                    location.X -= (float)(18 * (screenWidthMultiplier));
                    momentum = -1.6;
                }
                else
                {
                    location.X -= (float)(18 * (screenWidthMultiplier));
                    momentum = -1.6;
                }
                runTime++;
            }
            else if (direction == 1)
            {

                if (runTime < 10)
                {
                    location.X += (float)(18 * (screenWidthMultiplier));
                    momentum = 1.6;
                }
                else
                {
                    location.X += (float)(18 * (screenWidthMultiplier));
                    momentum = 1.6;
                }
                runTime++;
            }
        }

        public void SkidLagging()
        {
            if (momentum > 0)
            {
                location.X += (float)(momentum * 10 * (screenWidthMultiplier));
                momentum -= 0.2;
            }
            else if (momentum < 0)
            {
                location.X += (float)(momentum * 10 * (screenWidthMultiplier));
                momentum += 0.2;
            }
            lagTime++;
        }

        //Controls upward acceleration of player during jump
        //Player can only move upward for a few seconds
        //Vertical speed slows over time
        public void Jumping()
        {
            fallTime = 0;
            switch (jumpTime)
            {
                case 0:
                case 1:
                case 2:
                    break;
                case 3:
                case 4:                   
                case 5:
                    location.Y -= (float)(32 * (screenHeightMultiplier));
                    break;
                case 6:
                case 7:
                case 8:
                    location.Y -= (float)(28 * (screenHeightMultiplier));
                    break;
                case 9:
                case 10:
                case 11:
                    location.Y -= (float)(24 * (screenHeightMultiplier));
                    break;
                case 12:
                case 13:
                case 14:
                    location.Y -= (float)(20 * (screenHeightMultiplier));
                    break;
                case 15:
                case 16:
                case 17:
                    location.Y -= (float)(16 * (screenHeightMultiplier));
                    break;
                case 18:
                case 19:
                case 20:
                    location.Y -= (float)(12 * (screenHeightMultiplier));
                    break;
                case 21:
                case 22:
                case 23:
                    location.Y -= (float)(8 * (screenHeightMultiplier));
                    break;
                case 24:
                case 25:
                case 26:
                    location.Y -= (float)(4 * (screenHeightMultiplier));
                    break;
                default:
                    reachedApex = true;
                    break;

            }
            jumpTime++;

        }

        //Controls horizontal acceleration of player while jumping or falling
        public void Drifting(int driftDirection)
        {
            location.X += (int)(11 * screenWidthMultiplier * momentum);

            if (driftDirection == 0)
            {
                if (momentum > -1)
                {
                    momentum -= 0.1;
                }
            }
            else if (driftDirection == 1)
            {
                if (momentum < 1)
                {
                    momentum += 0.1;
                }
            }
            else
            {
                if (momentum > 0)
                {
                    momentum -= 0.1;
                }
                else if (momentum < 0)
                {
                    momentum += 0.1;
                }
            }
        }

        //Controls downward acceleration of player while falling
        //Player falls until they touch the floor. This is handled in the falling state code
        //Vertical speed increases over time until terminal velocity is reached
        public void Falling()
        {
            switch (fallTime)
            {
                case 0:
                case 1:
                case 2:
                    location.Y += (float)(0 * (screenHeightMultiplier));
                    break;
                case 3:
                case 4:
                case 5:
                    location.Y += (float)(4 * (screenHeightMultiplier));
                    break;
                case 6:
                case 7:
                case 8:
                    location.Y += (float)(8 * (screenHeightMultiplier));
                    break;
                case 9:
                case 10:
                case 11:
                    location.Y += (float)(12 * (screenHeightMultiplier));
                    break;
                case 12:
                case 13:
                case 14:
                    location.Y += (float)(16 * (screenHeightMultiplier));
                    break;
                case 15:
                case 16:
                case 17:
                    location.Y += (float)(20 * (screenHeightMultiplier));
                    break;
                case 18:
                case 19:
                case 20:
                    location.Y += (float)(24 * (screenHeightMultiplier));
                    break;
                case 21:
                case 22:
                case 23:
                    location.Y += (float)(28 * (screenHeightMultiplier));
                    break;
                default:
                    location.Y += (float)(32 * (screenHeightMultiplier));
                    break;
            }
            fallTime++;
        }

        public void LandLagging()
        {
            if (momentum > 0)
            {
                location.X += (float)(momentum * 10 * (screenWidthMultiplier));
                momentum -= 0.2;
            }
            else if (momentum < 0)
            {
                location.X += (float)(momentum * 10 * (screenWidthMultiplier));
                momentum += 0.2;
            }
            lagTime++;
        }

        public void Shifting()
        {
            location.X += (float)((30 - (shiftTime * 2)) * shiftX * screenWidthMultiplier);
            location.Y -= (float)((30 - (shiftTime * 2)) * shiftY * screenWidthMultiplier);
            //Flat descent value (allows for sliding with horizontal shifts)
            location.Y += (float)(1 * screenWidthMultiplier);
        }

        public void Sliding()
        {
            
            location.X += (float)((30 - (shiftTime * 2)) * shiftX * screenWidthMultiplier);
        }
    }
}
