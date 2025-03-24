using Ascent.Core.Systems.Particles.IKChain;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Ascent.Content.Particles.ChainsAndLinks
{
    public class ExampleChain : IKChain
    {

        public override void SetDefaults()
        {
            compositeLinks = new List<IKLink> { new ExampleLink1()};
            TimeLeft = 2400;

            LockToBase = true;
        }

        public override void AI()
        {
            target = Main.LocalPlayer.Center;
            target.Y -= 1000;

            SetOut();
            Return();
            SetOut();

            position.X = Main.LocalPlayer.Center.X;
            position.Y = Main.LocalPlayer.Center.Y;

            //position.Y++;

            target = Main.MouseWorld;
        }
    }

    public class ExampleLink1 : IKLink
    {
        public override void SetDefaults()
        {
            length = 100;
        }
    }
    public class ExampleLink2 : IKLink
    {
        public override void SetDefaults()
        {
            length = 25;
        }
    }
}
