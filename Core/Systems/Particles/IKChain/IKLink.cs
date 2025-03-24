using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Steamworks;

namespace Ascent.Core.Systems.Particles.IKChain
{
    public abstract class IKLink: Particle
    {
        public IKLink parent;
        public IKLink child;

        public IKChain chain;

        public Vector2 target;
        public Vector2 front;
        public Vector2 back;

        public int ChainPos;

        public float length = 1f;
        public double AngleRange = -1f;
        public double LinkAngle = 0f;

        public override void Update()
        {
            Vector2 dir = front - back;
        }

        public void Move(bool returning = false)
        {
            target = GetTarget(returning);

            Vector2 dir;
            Vector2 pos2D;

            if (returning)
            {
                back = target;
                dir = front - back;
                front = back + (Vector2.Normalize(dir) * length);

                pos2D = front + (dir / 2);
            }
            else
            {
                front = target;
                dir = back - front;
                back = front + (Vector2.Normalize(dir) * length);
            }

            dir = back - front;

            pos2D = front + (dir / 2);

            position.X = pos2D.X;
            position.Y = pos2D.Y;

            rotation = dir.ToRotation();
        }

        private Vector2 GetTarget(bool returning = false)
        {
            if (returning)
            {
                if(parent != null)
                {
                    return parent.front;
                }

                return new Vector2(chain.position.X, chain.position.Y);
            }

            if(child != null)
            {
                return child.back;
            }

            return chain.target;
        }
    }
}
