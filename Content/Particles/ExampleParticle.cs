using Ascent.Core;
using Ascent.Core.Systems.Particles;
using System;

namespace Ascent.Content.Particles
{
    public class ExampleParticle : Particle
    {
        float timer = 0;
        public override void SetDefaults()
        {
            TimeLeft = ModMath.SecondsToTicks(1);
        }

        public override void OnSpawn()
        {

        }

        public override void Update()
        {
            timer++;
            velocity *= .95f;
            Opacity -= 7;
            //position.Z = (float)(500 * Math.Sin(timer / 15));
        }
    }
}
