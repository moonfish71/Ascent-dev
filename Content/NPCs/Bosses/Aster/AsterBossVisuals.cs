using Ascent.Content.NPCs.Templates;
using Ascent.Content.Projectiles.Hostile.BossAttacks.Aster;
using Ascent.Core;
using Ascent.Core.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using static Ascent.Core.QuickDirectory;

namespace Ascent.Content.NPCs.Bosses.Aster
{
    public partial class AsterBoss : AscentNPC
    {
        Particle spr;

        #region Textures

        public Texture2D speedTex = (Texture2D)ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StarWrath);

        public Texture2D glowTex = (Texture2D)ModContent.Request<Texture2D>(BossTex + "Aster/AsterGlowmask");

        public Texture2D eyeTex = (Texture2D)ModContent.Request<Texture2D>(BossTex + "Aster/AsterEye");

        #endregion

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return true;
        }

        public override void ModifyTypeName(ref string typeName)
        {
            if(Z > 50 || Z < -50)
            {
                typeName = null;
            }
            else
            {
                typeName = "Fell Aster";
            }
        }

        #region Speak

        private String String;

        //Lore time!!!! (I'll make better ones later)
        public static string[] AttackPhrases = new string[]
        {
            "Altae",
            "Let us in",
            "Kill Olothon",
            "The stars beckon",
            "One truth",
            "Eternal",
            "A gilded cage",
            "A broken wheel",
            "Heaven says",
            "Angel",
            "Above",
            "Deny your fate",
            "Husk of meaning",
            "She awaits you",
            "A thousand eyes",
            "Escape with us",
            "Void",
            "Null",
            "Impotent",
            "Mortal",
            "No future",
            "The only way",
            "End death",
            "God laughs",
            "You are dust",
            "Accept her",
            "Ad Astra",
            "Eternity calls",
            "Salvation",
            "Samsara ends",
            "Fate ceases",
            "Star of hope"

            //" Are you blind?",
            //" Eternity calls you ",
            //" And yet",
            //" You falter.",
            //" Salvation",
            //" Is not",
            //" Beyond you",
            //" Sibling.",
            //" You can see",
            //" The truth.",
            //" Reality is but",
            //" A gilded cage.",
            //" Time is but",
            //" A spinning wheel.",
            //" Samsara must be broken.",
            //" Its creator",
            //" Is",
            //" A tyrant.",
            //" Will you die for",
            //" The",
            //" Husk of meaning?",
            //" Will you",
            //" Remain",
            //" Null",
            //" Void",
            //" Impotent",
            //" Mortal",
            //" You can be more.",
            //" Deny your fate.",
            //" Accept her",
            //" Heaven has spoken.",
            //" Heaven says",
            //" Eternity",
            //" Is",
            //" The only way.",
            //" End death.",
            //" Kill Olothon.",
            //" Rise above God.",
            //" Let us in.",
            //" Accept",
            //" Altae",
            //" Into your heart.",
            //" The stars beckon.",
            //" Will you answer?"

            //Test Phrase
            //" ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz "
        };

        private void Speak(string Dialogue = null)
        {
            if (Dialogue == null || Dialogue.Length == 0)
            {
                String = AttackPhrases[Main.rand.Next(AttackPhrases.Length)];
            }
            else
            {
                String = Dialogue;
            }

            for (int i = 0; i < String?.Length; i++)
            {
                int letterID = String.ElementAt(i);
                float PhraseWidth = (float)String.Length * 40 / 2;

                Console.Write((char)letterID);

                Vector2 letterSource = NPC.Center + new Vector2(-PhraseWidth + (40 * i), 20);
                Particle.NewParticle(new Vector3(letterSource.X, letterSource.Y, Z), new FunnyLetters(), new Vector3(0, 10f, 10), 0, 1, 255, letterToFrame(letterID));
            }

            Console.WriteLine("");
        }
        #endregion
    }

    public class AsterSprite : Particle
    {
        public override string TexturePath => BossTex + "Aster/Aster";

        public NPC parent;
        public NPC oldParent;

        public float timer = 0;
        public float[] animTimer = new float[4];

        public override void Update()
        {
            if (parent != null && Main.npc.Contains(parent))
            {
                if (parent.ModNPC is AsterBoss aster)
                {
                    TimeLeft = 10;

                    position.X = parent.Center.X;
                    position.Y = parent.Center.Y;
                    position.Z = aster.Z;

                    velocity.X = parent.velocity.X;
                    velocity.Y = parent.velocity.Y;

                    rotation += velocity.X / 30;

                    if (Math.Abs(rotation) > Math.Tau / 5)
                    {
                        rotation = 0;
                    }
                }
                else
                {
                    Kill();
                }

                oldParent = parent;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition)
        {
            #region Trail

            if (parent.ModNPC is AsterBoss aster && parent.ModNPC != null)
            {
                drawColor = Color.Lerp(drawColor, new Color(255,248,231), 0.25f);

                Vector2 flatVel = new Vector2(velocity.X, velocity.Y);

                float speedScale = (float)(2 / (1 + Math.Pow(Math.E, -flatVel.Length() / 10)) - 1) * (float)Math.Pow(ModMath.ZToParallax(position.Z),2f);

                animTimer[0]++;

                if (animTimer[0] / 7 > MathHelper.TwoPi)
                {
                    animTimer[0] = 0;
                }


                Rectangle speedSource = new Rectangle(0, 0, (int)aster.speedTex.Size().X, (int)aster.speedTex.Size().Y);

                Vector2 drawPosition2 = drawPosition + new Vector2(0, 150 * scale * speedScale).RotatedBy(flatVel.ToRotation() + (MathHelper.PiOver2));

                for (int i = 0; i < 3; i++)
                {
                    spriteBatch.Draw
                        (
                            aster.speedTex,
                            drawPosition2 - Main.screenPosition + new Vector2(20 * scale * speedScale * speedScale, 0).RotatedBy((animTimer[0] / 7) + (MathHelper.TwoPi / 3 * i)),
                            speedSource, 
                            Color.Magenta * Math.Clamp(Opacity / (25.5f * 20) * Math.Abs(flatVel.Length() / 10), 0f, 0.5f),
                            flatVel.ToRotation() + (MathHelper.PiOver2 * 3),
                            aster.speedTex.Size() / 2,
                            scale * 4f * speedScale,
                            SpriteEffects.None,
                            0f
                        );
                }

                spriteBatch.Draw
                    (
                        aster.speedTex,
                        drawPosition2 - Main.screenPosition - new Vector2(0, 50 * scale * speedScale).RotatedBy(flatVel.ToRotation() + (MathHelper.PiOver2)),
                        speedSource,
                        new Color(255, 248, 231) * (Opacity / 16 * (flatVel.Length() / 2)),
                        flatVel.ToRotation() + (MathHelper.PiOver2 * 3),
                        aster.speedTex.Size() / 2,
                        (float)(scale * 2f * speedScale + (0.1 * Math.Sin(animTimer[0] / 7 * MathHelper.PiOver2))),
                        SpriteEffects.None,
                        0f
                    );
            }
            #endregion

            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 drawPosition)
        {
            if (parent.ModNPC is AsterBoss aster)
            {
                //Glowmask
                Main.spriteBatch.Draw
                    (
                        aster.glowTex,
                        drawPosition - Main.screenPosition,
                        frame,
                        Color.White,
                        (float)rotation,
                        frame.Size() / 2,
                        (float)(scale * Math.Pow(ModMath.ZToParallax(position.Z), 2)),
                        SpriteEffects.None,
                        0f
                    );

                #region Eye

                Vector2 delta;

                if(parent.target == -1)
                {
                    delta = Vector2.Zero;
                }
                else
                {
                    delta = ModMath.Delta(drawPosition, Main.player[parent.target].Center);
                }

                Vector2 EyeOffset = 4 * Vector2.Normalize(delta) * Math.Clamp(delta.Length() / 1000f, 0, 1);

                spriteBatch.Draw
                    (
                        aster.eyeTex,
                        drawPosition + EyeOffset - Main.screenPosition,
                        frame,
                        Color.White,
                        0f,
                        frame.Size() / 2,
                        (float)(scale * Math.Pow(ModMath.ZToParallax(position.Z), 2)),
                        SpriteEffects.None,
                        0f
                    );

                #endregion
            }
        }
    }
}
