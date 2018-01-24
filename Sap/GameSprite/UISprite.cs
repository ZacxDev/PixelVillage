using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PixelVillage.GameSprite
{
    abstract class UISprite
        : Sprite
    {
        public UISprite(int x, int y, int width, int height, Bitmap image) 
            : base(x, y, width, height, image)
        {
        }

        public abstract void MouseOver(Rectangle click);

        public abstract void ClickedDown(Rectangle click);

        public abstract void ClickedUp(Rectangle click);
    }
}
