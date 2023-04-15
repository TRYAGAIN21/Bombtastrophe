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
	public class DivineBeing : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Divine Being");
		}

		public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 22;
			Projectile.hostile = false;
			Projectile.friendly = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = (int)Math.Pow(777.0, 2.0);
			Projectile.alpha = 50;
			Projectile.scale = 2f;
		}

		private Vector2 screenPlacement;
		private Vector2 playerPin;
		private bool firstTick = true;
        public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			BombtastrophePlayer modPlayer = player.GetModPlayer<BombtastrophePlayer>();
			if (firstTick)
			{
				screenPlacement.X = 128;
				screenPlacement.Y = -600;
				playerPin = player.Center;
				firstTick = false;
			}

			if (modPlayer.angelTimer > 3900)
				screenPlacement.Y = MathHelper.Lerp(0f, -600f, (float)(modPlayer.angelTimer - 3900) / 300f) * ((float)(modPlayer.angelTimer - 3900) / 300f);
			else
            {
				if (modPlayer.angelTimer <= 600)
					screenPlacement.Y = MathHelper.Lerp(-600f, 0f, (float)(modPlayer.angelTimer - 300) / 300f) * ((float)(300 - (modPlayer.angelTimer - 300)) / 300f);
				player.velocity = Vector2.Zero;
				player.Center = playerPin;
				player.direction = 1;
			}

			if (modPlayer.angelTimer == 720)
				Main.NewText("Hello, " + player.name + ".", 255, 255, 0);
			if (modPlayer.angelTimer == 900)
				Main.NewText("Do not fear me.", 255, 255, 0);
			if (modPlayer.angelTimer == 1020)
				Main.NewText("I am here to help.", 255, 255, 0);
			if (modPlayer.angelTimer == 1200)
				Main.NewText("For I believe you are worthy of a very special thing...", 255, 255, 0);
			if (modPlayer.angelTimer == 1500)
				Main.NewText("You sought to fulfill the dreams of over 1000 individuals of bombkind.", 255, 255, 0);
			if (modPlayer.angelTimer == 1800)
				Main.NewText("You sought to uncover all of the ancient messages that our ancestors left buried.", 255, 255, 0);
			if (modPlayer.angelTimer == 2100)
				Main.NewText("And succeeded at both of those.", 255, 255, 0);
			if (modPlayer.angelTimer == 2520)
				Main.NewText("So, I, the deity of bomb heaven, present a code that has been passed down for thousands of generations.", 255, 255, 0);
			if (modPlayer.angelTimer == 2820)
				Main.NewText("One that only the most worthy of individuals have been told.", 255, 255, 0);
			if (modPlayer.angelTimer == 3120)
				Main.NewText("[c/00FF00:Ultramerge].", 255, 255, 0);
			if (modPlayer.angelTimer == 3420)
				Main.NewText("I must go now.", 255, 255, 0);
			if (modPlayer.angelTimer == 3600)
				Main.NewText("My heavens need me.", 255, 255, 0);

			if (modPlayer.angelTimer > 0)
			{
				Projectile.velocity = (player.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * -1f;
				Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Projectile.velocity.ToRotation(), 0.72f);
				Projectile.velocity = Vector2.Zero;
			}
			if (modPlayer.angelTimer >= 4199)
				Projectile.active = false;
			Projectile.position.X = player.Center.X + screenPlacement.X - (Projectile.width / 2);
			Projectile.position.Y = player.Center.Y + screenPlacement.Y - (Projectile.height / 2);
		}

        public override bool PreDraw(ref Color lightColor)
		{
			int frames = Main.projFrames[Projectile.type];
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Vector2 textureVect = new Vector2((float)texture.Width / 2, (float)(texture.Height / (2 * frames)));
			Main.spriteBatch.Draw(texture, new Vector2(Projectile.Center.X, Projectile.Bottom.Y - (Projectile.height / 2)) - Main.screenPosition, null, Color.White * (1f - ((float)(Projectile.alpha) / 255f)), Projectile.rotation * -Projectile.spriteDirection, textureVect, Projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
	}
}
