using Ascent.Core;
using Ascent.Core.Systems.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using static Ascent.Core.QuickDirectory;

namespace Ascent.Content.Projectiles.Hostile.BossAttacks.Aster
{
    public class MadStar : ModProjectile
    {
        public override string Texture => AsterProjTex + Name;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(34);
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.maxPenetrate = 10;
            Projectile.tileCollide = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return base.OnTileCollide(oldVelocity);
        }

        public override void AI()
        {
            Projectile.rotation += Math.Clamp(Projectile.velocity.X / 30, -1f, 1f);
        }
    }
}
