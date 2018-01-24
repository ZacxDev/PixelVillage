using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using PixelVillage.Main;
using PixelVillage.GameWorld;
using System.IO;

namespace PixelVillage.UI
{
    public enum MenuState { MAIN, SELECT_WORLD }
    class MainMenu
    {
        private MenuState MenuState;
        private List<Button> Buttons = new List<Button>();
        private Button Main_Play, Main_Options, Main_Exit,
            Select_Load, Select_Create, Select_Delete, Select_Edit;

        public MainMenu()
        {
            ChangeMenuState(MenuState.MAIN);
        }

        private void _InitMainMenu()
        {
            // 96px apart (16px margin)
            Main_Play = new Button(Game.X_MID - 128, Game.CANVAS_HEIGHT - 384, 256, 64, "Play", delegate () { ChangeMenuState(MenuState.SELECT_WORLD); });
            Main_Options = new Button(Game.X_MID - 128, Game.CANVAS_HEIGHT - 288, 256, 64, "Options", delegate () { });
            Main_Exit = new Button(Game.X_MID - 128, Game.CANVAS_HEIGHT - 192, 256, 64, "Exit", delegate () { });
        }

        // add the relevant buttons to arry
        private void _PrepareMainMenu()
        {
            // Check if we need to init buttons before preparing
            if (Main_Play == null)
                _InitMainMenu();

            Buttons.Add(Main_Play);
            Buttons.Add(Main_Options);
            Buttons.Add(Main_Exit);
        }

        private void _InitSelectMenu()
        {
            Select_Load = new Button(Game.X_MID + 128, Game.CANVAS_HEIGHT - 128, 128, 32, "Load", C.MFont, delegate () { Game.GameState = GameState.LOADING; WorldHelper.LoadWorld(WorldButton.SELECTED_WORLDBUTTON.GetWorldName()); });
            Select_Edit = new Button(Game.X_MID + 128, Game.CANVAS_HEIGHT - 64, 128, 32, "Edit", C.MFont, delegate () {  });
            Select_Create = new Button(Game.X_MID - 256, Game.CANVAS_HEIGHT - 128, 128, 32, "Create", C.MFont, delegate () { });
            Select_Delete = new Button(Game.X_MID - 256, Game.CANVAS_HEIGHT - 64, 128, 32, "Delete", C.MFont, delegate () { });

            Select_Load.preRender = delegate (Button b) 
            {
                if (WorldButton.SELECTED_WORLDBUTTON == null)
                    b.Disable();
                else
                    b.Enable();

            };

            // Iterate subfolders in worlds dir to generate world buttons
            string worldsDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Sap\Worlds\";
            string[] worlds = Directory.GetDirectories(worldsDir);
            for (var i = 0; i < worlds.Length; i++)
            {
                var w = worlds[i];
                Buttons.Add(new WorldButton((int)(Game.CANVAS_WIDTH * 0.25), 96 * i, (int)(Game.CANVAS_WIDTH * 0.5), 64, w, delegate() { }));
            }

        }

        private void _PrepareSelectMenu()
        {
            // Check if we need to init buttons
            if (Select_Load == null)
                _InitSelectMenu();

            Buttons.Add(Select_Load);
            Buttons.Add(Select_Delete);
            Buttons.Add(Select_Create);
            Buttons.Add(Select_Edit);
        }

        public void ChangeMenuState(MenuState m)
        {
            Buttons.Clear();

            if (m == MenuState.SELECT_WORLD)
            {
                _PrepareSelectMenu();

            }
            else if (m == MenuState.MAIN)
            {
                _PrepareMainMenu();
            }

            MenuState = m;
        }

        public void render(ref Graphics g)
        {
            g.FillRectangle(Brushes.White, 0, 0, Game.CANVAS_WIDTH, Game.CANVAS_HEIGHT);

            for (int i = 0; i < Buttons.Count; i++)
            {
                Buttons[i].render(ref g);
            }
        }

        public void mouseDown(Rectangle cursor)
        {
            foreach (var b in Buttons)
            {
                if (cursor.IntersectsWith(b.GetBounds()))
                {
                    b.MouseDown();
                    return;
                }
            }
        }

        public void mouseMove(Rectangle cursor)
        {
            
        }
    }
}
