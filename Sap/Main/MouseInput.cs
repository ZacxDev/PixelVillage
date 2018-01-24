using PixelVillage.ClientHandler;
using PixelVillage.GameSprite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PixelVillage.Main
{
    class MouseInput
    {

        public static void HandleMouseDown(MouseEventArgs e)
        {
            SpriteHandler.clickDown(e);

            if (e.Button == MouseButtons.Right)
                C.M_RIGHTDOWN = true;
            else if (e.Button == MouseButtons.Left)
                C.M_LEFTDOWN = true;
        }

        public static void HandleMouseUp(MouseEventArgs e)
        {
            SpriteHandler.clickUp(e);

            if (e.Button == MouseButtons.Right)
                C.M_RIGHTDOWN = false;
            else if (e.Button == MouseButtons.Left)
                C.M_LEFTDOWN = false;
        }

        public static void HandleMouseMove(MouseEventArgs e)
        {
            SpriteHandler.mouseMove(e);
            C.MX = e.X + Game.Camera.camX;
            C.MY = e.Y + Game.Camera.camY;
        }

        public static void HandleResize(Form f)
        {
            Scalable.UpdateAllRelativeValues();
            Game.CANVAS_WIDTH = f.Width;
            Game.CANVAS_HEIGHT = f.Height;
            GEngine.TRIGGER_GRAPHICS_RECALC();
            Game.Camera.UpdateScale();
        }

    }
}
