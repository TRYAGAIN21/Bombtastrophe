using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace BombtastropheMod.Projectiles
{
	public class LingeringFlame : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lingering Flame");
			Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 12;
			Projectile.hostile = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 240;
		}

		private bool firstTick = true;
        public override void AI()
        {
            if (firstTick)
            {
                Projectile.frame = Main.rand.Next(3);
                firstTick = false;
            }

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 5f)
            {
                Projectile.ai[0] = 5f;
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X *= 0.97f;
                    if ((double)Projectile.velocity.X > -0.01 && (double)Projectile.velocity.X < 0.01)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.velocity.Y += 0.2f;
            }

            int num200 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100);
            Main.dust[num200].position.X -= 2f;
            Main.dust[num200].position.Y += 2f;
            Dust dust35 = Main.dust[num200];
            Dust dust2 = dust35;
            dust2.scale += (float)Main.rand.Next(50) * 0.01f;
            Main.dust[num200].noGravity = true;
            Main.dust[num200].velocity.Y -= 2f;
            if (Main.rand.Next(2) == 0)
            {
                int num201 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100);
                Main.dust[num201].position.X -= 2f;
                Main.dust[num201].position.Y += 2f;
                dust35 = Main.dust[num201];
                dust2 = dust35;
                dust2.scale += 0.3f + (float)Main.rand.Next(50) * 0.01f;
                Main.dust[num201].noGravity = true;
                dust35 = Main.dust[num201];
                dust2 = dust35;
                dust2.velocity *= 0.1f;
            }
            if ((double)Projectile.velocity.Y < 0.25 && (double)Projectile.velocity.Y > 0.15)
                Projectile.velocity.X *= 0.8f;

            Projectile.rotation = (0f - Projectile.velocity.X) * 0.05f;

            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = oldVelocity.X * -0.1f;
            return false;
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }


        public override bool PreDraw(ref Color lightColor)
		{
			int frames = Main.projFrames[Projectile.type];
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle textureRect = new Rectangle(0, Projectile.frame * (texture.Height / frames), (texture.Width), (texture.Height / frames));
			Vector2 textureVect = new Vector2((float)texture.Width / 2, (float)(texture.Height / (2 * frames)));
			Main.spriteBatch.Draw(texture, new Vector2(Projectile.Center.X, Projectile.Bottom.Y - (Projectile.height / 2)) - Main.screenPosition, new Rectangle?(textureRect), Color.White * (1f - ((float)(Projectile.alpha) / 255f)), Projectile.rotation, textureVect, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
			return false;
		}
	}
}
