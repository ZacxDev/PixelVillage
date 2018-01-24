using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PixelVillage.GameSprite
{
    class Scalable
    {
        public static List<Scalable> AllScalable = new List<Scalable>();

        public int X, Y, Width, Height;
        protected decimal canvas_rel_scale_width, canvas_rel_scale_height, canvas_rel_x, canvas_rel_y;

        public Scalable() { }

        public Scalable(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;

            canvas_rel_scale_width = ((decimal)width / (decimal)Game.CANVAS_WIDTH);
            canvas_rel_scale_height = ((decimal)height / (decimal)Game.CANVAS_HEIGHT);

            canvas_rel_x = ((decimal)x / (decimal)Game.CANVAS_WIDTH);
            canvas_rel_y = ((decimal)y / (decimal)Game.CANVAS_HEIGHT);

            AllScalable.Add(this);
        }

        public virtual void UpdateScale()
        {
            Width = (int)(canvas_rel_scale_width * Game.CANVAS_WIDTH);
            Height = (int)(canvas_rel_scale_height * Game.CANVAS_HEIGHT);
        }

        public virtual void UpdatePosition()
        {
            X = (int)(canvas_rel_x * Game.CANVAS_WIDTH);
            Y = (int)(canvas_rel_y * Game.CANVAS_HEIGHT);
        }

        public virtual void UpdateRelativeValue()
        {
            // only recalculate if it is a sentiant sprite since other stuff wont have changed coords
            if (!(this is SentiantSprite))
                return;

            canvas_rel_scale_width = ((decimal)Width / (decimal)Game.CANVAS_WIDTH);
            canvas_rel_scale_height = ((decimal)Height / (decimal)Game.CANVAS_HEIGHT);

            canvas_rel_x = ((decimal)X / (decimal)Game.CANVAS_WIDTH);
            canvas_rel_y = ((decimal)Y / (decimal)Game.CANVAS_HEIGHT);
        }

        // Call after canvas size changes
        public static void UpdateAllScaleAndPosition()
        {
            GEngine.pause();
            for (int i = 0; i < AllScalable.Count; i++)
            {
                AllScalable[i].UpdateScale();
                AllScalable[i].UpdatePosition();
            }
            GEngine.resume();
        }

        // Called before the canvas size is changed
        public static void UpdateAllRelativeValues ()
        {
            for (int i = 0; i < AllScalable.Count; i++)
            {
                AllScalable[i].UpdateRelativeValue();
            }
        }
    }
}
