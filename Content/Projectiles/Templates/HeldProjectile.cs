using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Ascent.Content.Projectiles.Templates
{
    public abstract class HeldProjectile : ModProjectile
    {
        public float length = 100;
        public float lengthMod = 150;
        public float rotation = 0;
        public float duration = 60;

        public int defDamage = 0;

        public float MouseRotation;
        public float RelativeRot = 0;

        public bool SetInitCons;

        public Vector2 armCenter;

        public float timer = 0;

        public override void AI()
        {
            base.AI();
        }
    }
}
