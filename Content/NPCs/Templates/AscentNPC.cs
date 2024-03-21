using Ascent.Core;
using Ascent.Configs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;

namespace Ascent.Content.NPCs.Templates
{
    public abstract class AscentNPC : ModNPC
    {
        public override string Texture => QuickDirectory.PlaceHolderTx;

        public override void SetDefaults()
        {
            NPC.life = 10;
            NPC.damage = 0;
            NPC.defense = 1;
            NPC.aiStyle = -1;
            NPC.Size = new Vector2(32);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
        }

        #region Basic Actions
        public void Move(Vector2 pos, Vector2 target, float speed)
        {
            Vector2 delta = target - pos;
            NPC.velocity = speed * Vector2.Normalize(delta);
        }

        public void Move(Vector2 pos, Vector2 target, float speed, float drag)
        {
            Vector2 delta = target - pos;
            NPC.velocity += speed * Vector2.Normalize(delta);
            NPC.velocity *= drag;
        }

        public virtual void Shoot(int projectile, Vector2 pos, Vector2 target, float speed = 0, bool friendly = false, int damage = 0, float knockBack = 0, float ai1 = 0, float ai2 = 0, float localAi1 = 0, float localAI2 = 0)
        {
            Vector2 delta = ModMath.Delta(pos, target);

            var entitySource = NPC.GetSource_FromAI();

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int type = Projectile.NewProjectile
                (
                    entitySource,
                    pos,
                    Vector2.Normalize(delta) * speed,
                    projectile,
                    damage / 2,
                    knockBack
                );

                Projectile proj = Main.projectile[type];

                if (!friendly)
                {
                    proj.hostile = true;
                    proj.friendly = false;
                }

                proj.ai[0] = ai1;
                proj.ai[1] = ai2;
                proj.localAI[0] = localAi1;
                proj.localAI[1] = localAI2;
            }
        }
        #endregion
    }
}
