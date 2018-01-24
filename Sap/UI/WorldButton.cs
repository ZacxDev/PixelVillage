using PixelVillage.GameSprite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using PixelVillage.Main;
using PixelVillage.GameWorld;

namespace PixelVillage.UI
{
    class WorldButton
        : Button
    {

        public static WorldButton SELECTED_WORLDBUTTON;

        private bool _Selected;
        private string _WorldName;
        private string _WorldPath;
        private int TextX, TextY;

        public WorldButton(int x, int y, int width, int height, string text, Action callback)
            : base(x, y, width, height, text, callback)
        {
            _Font = C.SFont;
            _Pen = new Pen(Brushes.Gray, 3);
            _WorldPath = text;
            _WorldName = text.Substring(text.LastIndexOf('\\') + 1);
        }

        public override void render(ref Graphics g)
        {
            g.FillRectangle(_Brush, GetBounds());
            g.DrawString(_WorldName, _Font, _FontBrush, X, Y);
            g.DrawString(_WorldPath, _Font, Brushes.Gray, GetBounds(), C.GetCenterYRightXFormat());

            if (_Selected)
                g.DrawRectangle(_Pen, GetBounds());
        }

        // Prevent default mouseover action
        public override void MouseOver()
        {}

        public override void MouseDown()
        {
            base.MouseDown();
            if (SELECTED_WORLDBUTTON != null)
                SELECTED_WORLDBUTTON._Selected = false;
            _Selected = true;
            SELECTED_WORLDBUTTON = this;
        }

        public static void LoadSelectedWorld()
        {
            if (SELECTED_WORLDBUTTON == null)
                return;

            Game.GameState = GameState.LOADING;
            WorldHelper.LoadWorld(SELECTED_WORLDBUTTON.GetWorldName());
        }

        public string GetWorldPath()
        {
            return _WorldPath;
        }

        public string GetWorldName()
        {
            return _WorldName;
        }

    }
}
