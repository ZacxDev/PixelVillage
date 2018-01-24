using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using PixelVillage.GameWorld;
using PixelVillage.Main;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Diagnostics;
using PixelVillage.GameSprite;
using PixelVillage.ClientHandler;

namespace PixelVillage.GameWorld
{
    class World
    {
        public List<Tile> Tiles = new List<Tile>();
        public List<StructureTile> StructureTiles = new List<StructureTile>();
        public List<FunctionTile> FunctionTiles = new List<FunctionTile>();

        public String Name;
        public int World_Width = C.WORLD_WIDTH, World_Height = C.WORLD_HEIGHT;

        public World(String name)
        {
            Name = name;

            for (int i = 0; i < World_Width / 64; i++)
            {
                new Tile(i * 64, 384, Material.Grass, this);
            }

            for (int i = 0; i < World_Height / 64; i++)
            {
                new Tile(960 - (i * 64), i * 64, Material.Grass, this);
            }

            for (int i = 0; i < 5; i++)
            {
                new FunctionTile((i * 64) + 256, 128, Material.FUNC_Forest, this);
            }

        }

        public World(BinWorld binw)
        {
            

        }

        public void render(ref Graphics g)
        {
            for (int i = 0; i < Tiles.Count; i++)
            {
                Tiles[i].Render(ref g);
            }
        }

        public string GetJSON()
        {
            return new BinWorld(this).GetJSON();
        }

        [DataContract]
        internal class BinWorld
        {
            [DataMember]
            internal Tile.BinTile[] tiles;
            [DataMember]
            internal Tile.BinTile[] struct_tiles;
            [DataMember]
            internal int PX;
            [DataMember]
            internal int PY;

            internal BinWorld() { }

            internal BinWorld(World w)
            {
                tiles = new Tile.BinTile[w.Tiles.Count];
                for (var i = 0; i < w.Tiles.Count; i++)
                {
                    if (w.Tiles[i] is FunctionTile)
                        tiles[i] = new FunctionTile.BinFuncTile(w.Tiles[i] as FunctionTile);
                    else if (w.Tiles[i] is StructureTile)
                        tiles[i] = new StructureTile.BinStructureTile(w.Tiles[i] as StructureTile);
                    else
                        tiles[i] = new Tile.BinTile(w.Tiles[i]);
                }

                //struct_tiles = new Tile.BinTile[w.StructureTiles.Count];
                //for (var i = 0; i < w.StructureTiles.Count; i++)
                //{
                //    struct_tiles[i] = new Tile.BinTile(w.StructureTiles[i]);
                //}


                PX = Game.P.X;
                PY = Game.P.Y;
                
            }

            public string GetJSON()
            {
                MemoryStream stream1 = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(BinWorld));
                ser.WriteObject(stream1, this);
                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);
                return sr.ReadToEnd();
            }
        }

    }
}
