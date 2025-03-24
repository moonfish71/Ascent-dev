using Ascent.Core.Systems;
using Ascent.Core.Systems.Particles;
using rail;
using System.Collections.Generic;
using Terraria.ModLoader;
using Ascent.Configs;
using System.Diagnostics;
using System;

namespace Ascent
{
    public class Ascent : Mod
	{
        public static Mod Instance;

        public Ascent() 
        {
            Instance = this;
        }

        public override void Load()
        {
            ParticleHandler.SetHooks();
            HordeManager.Load();
        }

        public override void Unload()
        {
            ParticleHandler.Unload();
            HordeManager.Unload();
        }

        public static void FriendInsideMe(int Friends = 1)
        {
            //Friend Inside Me.

            for (int i = 0; i < Friends; i++)
            {
                Console.WriteLine("Friend Inside Me");

                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://www.youtube.com/watch?v=C0PgRC2BF6c",
                    UseShellExecute = true
                });
            }

            Console.WriteLine("");
            Console.WriteLine("AND AS THE YEARS GO BY,");
            Console.WriteLine("I WILL NEVER DIE");
            Console.WriteLine("");
        }
    }
}