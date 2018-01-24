using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using PixelVillage.GameWorld;
using PixelVillage.Main;
using System.IO;
using System.Runtime.Serialization.Json;
using PixelVillage.ClientHandler;
using PixelVillage.GameSprite;
using System.Runtime.CompilerServices;
using PixelVillage.UI;

namespace PixelVillage.GameWorld
{
    class WorldHelper
    {

        public static Dictionary<Material, String> MaterialItemNames = new Dictionary<Material, String>();

        public WorldHelper()
        {
            MaterialItemNames[Material.FUNC_Forest] = "Wood";
            MaterialItemNames[Material.Grass] = "Soil";
            MaterialItemNames[Material.Stone] = "Stone";
            MaterialItemNames[Material.Dirt] = "Wood";
        }

        public static Image TileImageFromType(Material t)
        {
            return PVResources.MaterialImageMap[t];
        }

        public static int HarvestTimeFromType(Material t)
        {
            if (t == Material.FUNC_Forest)
                return 1 * 20;
            return 0;
        }

        public static int RegenerateTimeFromType(Material t)
        {
            if (t == Material.FUNC_Forest)
                return 3 * 20;
            return 0;
        }

        public static Tile GetTileFromPoint(Rectangle click)
        {
            // Prevent crash when creating tiles in world init
            if (Game.World == null)
                return null;

            for (int i = 0; i < Game.World.Tiles.Count; i++)
            {
                if (click.IntersectsWith(Game.World.Tiles[i].GetBounds()))
                    return Game.World.Tiles[i];
            }
            return null;
        }

        public static Tile GetTileFromPoint(int x, int y)
        {
            return GetTileFromPoint(new Rectangle(x, y, 1, 1));
        }

        public static void RemoveTileAtPoint(int x, int y)
        {
            Tile t;
            while ((t = GetTileFromPoint(x, y)) != null)
            {
                t.RemoveFromWorld();
            }
        }

        public static void RemoveTileAtPoint(Rectangle click)
        {
            RemoveTileAtPoint(click.X, click.Y);
        }

        //public static Tile[] GetAdjacentTiles(int x, int y, int width, int height)
        //{
        //    // add double interact range to width/height since that will create even interact range margin all the way around
        //    Rectangle area = new Rectangle(x - C.PLAYER_INTERACT_RANGE, y - C.PLAYER_INTERACT_RANGE, width + C.PLAYER_INTERACT_RANGE * 2, height + C.PLAYER_INTERACT_RANGE * 2);
        //    List<Tile> tiles = new List<Tile>();
        //    Tile t;
        //    for (int i = 0; i < Game.World.Tiles.Count; i++)
        //    {
        //        t = Game.World.Tiles[i];
        //        if (area.IntersectsWith(t.GetBounds()))
        //            tiles.Add(t);
        //    }
        //    return tiles.ToArray();
        //}

        public static void SaveWorld(World w)
        {
            string name = w.Name;
            
            // Create a string array with the lines of text
            string json = Game.World.GetJSON();

            // Set a variable to the My Documents path.
            string mydocpath =
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string path = mydocpath + @"\Sap\Worlds\" + name;
            Directory.CreateDirectory(path);

            // Write the string array to \Sap\Worlds\name\world.json
            using (StreamWriter outputFile = new StreamWriter(path + @"\world" + ".json"))
            {
                outputFile.WriteLine(json);
            }

            string playerjson = Game.P.GetJSON();

            using (StreamWriter outputFile = new StreamWriter(path + @"\player" + ".json"))
            {
                outputFile.WriteLine(playerjson);
            }

            Debug.WriteLine("World " + name + " saved successfully!");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void LoadWorld(string name)
        {
            string progfiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string json = System.IO.File.ReadAllText(progfiles + @"\Sap\Worlds\" + name + @"\" + "world.json");

            // Pause game loop before messing with data
            GEngine.pause();

            // Clear all world data
            Tile.RemoveAll();
            World w = new World(name);
            SpriteHandler.sprites.Clear();
            SpriteHandler.sentiants.Clear();
            //SpriteHandler.VisableSprites.Clear();

            World.BinWorld binw = new World.BinWorld();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(binw.GetType());

            binw = ser.ReadObject(ms) as World.BinWorld;

            // Load in all saved world data
            Tile[] tiles = new Tile[binw.tiles.Length];
            List<StructureTile> struct_tiles = new List<StructureTile>();

            for (var i = 0; i < binw.tiles.Length; i++)
            {
                // if its a function tile, load it as one
                if (binw.tiles[i] is FunctionTile.BinFuncTile)
                    tiles[i] = new FunctionTile(binw.tiles[i] as FunctionTile.BinFuncTile);
                else if (binw.tiles[i] is StructureTile.BinStructureTile)
                {
                    tiles[i] = new StructureTile(binw.tiles[i] as StructureTile.BinStructureTile);
                    struct_tiles.Add(tiles[i] as StructureTile);
                }
                else
                    tiles[i] = new Tile(binw.tiles[i]);
            }

            // Populate arrays with saved data (must be up here since function tiles are appended on init)
            w.Tiles = tiles.ToList();

           
            w.StructureTiles = struct_tiles;


            Game.World = w;

            Game.P = new Player();

            string playerjson = System.IO.File.ReadAllText(progfiles + @"\Sap\Worlds\" + name + @"\" + "player.json");

            Player.BinPlayer binp = new Player.BinPlayer();
            MemoryStream msp = new MemoryStream(Encoding.UTF8.GetBytes(playerjson));
            DataContractJsonSerializer serp = new DataContractJsonSerializer(binp.GetType());

            binp = serp.ReadObject(msp) as Player.BinPlayer;

            Game.P.LoadInventory(binp);

            Game.P.X = binp.X;
            Game.P.Y = binp.Y;
            Game.Camera = new Camera(Game.P);

            // Resume the game loop
            GEngine.resume();
            // Set the gamestate
            Game.GameState = GameState.GAME;
        }

    }
}
