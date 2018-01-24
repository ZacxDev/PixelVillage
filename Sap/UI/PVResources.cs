using PixelVillage.GameWorld;
using PixelVillage.Inventory;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PixelVillage.UI
{
    class PVResources
    {

        public static Image DEAD_IMAGE, EDITOR_ERASER, SPRITE_PLAYER, TILE_FOREST, TILE_STONE, TILE_GRASS, TILE_DIRT, TILE_GRASS_CLIFF,
            ITEM_WOOD_LOG, ITEM_LEAVES;

        public static Dictionary<Material, Image> MaterialImageMap;
        public static Dictionary<ItemMaterial, Image> ItemImageMap;

        static PVResources()
        {
            var progfiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var basedir = progfiles + @"\Sap\Resources\default";

            DEAD_IMAGE = Image.FromFile(basedir + @"\deadimage.bmp");
            EDITOR_ERASER = Image.FromFile(basedir + @"\editor\eraser.png");
            SPRITE_PLAYER = Image.FromFile(basedir + @"\sprites\player.png");
            TILE_FOREST = Image.FromFile(basedir + @"\tiles\forest.png");
            TILE_STONE = Image.FromFile(basedir + @"\tiles\stone.png");
            TILE_GRASS_CLIFF = Image.FromFile(basedir + @"\tiles\grass_cliff.png");
            TILE_GRASS = Image.FromFile(basedir + @"\tiles\grass.png");
            TILE_DIRT = Image.FromFile(basedir + @"\tiles\dirt.png");

            ITEM_WOOD_LOG = Image.FromFile(basedir + @"\items\wood_log.png");
            ITEM_LEAVES = Image.FromFile(basedir + @"\items\leaves.png");

            MaterialImageMap = new Dictionary<Material, Image>()
            {
                {Material.Grass, TILE_GRASS}, {Material.Stone, TILE_STONE}, {Material.Dirt, TILE_DIRT},
                { Material.FUNC_Forest, TILE_FOREST}, {Material.STRUCT_Grass_Cliff, TILE_GRASS_CLIFF}
            };

            ItemImageMap = new Dictionary<ItemMaterial, Image>()
            {
                {ItemMaterial.WOOD_LOG, ITEM_WOOD_LOG}, { ItemMaterial.LEAVES, ITEM_LEAVES }
            };
        }
    }
}
