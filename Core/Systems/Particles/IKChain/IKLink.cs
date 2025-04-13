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

        public Vector2 target = Vector2.Zero;
        public Vector2 front = Vector2.Zero;
        public Vector2 back = Vector2.Zero;

        public int ChainPos;

        public float length = 1f;
        public double AngleRange = -1f;
        public double LinkAngle = 0f;

        public override void SetDefaults()
        {
            ManualUpdate = true;
        }

        public override void Update()
        {
            Vector2 dir = front - back;
        }

        public void Move(bool returning = false)
        {
            target = GetTarget(returning);

            Vector2 Dir;
            Vector2 Pos2D;

            if (returning)
            {
                back = target;
                Dir = front - back;

                if (parent != null && AngleRange > 0)
                {
                    Vector2 ParentDir = parent.front - parent.back;

                    LinkAngle = Dir.ToRotation() - ParentDir.ToRotation();

                    if (LinkAngle > Math.PI)
                    {
                        LinkAngle -= 2 * Math.PI;
                    }
                    else if (LinkAngle <= -Math.PI)
                    {
                        LinkAngle += 2 * Math.PI;
                    }

                    LinkAngle = Math.Clamp(LinkAngle, -AngleRange, AngleRange);

                    Dir = Dir.RotatedBy(-Dir.ToRotation() + ParentDir.ToRotation() + LinkAngle);

                }

                front = back + (Vector2.Normalize(Dir) * length);
            }
            else
            {

                front = target;
                Dir = back - front;

                if (child != null && AngleRange > 0)
                {
                    Vector2 ChildDir = child.back - child.front;

                    LinkAngle = Dir.ToRotation() - ChildDir.ToRotation();

                    if (LinkAngle > Math.PI)
                    {
                        LinkAngle -= 2 * Math.PI;
                    }
                    else if (LinkAngle <= -Math.PI)
                    {
                        LinkAngle += 2 * Math.PI;
                    }

                    LinkAngle = Math.Clamp(LinkAngle, -AngleRange, AngleRange);

                    Dir = Dir.RotatedBy(-Dir.ToRotation() + ChildDir.ToRotation() + LinkAngle);

                }

                back = front + (Vector2.Normalize(Dir) * length);
            }

            Dir = back - front;

            Pos2D = front + (Dir / 2);

            position.X = Pos2D.X;
            position.Y = Pos2D.Y;

            rotation = Dir.ToRotation();
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
