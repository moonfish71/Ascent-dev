using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;

namespace Ascent.Content.NPCs.Bosses.Aster
{
    public class AsterBossBar: ModBossBar
    {
        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
        {
            info.showText = true;
            return base.ModifyInfo(ref info, ref life, ref lifeMax, ref shield, ref shieldMax);
        }
    }
}
