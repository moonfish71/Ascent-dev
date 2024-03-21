using Ascent.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Ascent.Core.ModWorlds;
using Ascent.Content.Events.Starfall;

namespace Ascent.Content.Items.Summoning
{
    public class SFSummon : ModItem
    {
        public override string Texture => QuickDirectory.PlaceHolderTx;

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.Size = new Vector2(16);
            Item.useTime = 20;
            Item.useAnimation = Item.useTime;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override bool? UseItem(Player player)
        {
            if (EventWorld.StarfallUp)
            {
                StarFall.StopEvent();
            }
            else
            {
                StarFall.StartEvent();
                StarFall.CentralCoords = player.Center;
            }

            return true;
        }
    }
}
