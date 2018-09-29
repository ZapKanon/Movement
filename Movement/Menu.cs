using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Movement
{
    //Handles menu tabs, all goings on regarding the pause menu

    enum MenuTab
    {
        status = 0,
        skills = 1,
        change = 2,
        settings = 3
    }

    //Basically a placeholder at this point
    enum Settings
    {
        resolution = 0,
        volumeMusic = 1,
        volumeSoundEffect = 2,
        framerate = 3,
        vsync = 4
    }

    class Menu
    {
        //Fields
        private bool inSubMenu;
        private MenuTab currentTab;
        private Vector2 selectorPosition;

        //Status Tab

        //Skills Tab

        //Change Tab
        private int selectedOrb;
        private List<Orb> ownedOrbs; 

        //Settings Tab
        private int selectedSetting;

        //Selector Size Values
        private int largeSelectorWidth;
        private int largeSelectorHeight;
        private int smallSelectorWidth;
        private int smallSelectorHeight;

        //Constructor
        public Menu(double screenWidthMultiplier, double screenHeightMultiplier, List<Orb> ownedOrbs)
        {
            inSubMenu = false;
            currentTab = MenuTab.status;

            this.ownedOrbs = ownedOrbs;

            largeSelectorWidth = (int)(525 * screenWidthMultiplier);
            largeSelectorHeight = (int) (200 * screenHeightMultiplier);
            smallSelectorWidth = (int)(150 * screenWidthMultiplier);
            smallSelectorHeight = (int)(150 * screenHeightMultiplier);

            selectorPosition = new Vector2(125, 132);
        }

        //Properties
        public bool InSubMenu
        {
            get
            {
                return inSubMenu;
            }
        }

        public MenuTab CurrentTab
        {
            get
            {
                return currentTab;
            }
        }

        public int SelectedOrb
        {
            get
            {
                return selectedOrb;
            }
        }

        public Vector2 SelectorPosition
        {
            get
            {
                return selectorPosition;
            }
        }
        
        //Methods
        //Moves the green selection border through the menu or a submenu
        public void MoveSelector(KeyboardState kb, KeyboardState prevKb, GamePadState gp, GamePadState prevGp)
        {
            //Initialize direction
            Direction direction = Direction.down;

            //Interpret input and choose a direction
            //Direcitons take priority down > right > up > left

            if ((kb.IsKeyDown(Keys.S) == true && prevKb.IsKeyUp(Keys.S) == true) || (gp.ThumbSticks.Left.Y < -0.3 && prevGp.ThumbSticks.Left.Y > -0.3))
            {
                direction = Direction.down;
            }
            else if ((kb.IsKeyDown(Keys.D) == true && prevKb.IsKeyUp(Keys.D) == true) || (gp.ThumbSticks.Left.X > 0.3 && prevGp.ThumbSticks.Left.X < 0.3))
            {
                direction = Direction.right;
            }
            else if ((kb.IsKeyDown(Keys.W) == true && prevKb.IsKeyUp(Keys.W) == true) || (gp.ThumbSticks.Left.Y > 0.3 && prevGp.ThumbSticks.Left.Y < 0.3))
            {
                direction = Direction.up;
            }
            else if ((kb.IsKeyDown(Keys.A) == true && prevKb.IsKeyUp(Keys.A) == true) || (gp.ThumbSticks.Left.X < -0.3 && prevGp.ThumbSticks.Left.X > -0.3))
            {
                direction = Direction.left;
            }
            else
            {
                direction = Direction.none;
            }

            //Do things in the menu
            //The player has not entered a submenu
            if (inSubMenu == false && direction != Direction.none)
            {
                switch (direction)
                {
                    //Move into displayed submenu if it has things to select
                    case Direction.right:
                        //Play bump sound
                        break;

                    //Move up one submenu, or wrap to bottom
                    case Direction.up:
                        if (currentTab > MenuTab.status)
                        {
                            currentTab = currentTab - 1;
                        }
                        else
                        {
                            currentTab = MenuTab.settings;
                        }
                        break;

                    //Move down one submenu, or wrap to top
                    case Direction.down:
                        if (currentTab < MenuTab.settings)
                        {
                            currentTab = currentTab + 1;
                        }
                        else
                        {
                            currentTab = MenuTab.status;
                        }
                        break;

                    case Direction.left:
                        //Play bump sound
                        break;
                }

                //Update Selector Data
                selectorPosition = new Vector2(125, 132 + 212 * (int)currentTab);
            }
            //when in a submenu
            else if (inSubMenu == true && direction != Direction.none)
            {
                switch (currentTab)
                {
                    case MenuTab.status:
                        //No submenu interaction
                        break;
                    case MenuTab.skills:
                        //No submenu interaction
                        break;
                    case MenuTab.change:
                        switch (direction)
                        {
                            case Direction.up:
                                //Move to new orb
                                if (selectedOrb >= 4)
                                {
                                    selectedOrb -= 4;
                                }
                                //wrap to bottom of list
                                else
                                {
                                    selectedOrb += 16;
                                }
                                break;
                            case Direction.down:
                                //Move to new orb
                                if (selectedOrb < 16)
                                {
                                    selectedOrb += 4;
                                }
                                //wrap to top of list
                                else
                                {
                                    selectedOrb -= 16;
                                }
                                break;
                            case Direction.left:
                                //Move to new orb
                                if (selectedOrb % 4 != 0)
                                {
                                    selectedOrb -= 1;
                                }
                                //Wrap to right of list
                                else
                                {
                                    selectedOrb += 3;
                                }                              
                                break;
                            case Direction.right:
                                //Move to new orb
                                if (selectedOrb % 4 != 3)
                                {
                                    selectedOrb += 1;
                                }
                                //Wrap to right of list
                                else
                                {
                                    selectedOrb -= 3;
                                }
                                break;
                        }

                        //Update Small Selector Data
                        selectorPosition = new Vector2(725 + 158 * ((int)selectedOrb % 4), 148 + 159 * (int)(selectedOrb / 4));

                        break;
                    case MenuTab.settings:
                        //Will have interaction, one day
                        break;
                }
            }

            //Press A to move into current submenu
            else if (inSubMenu == false && (kb.IsKeyDown(Keys.Space) == true && (prevKb.IsKeyUp(Keys.Space) == true) || (gp.IsButtonDown(Buttons.A) == true && prevGp.IsButtonUp(Buttons.A) == true)))
            {
                if (currentTab == MenuTab.change)
                {
                    inSubMenu = true;
                    //Set initial small selector position
                    selectorPosition = new Vector2(725 + 158 * ((int)selectedOrb % 4), 148 + 159 * (int)(selectedOrb / 4));
                }
            }
            //Press B to return to main menu from submenu
            else if (InSubMenu == true && (kb.IsKeyDown(Keys.R) == true && (prevKb.IsKeyUp(Keys.R) == true) || (gp.IsButtonDown(Buttons.B) == true && prevGp.IsButtonUp(Buttons.B) == true)))
            {
                inSubMenu = false;
                //Return large selector to correct position
                selectorPosition = new Vector2(125, 132 + 212 * (int)currentTab);
            }
        }

        //Closes submenus and resets selector to initial position
        public void Reset()
        {
            inSubMenu = false;
            selectorPosition = new Vector2(125, 132);
            currentTab = MenuTab.status;
        }
    }
}
