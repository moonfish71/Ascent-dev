using Ascent.Content.NPCs.Events.Starfall;
using Ascent.Core.ModWorlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Ascent.Content.Events.Starfall
{
    public class StarFall
    {
        public static int[] BasicEnemies =
        {
            ModContent.NPCType<ThrallS>(), 
            ModContent.NPCType<ThrallM>(),
            ModContent.NPCType<ThrallL>()
        };

        public void StartEvent()
        {
            EventWorld.StarfallUp = true;
        }

        public void Update()
        {

        }
    }
}
