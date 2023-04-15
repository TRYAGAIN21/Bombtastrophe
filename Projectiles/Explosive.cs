using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BombtastropheMod.Projectiles
{
	public abstract class Explosive : ModProjectile
	{
		public string[] modifiers = new string[2];
		public bool destructive;
		public bool exploding;
		private int presentFrame;

		public virtual Color NoneColor() => Color.White;
		public virtual float ExplosionSize() => 16f;
		public virtual int ExplosionDamage() => 40;
		public virtual int ClusterProj() => ModContent.ProjectileType<AnnihilationPellet>();

		public Color ColorTint()
		{
			Color[] colors = new Color[2];
			colors[0] = ExplosiveUtils.GetModifierColor(modifiers[0]);
			colors[1] = ExplosiveUtils.GetModifierColor(modifiers[1]);
			bool[] modifierExists = new bool[2];
			if (colors[0] == default(Color))
				colors[0] = NoneColor();
			else
				modifierExists[0] = true;
			if (colors[1] == default(Color))
				colors[1] = NoneColor();
			else
				modifierExists[1] = true;
			Color tint = NoneColor();
			if (modifierExists[0])
				tint = colors[0];
			if (modifierExists[1])
				tint = colors[1];
			if (modifierExists[0] && modifierExists[1])
				tint = Color.Lerp(colors[0], colors[1], 0.5f);
			return tint;
		}

		public virtual bool? SafePreAI() => null;
		public sealed override bool PreAI()
		{
			BombtastrophePlayer player = Main.player[Projectile.owner].GetModPlayer<BombtastrophePlayer>();
			if (player.angelTimer > 0)
            {
				Projectile.timeLeft = 777;
				if (player.angelTimer > 60)
					Projectile.velocity = new Vector2(0f, -MathHelper.Lerp(0f, 16f, (float)(player.angelTimer - 61) / 1800f)) * ((float)(player.angelTimer - 61) / 1800f);
				else
					Projectile.velocity = Vector2.Zero;
				if (Projectile.Center.Y < player.Player.Center.Y - 800f || exploding)
					Projectile.active = false;
				return false;
            }
			bool? returnValue = SafePreAI();
			if (returnValue == null)
				return base.PreAI();
			return (bool)returnValue;
		}

		private bool firstTic = true;
		public override void AI()
		{
			if (firstTic)
			{
				if (modifiers[0] == "Huge" || modifiers[1] == "Huge")
					Projectile.scale = 1.5f;
				if (modifiers[0] == "Mighty" || modifiers[1] == "Mighty")
					Projectile.scale = 2f;
				if (modifiers[0] == "Gigantic")
					Projectile.scale = 4f;
				if (modifiers[0] == "Speedy" || modifiers[1] == "Speedy")
					Projectile.extraUpdates = 1;
				if (modifiers[0] == "Supersonic")
					Projectile.extraUpdates = 4;
				if (modifiers[0] == "Stealthy" || modifiers[1] == "Stealthy" || modifiers[0] == "Assassinating" || modifiers[0] == "Light Essence" || modifiers[0] == "Ghostly" || modifiers[0] == "Dark Essence" || modifiers[0] == "Dark Essence 2" || modifiers[0] == "Wrathful 2")
					Projectile.alpha = 191;
				if (modifiers[0] == "Invisible")
					Projectile.alpha = 255;
				Projectile.Size = Projectile.Size * Projectile.scale;
				if (modifiers[0] == "Super Short-fused")
					Projectile.timeLeft /= 5;
				else if (modifiers[0] == "Short-fused" || modifiers[1] == "Short-fused")
					Projectile.timeLeft -= Projectile.timeLeft / 3;
				firstTic = false;
			}
			int explosiveTier = 1;
			if (ExplosionSize() >= 128f)
				explosiveTier++;
			if (ExplosionSize() >= 256f)
				explosiveTier++;
			int dustDivisor = 1;
			if (modifiers[0] == "Lingering" || modifiers[1] == "Lingering" || modifiers[0] == "Messy" || modifiers[0] == "Soiled" || modifiers[0] == "Long-lasting" || modifiers[0] == "Zombified 2" || modifiers[0] == "Dark Essence 2" || modifiers[0] == "Wrathful 2" || modifiers[0] == "Infested 2")
				dustDivisor = explosiveTier * 3;
			int[] dustType = new int[] { 6, 31 };
			for (int i = 0; i < 2; i++)
            {
				if (modifiers[i] == "Fiery" || modifiers[i] == "Blazing")
					dustType[1] = 6;
				if (modifiers[i] == "Light Essence" || modifiers[i] == "Ghostly")
					dustType[0] = 31;
				if (modifiers[i] == "Zombified" || modifiers[i] == "Zombified 2" || modifiers[i] == "Wrathful" || modifiers[i] == "Infested")
					dustType[1] = (dustType[0] = 5);
				if (modifiers[i] == "Dark Essence" || modifiers[i] == "Dark Essence 2" || modifiers[i] == "Wrathful 2")
					dustType[1] = (dustType[0] = 14);
				if (modifiers[i] == "Sandy" || modifiers[i] == "Dusty" || modifiers[i] == "Messy" || modifiers[i] == "Soiled")
					dustType[1] = (dustType[0] = 32);
			}
			if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 1 && !exploding)
			{
				Projectile.velocity = Vector2.Zero;
				Projectile.damage = ExplosionDamage();
				if (modifiers[0] == "Overjoyed")
					Projectile.damage += (Projectile.damage / 2) + Projectile.damage;
				if (modifiers[0] == "Happy" || modifiers[1] == "Happy")
					Projectile.damage += Projectile.damage / 2;
				if (modifiers[0] == "Mighty" || modifiers[1] == "Mighty")
					Projectile.damage += Projectile.damage / 2;
				if (modifiers[0] == "Merry")
					Projectile.damage += Projectile.damage - (Projectile.damage / 3);
				Projectile.hostile = true;
				Projectile.friendly = ModContent.GetInstance<BombtastropheConfigClient>().explosivesHurtFoes;
				Projectile.tileCollide = false;
				Projectile.alpha = 255;
				Projectile.Resize((int)(ExplosionSize() * Projectile.scale), (int)(ExplosionSize() * Projectile.scale));
				Projectile.knockBack = 8f;
				Projectile.timeLeft = 2;
				if (modifiers[0] == "Lingering" || modifiers[1] == "Lingering" || modifiers[0] == "Messy" || modifiers[0] == "Soiled" || modifiers[0] == "Zombified 2" || modifiers[0] == "Dark Essence 2" || modifiers[0] == "Wrathful 2")
					Projectile.timeLeft = 90;
				if (modifiers[0] == "Long-lasting")
					Projectile.timeLeft = 300;
				exploding = true;
				ExplosiveUtils.ExplosionParticles(Projectile, explosiveTier, dustDivisor, dustType[0], dustType[1], modifiers[0] == "Confetti-filled" || modifiers[1] == "Confetti-filled");
				if (modifiers[0] == "Fiery" || modifiers[1] == "Fiery" || modifiers[0] == "Blazing")
				{
					for (int i = 0; i < (modifiers[0] == "Blazing" ? 1 : Main.rand.Next(4, 6)); i++)
					{
						Vector2 vel = (new Vector2(0f, MathHelper.Lerp(4f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread * 2f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread / 8f))).RotatedByRandom(MathHelper.ToRadians(180f));
						if (Projectile.velocity.Y == 0f)
							vel = (new Vector2(0f, -MathHelper.Lerp(4f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread * 2f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread / 8f))).RotatedByRandom(MathHelper.ToRadians(60f));
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<LingeringFlame>(), 20, 0f, Projectile.owner);
					}
				}
				if (modifiers[0] == "Sandy" || modifiers[1] == "Sandy" || modifiers[0] == "Dusty" || modifiers[0] == "Messy" || modifiers[0] == "Soiled")
				{
					for (int i = 0; i < ((modifiers[0] == "Sandy" || modifiers[1] == "Sandy" || modifiers[0] == "Soiled") ? (modifiers[0] == "Soiled" ? 2 : Main.rand.Next(3, 4)) : (modifiers[0] == "Messy" ? 1 : Main.rand.Next(5, 7))); i++)
					{
						Vector2 vel = (new Vector2(0f, MathHelper.Lerp(4f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread * 2f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread / 8f))).RotatedByRandom(MathHelper.ToRadians(180f));
						if (Projectile.velocity.Y == 0f)
							vel = (new Vector2(0f, -MathHelper.Lerp(4f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread * 2f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread / 8f))).RotatedByRandom(MathHelper.ToRadians(60f));
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, 31, 20, 0f, Projectile.owner);
					}
				}
				if (modifiers[0] == "Zombified" || modifiers[0] == "Dark Essence" || modifiers[0] == "Wrathful" || modifiers[0] == "Infested")
                {
					Projectile proj = Main.projectile[Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, Projectile.type, 0, 0f, Projectile.owner)];
					proj.rotation = Projectile.rotation;
					if (proj.ModProjectile is Projectiles.Explosive explosive)
                    {
						explosive.modifiers[0] = modifiers[0] + " 2";
						if (modifiers[0] == "Infested")
							explosive.modifiers[0] = "Zombified 2";
						explosive.modifiers[1] = "None";
						if (destructive)
							explosive.destructive = true;
					}
				}
				if (destructive)
				{
					Vector2 position = Projectile.Center;
					SoundEngine.PlaySound(SoundID.Item14, position);
					int minTileX = (int)(Projectile.position.X / 16f);
					int maxTileX = (int)((Projectile.position.X + Projectile.width) / 16f);
					int minTileY = (int)(Projectile.position.Y / 16f);
					int maxTileY = (int)((Projectile.position.Y + Projectile.height) / 16f);
					if (minTileX < 0)
						minTileX = 0;
					if (maxTileX > Main.maxTilesX)
						maxTileX = Main.maxTilesX;
					if (minTileY < 0)
						minTileY = 0;
					if (maxTileY > Main.maxTilesY)
						maxTileY = Main.maxTilesY;
					for (int i = minTileX; i <= maxTileX; i++)
					{
						for (int j = minTileY; j <= maxTileY; j++)
						{
							if (Main.tile[i, j] != null && Main.tile[i, j].HasTile && TileLoader.CanExplode(i, j))
							{
								Tile tile = Main.tile[i, j];
								Player player = Main.player[Projectile.owner];
								for (int a = 0; a < 50; a++)
									player.PickTile(i, j, 50000);
							}
						}
					}
				}
				if (modifiers[0] == "Infested" || modifiers[0] == "Shattering")
				{
					for (int i = 0; i < 3; i++)
					{
						Vector2 vel = (new Vector2(0f, MathHelper.Lerp(4f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread * 2f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread / 8f))).RotatedByRandom(MathHelper.ToRadians(180f));
						if (Projectile.velocity.Y == 0f)
							vel = (new Vector2(0f, -MathHelper.Lerp(4f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread * 2f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread / 8f))).RotatedByRandom(MathHelper.ToRadians(60f));
						Projectile proj = Main.projectile[Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<Bombaggot>(), 0, 0f, Projectile.owner)];
						modifiers[0] = "None";
						modifiers[1] = "None";
					}
				}
				if (modifiers[0] == "Fragmenting" || modifiers[1] == "Fragmenting" || modifiers[0] == "Cluster")
				{
					for (int i = 0; i < 3; i++)
					{
						Vector2 vel = (new Vector2(0f, MathHelper.Lerp(4f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread * 2f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread / 8f))).RotatedByRandom(MathHelper.ToRadians(180f));
						if (Projectile.velocity.Y == 0f)
							vel = (new Vector2(0f, -MathHelper.Lerp(4f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread * 2f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread / 8f))).RotatedByRandom(MathHelper.ToRadians(60f));
						Projectile proj = Main.projectile[Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ClusterProj(), 0, 0f, Projectile.owner)];
						modifiers[0] = "None";
						modifiers[1] = "None";
						if (modifiers[0] == "Cluster" && proj.ModProjectile is Projectiles.Explosive explosive)
							explosive.modifiers[0] = modifiers[0];
					}
				}
			}
			Projectile.ai[0]++;
			if (Projectile.ai[0] >= 16f)
				Projectile.ai[0] = 16f;
			if (exploding)
			{
				if (Projectile.timeLeft % 10 == 0)
				{
					if (destructive)
					{
						Vector2 position = Projectile.Center;
						SoundEngine.PlaySound(SoundID.Item14, position);
						int minTileX = (int)(Projectile.position.X / 16f);
						int maxTileX = (int)((Projectile.position.X + Projectile.width) / 16f);
						int minTileY = (int)(Projectile.position.Y / 16f);
						int maxTileY = (int)((Projectile.position.Y + Projectile.height) / 16f);
						if (minTileX < 0)
							minTileX = 0;
						if (maxTileX > Main.maxTilesX)
							maxTileX = Main.maxTilesX;
						if (minTileY < 0)
							minTileY = 0;
						if (maxTileY > Main.maxTilesY)
							maxTileY = Main.maxTilesY;
						for (int i = minTileX; i <= maxTileX; i++)
						{
							for (int j = minTileY; j <= maxTileY; j++)
							{
								if (Main.tile[i, j] != null && Main.tile[i, j].HasTile && TileLoader.CanExplode(i, j))
								{
									Tile tile = Main.tile[i, j];
									Player player = Main.player[Projectile.owner];
									for (int a = 0; a < 50; a++)
										player.PickTile(i, j, 50000);
								}
							}
						}
					}

					ExplosiveUtils.ExplosionParticles(Projectile, explosiveTier, dustDivisor, dustType[0], dustType[1], modifiers[0] == "Confetti-filled" || modifiers[1] == "Confetti-filled", Projectile.timeLeft % 30 == 0);
					if (modifiers[0] == "Blazing")
					{
						for (int i = 0; i < 1; i++)
						{
							Vector2 vel = (new Vector2(0f, MathHelper.Lerp(4f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread * 2f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread / 8f))).RotatedByRandom(MathHelper.ToRadians(180f));
							if (Projectile.velocity.Y == 0f)
								vel = (new Vector2(0f, -MathHelper.Lerp(4f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread * 2f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread / 8f))).RotatedByRandom(MathHelper.ToRadians(60f));
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<LingeringFlame>(), 20, 0f, Projectile.owner);
						}
					}
					if (modifiers[0] == "Messy" || modifiers[0] == "Soiled")
					{
						for (int i = 0; i < (modifiers[0] == "Soiled" ? 2 : 1); i++)
						{
							Vector2 vel = (new Vector2(0f, MathHelper.Lerp(4f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread * 2f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread / 8f))).RotatedByRandom(MathHelper.ToRadians(180f));
							if (Projectile.velocity.Y == 0f)
								vel = (new Vector2(0f, -MathHelper.Lerp(4f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread * 2f, ModContent.GetInstance<BombtastropheConfigClient>().bombSpread / 8f))).RotatedByRandom(MathHelper.ToRadians(60f));
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, 31, 20, 0f, Projectile.owner);
						}
					}
				}
			}
			else if (modifiers[0] == "Blazing" && Projectile.ai[0] > 15f && Projectile.timeLeft % Main.rand.Next(21, 31) == 0)
				Main.projectile[Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.NextFloat(-1.25f, 1.25f), -2f), ModContent.ProjectileType<LingeringFlame>(), 20, 0f, Projectile.owner)].timeLeft = 150;
			if (Projectile.ai[0] > 15f && !exploding && ((modifiers[0] == "Heat-Seeking" || modifiers[1] == "Heat-Seeking" || modifiers[0] == "Zombified 2" || modifiers[0] == "Dark Essence" || modifiers[0] == "Dark Essence 2") && (modifiers[0] == "Gravity-defying" || modifiers[1] == "Gravity-defying" || modifiers[0] == "Dragon" || modifiers[0] == "Hydra" || modifiers[0] == "Light Essence") || modifiers[0] == "Mighty" || modifiers[1] == "Mighty" || modifiers[0] == "Ghostly" || modifiers[0] == "Wrathful 2"))
			{
				Player target = null;
				float maxDistance = 9001f;
				for (int i = 0; i < 255; i++)
				{
					if (Main.player[i].active && !Main.player[i].dead)
					{
						float distance = Projectile.Distance(Main.player[i].Center);
						if (distance < maxDistance)
						{
							target = Main.player[i];
							maxDistance = distance;
						}
					}
				}
				Projectile.velocity += (((target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 4f) - Projectile.velocity) / 15f;
			}
			if (!exploding && modifiers[0] != "Gravity-defying" && modifiers[1] != "Gravity-defying" && modifiers[0] != "Dragon" && modifiers[0] != "Hydra" && modifiers[0] != "Mighty" && modifiers[1] != "Mighty" && modifiers[0] != "Light Essence" && modifiers[0] != "Ghostly" && modifiers[0] != "Wrathful 2")
			{
				Projectile.velocity.Y += 0.2f;
				if (modifiers[0] == "Heavy" || modifiers[1] == "Heavy" || modifiers[0] == "Dense")
				{
					Projectile.velocity.Y += 0.1f;
					if (modifiers[0] == "Dense")
						Projectile.velocity.Y += 0.2f;
				}
				Projectile.rotation += Projectile.velocity.X * 0.1f;
			}
			else
			{
				if (Projectile.ai[0] > 15f && (modifiers[0] == "Mighty" || modifiers[1] == "Mighty"))
					Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f), 0.72f);
				else
					Projectile.rotation += Projectile.velocity.Length() * 0.05f;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			for (int i = 0; i < 2; i++)
            {
				if (i == 1 && Main.player[Projectile.owner].GetModPlayer<BombtastrophePlayer>().angelTimer <= 0)
					break;
				float alphaMult = 1f;
				if (i == 1)
					alphaMult = 0.4f;
				Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
				Vector2 textureVect = new Vector2((float)(texture.Width / 2), (float)(texture.Height / (2 * Main.projFrames[Projectile.type])));
				Rectangle textureRect = new Rectangle(i * (texture.Width / 2), 0, (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
				if (modifiers[0] != "Unaltered")
				{
					if (modifiers[0] == "Mighty" || modifiers[1] == "Mighty")
					{
						textureRect = new Rectangle(i * (texture.Width / 2), 8 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
						Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, textureRect, lightColor * (((255 - (float)(Projectile.alpha)) / 255f) * alphaMult), Projectile.rotation, textureVect, Projectile.scale, SpriteEffects.None, 0f);
						textureRect = new Rectangle(i * (texture.Width / 2), 0, (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
					}
					if (modifiers[0] == "Shattering")
						textureRect = new Rectangle(i * (texture.Width / 2), 11 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
					else if (modifiers[0] == "Volatile" || modifiers[1] == "Volatile")
						textureRect = new Rectangle(i * (texture.Width / 2), 4 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
					if (modifiers[0] == "Spring")
						textureRect = new Rectangle(i * (texture.Width / 2), 3 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
				}
				Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, textureRect, Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, ColorTint()) * (((255 - (float)(Projectile.alpha)) / 255f) * alphaMult), Projectile.rotation, textureVect, Projectile.scale, SpriteEffects.None, 0f);
				if (modifiers[0] != "Merry")
				{
					textureRect = new Rectangle(i * (texture.Width / 2), 1 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
					if (modifiers[0] != "Unaltered")
					{
						if (modifiers[0] == "Short-fused" || modifiers[1] == "Short-fused")
							textureRect = new Rectangle(i * (texture.Width / 2), 6 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
						if (modifiers[0] == "Super Short-fused")
							textureRect = new Rectangle(i * (texture.Width / 2), 7 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
					}
					Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, textureRect, Color.White * (((255 - (float)(Projectile.alpha)) / 255f) * alphaMult), Projectile.rotation, textureVect, Projectile.scale, SpriteEffects.None, 0f);
				}
				if (modifiers[0] != "Unaltered")
				{
					if (modifiers[0] == "Happy" || modifiers[1] == "Happy" || modifiers[0] == "Overjoyed" || modifiers[0] == "Merry")
					{
						textureRect = new Rectangle(i * (texture.Width / 2), 2 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
						if (modifiers[0] == "Overjoyed")
							textureRect = new Rectangle(i * (texture.Width / 2), 5 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
						Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, textureRect, Color.White * (((255 - (float)(Projectile.alpha)) / 255f) * alphaMult), Projectile.rotation, textureVect, Projectile.scale, SpriteEffects.None, 0f);
					}
					if (modifiers[0] == "Mighty" || modifiers[1] == "Mighty")
					{
						textureRect = new Rectangle(i * (texture.Width / 2), 9 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
						Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, textureRect, lightColor * (((255 - (float)(Projectile.alpha)) / 255f) * alphaMult), Projectile.rotation, textureVect, Projectile.scale, SpriteEffects.None, 0f);
					}
					if (modifiers[0] == "Merry")
					{
						textureRect = new Rectangle(i * (texture.Width / 2), 10 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
						Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, textureRect, lightColor * (((255 - (float)(Projectile.alpha)) / 255f) * alphaMult), Projectile.rotation, textureVect, Projectile.scale, SpriteEffects.None, 0f);
					}
				}
			}
			return false;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			BombtastrophePlayer player = Main.player[Projectile.owner].GetModPlayer<BombtastrophePlayer>();
			if (modifiers[0] != "Ghostly" || player.angelTimer <= 0)
            {
				if (modifiers[0] == "Volatile" || modifiers[1] == "Volatile")
				{
					Projectile.timeLeft = 2;
					Projectile.velocity = Vector2.Zero;
				}
				else if (modifiers[0] == "Bouncy" || modifiers[1] == "Bouncy" || modifiers[0] == "Platform-Sticky Block-Bouncy" || modifiers[0] == "Spring")
				{
					if (Projectile.velocity.X != oldVelocity.X)
					{
						Projectile.velocity.X = -oldVelocity.X;
						if (modifiers[0] == "Spring")
							Projectile.velocity.X *= 1.5f;
					}
					if (Projectile.velocity.Y != oldVelocity.Y)
					{
						Projectile.velocity.Y = -oldVelocity.Y;
						if (modifiers[0] == "Spring")
							Projectile.velocity.Y *= 1.5f;
					}
				}
				else if (Projectile.ai[0] > 15f && (modifiers[0] == "Heat-Seeking" || modifiers[1] == "Heat-Seeking" || modifiers[0] == "Zombified 2" || modifiers[0] == "Dark Essence 2") && modifiers[0] != "Gravity-defying" && modifiers[1] != "Gravity-defying" && modifiers[0] != "Dragon" && modifiers[0] != "Hydra" && modifiers[0] != "Mighty" && modifiers[1] != "Mighty" && modifiers[0] != "Wrathful 2")
				{
					Player target = null;
					float maxDistance = 9001f;
					for (int i = 0; i < 255; i++)
					{
						if (Main.player[i].active && !Main.player[i].dead)
						{
							float distance = Projectile.Distance(Main.player[i].Center);
							if (distance < maxDistance)
							{
								target = Main.player[i];
								maxDistance = distance;
							}
						}
					}
					Projectile.velocity.X += (((target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 4f).X - Projectile.velocity.X) / 15f;
				}
				else
				{
					Projectile.velocity.X *= 0.94f;
					if ((double)Projectile.velocity.X > -0.01 && (double)Projectile.velocity.X < 0.01)
					{
						Projectile.velocity.X = 0f;
						Projectile.netUpdate = true;
					}
				}
			}
			return false;
		}
	}

	public class Grenade : Explosive
	{
		public override Color NoneColor() => new Color(150, 160, 142);
		public override float ExplosionSize() => 64f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Grenade");
			Main.projFrames[Projectile.type] = 12;
		}

		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 180;
		}
	}

	public class Bomb : Explosive
	{
		public override Color NoneColor() => new Color(142, 149, 160);
		public override float ExplosionSize() => 128f;
		public override int ExplosionDamage() => 60;
		public override int ClusterProj() => ModContent.ProjectileType<Grenade>();

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bomb");
			Main.projFrames[Projectile.type] = 12;
		}

		public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 22;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 240;
		}
	}

	public class Dynamite : Explosive
	{
		public override Color NoneColor() => new Color(253, 62, 3);
		public override float ExplosionSize() => 256f;
		public override int ExplosionDamage() => 80;
		public override int ClusterProj() => ModContent.ProjectileType<Bomb>();

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dynamite");
			Main.projFrames[Projectile.type] = 13;
		}

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 300;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			for (int i = 0; i < 2; i++)
			{
				if (i == 1 && Main.player[Projectile.owner].GetModPlayer<BombtastrophePlayer>().angelTimer <= 0)
					break;
				float alphaMult = 1f;
				if (i == 1)
					alphaMult = 0.4f;
				Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
				Vector2 textureVect = new Vector2((float)(texture.Width / 2), (float)(texture.Height / (2 * Main.projFrames[Projectile.type])));
				Rectangle textureRect = new Rectangle(i * (texture.Width / 2), 0, (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
				if (modifiers[0] != "Unaltered")
				{
					if (modifiers[0] == "Mighty" || modifiers[1] == "Mighty")
					{
						textureRect = new Rectangle(i * (texture.Width / 2), 9 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
						Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, textureRect, lightColor * (((255 - (float)(Projectile.alpha)) / 255f) * alphaMult), Projectile.rotation, textureVect, Projectile.scale, SpriteEffects.None, 0f);
						textureRect = new Rectangle(i * (texture.Width / 2), 0, (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
					}
					if (modifiers[0] == "Shattering")
						textureRect = new Rectangle(i * (texture.Width / 2), 12 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
					else if (modifiers[0] == "Volatile" || modifiers[1] == "Volatile")
						textureRect = new Rectangle(i * (texture.Width / 2), 4 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
					if (modifiers[0] == "Spring")
						textureRect = new Rectangle(i * (texture.Width / 2), 3 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
				}
				Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, textureRect, Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, ColorTint()) * (((255 - (float)(Projectile.alpha)) / 255f) * alphaMult), Projectile.rotation, textureVect, Projectile.scale, SpriteEffects.None, 0f);
				textureRect = new Rectangle(i * (texture.Width / 2), 8 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
				Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, textureRect, lightColor * (((255 - (float)(Projectile.alpha)) / 255f) * alphaMult), Projectile.rotation, textureVect, Projectile.scale, SpriteEffects.None, 0f);
				textureRect = new Rectangle(i * (texture.Width / 2), 1 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
				if (modifiers[0] != "Unaltered")
				{
					if (modifiers[0] == "Short-fused" || modifiers[1] == "Short-fused")
						textureRect = new Rectangle(i * (texture.Width / 2), 6 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
					if (modifiers[0] == "Super Short-fused")
						textureRect = new Rectangle(i * (texture.Width / 2), 7 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
				}
				Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, textureRect, Color.White * (((255 - (float)(Projectile.alpha)) / 255f) * alphaMult), Projectile.rotation, textureVect, Projectile.scale, SpriteEffects.None, 0f);
				if (modifiers[0] != "Merry")
				{
					if (modifiers[0] != "Unaltered")
					{
						if (modifiers[0] == "Happy" || modifiers[1] == "Happy" || modifiers[0] == "Overjoyed" || modifiers[0] == "Merry")
						{
							textureRect = new Rectangle(i * (texture.Width / 2), 2 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
							if (modifiers[0] == "Overjoyed")
								textureRect = new Rectangle(i * (texture.Width / 2), 5 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
							Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, textureRect, Color.White * (((255 - (float)(Projectile.alpha)) / 255f) * alphaMult), Projectile.rotation, textureVect, Projectile.scale, SpriteEffects.None, 0f);
						}
						if (modifiers[0] == "Mighty" || modifiers[1] == "Mighty")
						{
							textureRect = new Rectangle(i * (texture.Width / 2), 10 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
							Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, textureRect, lightColor * (((255 - (float)(Projectile.alpha)) / 255f) * alphaMult), Projectile.rotation, textureVect, Projectile.scale, SpriteEffects.None, 0f);
						}
						BombtastrophePlayer player = Main.player[Projectile.owner].GetModPlayer<BombtastrophePlayer>();
						if (modifiers[0] == "Merry")
						{
							textureRect = new Rectangle(i * (texture.Width / 2), 11 * (texture.Height / Main.projFrames[Projectile.type]), (texture.Width / 2), (texture.Height / Main.projFrames[Projectile.type]));
							Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, textureRect, lightColor * (((255 - (float)(Projectile.alpha)) / 255f) * alphaMult), Projectile.rotation, textureVect, Projectile.scale, SpriteEffects.None, 0f);
						}
					}
				}
			}
			return false;
        }
    }

    public abstract class NoModifiersExplosive : Explosive
    {
		public float power = 1f;

		public override bool? SafePreAI()
		{
			if (modifiers[0] != "None")
				power += power * 1.25f;
			if (modifiers[1] != "None")
				power += power * 1.25f;
			modifiers[0] = "None";
			modifiers[1] = "None";
			return null;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Vector2 textureVect = new Vector2((float)(texture.Width / 2), (float)(texture.Height / (2 * Main.projFrames[Projectile.type])));
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor * ((255 - (float)(Projectile.alpha)) / 255f), Projectile.rotation, textureVect, Projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
	}

    public class AnnihilationPellet : NoModifiersExplosive
	{
		public override Color NoneColor() => Color.White;
		public override float ExplosionSize() => 32f * power;
		public override int ExplosionDamage() => (int)(20f * power);

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Annihilation Pellet");
		}

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 120;
		}
	}

	public class Bombaggot : NoModifiersExplosive
	{
		public override Color NoneColor() => Color.White;
		public override float ExplosionSize() => 32f * power;
		public override int ExplosionDamage() => 60;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bombaggot");
		}

		public override void SetDefaults()
		{
			Projectile.width = 12;
			Projectile.height = 12;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 240;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.ai[0] > 15f)
			{
				Player target = null;
				float maxDistance = 9001f;
				for (int i = 0; i < 255; i++)
				{
					if (Main.player[i].active && !Main.player[i].dead)
					{
						float distance = Projectile.Distance(Main.player[i].Center);
						if (distance < maxDistance)
						{
							target = Main.player[i];
							maxDistance = distance;
						}
					}
				}
				Projectile.velocity.X += (((target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * (4f * power)).X - Projectile.velocity.X) / 15f;
			}
			else
			{
				Projectile.velocity.X *= 0.94f;
				if ((double)Projectile.velocity.X > -0.01 && (double)Projectile.velocity.X < 0.01)
				{
					Projectile.velocity.X = 0f;
					Projectile.netUpdate = true;
				}
			}
			return false;
		}
	}
}