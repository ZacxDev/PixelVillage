using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using PixelVillage.GameWorld;
using PixelVillage.Main;
using System.Diagnostics;
using PixelVillage.Inventory;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Runtime.Serialization;
using PixelVillage.UI;

namespace PixelVillage.GameSprite
{
    class Player : SentiantSprite
    {

        public bool Crouching = false;
        private bool _IsWorking = false;
        private int _TarX, _TarY;
        private bool _TrackingTar;
        private PlayerInventory _Inventory = new PlayerInventory();

        // Render X / Y -> used to render player properly with shitty camera
        int _RenX, _RenY;

        public Player()
            : base(500, 400, 96, 110, PVResources.SPRITE_PLAYER)
        {
            _Image = new Bitmap(_Image, new Size(Width, Height));
        }

        public override void tick()
        {
            // disable player from moving when inv is open
            if (_Inventory.IsOpen())
                return;
            KeyInput.UpdatePlayerVelocity();
            base.tick();

            if (_TrackingTar && GetBounds().IntersectsWith(new Rectangle(_TarX, _TarY, 5, 5)))
            {
                VelX = 0;
                VelY = 0;
                _TrackingTar = false;
                return;
            }

        }

        public override void Render(ref Graphics g)
        {
            // Render x/y, when camera is not in starting position, using regular coordinates, otherwise use relative ones
            _RenX = X;
            _RenY = Y;

            // if camera going left/right, render the player at the max/min viewport coords
            if (-(int)g.Transform.OffsetX > 0)
            {
                if (Camera.CamMovingLeft)
                    _RenX = (int)(-(int)g.Transform.OffsetX + Camera.VIEWPORT_SIZE_X * C.PLAYER_MAX_ACROSS);
                else if (Camera.CamMovingRight)
                    _RenX = (int)(-(int)g.Transform.OffsetX + Camera.VIEWPORT_SIZE_X * C.PLAYER_MIN_ACROSS);
                
            }

            // if camera going down/up, render the player at the max/min viewport coords
            if (-(int)g.Transform.OffsetY > 0)
            {
                if (Camera.CamMovingDown)
                    _RenY = (int)(-(int)g.Transform.OffsetY + Camera.VIEWPORT_SIZE_Y * C.PLAYER_MAX_DOWN);
                else if (Camera.CamMovingUp)
                    _RenY = (int)(-(int)g.Transform.OffsetY + Camera.VIEWPORT_SIZE_Y * C.PLAYER_MIN_DOWN);
                
            }

            g.DrawImage(_Image, _RenX, _RenY);

            if (_Inventory.IsOpen())
                _Inventory.render(ref g);
        }

        public void SetTarget(Rectangle r)
        {
            if (_IsWorking)
            {
                SetIsTracking(false);
                return;
            }
            _TarX = r.X;
            _TarY = r.Y;
            _TrackingTar = true;
            _UpdateTracking();
        }

        public void SetIsTracking(bool b)
        {
            _TrackingTar = b;
            if (!b)
                _SetNoVelocity();
        }

        public bool isTracking()
        {
            return _TrackingTar;
        }

        private void _UpdateTracking()
        {
            int diffX = X - _TarX;
            int diffY = Y + Height / 2 - _TarY;
            double distance = Math.Sqrt(diffX * diffX + diffY * diffY);

            VelX = (int)((-1 / distance) * diffX * C.PLAYER_SPEED);
            VelY = (int)((-1 / distance) * diffY * C.PLAYER_SPEED);
        }

        private void _SetNoVelocity()
        {
            VelX = 0;
            VelY = 0;
        }

        public bool GetIsWorking()
        {
            return _IsWorking;
        }

        public void SetIsWorking(bool b)
        {
            _IsWorking = b;
        }

        //public bool isAdjacentTo(Tile t)
        //{
        //    return WorldHelper.GetAdjacentTiles(X, Y, Width, Height).Contains(t);
        //}

        public bool isAdjacentTo(Tile t)
        {
            Rectangle area = new Rectangle(X - C.PLAYER_INTERACT_RANGE, Y - C.PLAYER_INTERACT_RANGE, Width + C.PLAYER_INTERACT_RANGE * 2, Height + C.PLAYER_INTERACT_RANGE * 2);

            return area.IntersectsWith(t.GetBounds());
        }

        public PlayerInventory GetInventory()
        {
            return _Inventory;
        }

        public void AddItem(ItemStack i)
        {
            _Inventory.AddItem(i);
        }

        public void LoadInventory(BinPlayer bin)
        {
            _Inventory = new PlayerInventory(bin.inv);
        }

        public string GetJSON()
        {
            return new BinPlayer(this).GetJSON();
        }

        [DataContract]
        internal class BinPlayer
        {
            [DataMember]
            internal PlayerInventory.BinPlayerInv inv;
            [DataMember]
            internal int X;
            [DataMember]
            internal int Y;

            internal BinPlayer() { }

            internal BinPlayer(Player p)
            {
                inv = new PlayerInventory.BinPlayerInv(p.GetInventory());
                X = p.X;
                Y = p.Y;
            }

            public virtual string GetJSON()
            {
                MemoryStream stream1 = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(BinPlayer));
                ser.WriteObject(stream1, this);
                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);
                return sr.ReadToEnd();
            }
        }

    }
}
