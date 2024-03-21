using Ascent.Content.NPCs.Templates;
using Ascent.Core;
using Ascent.Core.ModPlayers;
using Ascent.Core.Systems.Particles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static Ascent.Core.QuickDirectory;

namespace Ascent.Content.NPCs.Bosses.Aster
{
    public partial class AsterBoss : AscentNPC
    {
        public override string Texture => BossTex + "Aster/Aster";

        public float Z = 0;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.Size = new Vector2(60);
            NPC.lifeMax = 3150;
            NPC.damage = 75;
            NPC.defense = 5;
            NPC.knockBackResist = 0;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Sound/Music/AsterTheme(Placeholder)");

            SetUpPhases();
        }

        public ref float Phase => ref NPC.ai[0];

        public float[] timer = new float[8];

        public override void OnSpawn(IEntitySource source)
        {
            spr = Particle.NewParticle(NPC.Center, new AsterSprite(), Vector2.Zero);
            if (spr is AsterSprite sprite)
            {
                sprite.parent = sprite.oldParent = Main.npc[NPC.whoAmI];
            }

            UpdateTargets(80f * 16f);

            NPC.target = 0;
        }

        int AttackIndex = 0;
        bool AttackActive = false;

        Player ActivePlayer = null;

        List<Player> Targets = new List<Player>();

        public override void AI()
        {
            ActivePlayer = Main.player[NPC.target];

            if (ActivePlayer != null && !ActivePlayer.dead)
            {

                if (!AttackActive)
                {
                    timer[0]++;
                }

                //Streamlined Attack wratchet

                switch (Phase) 
                {
                    case 0:
                        Phase = 1;
                        break;
                    case 1:
                        foreach (Player player in Targets)
                        {
                            ScreenMovementPlayer shakePlayer = player.GetModPlayer<ScreenMovementPlayer>();

                            shakePlayer.ScreenPosModified = true;

                            shakePlayer.ScreenCenter = (NPC.Center + player.Center) / 2;
                        }

                        Phase1();
                        break;
                }

                UpdateTargets(80f * 16f);
            }
            else if((ActivePlayer.dead || !Targets.Contains(ActivePlayer)) && Targets.Count > 0)
            {
                SelectTarget();
            }
            else
            {
                NPC.velocity.Y -= .2f;
                NPC.EncourageDespawn(120);

                foreach (Player player in Targets)
                {
                    ScreenMovementPlayer shakePlayer = player.GetModPlayer<ScreenMovementPlayer>();

                    shakePlayer.ScreenPosModified = false;
                }
            }

            if(NPC.timeLeft <= 0)
            {
                spr.TimeLeft = 0;
            }
        }

        private void UpdateTargets(float Dist)
        {
            for(int i = 0; i < Main.player?.Count(); i++)
            {
                Player select = Main.player[i];

                float dist = ModMath.Delta(NPC.Center, select.Center).Length();

                if(dist < Dist && !Targets.Contains(select))
                {
                    Targets.Add(select);
                }
                else if(Targets.Contains(select))
                {
                    Targets.Remove(select);
                }
            }
        }

        private void SelectTarget()
        {
            NPC.netUpdate = true;

            UpdateTargets(80f * 16f);

            if (Targets.Count > 0)
            {
                NPC.target = Targets[Main.rand.Next(Targets.Count)].whoAmI;
            }

            NPC.netUpdate = false;
        }

        public override void OnKill()
        {
            if(spr is AsterSprite sprite)
            {
                spr.TimeLeft = 0;
                sprite.parent = null;
            }
        }
    }
}
