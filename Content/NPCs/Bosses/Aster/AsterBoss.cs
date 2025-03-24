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

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Sound/Music/Aster2MainLoop");

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
            NPC.Opacity = 0;

            SelectTarget();
        }

        int AttackIndex = 0;
        bool AttackActive = false;

        Player ActivePlayer = null;

        List<Player> Targets = new List<Player>();

        public override void AI()
        {


            if (!ActivePlayer.dead && ActivePlayer != null)
            {

                if (!AttackActive)
                {
                    timer[0]++;
                }

                //Streamlined Attack ratchet

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

                            shakePlayer.MoveScreen((NPC.Center + player.Center) / 2, 40f);
                        }

                        Phase1();
                        break;
                }
                if (!Targets.Contains(ActivePlayer) && Targets.Count > 0)
                {
                    SelectTarget();
                }
                UpdateTargets(80f * 16f);
            }
            else
            {
                SelectTarget();

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

        private void UpdateTargets(float Range)
        {
            for(int i = 0; i < Main.player?.Count(); i++) //For each player in the server
            {
                Player select = Main.player[i];

                float dist = Vector2.Distance(NPC.Center, select.Center);

                //Check if the player is already a target
                if (Targets.Contains(select))
                {
                    //If so, remover them from targets if they're out of range
                    if (dist < Range)
                    {
                        Targets.Remove(select);
                    }
                    return;
                }

                //If the player is in range, set it as a target
                if (dist < Range)
                {
                    Targets.Add(select);
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
                ActivePlayer = Main.player[NPC.target];
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
