using PixelVillage.ClientHandler;
using PixelVillage.GameWorld;
using PixelVillage.Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PixelVillage.GameSprite
{
    abstract class SentiantSprite : Sprite
    {

        public int VelX, VelY;
        public World World;
        public bool Jumping = false, OnGround = true;

        public SentiantSprite(int x, int y, int width, int height, Image image)
            : base(x, y, width, height, image)
        {
            World = Game.World;
        }

        public virtual void tick()
        {

            //if outsdie of canvas
            if (X + Width + VelX > Game.WORLD_WIDTH || X + VelX < 0 || Y + VelY > Game.WORLD_HEIGHT)
            {
                return;
            }
            
            for (int i = 0; i < SpriteHandler.VisableObstructiveSprites.Count; i++)
            {
                if (GetNextBoundsX().IntersectsWith(SpriteHandler.VisableObstructiveSprites[i].GetBounds()))
                    VelX = 0;
                if (GetNextBoundsY().IntersectsWith(SpriteHandler.VisableObstructiveSprites[i].GetBounds()))
                    VelY = 0;
            }
            
            X += VelX;
            Y += VelY;
        }

        public void Jump()
        {
            if (Jumping || !OnGround)
                return;

            //Debug.WriteLine("fuuu");
            Jumping = true;
            OnGround = false;
            VelY = -C.PLAYER_JUMP;
        }

        public override Rectangle GetBounds()
        {
            return new Rectangle(X, Y, Width, Height);
        }

        //returns bounds after next tick
        public Rectangle GetNextBounds()
        {
            return new Rectangle(X + VelX, Y + VelY, Width, Height);
        }

        public Rectangle GetNextBoundsX()
        {
            return new Rectangle(X + VelX, Y, Width, Height - 5);
        }

        public Rectangle GetNextBoundsY()
        {
            return new Rectangle(X, Y + VelY, Width, Height - 5);
        }

    }
}
