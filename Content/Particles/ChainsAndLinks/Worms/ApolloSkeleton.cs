using Ascent.Core;
using Ascent.Core.Systems.Particles;
using Ascent.Core.Systems.Particles.IKChain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Ascent.Content.Particles.ChainsAndLinks.Worms
{
    public class ApolloSkeleton :IKChain
    {
        public override void SetDefaults()
        {
            TimeLeft = ModMath.SecondsToTicks(60);
            LockToBase = false;
        }

        public override void SetUpCompositeLinks()
        {
            for (int i = 0; i < length; i++)
            {
                IKLink NextLink;

                if (i == 0)
                {
                    NextLink = new ApolloTailLink();
                }
                else if (i == length - 1)
                {
                    NextLink = new ApolloHeadLink();
                }
                else
                {
                    NextLink = new ApolloBodyLink();
                }

                compositeLinks.Add(NextLink);
            }
        }

        public override void OnSpawn()
        {
            target = new Vector2 (position.X, position.Y);
        }

        public override void AI()
        {
            Vector2 TargetVelocity = 2f * Vector2.Normalize(Main.LocalPlayer.Center - target);
            target += TargetVelocity;
        }
    }

    public class ApolloHeadLink : IKLink
    {
        public override void SetDefaults()
        {
            length = 104;
            AngleRange = MathHelper.ToRadians(35);
        }
    }

    public class ApolloBodyLink : IKLink
    {
        public override string TexturePath => QuickDirectory.BossTex + "GKU/Apollo/ApolloBodyLink";

        public override void SetDefaults()
        {
            length = 52;
            AngleRange = MathHelper.ToRadians(35f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(TexturePath);

            spriteBatch.Draw
                (
                    texture, 
                    drawPosition - Main.screenPosition, 
                    frame, 
                    drawColor * (Opacity / 255f), 
                    (float)rotation, 
                    frame.Size() / 2, 
                    StretchScale * scale, 
                    SpriteEffects.None, 
                    0f
                );
            
            return true;
        }
    }

    public class ApolloTailLink : IKLink
    {
        public override void SetDefaults()
        {
            length = 76;
            AngleRange = MathHelper.ToRadians(35);
        }
    }
}
