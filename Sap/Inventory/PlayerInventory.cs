using PixelVillage.Events;
using PixelVillage.GameSprite;
using PixelVillage.Main;
using PixelVillage.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PixelVillage.Inventory
{
    class PlayerInventory 
        : BasicSprite, MouseEventListener
    {
        private List<ItemStack> _Items = new List<ItemStack>();
        private bool _IsOpen;
        private Image _Image;

        public PlayerInventory()
            : base((int)(Game.CANVAS_WIDTH * 0.25), (int)(Game.CANVAS_HEIGHT * 0.1), (int)(Game.CANVAS_WIDTH * 0.5), (int)(Game.CANVAS_HEIGHT * 0.6))
        {
            _Image = PVResources.ITEM_WOOD_LOG;
        }

        public PlayerInventory(BinPlayerInv bin)
            : this()
        {
            for (var i = 0; i < bin.items.Length; i++)
            {
                _Items.Add(new ItemStack(bin.items[i]));
            }
        }

        public void render(ref Graphics g)
        {
            g.FillRectangle(Brushes.LightGray, GetCamRelBounds());
            g.DrawString("Inventory", C.MFont, Brushes.Black, GetCamRelX() + Width * 0.05f, GetCamRelY() + Height * 0.025f);

            var ix = GetCamRelX() + Width * 0.05f;
            var iy = GetCamRelY() + Height * 0.25f;
            for (var i = 0; i < _Items.Count; i++)
            {
                _Items[i].render(ref g, ix, iy);

                ix += (int)(_Items[i].Width * 1.5);

                if (ix > GetCamRelX() + Height * 0.9)
                {
                    ix = GetCamRelX() + Width * 0.05f;
                    iy += (int)(_Items[i].Height * 1.5);
                }
            }
        }

        public void AddItem(ItemStack i)
        {
            IncrementItem(i);
        }

        public bool ContainsItem(ItemMaterial m)
        {
            for (int i = 0; i < _Items.Count; i++)
            {
                if (_Items[i].GetType() == m)
                    return true;
            }
            return false;
        }

        public void IncrementItem(ItemStack i)
        {
            for (int n = 0; n < _Items.Count; n++)
            {
                if (_Items[n].GetType() == i.GetType())
                {
                    _Items[n].IncrementAmount();
                    return;
                }
            }
            _Items.Add(i);
        }

        public int Size()
        {
            return _Items.Count();
        }

        public bool IsOpen()
        {
            return _IsOpen;
        }

        public void Open()
        {
            _IsOpen = true;
        }

        public void Close()
        {
            _IsOpen = false;
        }

        public void ToggleOpen()
        {
            if (_IsOpen)
                Close();
            else
                Open();
        }

        // Event Handling Begin

        public void OnClickDown(Rectangle cursor)
        {
            Debug.WriteLine("hey");
        }

        public void OnClickUp(Rectangle cursor)
        {
            throw new NotImplementedException();
        }

        public void onMouseMove(Rectangle cursor)
        {
            throw new NotImplementedException();
        }

        // Event Handling End

        [DataContract]
        internal class BinPlayerInv
        {
            [DataMember]
            internal ItemStack.BinItemStack[] items;

            internal BinPlayerInv(PlayerInventory inv)
            {
                items = new ItemStack.BinItemStack[inv._Items.Count];
                for (int i = 0; i < inv._Items.Count; i++)
                {
                    items[i] = new ItemStack.BinItemStack(inv._Items[i]);
                }
            }

            public virtual string GetJSON()
            {
                MemoryStream stream1 = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(BinPlayerInv));
                ser.WriteObject(stream1, this);
                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);
                return sr.ReadToEnd();
            }
        }
    }
}
