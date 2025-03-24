using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using static Ascent.Core.QuickDirectory;

namespace Ascent.Content.Projectiles.Hostile.BossAttacks.Aster
{
    public class FunnyLetters : ModProjectile
    {
        public override string Texture => AsterProjTex + Name;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 27;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = Projectile.width;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
        }

        public float timer
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public float letter
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }

        public override void AI()
        {
            timer++;
            Projectile.frame = (int)letter;
            Projectile.velocity.Y += 0.01f * (float)Math.Sin(timer / 20);
            if((int)letter <= 0)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            return base.PreDraw(ref lightColor);
        }
    }
}
