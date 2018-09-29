using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Movement
{

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D spriteSheet;
        Texture2D testLevel;

        //Menu Textures
        Texture2D menuImage;
        Texture2D menuBlocks;
        Texture2D menuSelectors;

        //Texture2D orbSheet, for full game
        //For now, individual orbs
        Texture2D passionVector;
        Texture2D passionVectorInfo;
        Texture2D splitChopper;
        Texture2D splitChopperInfo;

        double mapPosX;
        double mapPosY;

        int screenWidth;
        int screenHeight;

        double screenWidthMultiplier;
        double screenHeightMultiplier;

        int totalFrames;
        int currentFPS;
        double lastFPSReport;

        GameStateManager gameState;

        Player player;
        Vector2 playerRect;

        KeyboardState kb;
        KeyboardState prevKb;

        GamePadState gp;
        GamePadState prevGp;

        SpriteFont arial20;

        List<Orb> orbsList;
        Menu menu;

        // Animation
        int frame;              // The current animation frame
        int frameToUse;
        double timeCounter;     // The amount of time that has passed
        double fps;             // The speed of the animation
        double timePerFrame;    // The amount of time (in fractional seconds) per frame

        // Constants for "source" rectangle (inside the image)
        const int RunFrameCount = 5;       // The number of frames in the animation
        const int RunOffsetY = 720;        // How far down in the image are the frames?
        const int RunOffsetX = 0;
        const int RunRectHeight = 720;     // The height of a single frame
        const int RunRectWidth = 640;      // The width of a single frame

        const int JumpOffsetY = 0;        // How far down in the image are the frames?
        const int JumpOffsetX = 1280;
        const int JumpRectHeight = 720;     // The height of a single frame
        const int JumpRectWidth = 640;      // The width of a single frame

        //Orb Sprites Sheet

        const int OrbOffsetY = 0;        // How far down in the image are the frames?
        const int OrbOffsetX = 0;
        const int OrbRectHeight = 400;     // The height of a single frame
        const int OrbRectWidth = 400;      // The width of a single frame

        //Menu Selectors Sheet

        //Large Selector
        const int SelectorLOffsetX = 50;       
        const int SelectorLRectWidth = 525;

        //Small Selector
        const int SelectorSOffsetX = 625;
        const int SelectorSRectWidth = 150;

        //Same Y dimensions for both selectors
        const int SelectorOffsetY = 50;
        const int SelectorRectHeight = 200;

        //Bridge Block
        const int BridgeOffsetX = 825;
        const int BridgeRectWidth = 150;
        const int BridgeRectHeight = 50;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1920;  // window width
            graphics.PreferredBackBufferHeight = 1080;   // window height
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            CreateOrbList();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            
            screenWidth = graphics.PreferredBackBufferWidth;
            screenHeight = graphics.PreferredBackBufferHeight;

            
            screenHeightMultiplier = (double)screenHeight / 1080; 
            screenWidthMultiplier = (double)screenHeight * 16 / 9 / 1920; //Game will size to a 16x9 aspect ratio based on the window's height

            //FPS rules
            fps = 60.0;
            timePerFrame = 5 / fps; //Time between each frame of animation

            //Player starts in center of screen, standing facing right
            playerRect = new Vector2((int)(100 * screenWidthMultiplier), (int)(400 * screenHeightMultiplier));
            player = new Player(1, playerRect, screenWidthMultiplier, screenHeightMultiplier);

            gameState = new GameStateManager();
            menu = new Menu(screenWidthMultiplier, screenHeightMultiplier, orbsList);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //SpriteFont used to display text onscreen
            arial20 = Content.Load<SpriteFont>("Arial20");

            //SpriteSheet for player character
            spriteSheet = Content.Load<Texture2D>("ElixSpriteSheet");

            //Background for test level
            testLevel = Content.Load<Texture2D>("TestLevel");

            //Orbs in inventory
            passionVector = Content.Load<Texture2D>("PassionVector");
            passionVectorInfo = Content.Load<Texture2D>("PassionVectorInfo");
            splitChopper = Content.Load<Texture2D>("SplitChopper");
            splitChopperInfo = Content.Load<Texture2D>("SplitChopperInfo");

            //Menu Assets
            menuImage = Content.Load<Texture2D>("Menu");
            menuBlocks = Content.Load<Texture2D>("MenuBlocks");
            menuSelectors = Content.Load<Texture2D>("Selectors");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            //Controls whether to update FPS display
            totalFrames++;
            if ((gameTime.TotalGameTime.TotalSeconds - lastFPSReport) >= 1)
            {
                currentFPS = totalFrames;
                totalFrames = 0;
                lastFPSReport = gameTime.TotalGameTime.TotalSeconds;
            }

            // Handles animation for you
            UpdateAnimation(gameTime);

            // Update both keyboard states and gamepad states
            prevKb = kb;
            kb = Keyboard.GetState();
            prevGp = gp;
            gp = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.IndependentAxes);

            //Game will only proceed if unpaused
            gameState.UpdateGameState(kb, prevKb, gp, prevGp);

            if (gameState.GameState == GameState.play)
            {
                //Run the player's update method, passing in current and previous keyboard states
                player.UpdateCharacter();
                player.UpdatePlayer(kb, prevKb, gp, prevGp);

                //Reset Menu Positions
                //NOTE: This should not be running every frame, but switching between states is currently done in the gameStateManager, which doesn't have access to the menu instance
                menu.Reset();

                base.Update(gameTime);
            }
            else if (gameState.GameState == GameState.menu)
            {
                menu.MoveSelector(kb, prevKb, gp, prevGp);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            if (player.LocationX != (960 * screenWidthMultiplier))
            {
                mapPosX += player.LocationX - 960 * screenWidthMultiplier;
                player.LocationX -= (float)(player.Location.X - 960 * screenWidthMultiplier);
            }           
            /*
            if (player.Location.Y < (240 * screenHeightMultiplier))
            {
                mapPosY += player.LocationY - 480 * screenWidthMultiplier;
                player.LocationY -= (float)(player.Location.Y - 240 * screenHeightMultiplier);
            }
            */

            //Draw the background
            spriteBatch.Draw(testLevel, new Rectangle((int)(0 - mapPosX), (int)(-1080 * screenHeightMultiplier + mapPosY), (int)(4000 * screenWidthMultiplier), (int)(2160 * screenHeightMultiplier)), Color.White);

            //Display the player's current state in the top left corner of the screen
            spriteBatch.DrawString(arial20, "Current State: " + player.CharacterState.ToString(), new Vector2(screenWidth / 20, screenWidth / 20), Color.White);
            spriteBatch.DrawString(arial20, "Momentum: " + player.Momentum, new Vector2(screenWidth / 20, screenWidth / 15), Color.White);

            //Display current fps
            spriteBatch.DrawString(arial20, "FPS: " + currentFPS, new Vector2(screenWidth / 20, screenWidth / 10), Color.White);            
            
            //Animate player character based on their state
            switch (player.CharacterState)
            {
                case CharacterState.stand:
                    if (player.direction == 0)
                    {
                        DrawPlayerStanding(SpriteEffects.FlipHorizontally);
                    }
                    else
                    {
                        DrawPlayerStanding(SpriteEffects.None);
                    }
                    break;

                case CharacterState.dash:
                    if (player.direction == 0)
                    {
                        DrawPlayerDashing(SpriteEffects.FlipHorizontally);
                    }
                    else
                    {
                        DrawPlayerDashing(SpriteEffects.None);
                    }
                    break;

                case CharacterState.run:
                    if (player.direction == 0)
                    {
                        DrawPlayerRunning(SpriteEffects.FlipHorizontally);
                    }
                    else
                    {
                        DrawPlayerRunning(SpriteEffects.None);
                    }
                    break;
                
                case CharacterState.jump:
                    if (player.direction == 0)
                    {
                        DrawPlayerJumping(SpriteEffects.FlipHorizontally);
                    }
                    else
                    {
                        DrawPlayerJumping(SpriteEffects.None);
                    }
                    break;

                case CharacterState.fall:
                    if (player.direction == 0)
                    {
                        DrawPlayerFalling(SpriteEffects.FlipHorizontally);
                    }
                    else
                    {
                        DrawPlayerFalling(SpriteEffects.None);
                    }
                    break;

                case CharacterState.skidLag:
                    if (player.direction == 0)
                    {
                        DrawPlayerSkidLag(SpriteEffects.FlipHorizontally);
                    }

                    else
                    {
                        DrawPlayerSkidLag(SpriteEffects.None);
                    }
                    break;

                case CharacterState.landLag:         
                    if (player.direction == 0)
                    {
                        DrawPlayerLandLag(SpriteEffects.FlipHorizontally);
                    }
                    
                    else
                    {
                        DrawPlayerLandLag(SpriteEffects.None);
                    }
                    break;

                case CharacterState.shift:
                    if (player.direction == 0)
                    {
                        DrawPlayerStanding(SpriteEffects.FlipHorizontally);
                    }

                    else
                    {
                        DrawPlayerStanding(SpriteEffects.None);
                    }
                    break;

                case CharacterState.slide:
                    if (player.direction == 0)
                    {
                        DrawPlayerStanding(SpriteEffects.FlipHorizontally);
                    }

                    else
                    {
                        DrawPlayerStanding(SpriteEffects.None);
                    }
                    break;
            }

            //Draw the menu if the game is paused
            if (gameState.GameState == GameState.menu)
            {
                DrawMenu(SpriteEffects.None);
                DrawBridge(SpriteEffects.None);

                if (menu.CurrentTab == MenuTab.change)
                {
                    spriteBatch.Draw(menuBlocks, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    spriteBatch.Draw(passionVector, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    spriteBatch.Draw(splitChopper, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                }

                if (menu.InSubMenu == true)
                {
                    DrawSelectorS(SpriteEffects.None);

                    //Draw orb description for highlighted orb
                    if (menu.SelectedOrb == 0)
                    {
                        spriteBatch.Draw(passionVectorInfo, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    }
                    else if (menu.SelectedOrb == 1)
                    {
                        spriteBatch.Draw(splitChopperInfo, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    }

                }
                else
                {
                    DrawSelectorL(SpriteEffects.None);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            // Handle animation timing
            // - Add to the time counter
            // - Check if we have enough "time" to advance the frame
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;
            if (timeCounter >= timePerFrame)
            {
                //This statement moves backwards across the spritesheet to create the second half of the run cycle
                //It's hardcoded to the number of run frames, for now
                if (player.characterState == CharacterState.run)
                {
                    frame += 1; //Adjust the frame


                    if (frame <= RunFrameCount)
                    {
                        frameToUse = frame;
                    }
                    else if (frame == RunFrameCount + 1)
                    {
                        frameToUse = 4;
                    }
                    else if (frame == RunFrameCount + 2)
                    {
                        frameToUse = 3;
                    }
                    else if (frame == RunFrameCount + 3)
                    {
                        frameToUse = 2;
                    }
                    else if (frame == RunFrameCount + 4)
                    {
                        frameToUse = 1;
                    }
                    else if (frame >= RunFrameCount + 5)
                    {
                        frame = 0;
                        frameToUse = frame;
                    }
                }

                timeCounter -= timePerFrame;    // Remove the time we "used"
            }           
        }

        //Player Animations
        private void DrawPlayerStanding(SpriteEffects flipSprite)
        {
            spriteBatch.Draw(spriteSheet, player.Location, new Rectangle(0, 0, RunRectWidth, RunRectHeight), Color.White, 0, Vector2.Zero, (float)(0.35f * screenWidthMultiplier), flipSprite, 0);
        }

        private void DrawPlayerDashing(SpriteEffects flipSprite)
        {
            spriteBatch.Draw(spriteSheet, player.Location, new Rectangle(0 + RunOffsetX, RunOffsetY, RunRectWidth, RunRectHeight), Color.White, 0, Vector2.Zero, (float)(0.35f * screenWidthMultiplier), flipSprite, 0);
        }

        private void DrawPlayerRunning(SpriteEffects flipSprite)
        {
            spriteBatch.Draw(spriteSheet, player.Location, new Rectangle(frameToUse * RunRectWidth + RunOffsetX, RunOffsetY, RunRectWidth, RunRectHeight), Color.White, 0, Vector2.Zero, (float)(0.35f * screenWidthMultiplier), flipSprite, 0);
        }

        private void DrawPlayerJumping(SpriteEffects flipSprite)
        {
            spriteBatch.Draw(spriteSheet, player.Location, new Rectangle(JumpOffsetX, JumpOffsetY, JumpRectWidth, JumpRectHeight), Color.White, 0, Vector2.Zero, (float)(0.35f * screenWidthMultiplier), flipSprite, 0);
        }

        private void DrawPlayerFalling(SpriteEffects flipSprite)
        {
            spriteBatch.Draw(spriteSheet, player.Location, new Rectangle(JumpOffsetX + JumpRectWidth, JumpOffsetY, JumpRectWidth, JumpRectHeight), Color.White, 0, Vector2.Zero, (float)(0.35f * screenWidthMultiplier), flipSprite, 0);
        }

        private void DrawPlayerSkidLag(SpriteEffects flipSprite)
        {
            spriteBatch.Draw(spriteSheet, player.Location, new Rectangle(JumpRectWidth, 0, JumpRectWidth, JumpRectHeight), Color.White, 0, Vector2.Zero, (float)(0.35f * screenWidthMultiplier), flipSprite, 0);
        }

        private void DrawPlayerLandLag(SpriteEffects flipSprite)
        {
            spriteBatch.Draw(spriteSheet, player.Location, new Rectangle(JumpOffsetX + JumpRectWidth + JumpRectWidth, JumpOffsetY, JumpRectWidth, JumpRectHeight), Color.White, 0, Vector2.Zero, (float)(0.35f * screenWidthMultiplier), flipSprite, 0);
        }

        //Menu
        private void DrawMenu(SpriteEffects flipSprite)
        {
            spriteBatch.Draw(menuImage, new Vector2(0, 0), new Rectangle(0, 0, screenWidth, screenHeight), Color.White, 0, Vector2.Zero, (float)(1.0f * screenWidthMultiplier), flipSprite, 0);
        }

        private void DrawSelectorL(SpriteEffects flipSprite)
        {
            spriteBatch.Draw(menuSelectors, menu.SelectorPosition, new Rectangle(SelectorLOffsetX, SelectorOffsetY, SelectorLRectWidth, SelectorRectHeight), Color.White, 0, Vector2.Zero, (float)(1.0f * screenWidthMultiplier), flipSprite, 0);
        }

        private void DrawSelectorS(SpriteEffects flipSprite)
        {
            spriteBatch.Draw(menuSelectors, menu.SelectorPosition, new Rectangle(SelectorSOffsetX, SelectorOffsetY, SelectorSRectWidth, SelectorRectHeight), Color.White, 0, Vector2.Zero, (float)(1.0f * screenWidthMultiplier), flipSprite, 0);
        }  

        private void DrawBridge(SpriteEffects flipSprite)
        {
            spriteBatch.Draw(menuSelectors, menu.SelectorPosition, new Rectangle(BridgeOffsetX, SelectorOffsetY, BridgeRectWidth, BridgeRectHeight), Color.White, 0, Vector2.Zero, (float)(1.0f * screenWidthMultiplier), flipSprite, 0);
        }

        //For now, creates a list of all four orbs present in the prototype
        //Also sets their owned values to true
        public void CreateOrbList()
        {
            orbsList = new List<Orb>();
            for (int i = 0; i < 4; i++)
            {
                Orb newOrb = new Orb((OrbName)i);
                newOrb.Owned = true;
                orbsList.Add(newOrb);
            }
        }
    }
}
