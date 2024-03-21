using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Ascent.Core.Systems.Particles.IKChain
{
    public abstract class IKChain
    {
        public static List<IKLink> links;

        public static List<IKLink> compositeLinks;

        public int ID;

        public Vector2 based;
        public bool LockToBase = false;

        public Vector2 target;

        public int length;

        public IKChain()
        {
            SetDefaults();
        }

        public virtual void SetDefaults()
        {

        }

        public void Update()
        {
            if (LockToBase)
            {
                for (int i = 0; i < 5; i++)
                {
                    SetOut();
                    Return();
                }

                SetRotation();

                return;
            }

            SetOut();

            SetRotation();
        }

        #region Link Behaviors
        public void SetOut() 
        {
            for(int j = (int)(links?.Count); j > 0; j++)
            {
                IKLink link = links[j];

                link.Move(false);
            }
        }
        public void Return()
        {
            for (int j = 0; j < links?.Count; j++)
            {
                IKLink link = links[j];

                link.Move(true);
            }
        }

        public void SetRotation()
        {
            for (int j = 0; j < links?.Count; j++)
            {
                IKLink link = links[j];

                Vector2 delta = link.back - link.front;

                link.rotation = delta.ToRotation();
            }
        }
        #endregion

        #region Spawning
        public static IKChain NewChain(Vector2 based, Vector2 target, int length, IKChain chain)
        {
            if (ParticleHandler.Particles.Count >= ParticleHandler.Particles.Capacity)
            {
                return null;
            }

            IKChain newChain = (IKChain)Activator.CreateInstance(chain.GetType());

            newChain.based = based;
            newChain.target = target;
            newChain.length = length;

            newChain.SetupLinks();

            ChainManager.chains.Add(newChain);

            newChain.ID = ChainManager.chains.IndexOf(newChain);

            return newChain;
        }

        public void SetupLinks()
        {
            int runningI = 0;
            for (int i = 0; i < length; i++) 
            {
                IKLink newLink = (IKLink)Particle.NewParticle(based + new Vector2(compositeLinks[runningI].length * i, 0), compositeLinks[runningI], Vector2.Zero);

                newLink.ChainPos = i;

                links.Add(newLink);

                runningI++;
            }
        }
        #endregion
    }

    public class ChainManager
    {
        public static List<IKChain> chains;

        public static void Update()
        {
            foreach (IKChain chain in chains)
            {
                chain.Update();
            }
        }
    }
}
