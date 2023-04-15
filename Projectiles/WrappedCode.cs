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
	public class WrappedCode : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wrapped Code");
		}

		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 26;
			Projectile.hostile = false;
			Projectile.friendly = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 2;
		}

        public override void AI()
		{
			Projectile.timeLeft = 2;
		}

        public override bool PreDraw(ref Color lightColor)
		{
			int frames = Main.projFrames[Projectile.type];
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Vector2 textureVect = new Vector2((float)texture.Width / 2, (float)(texture.Height / (2 * frames)));
			Rectangle textureRect = new Rectangle(0, 0, (texture.Width), (texture.Height / Main.projFrames[Projectile.type]));
			Main.spriteBatch.Draw(texture, new Vector2(Projectile.Center.X, Projectile.Bottom.Y - (texture.Height / Main.projFrames[Projectile.type])) - Main.screenPosition, null, Color.White * (1f - ((float)(Projectile.alpha) / 255f)), Projectile.rotation, textureVect, Projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
	}
}
