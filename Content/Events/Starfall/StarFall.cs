using Ascent.Content.NPCs.Events.Starfall;
using Ascent.Core.ModWorlds;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ascent.Content.Events.Starfall
{
    public class StarFall
    {
        public static Vector2 CentralCoords;

        public static int[] BasicEnemies =
        {
            ModContent.NPCType<ThrallS>(), 
            ModContent.NPCType<ThrallM>(),
            ModContent.NPCType<ThrallL>()
        };

        public static void StartEvent()
        {
            EventWorld.StarfallUp = true;
            wavetimer = 0;
        }

        public static void StopEvent()
        {
            EventWorld.StarfallUp = false;
        }

        public static float wavetimer = 0; 

        public static void Update()
        {
            wavetimer++;

            if (Main.rand.NextBool(90))
            {
                for (int i = 0; i < Main.rand.Next(1, 3); i++)
                {
                    Projectile.NewProjectile(Entity.GetSource_NaturalSpawn(), CentralCoords + new Vector2(750 - (1500f * Main.rand.Next(0, 2)), -850), Vector2.Zero, ModContent.ProjectileType<EGG>(), 50, 10);
                }
            }
        }
    }
}
