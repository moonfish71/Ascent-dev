using Ascent.Content.NPCs.Templates;
using Humanizer.Localisation;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using static Ascent.Core.QuickDirectory;

namespace Ascent.Content.NPCs.Events.Starfall
{
    public class ThrallL : AscentNPC
    {
        public override string Texture => PlaceHolderTx;
        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 64;
            NPC.lifeMax = 150;
            NPC.aiStyle = -1;
            NPC.damage = 60;
            NPC.knockBackResist = 0;
            NPC.defense = 1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            string Desc = "A thrall of the Astral Gestalt. This thrall has grown drastically in size and strength, letting it smash through defenses and throw its smaller bretheren, paving the way forward for conversion.";

            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Visuals.Meteor,
                new FlavorTextBestiaryInfoElement(Desc)
            }); ;
        }
    }
}
