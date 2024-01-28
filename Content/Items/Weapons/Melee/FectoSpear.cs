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
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Terraria.DataStructures;
using Mono.Cecil;

namespace Ascent.Content.Items.Weapons.Melee
{
    public class FectoSpear : ModItem
    {
        public override string Texture => QuickDirectory.MeleeTex + "FectoSpear";

        int swing = 0;

        public override void SetDefaults()
        {
            Item.Size = new Vector2(64);
            Item.damage = 50;
            Item.crit = 4;
            Item.knockBack = 6;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<FectoSpearProj>();
            Item.shootSpeed = 1;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            base.UseStyle(player, heldItemFrame);
        }

        public override bool? UseItem(Player player)
        {
            swing++;

            if (swing > 1) swing = 0;

            return base.UseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback);
            Projectile spear = Main.projectile[proj];

            if (spear.ModProjectile is FectoSpearProj AHWA)
            {
                AHWA.Swing = swing;
                AHWA.duration = Item.useTime;
            }

            return false;
        }
    }

    public class FectoSpearProj : ModProjectile
    {
        public override string Texture => $"{QuickDirectory.MeleeTex}FectoSpearProj";

        public Texture2D tex = (Texture2D)ModContent.Request<Texture2D>($"{QuickDirectory.MeleeTex}FectoSpearProj");

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(32);
            Projectile.damage = 140;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        float length = 100;
        float lengthMod = 150;
        float rotation = 0;
        public float duration = 60;

        int defDamage = 0;

        float MouseRotation;
        float SwingMagnitude = (float)Math.PI;

        public int Swing = 0;

        bool SetInitCons;
        private int InMouseDir;

        Vector2 armCenter;

        float timer = 0;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            armCenter = player.Center - new Vector2((player.width/2)+5f,(player.height/2) - 5f);

            if (!SetInitCons)
            {
                Vector2 MouseDelta = Main.MouseWorld - player.Center;

                Projectile.timeLeft = (int)duration;

                MouseRotation = (float)(MouseDelta.ToRotation() + (Math.Tau/4));
                InMouseDir = -Math.Sign(MouseDelta.X);
                SetInitCons = true;

                defDamage = Projectile.damage;

                switch (Swing)
                {
                    case 0:
                        SwingMagnitude = (float)Math.PI * 1.25f;
                        lengthMod = 95;
                        break;
                    case 1:
                        SwingMagnitude = -(float)Math.PI * 1.8f;
                        lengthMod = 150;
                        break;
                }
            }

            timer++;

            //rotation += 2*(float)(1 / Math.Tau)/3.2f;

            rotation = MathHelper.Lerp((float)(MouseRotation + InMouseDir * SwingMagnitude / 2), (float)(MouseRotation - InMouseDir * SwingMagnitude / 2), ModMath.easeInOutBack(timer / duration));

            float DLength = 1;

            if (timer < 0.1f * duration)
            {
                DLength = ModMath.QuadEase(2 * timer / duration);
                Projectile.damage = 0;
            }
            else if(timer > 0.9f * duration)
            {
                DLength = ModMath.QuadEase(2 * (timer - (duration / 2)) / duration);
                Projectile.damage = 0;
            }
            else
            {
                Projectile.damage = defDamage;
            }

            DLength *= lengthMod;

            length = 50 + DLength;

            Vector2 spearPath = armCenter + new Vector2(0,-length).RotatedBy(rotation);

            Projectile.position = spearPath;
            Projectile.rotation = armCenter.AngleTo(Projectile.position) + (float)(Math.PI/4);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Draw
            (
                tex,
                Projectile.Center - Main.screenPosition, 
                new Rectangle(0,0,tex.Width,tex.Height), 
                lightColor, Projectile.rotation, 
                new Vector2(tex.Width, 0), 
                Projectile.scale, 
                SpriteEffects.None, 
                0f
            );

            return false;
        }

        bool Tipper = false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Tipper = false;

            Player player = Main.player[Projectile.owner];
            Vector2 playerCenter = player.RotatedRelativePoint(armCenter, reverseRotation: false, addGfxOffY: false);
            Vector2 normalVel = new Vector2(0, -length).RotatedBy(rotation);

            Vector2 start = playerCenter;
            Vector2 end = start + normalVel * Projectile.scale;
            float collisionPoint = 0f;

            Tipper = projHitbox.Intersects(targetHitbox);

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 10, ref collisionPoint) || Tipper;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Tipper)
            {
                modifiers.FinalDamage *= 2f;
                modifiers.SetCrit();
            }
        }

        private void UpdatePlayer(Player player)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 diff = Main.MouseWorld - player.Center;
                diff.Normalize();
                Projectile.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
                Projectile.netUpdate = true;
            }
            int dir = Projectile.direction;
            player.ChangeDir(dir);
            player.heldProj = Projectile.whoAmI;
            player.itemRotation = Projectile.rotation;
        }
    }
}
