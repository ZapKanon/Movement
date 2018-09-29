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
    class Player : Character
    {
        //Fields

        //Input States
        KeyboardState kb;
        KeyboardState prevKb;
        GamePadState gp;
        GamePadState prevGp;

        //Stored Input Info.
        bool storedJump;
        int storedJumpTime;

        //Properties
        public Vector2 Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }

        public float LocationX
        {
            get
            {
                return location.X;
            }
            set
            {
                location.X = value;
            }
        }

        public float LocationY
        {
            get
            {
                return location.Y;
            }
            set
            {
                location.Y = value;
            }
        }

        public CharacterState CharacterState
        {
            get
            {
                return characterState;
            }
            set
            {
                characterState = value;
            }
        }

        public double Momentum
        {
            get
            {
                return momentum;
            }
            set
            {
                momentum = value;
            }
        }


        //Constructor
        public Player(int id, Vector2 location, double screenWidthMultiplier, double screenHeightMultiplier) : base(id, location, screenWidthMultiplier, screenHeightMultiplier)
        {

        }

        //Methods

        //Runs every frame to inform player class on control states
        public void UpdatePlayer(KeyboardState kb, KeyboardState prevKb, GamePadState gp, GamePadState prevGp)
        {
            this.kb = kb;
            this.prevKb = prevKb;
            this.gp = gp;
            this.prevGp = prevGp;
            AddInputQueue(kb, prevKb, gp, prevGp);
        }

        //Queue holds inputs if the player character is in lag when buttons are pressed
        //Inputs are held for 0.1 seconds and carried out automatically if the player character leaves lag in time
        //Otherwise the inputs are discarded
        public void AddInputQueue(KeyboardState kb, KeyboardState prevKb, GamePadState gp, GamePadState prevGp)
        {
            if (storedJump == true)
            {
                if (storedJumpTime > 6)
                {
                    storedJumpTime = 0;
                    storedJump = false;
                }
                else if (characterState == CharacterState.stand)
                {
                    Jumping();
                    storedJump = false;
                }
                else
                {
                    storedJumpTime++;
                }
            }

            if ((kb.IsKeyDown(Keys.W) && prevKb.IsKeyUp(Keys.W)) || gp.IsButtonDown(Buttons.B) && prevGp.IsButtonUp(Buttons.B))
            {
                storedJump = true;
                storedJumpTime = 0;
            }
        }

        //STAND STATE
        public override void StandState()
        {
            //Check for jump input
            if ((kb.IsKeyDown(Keys.W) && prevKb.IsKeyUp(Keys.W)) || (gp.IsButtonDown(Buttons.B) && prevGp.IsButtonUp(Buttons.B)))
            {
                characterState = CharacterState.jump;
            }

            //Player is facing right
            else if (direction == 1)
            {
                //Start dash
                if (kb.IsKeyDown(Keys.D) || gp.ThumbSticks.Left.X > 0.3)
                {
                    characterState = CharacterState.dash;
                }
                //Turn around
                else if (kb.IsKeyDown(Keys.A) || gp.ThumbSticks.Left.X < -0.3)
                {
                    direction = 0;
                    characterState = CharacterState.stand;
                }
            }

            //Player is facing left
            else if (direction == 0)
            {
                //Start dash
                if (kb.IsKeyDown(Keys.A) || gp.ThumbSticks.Left.X < -0.3)
                {
                    characterState = CharacterState.dash;
                }
                //Turn around
                else if (kb.IsKeyDown(Keys.D) || gp.ThumbSticks.Left.X > 0.3)
                {
                    direction = 1;
                    characterState = CharacterState.stand;
                }
            }
        }

        //DASH STATE - Incomplete
        public override void DashState()
        {
            //If dash hasn't ended naturally
            if (dashTime < 24)
            {
                //Jump out of dash
                if ((kb.IsKeyDown(Keys.W) && prevKb.IsKeyUp(Keys.W)) || (gp.IsButtonDown(Buttons.B) && prevGp.IsButtonUp(Buttons.B)))
                {
                    characterState = CharacterState.jump;
                }
                //If player is facing right
                else if (direction == 1) 
                {                   
                    //Start a new dash in the opposite direction
                    if ((kb.IsKeyDown(Keys.A) || gp.ThumbSticks.Left.X < -0.3))
                    {
                        dashTime = 0;
                        direction = 0;
                        Dashing();
                    }
                    //Continue current dash
                    else
                    {
                        Dashing();
                    }
                }
                //If player is facing left
                else if (direction == 0)
                {
                    
                    //Start a new dash in the opposite direction
                    if ((kb.IsKeyDown(Keys.D) || gp.ThumbSticks.Left.X > 0.3))
                    {
                        dashTime = 0;
                        direction = 1;
                        Dashing();
                    }
                    //Continue current dash
                    else
                    {
                        Dashing();
                    }
                }               
            }
            //Transition from dash to run
            else if (direction == 1)
            {
                dashTime = 0;
                if (kb.IsKeyDown(Keys.D) || gp.ThumbSticks.Left.X > 0.3)
                {                    
                    characterState = CharacterState.run;
                }
                else
                {
                    characterState = CharacterState.stand;
                }                   
            }
            else if (direction == 0)
            {
                dashTime = 0;
                if (kb.IsKeyDown(Keys.A) || gp.ThumbSticks.Left.X < -0.3)
                {
                    characterState = CharacterState.run;
                }
                else
                {
                    characterState = CharacterState.stand;
                }
            }
        }

        //RUN STATE
        public override void RunState()
        {
            //Check for jump out of run
            if ((kb.IsKeyDown(Keys.W) && prevKb.IsKeyUp(Keys.W)) || (gp.IsButtonDown(Buttons.B) && prevGp.IsButtonUp(Buttons.B)))
            {
                characterState = CharacterState.jump;
            }
            //Player is facing right
            else if (direction == 1)
            {
                //Continue running
                if (kb.IsKeyDown(Keys.D) || gp.ThumbSticks.Left.X > 0.3)
                {
                    Running();
                }
                //Turn around, suffer lag
                else if ((kb.IsKeyDown(Keys.A) || gp.ThumbSticks.Left.X < -0.3))
                {
                    direction = 0;
                    characterState = CharacterState.skidLag;
                }
                //Come to a halt, suffer lag
                else if ((kb.IsKeyUp(Keys.D) || gp.ThumbSticks.Left.X < 0.3))
                {
                    characterState = CharacterState.skidLag;
                }
            }
            //Player is facing left
            if (direction == 0)
            {
                //Continue running
                if (kb.IsKeyDown(Keys.A) || gp.ThumbSticks.Left.X < -0.3)
                {
                    Running();
                }
                //Turn around, suffer lag
                else if (kb.IsKeyDown(Keys.D) || gp.ThumbSticks.Left.X > 0.3)
                {
                    direction = 0;
                    characterState = CharacterState.skidLag;
                }
                //Come to a halt, suffer lag
                else if (kb.IsKeyUp(Keys.A) || gp.ThumbSticks.Left.X > -0.3)
                {
                    characterState = CharacterState.skidLag;
                }
            }
        }

        //SKIDLAG STATE
        public override void SkidLagState()
        {
            if (lagTime > 12)
            {
                lagTime = 0;
                characterState = CharacterState.stand;
            }
            else
            {
                SkidLagging();
            }
        }

        //JUMP STATE
        public override void JumpState()
        {
            //Begin shift
            if (((kb.IsKeyDown(Keys.LeftShift) && prevKb.IsKeyUp(Keys.LeftShift)) || gp.IsButtonDown(Buttons.LeftTrigger) && prevGp.IsButtonUp(Buttons.LeftTrigger)) && didShift == false)
            {
                characterState = CharacterState.shift;
            }
            //Continue rising
            else if (((kb.IsKeyDown(Keys.W) || gp.IsButtonDown(Buttons.B)) && reachedApex == false) || jumpTime < 8) //Controls minimum jump height
            {
                Jumping();
            }
            //Begin fall
            else
            {
                characterState = CharacterState.fall;
            }

            //Drift to the right
            if ((kb.IsKeyDown(Keys.D) && kb.IsKeyUp(Keys.A)) || (gp.ThumbSticks.Left.X > 0.3 && gp.ThumbSticks.Left.X > -0.3))
            {
                Drifting(1);
            }
            //Drift to the left
            else if ((kb.IsKeyDown(Keys.A) && kb.IsKeyUp(Keys.D)) || (gp.ThumbSticks.Left.X < -0.3 && gp.ThumbSticks.Left.X < 0.3))
            {
                Drifting(0);
            }
            //Drift toward neutral
            else
            {
                Drifting(2);
            }
        }

        //FALL STATE
        public override void FallState()
        {
            //Begin shift
            if (((kb.IsKeyDown(Keys.LeftShift) && prevKb.IsKeyUp(Keys.LeftShift)) || gp.IsButtonDown(Buttons.LeftTrigger) && prevGp.IsButtonUp(Buttons.LeftTrigger)) && didShift == false)
            {
                characterState = CharacterState.shift;
            }
            //Drift to the right
            else if ((kb.IsKeyDown(Keys.D) && kb.IsKeyUp(Keys.A)) || (gp.ThumbSticks.Left.X > 0.3 && gp.ThumbSticks.Left.X > -0.3))
            {
                Drifting(1);
            }
            //Drift to the left
            else if ((kb.IsKeyDown(Keys.A) && kb.IsKeyUp(Keys.D)) || (gp.ThumbSticks.Left.X < -0.3 && gp.ThumbSticks.Left.X < 0.3))
            {
                Drifting(0);
            }
            //Drift toward neutral
            else
            {
                Drifting(2);
            }

            //Continue falling
            if (location.Y < floorHeight)
            {
                Falling();
            }
            //Land on ground
            else
            {
                location.Y = floorHeight;
                characterState = CharacterState.stand;
            }
        }

        //LANDLAG STATE
        public override void LandLagState()
        {
            if (lagTime > 12)
            {
                lagTime = 0;
                characterState = CharacterState.stand;
            }
            else
            {
                lagTime++;
            }
            
        }

        //SHIFT STATE
        public override void ShiftState()
        {
            momentum = 0;
            //Proceed to fall when shift ends
            //Mark shift as used until player touches the ground
            if (shiftTime > 15)
            {
                characterState = CharacterState.fall;
                didShift = true;
            }
            //Determine shift direction
            else if (shiftTime == 0)
            {
                if (gp.ThumbSticks.Left.X > 0.3)
                {
                    shiftX = 1;

                }
                else if (gp.ThumbSticks.Left.X < -0.3)
                {
                    shiftX = -1;
                }
                else
                {
                    shiftX = 0;
                }

                if (gp.ThumbSticks.Left.Y > 0.2)
                {
                    shiftY = 1;
                }
                else if (gp.ThumbSticks.Left.Y < -0.2)
                {
                    shiftY = -1;
                }
                else
                {
                    shiftY = 0;
                }
            }

            //Transition to slide if touching the ground (will update when collision is implemented)
            if (LocationY >= floorHeight)
            {
                characterState = CharacterState.slide;
            }
            else
            {
                Shifting();
                shiftTime++;
            }   
        }

        //SLIDE STATE
        public override void SlideState()
        {
            if (shiftTime > 15)
            {
                characterState = CharacterState.stand;
                didShift = true;
            }
            else
            {
                Sliding();
                shiftTime++;
            }
        }
    }
}
