using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Ascent.Configs;
using Ascent.Core.Systems.Particles.IKChain;
using Terraria.GameContent.ItemDropRules;

namespace Ascent.Core.Systems.Particles
{
    public class ParticleHandler
    {
        public static int ParticleCount = (int)ModContent.GetInstance<AscentServerConfig>().ParticleLimit;

        public static List<Particle> Particles;

        public static List<Particle> BGParticles;
        public static List<Particle> PLParticles;
        public static List<Particle> FGParticles;

        public static List<Particle> IKChains;

        public static void SetHooks()
        {
            Particles = new List<Particle>(ParticleCount);

            BGParticles = new List<Particle>(ParticleCount);
            PLParticles = new List<Particle>(ParticleCount);
            FGParticles = new List<Particle>(ParticleCount);

            IKChains = new List<Particle>(ParticleCount);

            On_Main.DrawBackGore += DrawBG;
            On_Main.DrawInfernoRings += DrawFG;
            On_Main.DrawProjectiles += DrawPL;

            On_Dust.UpdateDust += Update;
        }

        public static void Unload()
        {
            Particles = null;

            BGParticles = null;
            PLParticles = null;
            FGParticles = null;

            IKChains = null;

            On_Main.DrawBackGore -= DrawBG;
            On_Main.DrawInfernoRings -= DrawFG;

            On_Dust.UpdateDust -= Update;
        }

        private static void Update(On_Dust.orig_UpdateDust orig)
        {
            List<Particle> Dead = new List<Particle>();

            for (int i = 0; i < Particles?.Count; i++)
            {
                Particle particle = Particles[i];

                if (Main.hasFocus && !Main.gamePaused && particle != null)
                {
                    particle.TimeLeft--;

                    UpdateArrays(particle);

                    if (IKChains.Contains(particle))
                    {
                        particle.Update();
                    }
                    else if (!particle.ManualUpdate)
                    {
                        particle.position += particle.velocity;
                        particle.Update();
                    }

                    if (Math.Sign(particle.position.Z) != Math.Sign(particle.OldPos[0].Z))
                    {
                        CategorizeParticle(particle);
                    }

                    if (particle.TimeLeft <= 0 || particle.Die)
                    {
                        Dead.Add(particle);
                    }
                }
            }

            for (int i = 0; i < Dead?.Count; i++)
            {
                Particles.Remove(Dead[i]);
            }

            UpdateLists();

            orig();
        }

        private static void CategorizeParticle(Particle particle)
        {
            switch (Math.Sign(particle.position.Z))
            {
                case 0:
                    PLParticles.Add(particle);

                    if (BGParticles.Contains(particle)) { BGParticles.Remove(particle); }

                    if (FGParticles.Contains(particle)) { FGParticles.Remove(particle); }

                    break;

                case 1:
                    FGParticles.Add(particle);

                    if (BGParticles.Contains(particle)) { BGParticles.Remove(particle); }

                    if (PLParticles.Contains(particle)) { PLParticles.Remove(particle); }

                    break;

                case -1:
                    BGParticles.Add(particle);

                    if (PLParticles.Contains(particle)) { PLParticles.Remove(particle); }

                    if (FGParticles.Contains(particle)) { FGParticles.Remove(particle); }

                    break;
            }
        }

        private static void UpdateArrays(Particle particle)
        {
            for (int i = 0; i < particle.OldPos?.Length; i++)
            {
                if (i == 0)
                {
                    particle.OldPos[i] = particle.position;
                }

                else
                {
                    particle.OldPos[i] = particle.OldPos[i - 1];
                }
            }

            for (int i = 0; i < particle.OldVel?.Length; i++)
            {
                if (i == 0)
                {
                    particle.OldVel[i] = particle.velocity;
                }

                else
                {
                    particle.OldVel[i] = particle.OldVel[i - 1];
                }
            }

            for (int i = 0; i < particle.OldRot?.Length; i++)
            {
                if (i == 0)
                {
                    particle.OldRot[i] = particle.rotation;
                }

                else
                {
                    particle.OldRot[i] = particle.OldRot[i - 1];
                }
            }
        }

        #region Drawing

        private static void DrawBG(On_Main.orig_DrawBackGore orig, Main self)
        {
            orig(self);

            SpriteBatch spriteBatch = Main.spriteBatch;

            IComparer<Particle> comparer = new ParallaxComparer();

            BGParticles.Sort(comparer);

            DrawList(spriteBatch, BGParticles);
        }

        private static void DrawFG(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            orig(self);

            IComparer<Particle> comparer = new ParallaxComparer();

            FGParticles.Sort(comparer);

            DrawList(spriteBatch, FGParticles);
        }
        private static void DrawPL(On_Main.orig_DrawProjectiles orig, Main self)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            spriteBatch.Begin();

            DrawList(spriteBatch, PLParticles);
            spriteBatch.End();

            orig(self);
        }

        private static void DrawList(SpriteBatch spriteBatch, List<Particle> reference)
        {

            for (int i = 0; i < reference?.Count; i++)
            {
                Particle particle = reference[i];

                Vector2 pos2D = new Vector2(particle.position.X, particle.position.Y);

                Asset<Texture2D> texture = particle.texture;

                particle.FindFrame();

                #region Parallax Code

                Vector2 DrawPosition;

                float Parallax = ModMath.ZToParallax(particle.position.Z);

                if (Parallax < 0f)
                {
                    Parallax = 0f;
                }

                float scale3D = Parallax * Parallax;

                Vector2 Pos = Main.screenPosition + (Main.ScreenSize.ToVector2() / 2);
                Vector2 offset = (Pos - pos2D) * -(scale3D / 2 - 0.5f);

                DrawPosition = pos2D + offset;

                #endregion

                Color lightColour = Lighting.GetColor((int)(DrawPosition.X / 16f), (int)(DrawPosition.Y / 16f));
                Color frontColour = (pos2D.Y / 16f < Main.worldSurface) ? Main.ColorOfTheSkies : new Color(85, 85, 85);
                particle.drawColor = Color.Lerp(lightColour, frontColour, 10*ModMath.ZToParallax(particle.position.Z - 5500));

                bool PreDraw = particle.PreDraw(spriteBatch, DrawPosition);

                if (PreDraw)
                {
                    spriteBatch.Draw
                    (
                        texture.Value,
                        DrawPosition - Main.screenPosition,
                        particle.frame,
                        particle.drawColor * (particle.Opacity / 255f),
                        (float)particle.rotation,
                        particle.frame.Size() / 2,
                        particle.StretchScale * particle.scale * scale3D,
                        SpriteEffects.None,
                        0f
                    );
                }

                particle.PostDraw(spriteBatch, DrawPosition);
            }
        }

        #endregion

        private static void UpdateLists()
        {
            LookforDead(BGParticles);
            LookforDead(PLParticles);
            LookforDead(FGParticles);
            LookforDead(IKChains);
        }

        private static void LookforDead(List<Particle> reference)
        {
            for (int i = 0; i < reference?.Count; i++)
            {
                Particle particle = reference[i];

                if (!Particles.Contains(particle))
                {
                    reference.Remove(particle);
                }
            }
        }
    }

    class ParallaxComparer : IComparer<Particle>
    {
        public int Compare(Particle x, Particle y)
        {
            if (x == null || y == null) return 0;

            return x.position.Z.CompareTo(y.position.Z);
        }
    }
}
