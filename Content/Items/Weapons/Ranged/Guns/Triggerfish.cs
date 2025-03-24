using Ascent.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Ascent.Content.Items.Weapons.Ranged.Guns
{
    public class Triggerfish : ModItem
    {
        public override string Texture => QuickDirectory.GunTex + "Triggerfish";

        bool holdingOtherGun = true;

        public Projectile Gun;
        public Projectile OtherGun;

        public int delay = 0;

        public override void SetDefaults()
        {
            Item.Size = new Vector2(36, 34);
            Item.damage = 10;
            Item.crit = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 8;
            Item.useAnimation = 16;
            Item.UseSound = SoundID.Item11;
            Item.DamageType = DamageClass.Ranged;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override void HoldItem(Player player)
        {
            if(Gun == null)
            {
                int fishgun = Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<TriggerFish>(), Item.damage, Item.knockBack);
            }
        }

        public override void UpdateInventory(Player player)
        {
            delay--;

            if(delay <= 0)
            {
                Gun = null;
            }
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(0);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return !(player.itemAnimation < Item.useAnimation - 7);
        }
    }

    public class TriggerFish : ModProjectile
    {
        public override string Texture => $"{QuickDirectory.GunTex}TriggerfishRight";

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(24);
            Projectile.damage = 0;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        Player player = Main.player[0];

        public override void AI()
        {
            player = Main.player[Projectile.owner];
            Vector2 armCenter = player.Center - new Vector2((player.width / 4) + (6f * player.direction), (player.height / 16));

            if (Projectile.owner != Main.myPlayer | player.HeldItem.type != ModContent.ItemType<Triggerfish>())
            {
                Projectile.timeLeft = 0;
                Projectile.Kill();
            }
            else
            {
                if(player.HeldItem.ModItem is Triggerfish fish)
                {
                    fish.Gun = Main.projectile[Projectile.whoAmI];
                    fish.delay = 2;
                }

                Vector2 offset = new Vector2(16, 0);

                Projectile.rotation = Vector2.Add(Main.MouseWorld, -player.Center).ToRotation();

                Projectile.Center = armCenter + offset.RotatedBy(Projectile.rotation);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (player.direction == 1)
            {
                return true;
            }

            return true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if(player.direction == 1)
            {
                overPlayers.Add(index);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
    }
}
