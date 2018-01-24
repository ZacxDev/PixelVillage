using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using PixelVillage.ClientHandler;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using PixelVillage.Inventory;
using PixelVillage.Main;

namespace PixelVillage.GameWorld
{

    class FunctionTile
        : Tile
    {

        public static Dictionary<Material, Material> FunctionTileBases = new Dictionary<Material, Material>()
        {
            { Material.FUNC_Forest, Material.Grass }
        };

        // Make a new map for rare drops
        public static Dictionary<Material, ItemMaterial[]> TileDrops = new Dictionary<Material, ItemMaterial[]>()
        {
            { Material.FUNC_Forest, new ItemMaterial[] { ItemMaterial.WOOD_LOG, ItemMaterial.LEAVES } }
        };

        private Material _Type, _BaseType;
        // harvesttime is how long it takes to harvest, timer is how long remaining
        private int _HarvestTime, _HarvestTimer;
        // timer for on harvest animation
        private int _OnHarvestTimer;
        private bool _BeingHarvested = false;
        private bool _Harvested = false;
        private int TextX, TextY;
        private int _RegenerateTime, _RegenerateTimer;
        private ItemStack _Drop;

        public FunctionTile(int x, int y, Material type, World world)
            : base(x, y, type, world)
        {
            _Type = type;
            _BaseType = FunctionTile.FunctionTileBases[_Type];
            _HarvestTime = WorldHelper.HarvestTimeFromType(_Type);
            _HarvestTimer = _HarvestTime;
            _RegenerateTime = WorldHelper.RegenerateTimeFromType(_Type);
            _RegenerateTimer = _RegenerateTime;
            _Image = WorldHelper.TileImageFromType(_Type);
            TextX = X;
            TextY = Y;
            // +1 because array is 0-index, and rdm max = parm -1
            _Drop = new ItemStack(TileDrops[_Type][C.RDM.Next(TileDrops.Count + 1)], 1);

            world.FunctionTiles.Add(this);
        }

        // Must have this init sepearte as instance is added to world tiles array in differnent place
        public FunctionTile(BinFuncTile bint)
            :base(bint)
        {
            _Type = (Material)Enum.Parse(typeof(Material), bint.mat);
            _HarvestTime = WorldHelper.HarvestTimeFromType(_Type);
            _HarvestTimer = _HarvestTime;
            _RegenerateTime = WorldHelper.RegenerateTimeFromType(_Type);
            _RegenerateTimer = bint.RegenerateTimer;
            _Harvested = bint.Harvested;
            if (_Harvested)
                _Image = WorldHelper.TileImageFromType(_BaseType);
            else
                _Image = WorldHelper.TileImageFromType(_Type);
            TextX = X;
            TextY = Y;
            // +1 because array is 0-index, and rdm max = parm -1
            _Drop = new ItemStack(TileDrops[_Type][C.RDM.Next(TileDrops.Count + 1)], 1);
        }

        int Flip = 1;
        public void Tick()
        {
            if (_BeingHarvested)
            {
                // Check if mouse is still down and player is still in range
                if (!Game.P.GetIsWorking() || !Game.P.isAdjacentTo(this))
                {
                    _ResetHarvest();
                    return;
                }
                _HarvestTimer--;
                if (_HarvestTimer <= 0)
                    _Harvest();
                return;
            }
            // check if onharvest animation is playing
            if (_OnHarvestTimer > 0)
            {
                _OnHarvestTimer--;
                TextY -= 1;
                // logic to make harvest text drift back and forth
                if (_OnHarvestTimer % 8 == 0)
                    Flip *= -1;
                TextX += 1 * Flip;
                // when animation is done, remove from array
                if (_OnHarvestTimer <= 0)
                {
                    _AfterHarvest();
                }
                return;
            }
            if (_Harvested)
            {
                _RegenerateTimer--;
                if (_RegenerateTimer <= 0)
                    _ResetTile();
            }
        }

        public override void Render(ref Graphics g)
        {
            base.Render(ref g);

            if (_OnHarvestTimer > 0)
            {
                _RenderOnHarvest(ref g);
                return;
            }
            if (_Harvested)
            {
                return;
            }

            if (_BeingHarvested)
            {
                int i = (100 * _HarvestTimer) / _HarvestTime;
                int w = (i * Width) / 100;

                g.FillRectangle(new SolidBrush(Color.Gray), X, Y, Width - w, Height);
                g.DrawString(((double)_HarvestTimer / 20) + "", new Font("Arial", 16), new SolidBrush(Color.Red), X + 8, Y + 16);
            }
        }

        public void ClickedDown()
        {
            if (!Game.P.isAdjacentTo(this) || _Harvested)
                return;
            _BeingHarvested = true;
            Game.P.SetIsWorking(true);
        }

        public void ClickedUp()
        {
            if (_BeingHarvested)
            {
                _ResetHarvest();
            }
        }

        private void _ResetHarvest()
        {
            _BeingHarvested = false;
            _HarvestTimer = _HarvestTime;
            _Harvested = false;
            /* +1 because array is 0-index, and rdm max = parm -1
             * set drop to random item on reset
             */ 
            _Drop = new ItemStack(TileDrops[_Type][C.RDM.Next(TileDrops.Count + 1)], 1);
        }

        private void _ResetTile()
        {
            _Image = (Image) new Bitmap(WorldHelper.TileImageFromType(_Type), Width, Height);
            _RegenerateTimer = _RegenerateTime;
            _ResetHarvest();
        }

        private void _Harvest()
        {
            _BeingHarvested = false;
            _Harvested = true;
            _OnHarvestTimer = 1 * 20;
            Game.P.SetIsWorking(false);
            SetImage((Image) new Bitmap(WorldHelper.TileImageFromType(_BaseType), Width, Height));
            Game.P.AddItem(_Drop);
        }

        private void _RenderOnHarvest(ref Graphics g)
        {
            g.DrawString("+1 " + _Drop.GetType().ToString(), new Font("Century", 8), new SolidBrush(Color.Green), TextX + 8, TextY + 16);
        }

        private void _AfterHarvest()
        {
            TextX = X;
            TextY = Y;
            //SpriteHandler.sprites.Remove(this);
            //RemoveFromWorld();
        }

        public bool isHarvested()
        {
            return _Harvested;
        }

        public int GetRegenerateTimer()
        {
            return _RegenerateTimer;
        }

        [DataContract]
        internal class BinFuncTile
            : BinTile
        {

            [DataMember]
            internal int RegenerateTimer;
            [DataMember]
            internal bool Harvested;

            public BinFuncTile(FunctionTile t)
                : base(t)
            {
                RegenerateTimer = t.GetRegenerateTimer();
                Harvested = t.isHarvested();
            }

            public override string GetJSON()
            {
                MemoryStream stream1 = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(BinFuncTile));
                ser.WriteObject(stream1, this);
                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);
                return sr.ReadToEnd();
            }

        }
    }
}