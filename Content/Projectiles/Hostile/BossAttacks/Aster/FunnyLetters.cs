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
using Ascent.Core.Systems.Particles;

namespace Ascent.Content.Projectiles.Hostile.BossAttacks.Aster
{
    public class FunnyLetters : Particle
    {
        public override string TexturePath => AsterProjTex + "FunnyLetters";

        public override void SetDefaults()
        {
            frameCount.Y = 27f;
            TimeLeft = 60;
        }

        public float letter
        {
            get => ai[0];
            set => ai[0] = value;
        }

        public override void Update()
        {
            frame.Y = (int)letter * 34;
            velocity.Y = 1f;
            Opacity -= 8;
            if((int)letter <= 0)
            {
                Kill();
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition)
        {
            drawColor = Color.White;

            return true;
        }
    }
}
