using Ascent.Content.NPCs.Templates;
using Ascent.Core.ModPlayers;
using Humanizer.Localisation;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using static Ascent.Core.QuickDirectory;

namespace Ascent.Content.NPCs.Events.Starfall
{
    public class ThrallM : AscentNPC
    {
        public override string Texture => PlaceHolderTx;

        public enum States
        {
            Walking,
            Slash,
            Dashing
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 48;
            NPC.lifeMax = 100;
            NPC.aiStyle = -1;
            NPC.damage = 60;
            NPC.knockBackResist = 0.3f;
            NPC.defense = 1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Visuals.Meteor,
                new FlavorTextBestiaryInfoElement("A thrall of the Astral Gestalt. This thrall has been granted a pair of knife-like limbs to tear through those that would stop the propagation of The Word.")
            }); ;
        }

        public float[] Timer = new float[4] { 0, 0, 0, 0 };

        public ref float State => ref NPC.ai[2];

        float oldState = 0;

        public Vector2 delta = Vector2.Zero;

        public override void AI()
        {
            NPC.TargetClosestUpgraded();
            Player player = Main.player[NPC.target];

            oldState = State;

            delta = player.Center - NPC.Center;

            Timer[0]++;
            Timer[1]++;

            NPC.netUpdate = true;

            switch (State)
            {
                case (float)States.Walking:

                    if (NPC.Distance(Main.player[NPC.target].Center) < 100 && Main.rand.NextBool(10))
                    {
                        State = (float)States.Slash;
                        Timer[0] = 0;
                    }
                    break;
                case (float)States.Slash:
                    if (Timer[0] > 100)
                    {
                        Timer[0] = 0;
                        State = (float)States.Walking;
                    }
                    break;
                case (float)States.Dashing:
                    if (Timer[0] > 180 && NPC.velocity.Y == 0f)
                    {
                        State = (float)States.Walking;
                        Timer[0] = 0;
                    }
                    break;
            }

            NPC.netUpdate = false;

            switch (State)
            {
                case (float)States.Walking:
                    NPC.aiStyle = NPCAIStyleID.Fighter;
                    NPC.velocity.X = NPC.direction * 1.2f;
                    break;
                case (float)States.Slash:
                    NPC.velocity.X *= 0.95f;
                    NPC.aiStyle = -1;
                    

                    if (Timer[0] == 40)
                    {
                        NPC.velocity.X += NPC.direction * 8f;
                    }
                    break;
                case (float)States.Dashing:
                    NPC.velocity.X *= .99f;
                    NPC.aiStyle = -1;
                    break;
            }

            bool dashCheck = false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot);
        }

        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            return base.ModifyCollisionData(victimHitbox, ref immunityCooldownSlot, ref damageMultiplier, ref npcHitbox);
        }
    }
}
