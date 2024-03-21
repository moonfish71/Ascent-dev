using Ascent.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ascent.Content.NPCs.Events.Starfall
{
    public class EGG : ModProjectile
    {
        public override string Texture => QuickDirectory.PlaceHolderTx;

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.damage = 50;
            Projectile.Size = Vector2.One * 32;
            Projectile.scale = Main.rand.NextFloat(0.8f, 1.2f);
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.netUpdate = true;

            Projectile.velocity.Y = Main.rand.NextFloat(7f, 15f);
            Projectile.velocity.X = Main.rand.NextFloat(-3f, 3f);

            Projectile.netUpdate = false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

            if (Main.netMode != 1)
            {
                var source = Projectile.GetSource_FromAI();

                Projectile.netUpdate = true;

                float spawnSelector = Main.rand.NextFloat();
                int thrall;

                if (spawnSelector < .5f)
                {
                    thrall = ModContent.NPCType<ThrallS>();
                }
                else if (spawnSelector < .85f)
                {
                    thrall = ModContent.NPCType<ThrallM>();
                }
                else
                {
                    thrall = ModContent.NPCType<ThrallL>();
                }

                Projectile.netUpdate = false;

                int SPAWN = NPC.NewNPC
                    (
                        source,
                        (int)Projectile.Center.X,
                        (int)Projectile.Center.Y,
                        thrall
                    );

            }
            return true;
        }
    }
}
