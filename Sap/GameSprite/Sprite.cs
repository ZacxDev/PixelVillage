using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using PixelVillage.ClientHandler;
using PixelVillage.GameWorld;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;

namespace PixelVillage.GameSprite
{
    abstract class Sprite
        : BasicSprite
    {
        //public int X, Y, Width, Height;
        // Full image is base image and should never change, since image will be scaled
        protected Image _Image, _FullImage;
        private Rectangle Bounds;

        public Sprite(int x, int y, int width, int height, Image image)
            : base(x, y, width, height)
        {
            _Image = image;
            _FullImage = _Image;

            if (!(this is WorldBuilder))
                if (this is SentiantSprite)
                    SpriteHandler.sentiants.Add((this as SentiantSprite));
                else
                    SpriteHandler.sprites.Add(this);
        }

        // UNSAFE, FOR TEMP JSON DESEARILIZATION ONLY
      //  public Sprite() { }

        public abstract void Render(ref Graphics g);

        public virtual Rectangle GetBounds()
        {
            return new Rectangle(X, Y, Width, Height);
        }

        public Image GetImage()
        {
            return _Image;
        }

        public void SetImage(Image b)
        {
            _Image = b;
        }

        public override void UpdateScale()
        {
            base.UpdateScale();
            _Image = (Image)new Bitmap(_FullImage, Width, Height);
        }

    }
}
