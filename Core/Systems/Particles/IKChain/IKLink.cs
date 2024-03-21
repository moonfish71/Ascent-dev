using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Ascent.Core.Systems.Particles.IKChain
{
    public abstract class IKLink: Particle
    {
        public static IKLink parent;
        public static IKLink child;

        public static IKChain chain;

        public static Vector2 target;
        public Vector2 front;
        public Vector2 back;

        public float length = 1f;

        public override void OnSpawn()
        {
            front = Vector2.Zero;
            back = Vector2.Zero;
        }

        public void Move(bool returning = false)
        {
            target = GetTarget(returning);

            if (returning) 
            {
                if(target == null)
                {
                    return;
                }

                back = target;

                front = front.DirectionFrom(back) * length;

                return;
            }

            front = target;

            back = front.DirectionFrom(back) * length;
        }

        public static Vector2 GetTarget(bool returning = false)
        {
            if (returning)
            {
                if (parent == null)
                {
                    if (chain.based == null)
                    {
                        return Vector2.Zero;
                    }

                    return chain.based;
                }

                return parent.front;
            }

            if (child == null)
            {
                return chain.target;
            }

            return child.back;
        }
    }
}
