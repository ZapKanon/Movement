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
    enum GameState
    {
        play,
        menu,
        transform,

    }
    enum Direction
    {
        up = 0,
        down = 1,
        left = 2,
        right = 3,
        none = 4
    }

    class GameStateManager
    {
        //Fields
        private GameState gameState;

        //Properties
        public GameState GameState
        {
            get
            {
                return gameState;
            }
        }

        //Constructor
        public GameStateManager()
        {
            gameState = GameState.play;
        }

        //Methods
        public void UpdateGameState(KeyboardState kb, KeyboardState prevKb, GamePadState gp, GamePadState prevGp)
        {
            switch (gameState)
            {
                case (GameState.play):

                    if ((kb.IsKeyDown(Keys.Escape) && prevKb.IsKeyUp(Keys.Escape)) || gp.IsButtonDown(Buttons.Start) && prevGp.IsButtonUp(Buttons.Start))
                    {
                        gameState = GameState.menu;
                    }
                    break;

                case (GameState.menu):

                    if ((kb.IsKeyDown(Keys.Escape) && prevKb.IsKeyUp(Keys.Escape)) || gp.IsButtonDown(Buttons.Start) && prevGp.IsButtonUp(Buttons.Start))
                    {
                        gameState = GameState.play;
                        
                    }
                    break;
            }
        }
    }
}
