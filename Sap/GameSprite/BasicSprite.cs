using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PixelVillage.GameSprite
{
    // Basic frame for a sprite, NOT auto stored
    class BasicSprite
        : Scalable
    {
        
        public BasicSprite()
        {}

        public BasicSprite(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            
        }

        public int GetX()
        {
            return X;
        }

        public void SetX(int i)
        {
            X = i;
        }

        public int GetY()
        {
            return Y;
        }

        public void SetY(int i)
        {
            Y = i;
        }

        public int GetWidth()
        {
            return Width;
        }

        public void SetWidth(int i)
        {
            Width = i;
        }

        public int GetHeight()
        {
            return Height;
        }

        public void SetHeight(int i)
        {
            Height = i;
        }

        public Rectangle GetBounds()
        {
            return new Rectangle(X, Y, Width, Height);
        }

        // Returns Coords relative to the game camera, unsafe for drawing, will cause flickering
        public int GetCamRelX()
        {
            return Game.Camera.camX + X;
        }

        public int GetCamRelY()
        {
            return Game.Camera.camY + Y;
        }

        // Returns bounds relative to the game camera, unsafe for drawing
        public Rectangle GetCamRelBounds()
        {
            return new Rectangle(Game.Camera.camX + X, Game.Camera.camY + Y, Width, Height);
        }

        // Returns coords relative to the graphics traslation context, safe for drawing as it will match offset exactly
        public int GetGraphRelX(Graphics g)
        {
            return -(int)g.Transform.OffsetX + X;
        }

        public int GetGraphRelY(Graphics g)
        {
            return -(int)g.Transform.OffsetY + Y;
        }

        // Returns bounds relative to the graphics traslation context, safe for drawing
        public Rectangle GetGraphBounds(ref Graphics g)
        {
            return new Rectangle(-(int)g.Transform.OffsetX + X, -(int)g.Transform.OffsetY + Y, Width, Height);
        }

        // Returns coords relative to the graphics traslation context, safe for drawing as it will match offset exactly
        public int GetGraphRelXFrom(Graphics g, int i)
        {
            return -(int)g.Transform.OffsetX + i;
        }

        public int GetGraphRelYFrom(Graphics g, int i)
        {
            return -(int)g.Transform.OffsetY + i;
        }

    }
}
