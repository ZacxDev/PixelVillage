using PixelVillage.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PixelVillage.Main
{
    class KeyInput
    {
        // Left, Right
        private static bool[] _KeyDown = new bool[4];

        public static void HandleKeyDown(KeyEventArgs e)
        {
           

            // CHANGE TO SWITCH CASE
            Keys k = e.KeyCode;
            //System.Diagnostics.Debug.WriteLine(k);
            if (k == Keys.Escape)
            {
                if (Game.P != null && Game.P.GetInventory().IsOpen())
                    Game.P.GetInventory().Close();
                else
                    Application.Exit();
            }

            // if on menu, dont handle all keys
            if (Game.GameState == GameState.MENU)
            {
                if (k == Keys.Enter)
                    WorldButton.LoadSelectedWorld();
                return;
            }

            if (k == Keys.Right && !_KeyDown[1])
            {
                _KeyDown[1] = true;
                // _KeyDown[0] = false;
                Game.P.VelX = C.PLAYER_SPEED;
            }
            else if (k == Keys.Left && !_KeyDown[0])
            {
                _KeyDown[0] = true;
                //_KeyDown[1] = false;
                Game.P.VelX = -C.PLAYER_SPEED;
            }
            else if (k == Keys.Up && !_KeyDown[2])
            {
                _KeyDown[2] = true;
                // _KeyDown[0] = false;
                Game.P.VelY = C.PLAYER_SPEED;
            }
            else if (k == Keys.Down && !_KeyDown[3])
            {
                _KeyDown[3] = true;
                //_KeyDown[1] = false;
                Game.P.VelY = -C.PLAYER_SPEED;
            }
            else if (k == Keys.D && e.Control)
                Game.WB.Toggle();
            else if (k == Keys.Menu)
            {
                // Game.P.Jump();
                e.Handled = true;
            }

            else if (k == Keys.E)
                Game.P.GetInventory().ToggleOpen();

        }

        public static void HandleKeyUp(KeyEventArgs e)
        {
            Keys k = e.KeyCode;

            if (k == Keys.Right)
            {
                _KeyDown[1] = false;
                Game.P.VelX = 0;

                if (_KeyDown[0])
                    Game.P.VelX = -C.PLAYER_SPEED;

            }
            else if (k == Keys.Left)
            {
                _KeyDown[0] = false;
                Game.P.VelX = 0;

                if (_KeyDown[1])
                    Game.P.VelX = C.PLAYER_SPEED;

            }
            else if (k == Keys.Up)
            {
                _KeyDown[2] = false;
                Game.P.VelY = 0;

                if (_KeyDown[3])
                    Game.P.VelY = -C.PLAYER_SPEED;

            }
            else if (k == Keys.Down)
            {
                _KeyDown[3] = false;
                Game.P.VelY = 0;

                if (_KeyDown[2])
                    Game.P.VelY = C.PLAYER_SPEED;

            }


        }

        public static void UpdatePlayerVelocity()
        {
            //left/right
            if (_KeyDown[0])
                Game.P.VelX = -C.PLAYER_SPEED;
            else if (_KeyDown[1])
                Game.P.VelX = C.PLAYER_SPEED;

            //up/down
            if (_KeyDown[2])
                Game.P.VelY = -C.PLAYER_SPEED;
            else if (_KeyDown[3])
                Game.P.VelY = C.PLAYER_SPEED;
        }

    }
}
