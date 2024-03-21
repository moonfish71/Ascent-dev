using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ascent.Core
{
    public static class QuickDirectory
    {
        //The indents are only there for more clear organization, just like this entire class. If you don't like it, too bad :trole:

        public static string Textures =     "Ascent/Assets/Textures/";

        public static string PlaceHolderTx =        Textures + "placeholder";

        public static string ProjectileTex =            Textures + "Projectiles/";

        public static string HostileProjTex =               ProjectileTex + "Hostile/";

        public static string BossProjTex =                      HostileProjTex + "BossAttacks/";

        public static string AsterProjTex =                         BossProjTex + "Aster/";


        public static string WeaponTex =            Textures + "Items/Weapons/";
        public static string MeleeTex =                         WeaponTex + "Melee/";
        public static string RangedTex =                        WeaponTex + "Ranged/";
        public static string GunTex =                               RangedTex + "Guns/";

        public static string NPCTex =               Textures + "NPCs/";
        public static string BossTex =                          NPCTex + "Bosses/";

        public static string ParticleTex =          Textures + "Particles/";
    }
}
