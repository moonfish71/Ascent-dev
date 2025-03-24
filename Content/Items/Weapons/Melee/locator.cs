using Ascent.Content.Events.DarkNight;
using Ascent.Content.Particles.ChainsAndLinks;
using Ascent.Content.Particles.ChainsAndLinks.Worms;
using Ascent.Core.ModPlayers;
using Ascent.Core.ModWorlds;
using Ascent.Core.Systems.Particles;
using Ascent.Core.Systems.Particles.IKChain;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Ascent.Core.QuickDirectory;

namespace Ascent.Content.Items.Weapons.Melee
{
    public class locator : ModItem
    {
        public override string Texture => MeleeTex + Name;

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 100;
            Item.autoReuse = true;
        }

        public override bool? UseItem(Player player)
        {
            ScreenMovementPlayer shakePlayer = player.GetModPlayer<ScreenMovementPlayer>();

            shakePlayer.ScreenShakeStrength = 1f;

            return base.UseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            position = player.Center;

            IKChain.NewChain(Main.MouseWorld, player.Center, 50, new ApolloSkeleton());

            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}