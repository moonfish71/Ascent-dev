using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Ascent.Core.Systems.Particles.IKChain
{
    public abstract class IKChain : Particle
    {
        struct LinkTemplate
        {
            IKLink type;
            float length;
        }

        public List<IKLink> links;

        public List<IKLink> compositeLinks;

        public bool LockToBase = false;

        public Vector2 target = Vector2.Zero;

        public int length;
        public double AngleRange = -1;
        public double LinkAngle = 0;

        public IKChain()
        {
            ManualUpdate = true;
            links = new List<IKLink>();
            compositeLinks = new List<IKLink>();
        }

        public virtual void SetUpCompositeLinks() { }

        public override void Update()
        {
            AI();

            if (LockToBase)
            {
                for (int i = 0; i < 10; i++)
                {
                    SetOut();
                    Return();
                }
            }
            else
            {
                SetOut();
            }

            foreach (IKLink link in links)
            {
                if (link != null)
                {
                    link.Update();

                    link.position.Z = position.Z;
                }
            }

            if (TimeLeft < 1)
            {
                foreach (IKLink link in links)
                {
                    if (link != null) { link.Kill(); }
                }

                Kill();
            }
        }

        public virtual void AI() { }

        #region Link Behaviors

        public void SetOut() 
        {
            for (int j = links.Count - 1; j >= 0; j--)
            {
                IKLink link = links[j];

                if (link != null)
                {
                    link.Move(false);
                }
            }
        }
        public void Return()
        {
            for (int j = 0; j < links.Count - 1; j++)
            {
                IKLink link = links[j];

                if (link != null)
                {
                    link.Move(true);
                }
            }
        }
        #endregion

        #region Spawning
        public static IKChain NewChain(Vector2 center2D, Vector2 target, int length, IKChain chain)
        {
            if (ParticleHandler.Particles.Count >= ParticleHandler.Particles.Capacity)
            {
                return null;
            }

            IKChain newChain = (IKChain)Activator.CreateInstance(chain.GetType());

            newChain.position.X = center2D.X;
            newChain.position.Y = center2D.Y;

            newChain.target = target;
            newChain.length = length;

            newChain.SetUpCompositeLinks();

            ParticleHandler.Particles.Add(newChain);

            ParticleHandler.IKChains.Add(newChain);

            newChain.ID = ParticleHandler.Particles.IndexOf(newChain);

            newChain.SetupLinks(newChain);

            return newChain;
        }

        private void SetupLinks(IKChain chain)
        {
            int runningI = 0;

            for (int i = 0; i < length; i++) 
            {
                Vector2 pos2D = new Vector2(position.X, position.Y);

                IKLink newLink = (IKLink)Particle.NewParticle(pos2D, compositeLinks[runningI], Vector2.Zero);

                newLink.ManualUpdate = true;

                newLink.ChainPos = i;
                newLink.TimeLeft = int.MaxValue;
                newLink.front = pos2D;

                newLink.chain = chain;

                CustomSpawnBehavior(chain, i);

                links.Add(newLink);

                runningI++;
                
                if (runningI > compositeLinks?.Count - 1)
                {
                    runningI = 0;
                }
            }

            IKLink nullLink = null;
            links.Add(nullLink);

            for (int i = 0; i < links.Count - 1; i++)
            {
                IKLink link = links[i];

                if (i == 0) 
                {
                    link.parent = null;
                    link.child = links[1];
                }
                else
                {
                    link.parent = links[i - 1];
                    link.child = links[i + 1];
                }
            }
        }

        public virtual void CustomSpawnBehavior(IKChain chain, int i) { }
        #endregion

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition)
        {
            return false;
        }
    }
}
