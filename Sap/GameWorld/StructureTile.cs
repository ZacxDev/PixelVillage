using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PixelVillage.GameWorld
{
    class StructureTile
        : Tile
    {
        public StructureTile(int x, int y, Material type, World world)
            : base(x, y, type, world)
        {
            world.StructureTiles.Add(this);
        }

        // All init is done in worldhelper.loadworld
        public StructureTile(BinStructureTile t)
            : base(t)
        { }

        [DataContract]
        internal class BinStructureTile
            : BinTile
        {

            internal BinStructureTile(StructureTile t)
                : base(t)
            {
                
            }

            public override string GetJSON()
            {
                MemoryStream stream1 = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(BinStructureTile));
                ser.WriteObject(stream1, this);
                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);
                return sr.ReadToEnd();
            }
        }
    }
}
