using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using PixelVillage.GameWorld;
using PixelVillage.GameSprite;

namespace PixelVillage.UI
{
    class HUD
    {
        private List<Button> _Buttons = new List<Button>();
        private BasicSprite _BottomBar;
        private Button _InvButton;

        public HUD()
        {
            _BottomBar = new BasicSprite(0, Game.CANVAS_HEIGHT - 64, Game.CANVAS_WIDTH, 64);

            // temp y coord for _BottomBar
            var ry = _BottomBar.GetBounds().Y;

            _InvButton = new Button(4, ry + 4, 96, 56, "Inventory", delegate () { Game.P.GetInventory().ToggleOpen(); });
            _InvButton.SetFont(new Font("ariel", 8));
            _InvButton.SetColor(Brushes.White);
            _InvButton.SetFontColor(Brushes.Black);
            _InvButton.SetFocusedColor(Brushes.LightGray);
            _Buttons.Add(_InvButton);
        }

        public void render(ref Graphics g)
        {
            // Don't render if worldbuilder is open
            if (WorldBuilder.Enabled)
                return;

            g.FillRectangle(Brushes.Gray, _BottomBar.GetGraphBounds(ref g));
            _InvButton.SetAppendText("(" + Game.P.GetInventory().Size() + ")");

            foreach (var b in _Buttons)
            {
                b.render(ref g);
            }
        }

        public void MouseOver(Rectangle cursor)
        {
            // Don't need to update button focus since all buttons are updated thru global list
        }

        public void MouseDown(Rectangle cursor)
        {
            foreach (var b in _Buttons)
            {
                if (cursor.IntersectsWith(b.GetCamRelBounds()))
                {
                    b.MouseDown();
                    return;
                }
            }
        }

        // Returns bottom bar's camrel bounds
        public Rectangle GetCamRelBounds()
        {
            return _BottomBar.GetCamRelBounds();
        }
    }
}
