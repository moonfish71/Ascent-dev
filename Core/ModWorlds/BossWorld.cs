using Ascent.Content.Events.Starfall;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Ascent.Core.ModWorlds
{
    public class BossWorld : ModSystem
    {
        public static bool downedAster;

        public override void OnWorldLoad()
        {
            downedAster = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (downedAster)
            {
                tag["Aster"] = true;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedAster = tag.ContainsKey("Aster");
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = downedAster;
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedAster = flags[0];
        }
    }
}
