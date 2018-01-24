using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PixelVillage.GameSprite;
using System.Drawing;
using PixelVillage.Main;
using System.Windows.Forms;

namespace PixelVillage.UI
{
    delegate void PreRenderDelegate(Button b);
    // Genaric Button with click callback, auto centering text
    class Button
            : BasicSprite
    {
        private static Button FOCUSED_BUTTON;
        public static List<Button> AllButtons = new List<Button>();


        protected bool _Focused;
        protected string Text, AppendText = "";
        protected Brush _Brush;
        protected Font _Font;
        protected Brush _FontBrush;
        protected Brush _FocusedBrush;
        protected Pen _Pen;
        protected Action clickCallback;
        public PreRenderDelegate preRender;
        protected bool _Disabled;


        public Button(int x, int y, int width, int height, string text, Font font, Action callback)
            : base(x, y, width, height)
        {
            Text = text;
            _Font = font;
            _Brush = Brushes.Black;
            _FontBrush = Brushes.White;
            _FocusedBrush = Brushes.Gray;
            clickCallback = callback;
            _Pen = new Pen(_Brush);
            preRender = delegate(Button b){ };

            AllButtons.Add(this);
        }

        public Button(int x, int y, int width, int height, string text, Action callback)
            : this(x, y, width, height, text, C.LFont, callback)
        {}

        public void SetColor(Brush b)
        {
            _Brush = b;
        }

        public void SetFontColor(Brush b)
        {
            _FontBrush = b;
        }

        public void SetFocusedColor(Brush b)
        {
            _FocusedBrush = b;
        }

        public void SetText(string s)
        {
            Text = s;
        }

        public void SetFont(Font f)
        {
            _Font = f;
        }


        public virtual void MouseDown()
        {
            if (!_Disabled)
               clickCallback();
        }

        public virtual void MouseOver()
        {
            Focus();
        }

        public virtual void render(ref Graphics g)
        {
            if (_Focused || _Disabled)
                g.FillRectangle(_FocusedBrush, GetGraphBounds(ref g));
            else
                g.FillRectangle(_Brush, GetGraphBounds(ref g));

            g.DrawString(Text + AppendText, _Font, _FontBrush, GetGraphBounds(ref g), C.GetCenterFormat());
            preRender(this);
        }

        public void tick()
        { }

        public void Disable()
        {
            _Disabled = true;
        }

        public void Enable()
        {
            _Disabled = false;
        }

        public void Focus()
        {
            if (FOCUSED_BUTTON != null && FOCUSED_BUTTON != this)
                FOCUSED_BUTTON.Unfocus();
            FOCUSED_BUTTON = this;
            _Focused = true;
        }

        public void Unfocus()
        {
            _Focused = false;
        }

        public static void UnFocusAll()
        {
            if (FOCUSED_BUTTON != null)
                FOCUSED_BUTTON.Unfocus();
        }

        public void SetAppendText(string s)
        {
            AppendText = s;
        }
    }
}
