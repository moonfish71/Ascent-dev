using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Ascent.Core.ModPlayers
{
    public class ScreenMovementPlayer : ModPlayer
    {
        public float ScreenShakeStrength;
        public Vector2 ScreenCenter;
        public float ScreenScale;
        
        private Vector2 OldScreenCenter;
        private Vector2 BaseScreenPos;


        public bool ScreenPosModified;

        public float OutCounter;
        public float InCounter;

        public override void Initialize()
        {
            ScreenShakeStrength = 0;
            ScreenCenter = Main.screenPosition + (Main.ScreenSize.ToVector2() / 2);
            ScreenPosModified = false;

            OutCounter = 0;
            InCounter = 0;
        }

        public override void ResetEffects()
        {
            ScreenPosModified = false;
        }

        public override void ModifyScreenPosition()
        {
            Vector2 BaseScreenPos = Main.screenPosition;

            if (!ScreenPosModified)
            {
                if (OutCounter < 40f)
                {
                    ScreenCenter = Vector2.Lerp(OldScreenCenter, BaseScreenPos + (Main.ScreenSize.ToVector2() / 2), ModMath.easeInOutQuad(OutCounter / 40f));
                    OutCounter++;

                    Main.screenPosition = ScreenCenter - (Main.ScreenSize.ToVector2() / 2);
                } 
                InCounter = 0;
            }
            else
            {
                OutCounter = 0;
                OldScreenCenter = ScreenCenter;

                Main.screenPosition = ScreenCenter - (Main.ScreenSize.ToVector2() / 2);
            }

            if (ScreenShakeStrength > 0)
            {
                Main.screenPosition += Main.rand.NextVector2Circular(ScreenShakeStrength, ScreenShakeStrength);
                ScreenShakeStrength = Math.Clamp(ScreenShakeStrength, 0, 400);

                ScreenShakeStrength -= 1.95f;
            }
        }

        public void MoveScreen(Vector2 MoveTarget, float Duration)
        {
            ScreenPosModified = true;

            if ((int)InCounter == 0)
            {
                BaseScreenPos = Main.screenPosition;
            }

            float Index = InCounter / Duration;

            ScreenCenter = Vector2.Lerp(BaseScreenPos + (Main.ScreenSize.ToVector2() / 2), MoveTarget, ModMath.easeInOutQuad(Index));

            InCounter++;
            InCounter = Math.Clamp(InCounter, 0, Duration);
        }
    }
}
