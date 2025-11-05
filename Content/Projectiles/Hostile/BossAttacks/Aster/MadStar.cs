using Ascent.Core;
using Ascent.Core.Systems.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static Ascent.Core.QuickDirectory;

namespace Ascent.Content.Projectiles.Hostile.BossAttacks.Aster
{
    public class MadStar : ModProjectile
    {
        public override string Texture => AsterProjTex + Name;

        Vector2 oVel;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(34);
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.maxPenetrate = 10;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            oVel = Projectile.velocity;
        }

        public override void AI()
        {
            Projectile.velocity += -.05f * oVel;
            Projectile.rotation += Math.Clamp(Projectile.velocity.X / 30, -1f, 1f);
        }
    }
}
