using Ascent.Content.Events.DarkNight;
using Ascent.Content.Events.Starfall;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Ascent.Core.ModWorlds
{
    public class EventWorld : ModSystem
    {
        public static bool DarkNightActive;
        public static bool StarryNight;

        public static bool StarfallUp;

        public static bool DownedStarfall;
        public static bool DownedAster;

        public override void OnWorldLoad()
        {
            DarkNightActive = false; 
            StarfallUp = false;

            DownedStarfall = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if(DownedStarfall)
            {
                tag["Starfall"] = true;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            DownedStarfall = tag.ContainsKey("Starfall");
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = DownedStarfall;
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            DownedStarfall = flags[0];
        }

        public override void PostUpdateWorld()
        {
            if (!Main.dayTime && Main.time == 0 && Main.rand.NextBool(1))
            {
                StarryNight = true;
                DarkNight.Warning();
            }
            else if (DarkNightActive)
            {
                DarkNight.UpdateEvent();
            }

            if(StarfallUp)
            {
                StarFall.Update();
            }
        }

        public override void ModifyLightingBrightness(ref float scale)
        {
            if (DarkNightActive)
            {
                scale = DarkNight.UpdatedLighting();
            }
        }
    }
}
