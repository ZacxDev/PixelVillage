using PixelVillage.GameSprite;
using PixelVillage.Main;
using PixelVillage.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace PixelVillage.GameWorld
{

    public enum WorldTool {NONE, TILE, FUNCTION_TILE, STRUCTURE_TILE, ERASER};

    class WorldBuilder
        : UISprite
    {

        public static bool Enabled = false;

      //  private Rectangle _ToolBar = new Rectangle(0, 0, Game.CANVAS_WIDTH, 64);
        public List<ToolBarButton> ToolBarButtons = new List<ToolBarButton>();

        private Rectangle TileButton = new Rectangle(8, 8, 48, 48);
        private Image CurrentTile = PVResources.TILE_GRASS;
        private Image CurrentTileThumb = PVResources.TILE_GRASS;
        public static WorldTool SELECTED_WORLDTOOL = WorldTool.NONE;

        public WorldBuilder(Bitmap image) 
            : base(0, 0, Game.CANVAS_WIDTH, 64, image)
        {
            LoadToolBarButtons();
        }

        public override void Render(ref Graphics g)
        {
            
            if (!Enabled)
                return;

            // If mouse over toolbar, don't render the tool
            if (!new Rectangle(C.MX, C.MY, 1, 1).IntersectsWith(GetBounds()) && !_GetBottomBounds().IntersectsWith(new Rectangle(C.MX, C.MY, 1, 1)))
            {
                if (SELECTED_WORLDTOOL != WorldTool.NONE && SELECTED_WORLDTOOL != WorldTool.ERASER)
                    g.DrawImage(ToolBarButton.SELECTED_TOOL.GetSelectedOptionImage(), C.round(C.MX, 64), C.round(C.MY, 64));
                else if (SELECTED_WORLDTOOL == WorldTool.ERASER)
                    g.DrawImage(ToolBarButton.SELECTED_TOOL.GetSelectedOptionImage(), C.MX - 16, C.MY - 16, 32, 32);
            }

            // Deprecated on basicsprite implementation
            // g.FillRectangle(new SolidBrush(Color.Black), _GetRenderBounds(ref g));
            g.FillRectangle(new SolidBrush(Color.Black), GetGraphBounds(ref g));
            _RenderMenu(ref g);

            foreach (var tb in ToolBarButtons)
            {
                tb.Render(ref g);
            }
        }

        private void _RenderMenu(ref Graphics g)
        {
            var m = _GetBottomRenderBounds(ref g);
            g.FillRectangle(new SolidBrush(Color.Black), m);
            var save = new Rectangle(m.X + m.Width / 2 - 32, m.Y + 5, 64, 16);
            var load = new Rectangle(m.X + m.Width / 2 - 32, m.Y + m.Height - 16 - 5, 64, 16);
            g.DrawRectangle(Pens.Gray, save);
            g.DrawString("Save", new Font("Arial", 8), Brushes.Blue, save.X + 16, save.Y + 1);
            g.DrawRectangle(Pens.Gray, load);
            g.DrawString("Load", new Font("Arial", 8), Brushes.Blue, load.X + 16, load.Y + 1);
        }

        private Rectangle _GetSaveButtonBounds()
        {
            var m = _GetBottomBounds();
            return new Rectangle(m.X + m.Width / 2 - 32, m.Y + 5, 64, 16);
        }

        private Rectangle _GetLoadButtonBounds()
        {
            var m = _GetBottomBounds();
            return new Rectangle(m.X + m.Width / 2 - 32, m.Y + m.Height - 16 - 5, 64, 16); ;
        }

        public void Tick()
        {
            
        }

        public override void ClickedDown(Rectangle click)
        {
            if (!Enabled)
                return;

            if (click.IntersectsWith(_GetSaveButtonBounds()))
            {
                WorldHelper.SaveWorld(Game.World);
                return;
            } else if (click.IntersectsWith(_GetLoadButtonBounds()))
            {
                WorldHelper.LoadWorld("temp_world");
                return;
            }

            //if (click.IntersectsWith(GetBounds()))
            foreach (var tb in ToolBarButtons)
            {
                if (click.IntersectsWith(tb.GetCamRelBounds()) || (click.IntersectsWith(tb.GetDropDownBounds()) && tb.isFocused()))
                {
                    tb.Select(click);
                    return;
                }
            }
            
            // If no tool or option selected
            if (ToolBarButton.SELECTED_TOOL == null || !ToolBarButton.SELECTED_TOOL.HasSelectedOption())
                return;

            _TriggerSelectedWorldTool(click);

        }

        public override void ClickedUp(Rectangle click)
        {
            //this will not be called unless done explicitly in SpriteHandler.cs
        }

        public override void MouseOver(Rectangle cursor)
        {

            if (C.M_RIGHTDOWN)
            {
                _TriggerSelectedWorldTool(cursor);
            }

            // check if cursor intersects with focused toolbarbutton or its dropdown
            var f = ToolBarButton.FOCUSED_TOOL;
            if (f != null)
                if (cursor.X > f.GetCamRelX() && cursor.X < f.GetCamRelX() + f.Width && cursor.Y > f.GetCamRelY() && cursor.Y < f.GetCamRelY() + f.Height + f.DropDownPane.Height)
                    return;

            foreach (var tb in ToolBarButtons)
            {
                // check if cursor intersects with tb
                if (cursor.IntersectsWith(tb.GetCamRelBounds()))
                {
                    ToolBarButton.ChangeFocus(tb);
                    return;
                }
            }
            ToolBarButton.UnFocusAll(cursor);
            return;
        }

        private void _TriggerSelectedWorldTool(Rectangle click)
        {
            if (SELECTED_WORLDTOOL == WorldTool.TILE)
                new Tile(C.round(C.MX, 64), C.round(C.MY, 64), ToolBarButton.SELECTED_TOOL.GetSelectedOption().GetMaterial(), Game.World);
            else if (SELECTED_WORLDTOOL == WorldTool.FUNCTION_TILE)
                new FunctionTile(C.round(C.MX, 64), C.round(C.MY, 64), ToolBarButton.SELECTED_TOOL.GetSelectedOption().GetMaterial(), Game.World);
            else if (SELECTED_WORLDTOOL == WorldTool.STRUCTURE_TILE)
                new StructureTile(C.round(C.MX, 64), C.round(C.MY, 64), ToolBarButton.SELECTED_TOOL.GetSelectedOption().GetMaterial(), Game.World);
            else if (SELECTED_WORLDTOOL == WorldTool.ERASER)
            {
                WorldHelper.RemoveTileAtPoint(click);
            }
        }

        public void Toggle()
        {
            if (Enabled)
                Enabled = false;
            else
                Enabled = true;
        }

        private void LoadToolBarButtons()
        {
            var _TileButton = new ToolBarButton(WorldTool.TILE, PVResources.TILE_GRASS, 8, 4);
            var _FunctionTileButton = new ToolBarButton(WorldTool.FUNCTION_TILE, PVResources.TILE_FOREST, 72, 4);
            var _StructureButton = new ToolBarButton(WorldTool.STRUCTURE_TILE, PVResources.TILE_GRASS_CLIFF, 144, 4);
            var _EraseButton = new ToolBarButton(WorldTool.ERASER, PVResources.EDITOR_ERASER, 216, 4);
            // Add single function option for toolbarbuttons (no options, doesn't need material)
            ToolBarButtons.Add(_TileButton);
            ToolBarButtons.Add(_FunctionTileButton);
            ToolBarButtons.Add(_StructureButton);
            ToolBarButtons.Add(_EraseButton);

            //pull values from tiletype enum and cast from object<>, then display in toolbar
            var ix = 8;
            foreach (var t in Enum.GetValues(typeof(Material)).Cast<Material>())
            {
                if (t.ToString().Contains("FUNC"))
                    new ToolBarOption(ix, 72, _FunctionTileButton, t);
                else if (t.ToString().Contains("STRUCT"))
                    new ToolBarOption(ix, 72, _StructureButton, t);
                else
                    new ToolBarOption(ix, 72, _TileButton, t);

                // 48 + 16px margin
                ix += 64;
            }
        }

        // Deprecated on BasicSprite implementation

        //public override Rectangle GetBounds()
        //{
        //    return new Rectangle(Game.Camera.camX, Game.Camera.camY, Game.CANVAS_WIDTH, 64);
        //}

        // Deprecated on BasicSprite implementation

        //private Rectangle _GetRenderBounds(ref Graphics g)
        //{
        //    return new Rectangle(-(int)g.Transform.OffsetX, -(int)g.Transform.OffsetY, Game.CANVAS_WIDTH, 64);
        //}

        private Rectangle _GetBottomBounds()
        {
            return new Rectangle(Game.Camera.camX + (Game.CANVAS_WIDTH - 128), Game.Camera.camY + Game.CANVAS_HEIGHT - 64, 128, 48);
        }

        private Rectangle _GetBottomRenderBounds(ref Graphics g)
        {
            return new Rectangle(-(int)g.Transform.OffsetX + (Game.CANVAS_WIDTH - 128), -(int)g.Transform.OffsetY + Game.CANVAS_HEIGHT - 64, 128, 48);
        }

        public class ToolBarButton
            : Sprite
        {

            public static ToolBarButton SELECTED_TOOL;
            public static ToolBarButton FOCUSED_TOOL;

            private WorldTool _ToolType = WorldTool.NONE;
            private bool _Selected = false;
            private bool _Focused = false;
            public Rectangle DropDownPane;

            private List<ToolBarOption> _Options = new List<ToolBarOption>();
            private ToolBarOption _SelectedOption;

            public ToolBarButton(WorldTool type, Image image, int x, int y)
                : base(x, y, 56, 56, PVResources.DEAD_IMAGE)
            {
                _ToolType = type;
                _Image = image;
                X = x;
                Y = y;
            }

            public override void Render(ref Graphics g)
            {
                if (!WorldBuilder.Enabled)
                    return;
                // if it is the selected tool
                if (_Selected)
                    g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(GetGraphRelX(g) - 4, GetGraphRelY(g) - 4, 64, 64));

                if (_Focused)
                {
                    DropDownPane = new Rectangle(GetGraphRelX(g) - 4, GetGraphRelY(g) + 64, 56, 56 * _Options.Count);
                    g.FillRectangle(new SolidBrush(Color.Black), DropDownPane);
                    for (var i = 0; i < _Options.Count; i++)
                    {
                        _Options[i].Draw(ref g, GetGraphRelX(g), GetGraphRelY(g) + 12 + (56 * (i + 1)));
                        //g.DrawImage(_Options[i].GetImage(), X, Y + (56 * (i + 1)), 48, 48);
                    }
                }

                // Deprecated on BasicSprite implementation
               // g.DrawImage(GetSelectedOptionImage(), _GetRenderBounds(ref g));
                g.DrawImage(GetSelectedOptionImage(), GetGraphBounds(ref g));

            }

            public void Select(Rectangle cursor)
            {
                if (SELECTED_TOOL != null)
                    SELECTED_TOOL.Deselect();

                _Selected = true;
                SELECTED_TOOL = this;
                WorldBuilder.SELECTED_WORLDTOOL = _ToolType;

                // if it's the eraser, don't worry about options since it only does one thing
                if (_ToolType == WorldTool.ERASER)
                    return;

                if (_SelectedOption == null)
                    _SelectedOption = _Options[0];

                foreach (var option in _Options)
                {
                    if (cursor.IntersectsWith(option.GetBounds()))
                    {
                        option.Select();
                        return;
                    }
                }
            }
            public void Deselect()
            {
                _Selected = false;
            }

            public bool isFocused()
            {
                return _Focused;
            }

            public void Focus()
            {
                _Focused = true;
                FOCUSED_TOOL = this;
            }
            public void UnFocus()
            {
                _Focused = false;
            }
            public static void ChangeFocus(ToolBarButton tb)
            {
                if (FOCUSED_TOOL != null)
                    FOCUSED_TOOL.UnFocus();
                tb.Focus();
            }
            public static void UnFocusAll(Rectangle cursor)
            {
                if (FOCUSED_TOOL != null)
                    if (!cursor.IntersectsWith(FOCUSED_TOOL.DropDownPane))
                    {
                        FOCUSED_TOOL.UnFocus();
                        FOCUSED_TOOL = null;
                    }
            }

            public Rectangle GetDropDownBounds()
            {
                return DropDownPane;
            }

            public Image GetImage()
            {
                return _Image;
            }

            public ref List<ToolBarOption> GetOptions()
            {
                return ref _Options;
            }

            private bool _HasOptions()
            {
                return _Options.Count != 0;
            }

            public bool HasSelectedOption()
            {
                // if they have no options, return true, if they do, return if one is selected
                if (!_HasOptions())
                    return true;
                return _SelectedOption != null;
            }

            public ToolBarOption GetSelectedOption()
            {
                return _SelectedOption;
            }
            public void SetSelectedOption(ToolBarOption s)
            {
                if (_SelectedOption != null)
                    _SelectedOption.Deselect();
                _SelectedOption = s;
            }

            public Image GetSelectedOptionImage()
            {
                if (_HasOptions() && HasSelectedOption())
                    return GetSelectedOption().GetImage();
                return GetImage();
            }

            public WorldTool GetToolType()
            {
                return _ToolType;
            }

            // Deprecated on BasicSprite implementation

            //public override Rectangle GetBounds()
            //{
            //    return new Rectangle(Game.Camera.camX + X, Game.Camera.camY + Y, Width, Height);
            //}
            // Deprecated on BasicSprite implementation
            //private Rectangle _GetRenderBounds(ref Graphics g)
            //{
            //    return new Rectangle(-(int)g.Transform.OffsetX + X, -(int)g.Transform.OffsetY + Y, Width, Height);
            //}

            // Deprecated on BasicSprite implementation
            // UNSAFE FOR DRAWING! MUST PASS GRAPHICS OBJECT TO PREVENT FLICKERING
            //public int GetRelX()
            //{
            //    return Game.Camera.camX + X;
            //}

            //public int GetRelY()
            //{
            //    return Game.Camera.camY + Y;
            //}

            // Deprecated on BasicSprite implementation
            //public int GetRelX(Graphics g)
            //{
            //    return -(int)g.Transform.OffsetX + X;
            //}

            //public int GetRelY(Graphics g)
            //{
            //    return -(int)g.Transform.OffsetY + Y;
            //}

        }

        public class ToolBarOption
            : Sprite
        {


            private bool _Selected = false;
            private ToolBarButton _Parent;
            private Material _Type;

            public ToolBarOption(int x, int y, ToolBarButton tool, Material m)
                : base(x, y, 48, 48, PVResources.DEAD_IMAGE)
            {
                _Parent = tool;
                _Type = m;
                _Parent.GetOptions().Add(this);
                _Image = WorldHelper.TileImageFromType(_Type);
            }

            public override void Render(ref Graphics g)
            {}

            public void Draw(ref Graphics g, int x, int y)
            {
                //This is where the cords are updated
                X = x;
                Y = y;
                // if it is the selected option
                if (_Selected)
                    g.FillRectangle(new SolidBrush(Color.Red), x - 4, y - 4, 56, 56);
                g.DrawImage(GetImage(), x, y, 48, 48);
            }

            public void Select()
            {
                //if (_Parent.GetSelectedOption() != null)
                //    _Parent.GetSelectedOption().Deselect();

                _Parent.SetSelectedOption(this);
                _Selected = true;
            }
            public void Deselect()
            {
                _Selected = false;
            }

            public Material GetMaterial()
            {
                return _Type;
            }

            //private Rectangle _GetRenderBounds(ref Graphics g)
            //{
            //    return new Rectangle(-(int)g.Transform.OffsetX + X, -(int)g.Transform.OffsetY + Y, Width, Height);
            //}

        }

    }

}
