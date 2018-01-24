using PixelVillage.GameSprite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PixelVillage.Main
{
    class Camera
    {

        public Sprite Target;

        public static int VIEWPORT_SIZE_X = Game.CANVAS_WIDTH, VIEWPORT_SIZE_Y = Game.CANVAS_HEIGHT;
        public int offsetMaxX = Game.WORLD_WIDTH - Game.CANVAS_WIDTH,
                offsetMaxY = Game.WORLD_HEIGHT - Game.CANVAS_HEIGHT, offsetMinX = 0,
                offsetMinY = 0, camX, camY;
        // used in player render to decide what fake coords to give them
        public static bool CamMovingLeft, CamMovingRight, CamMovingDown, CamMovingUp;
        public int tx, ty;

        public Camera(Sprite tar)
        {
            Target = tar;
        }

        public void UpdateScale()
        {
            VIEWPORT_SIZE_X = Game.CANVAS_WIDTH;
            VIEWPORT_SIZE_Y = Game.CANVAS_HEIGHT;
            offsetMaxX = Game.WORLD_WIDTH - Game.CANVAS_WIDTH;
            offsetMaxY = Game.WORLD_HEIGHT - Game.CANVAS_HEIGHT;
            offsetMinX = 0;
            offsetMinY = 0;
        }

        public void tick()
        {
            if (Game.GameState == GameState.MENU)
            {
                camX = 0;
                camY = 0;
                return;
            }

            // Reset cammoving bool's 
            CamMovingLeft = false;
            CamMovingRight = false;
            CamMovingDown = false;
            CamMovingUp = false;

            tx = Target.X;
            ty = Target.Y;

            // if player is past 0.6* VIewportX, update camera position to be that much behind player
            if (tx > camX + VIEWPORT_SIZE_X * C.PLAYER_MAX_ACROSS)
            {
                camX = (int)(tx - VIEWPORT_SIZE_X * C.PLAYER_MAX_ACROSS);
                CamMovingLeft = true;
            }
            // if player is past 0.4* VIewportX, update camera position to be that much behind player
            else if (tx < camX + VIEWPORT_SIZE_X * C.PLAYER_MIN_ACROSS)
            {
                camX = (int)(tx - VIEWPORT_SIZE_X * C.PLAYER_MIN_ACROSS);
                CamMovingRight = true;
            }

            // if player is past 0.6* VIewportY, update camera position to be that much above player
            if (ty > camY + VIEWPORT_SIZE_Y * C.PLAYER_MAX_DOWN)
            {
                camY = (int)(ty - VIEWPORT_SIZE_Y * C.PLAYER_MAX_DOWN);
                CamMovingDown = true;
            }
            // if player is past 0.4* VIewportY, update camera position to be that much above player
            else if (ty < camY + VIEWPORT_SIZE_Y * C.PLAYER_MIN_DOWN)
            {
                camY = (int)(ty - VIEWPORT_SIZE_Y * C.PLAYER_MIN_DOWN);
                CamMovingUp = true;
            }


            // camY = Target.Y - VIEWPORT_SIZE_Y / 2;
            // camX = Target.X - VIEWPORT_SIZE_X / 2;

            //if (camX > offsetMaxX)
            //{
            //    camX = offsetMaxX;
            //}
            //else if (camX < offsetMinX)
            //{
            //    camX = offsetMinX;
            //}

            //if (camY > offsetMaxY)
            //{
            //    camY = offsetMaxY;
            //}
            //else if (camY < offsetMinY)
            //{
            //    camY = offsetMinY;
            //}

        }

        public void Render(ref Graphics g)
        {
        }

        public Rectangle getBounds()
        {
            return new Rectangle(camX, camY, VIEWPORT_SIZE_X, VIEWPORT_SIZE_Y);
        }

    }
}
