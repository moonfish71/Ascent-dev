using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using static Ascent.Core.QuickDirectory;

namespace Ascent.Core.Systems.Particles
{
    public abstract class Particle
    {
        public virtual string TexturePath => PlaceHolderTx;
        //public virtual string TexturePath => QuickDirectory.Textures + "ApolloBody";

        public Particle()
        {
            SetDefaults();
            SetPrivateDefaults();
        }

        public virtual void SetDefaults() { }

        private void SetPrivateDefaults()
        {
            OldPos = new Vector3[10];
            OldVel = new Vector3[10];
            OldRot = new double[10];

            texture = ModContent.Request<Texture2D>(TexturePath);

            frame = new Rectangle(0, 0, (int)(texture.Width() / frameCount.X), (int)(texture.Height() / frameCount.Y));
        }


        public int ID;

        public bool Die = false;


        public Vector3 position = Vector3.Zero;

        public Vector3 velocity;

        public double rotation = 0;


        public Vector3[] OldPos;

        public Vector3[] OldVel;

        public double[] OldRot;


        public float scale = 1;

        public Vector2 StretchScale = Vector2.One;

        public float Opacity = 255;

        public Color Color;

        public Color drawColor;

        public float TimeLeft = ModMath.SecondsToTicks(10);

        public virtual void Update() { }

        public bool ManualUpdate = false;

        public Asset<Texture2D> texture;

        public Rectangle frame = new Rectangle(0, 0, 16, 16);

        public Vector2 frameCount = new Vector2(1);


        public virtual void FindFrame() { }


        public virtual bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition) { return true; }

        public virtual void PostDraw(SpriteBatch spriteBatch, Vector2 drawPosition) { }


        public virtual void OnSpawn() { }

        public virtual void OnDeath() { }

        public static Particle NewParticle(Vector2 position2D, Particle particle, Vector2 velcoity2D, double rotation = 0, float scale = 1, float Opacity = 255)
        {
            if (ParticleHandler.Particles.Count >= ParticleHandler.Particles.Capacity)
            {
                return null;
            }

            Particle newParticle = (Particle)Activator.CreateInstance(particle.GetType());

            newParticle.position.X = position2D.X;
            newParticle.position.Y = position2D.Y;

            newParticle.velocity.X = velcoity2D.X;
            newParticle.velocity.Y = velcoity2D.Y;

            newParticle.rotation = rotation;
            newParticle.scale = scale;
            newParticle.Opacity = Opacity;

            ParticleHandler.Particles.Add(newParticle);

            ParticleHandler.PLParticles.Add(newParticle);

            newParticle.ID = ParticleHandler.Particles.IndexOf(newParticle);

            newParticle.OnSpawn();

            return newParticle;
        }

        public static Particle NewParticle(Vector3 position, Particle particle, Vector3 velcoity, double rotation = 0, float scale = 1, float Opacity = 255)
        {
            if (ParticleHandler.Particles.Count >= ParticleHandler.Particles.Capacity)
            {
                return null;
            }

            Particle newParticle = (Particle)Activator.CreateInstance(particle.GetType());

            newParticle.position = position;

            newParticle.velocity = velcoity;

            newParticle.rotation = rotation;
            newParticle.scale = scale;
            newParticle.Opacity = Opacity;

            ParticleHandler.Particles.Add(newParticle);

            if (position.Z > 0)
            {
                ParticleHandler.FGParticles.Add(newParticle);
            }

            if (position.Z < 0)
            {
                ParticleHandler.BGParticles.Add(newParticle);
            }

            if (position.Z == 0)
            {
                ParticleHandler.PLParticles.Add(newParticle);
            }

            newParticle.ID = ParticleHandler.Particles.IndexOf(newParticle);

            newParticle.OnSpawn();

            return newParticle;
        }

        public void Kill()
        {
            TimeLeft = 0;
            Die = true;
        }
    }
}
