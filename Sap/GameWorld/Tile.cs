using PixelVillage.GameSprite;
using PixelVillage.Main;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PixelVillage.ClientHandler;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Diagnostics;
using PixelVillage.UI;

namespace PixelVillage.GameWorld
{
    // prefix with FUNC_ to make function tile option, STRUCT_ to make structure tile option, none for ground tile (no interaction)
    enum Material { Grass, Dirt, Stone, FUNC_Forest, STRUCT_Grass_Cliff }


    class Tile 
        : Sprite
    {

        // store focused tile so we can easily unfocus
        public static Tile FOCUSED_TILE;

        //public int X, Y;
        //public Bitmap Image;
        private Material _Material;
        private Boolean _Focused;


        public Tile(int x, int y, Material type, World world)
            : base(x, y, C.TILE_WIDTH, C.TILE_HEIGHT, PVResources.MaterialImageMap[type])
        {
            X = x;
            Y = y;
            
            _Material = type;
            // Remove underlying tile (before we add this tile)
            WorldHelper.RemoveTileAtPoint(X, Y);
            world.Tiles.Add(this);
            _Image = PVResources.MaterialImageMap[_Material];
        }

        //is not added to world tile list with this init, must add seperatly
        public Tile(BinTile bint)
            :base(bint.X, bint.Y, C.TILE_WIDTH, C.TILE_HEIGHT, PVResources.MaterialImageMap[(Material)Enum.Parse(typeof(Material), bint.mat)])
        {
            _Material = (Material) Enum.Parse(typeof(Material), bint.mat);
            _Image = WorldHelper.TileImageFromType(_Material);
        }

        public override void Render(ref Graphics g)
        {
            // check if focused, draw border
            if (_Focused)
                g.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(X-4, Y-4, Width + 8, Height + 8));
            //                                              ^^^ 4 is the border width ^^^
            g.DrawImage(_Image, X, Y);

        }

        public void MouseOver(Rectangle mouse)
        {
            _Focus(); 
        }

        private void _Focus()
        {
            if (FOCUSED_TILE != null)
                FOCUSED_TILE._UnFocus();
            _Focused = true;
            FOCUSED_TILE = this;
        }

        public void _UnFocus()
        {
            _Focused = false;
        }

        public void RemoveFromWorld()
        {
            Game.World.Tiles.Remove(this);
            SpriteHandler.spriteRemoveQueue.Add(this);
            //SpriteHandler.sprites.Remove(this);
            if (this is StructureTile)
                Game.World.StructureTiles.Remove((this as StructureTile));
        }

        public static void RemoveAll()
        {
            for (var i = 0; i < Game.World.Tiles.Count; i++)
                Game.World.Tiles[i].RemoveFromWorld();
        }

        public static void SetNoFocus()
        {
            if (FOCUSED_TILE != null)
            {
                FOCUSED_TILE._UnFocus();
                FOCUSED_TILE = null;
            }
        }

        public static void RenderFocusedTile(ref Graphics g)
        {
            if (FOCUSED_TILE != null)
                FOCUSED_TILE.Render(ref g);
        }

        public Material GetMaterial()
        {
            return _Material;
        }

        public override void UpdatePosition()
        {
            var t = 0m;
            var v = 0m;
            if (Width > 48)
                t = canvas_rel_scale_width;
            if (Height > 48)
                v = canvas_rel_scale_height;

            X = C.round((int)(canvas_rel_x * Game.CANVAS_WIDTH), (int)(Width - t));
            Y = C.round((int)(canvas_rel_y * Game.CANVAS_HEIGHT), (int)(Height - v));
        }

        [DataContract, KnownType(typeof(FunctionTile.BinFuncTile)), KnownType(typeof(StructureTile.BinStructureTile))]
        internal class BinTile
        {
            [DataMember]
            internal string mat;
            [DataMember]
            internal int X;
            [DataMember]
            internal int Y;

            internal BinTile(Tile t)
            {
                mat = t._Material.ToString();
                X = t.X;
                Y = t.Y;
            }

            public virtual string GetJSON()
            {
                MemoryStream stream1 = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(BinTile));
                ser.WriteObject(stream1, this);
                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);
                return sr.ReadToEnd();
            }
        }

    }
}
