using Ascent.Content.NPCs.Templates;
using Ascent.Core.ModPlayers;
using Ascent.Core.Systems;
using Humanizer.Localisation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using static Ascent.Core.QuickDirectory;

namespace Ascent.Content.NPCs.Events.Starfall
{
    public class ThrallS : AscentNPC
    {
        public override string Texture => NPCTex + Name;

        public override void SetStaticDefaults() 
        {
            Main.npcFrameCount[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.lifeMax = 50;
            NPC.aiStyle = -1;
            NPC.damage = 60;
            NPC.knockBackResist = 0;
            NPC.defense = 1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Visuals.Meteor,
                new FlavorTextBestiaryInfoElement("A thrall of the Astral Gestalt. This thrall has been shaped into a spider-like form, letting it deftly jump and climb walls. It beams the dark truths of its kin directly into the skulls of those it latches onto.")
            }); ;
        }

        public ref float timer => ref NPC.ai[0];

        public ref float State => ref NPC.ai[1];

        public float OldState;

        public Vector2 delta = Vector2.Zero;

        public Vector2 relativeDelta = Vector2.Zero;

        Tile Front;
        Tile FrontBottom;
        Tile FrontTop;

        bool LineOfSight;

        public override void AI()
        {
            NPC.TargetClosestUpgraded();
            timer++;

            int x = (int)((NPC.Center.X + (NPC.width * NPC.direction / 3)) / 16) + (1 * NPC.direction);
            int y = (int)((NPC.Center.Y + 8) / 16);

            Front = Framing.GetTileSafely(x, y);
            FrontBottom = Framing.GetTileSafely(x, y + 1);
            FrontTop = Framing.GetTileSafely(x, y - 1);

            Tile Top = Framing.GetTileSafely(x - NPC.direction, y - 1);
            Tile Bottom = Framing.GetTileSafely(x - NPC.direction, y + 1);
            Tile Back = Framing.GetTileSafely(x - (2 * NPC.direction), y);

            Player player = Main.player[NPC.target];
            ScreenMovementPlayer screen = player.GetModPlayer<ScreenMovementPlayer>();

            float CollisionPoint = 0f;

            LineOfSight = Collision.CheckAABBvLineCollision(NPC.Center, NPC.Size, player.Center, player.Size, 1, ref CollisionPoint);

            delta = player.Center - NPC.Center;

            OldState = State;

            switch (State)
            {
                case 0:
                    NPC.aiStyle = NPCAIStyleID.Fighter;
                    NPC.velocity.X = Math.Sign(delta.X) * 2.25f;
                    NPC.spriteDirection = -NPC.direction;
                    break;
                case 1:
                    if (timer < 40)
                    {
                        NPC.velocity.X *= .85f;
                        relativeDelta = CalcDelta(NPC.Center, player.Center + new Vector2(0, -750 + delta.Y));
                    }
                    else if(timer == 40)
                    {
                        NPC.velocity = Vector2.Normalize(relativeDelta) * 12;
                        NPC.direction = Math.Sign(NPC.velocity.X);
                    }
                    else
                    {
                        if(NPC.velocity.Y == 0)
                        {
                            timer = 0;
                        }
                    }
                    NPC.spriteDirection = -NPC.direction;
                    break;
                case 2:
                    NPC.noGravity = true;
                    NPC.velocity = new Vector2(2 * NPC.direction, -2);
                    break;
                case 3:
                    NPC.Center = player.Center + new Vector2(4 * player.direction, -16);
                    NPC.velocity = Vector2.Zero;
                    NPC.spriteDirection = -NPC.direction;
                    break;
            }

            /*
             * State controlls behavior
             * 
             * State = 0, AIStyle is fighter AI
             * 
             * State = 1, Facehugger
             * 
             * State = 2, Climbing type 1, for walls of blocks
             * 
             * State = 3, Face-eating
             * 
             * Other states do something else idk
             */

            SetNewState();
        }

        public Vector2 CalcDelta(Vector2 Start, Vector2 End)
        {
            return End - Start;
        }

        public void AimedJump(Vector2 speed, Vector2 delta)
        {

        }

        public override bool? CanFallThroughPlatforms()
        {
            Player player = Main.player[NPC.target];

            if (player.Center.Y < NPC.Center.Y) return true;

            return false;
        }

        public void SetNewState()
        {
            NPC.netUpdate = true;
            switch (State)
            {
                case 0:
                    if (delta.Length() < 400f && Main.rand.NextBool(10))
                    {
                        State = 1;
                    }
                    if (Front.HasTile && FrontBottom.HasTile && FrontTop.HasTile && Main.tileSolid[Front.TileType] && Main.tileSolid[FrontBottom.TileType] && Main.tileSolid[FrontTop.TileType] && FrontBottom.BlockType == BlockType.Solid && (Math.Abs(delta.X) > 16 | delta.Y > 5))
                    {
                        State = 2;
                    }
                    break;
                case 1:
                    if (delta.Length() > 400f && NPC.velocity.Y == 0)
                    {
                        State = 0;
                        NPC.noTileCollide = false;
                    }
                    if (Front.HasTile && FrontBottom.HasTile && FrontTop.HasTile && Main.tileSolid[Front.TileType] && Main.tileSolid[FrontBottom.TileType] && Main.tileSolid[FrontTop.TileType] && FrontBottom.BlockType == BlockType.Solid && !LineOfSight && (Math.Abs(delta.X) > 16 | delta.Y > 5))
                    {
                        State = 2;
                    }

                    break;
                case 2:
                    if (!(Front.HasTile && FrontBottom.HasTile && Main.tileSolid[Front.TileType] && Main.tileSolid[FrontBottom.TileType] && FrontBottom.BlockType == BlockType.Solid))
                    {
                        if (timer > 3)
                        {
                            NPC.noGravity = false;
                            if (delta.Length() < 400f)
                            {
                                State = 1;
                            }

                            if (delta.Length() > 400f)
                            {
                                State = 0;
                            }
                        }
                    }
                    else
                    {
                        timer = 0;
                    }
                    break;
                case 3:
                    NPC.damage = 0;
                    if(timer > 60)
                    {
                        State = 0;
                    }
                    break;

            }

            NPC.netUpdate = false;

            if (State != OldState)
            {
                if (State != 0)
                {
                    NPC.aiStyle = -1;
                }

                if(State != 3)
                {
                    NPC.damage = NPC.defDamage;
                }

                timer = 0;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if(State == 1 && NPC.velocity.Y != 0)
            {
                State = 3;
                timer = 0;

                target.AddBuff(BuffID.Obstructed, 60);
            }
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsOverPlayers.Add(index);
        }

        int[] animtimer = new int[] {0, 0, 0, 0};

        public override void FindFrame(int frameHeight)
        {
            animtimer[0]++;

            switch(State)
            {
                case 0:
                    if (animtimer[0] > 4)
                    {
                        NPC.frameCounter++;
                        animtimer[0] = 0;

                        if(NPC.frameCounter > 3)
                        {
                            NPC.frameCounter = 0;
                        }
                    }
                    break;
            }

            NPC.frame.Y = (int)(frameHeight * NPC.frameCounter);
        }
    }
}
