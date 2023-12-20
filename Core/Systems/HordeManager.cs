using Ascent.Content.NPCs.Events.Starfall;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace Ascent.Core.Systems
{
    public class HordeManager
    {
        public static List<List<NPC>> Hordes = new List<List<NPC>>(25);

        public static void Load()
        {

            On_NPC.UpdateNPC += updateHordes;
        }

        public static void Unload()
        {
            Hordes.Clear();

            On_NPC.UpdateNPC -= updateHordes;
        }

        private static void updateHordes(On_NPC.orig_UpdateNPC orig, NPC self, int i)
        {
            orig(self, i);

            for (int k = 0; k < Hordes.Capacity; k++)
            {
                Hordes.Add(new List<NPC>());
            }

            foreach (NPC npc in Main.npc)
            {
                if (npc.ModNPC is ThrallS)
                {
                    Hordes[0].Add(npc);
                }
                else if (Hordes[0].Contains(npc))
                {
                    Hordes[0].Remove(npc);
                }
            }

            //foreach (List<NPC> horde in Hordes)
            //{
            //    foreach (NPC npc in horde)
            //    {
            //        Vector2 netDist = Vector2.Zero;

            //        foreach (NPC npc2 in horde)
            //        {
            //            if (npc2 != npc)
            //            {
            //                Vector2 delta = npc2.Center - npc.Center;
            //                netDist.X += delta.X / (delta.Length() * delta.Length());
            //            }
            //        }
            //        netDist /= horde.Count;

            //        if (npc.velocity.Y == 0)
            //        {
            //            npc.velocity -= .001f * netDist;
            //        }
            //    }
            //}

            Hordes.Clear();
        }
    }
}
