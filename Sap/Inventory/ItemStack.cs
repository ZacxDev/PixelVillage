using PixelVillage.GameSprite;
using PixelVillage.GameWorld;
using PixelVillage.Main;
using PixelVillage.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PixelVillage.Inventory
{
    enum ItemMaterial { WOOD_LOG, LEAVES, DIRT, STONE }

    class ItemStack
        : BasicSprite
    {

        private ItemMaterial _Type;
        private int _Amount;
        private Image _Image;

        public ItemStack(ItemMaterial type, int amount)
            : base(0, 0, 64, 64)
        {
            _Type = type;
            _Amount = amount;
            _Image = (Image)new Bitmap(PVResources.ItemImageMap[_Type], Width, Height);
        }

        public ItemStack(BinItemStack bin)
            : this((ItemMaterial)Enum.Parse(typeof(ItemMaterial), bin.mat), bin.amount)
        {
        }

        public void render(ref Graphics g, float ix, float iy)
        {
            X = (int)ix;
            Y = (int)iy;
            //g.FillRectangle(Brushes.Black, ix, iy, Width, (int)(Height * 0.5));
            g.DrawImage(_Image, (int)ix, iy);
            var amountMsg = GetAmount() + "x";
            var s = g.MeasureString(amountMsg, C.TFont);
            g.FillRectangle(Brushes.Aqua, X, Y, s.Width, s.Height);
            g.DrawString(amountMsg, C.TFont, Brushes.Black, ix, iy);
        }

        new public ItemMaterial GetType()
        {
            return _Type;
        }

        public int GetAmount()
        {
            return _Amount;
        }

        public void IncrementAmount()
        {
            _Amount += 1;
        }

        [DataContract]
        internal class BinItemStack
        {
            [DataMember]
            internal string mat;
            [DataMember]
            internal int amount;

            internal BinItemStack(ItemStack item)
            {
                mat = item._Type.ToString();
                amount = item._Amount;
            }

            public virtual string GetJSON()
            {
                MemoryStream stream1 = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(BinItemStack));
                ser.WriteObject(stream1, this);
                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);
                return sr.ReadToEnd();
            }
        }

    }
}
