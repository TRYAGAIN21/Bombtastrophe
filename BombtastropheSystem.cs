using Terraria.GameContent;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.UI;
using ReLogic.Graphics;
using System.IO;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.UI.Chat;
using TRYAGAINLib;
using Terraria.Audio;
using BombtastropheMod.Challenges;

namespace BombtastropheMod
{
	public class BombtastropheSystem : ModSystem
	{
		public static ModKeybind OpenUI { get; private set; }

		public override void Load()
		{
			OpenUI = KeybindLoader.RegisterKeybind(Mod, "Open Bombtastrophe Settings", "B");
		}

		public override void Unload()
		{
			OpenUI = null;
		}

		public static int wrappedCodeProgress;

		public override void PreUpdateWorld()
		{
			if (wrappedCodeProgress > 0)
				wrappedCodeProgress++;
			if (wrappedCodeProgress > 18000)
				wrappedCodeProgress = 0;
		}

		public override void SaveWorldData(TagCompound tag)
		{
			tag["wrappedCodeProgress"] = wrappedCodeProgress;
		}

		public override void LoadWorldData(TagCompound tag)
		{
			if (tag.ContainsKey("wrappedCodeProgress"))
            {
				wrappedCodeProgress = (tag.GetInt("wrappedCodeProgress") / 6000) * 6000;
				if (tag.GetInt("wrappedCodeProgress") > 0)
					wrappedCodeProgress = Math.Max(wrappedCodeProgress, 1);
			}
		}

		private Color DarkenUI(Color color, float amplifier = 1f) { return Color.Lerp(color == default(Color) ? TRYAGAINLib.LibUtils.InvBGColor * TRYAGAINLib.LibUtils.InvBGOpacity : color, Color.Black * (color == default(Color) ? TRYAGAINLib.LibUtils.InvBGOpacity : 1f), Math.Max(Math.Min(0.23f * amplifier, 1f), 0f)); }
		private Color BrightenUI(Color color, float amplifier = 1f) { return Color.Lerp(color == default(Color) ? TRYAGAINLib.LibUtils.InvBGColor * TRYAGAINLib.LibUtils.InvBGOpacity : color, Color.White * (color == default(Color) ? TRYAGAINLib.LibUtils.InvBGOpacity : 1f), Math.Max(Math.Min(0.23f * amplifier, 1f), 0f)); }
		private Color MouseAlphaText(Color color) { return color * Main.mouseTextColor; }
		private bool OnPage(BombtastrophePlayer modPlayer, string pageName)
		{
			if (pageName == default(string))
				pageName = "Bombtastrophe Menu";
			return modPlayer.pageDirectory[modPlayer.pageDirectory.Length - 1] == pageName;
		}
		private void AddPage(BombtastrophePlayer modPlayer, string pageName)
		{
			string[] oldPageDirectory = modPlayer.pageDirectory;
			modPlayer.pageDirectory = new string[modPlayer.pageDirectory.Length + 1];
			for (int i = 0; i < oldPageDirectory.Length; i++)
				modPlayer.pageDirectory[i] = oldPageDirectory[i];
			modPlayer.pageDirectory[modPlayer.pageDirectory.Length - 1] = pageName;
		}
		private void DrawStringWithBorder(SpriteBatch spriteBatch, string text, Vector2 position, Color color, float scale, float boxWidth, bool CenteredX = false, bool CenteredY = true, bool large = false, float wrapAt = 0f)
		{
			if (ChatManager.GetStringSize(FontAssets.MouseText.Value, text, new Vector2(1f)).X > boxWidth && boxWidth != 0f)
				scale *= boxWidth / ChatManager.GetStringSize(FontAssets.MouseText.Value, text, new Vector2(1f)).X;
			if (CenteredX)
				position.X -= ChatManager.GetStringSize(FontAssets.MouseText.Value, text, new Vector2(1f)).X * scale / 2f;
			if (CenteredY)
				position.Y -= ChatManager.GetStringSize(FontAssets.MouseText.Value, text, new Vector2(1f)).Y * scale / 2f;
			if (large)
				Utils.DrawBorderStringBig(spriteBatch, text, position, color, scale);
			else
				Utils.DrawBorderString(spriteBatch, text, position, color, scale);
		}
		private void DrawInvBG(SpriteBatch spriteBatch, Vector2 position, Vector2 size, Color color = default(Color))
		{
			Utils.DrawInvBG(spriteBatch, position.X, position.Y, size.X, size.Y, color);
		}
		private void DrawInvBG(SpriteBatch spriteBatch, Vector2 position, float w, float h, Color color = default(Color))
		{
			Utils.DrawInvBG(spriteBatch, position.X, position.Y, w, h, color);
		}
		private Color buildABombColor = new Color(255, 127, 0);
		private Color codeKeyboardColor = new Color(127, 0, 255);
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			BombtastrophePlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<BombtastrophePlayer>();
			int i = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hotbar"));
			if (i != -1)
			{		
				layers.Insert(i + 1, new LegacyGameInterfaceLayer(
					"BombtastropheMod: Bomb UI",
					delegate
					{
						if (modPlayer.UIOpen)
						{
							if (modPlayer.backgroundUISize == default(Vector2))
								modPlayer.backgroundUISize = new Vector2(1008, 576);
							Vector2 targetBackgroundUISize = new Vector2(1008, 576);
							if (OnPage(modPlayer, "Challenges"))
								targetBackgroundUISize.Y += 160;
							modPlayer.backgroundUISize = Vector2.Lerp(new Vector2(Math.Max(modPlayer.backgroundUISize.X, 1), Math.Max(modPlayer.backgroundUISize.Y, 1)), targetBackgroundUISize, 0.2f);
							Vector2 topRight = new Vector2(Main.screenWidth, Main.screenHeight) / 2 - modPlayer.backgroundUISize / 2;
							if (Main.playerInventory)
								modPlayer.UIOpen = false;
							DrawInvBG(Main.spriteBatch, topRight, modPlayer.backgroundUISize, DarkenUI(default(Color), 1.5f));
							Vector2 uiPos = topRight + new Vector2(16, 16);
							Vector2 uiSize = new Vector2(modPlayer.backgroundUISize.X - 160, 32);
							DrawInvBG(Main.spriteBatch, uiPos, uiSize, default(Color));
							string directory = modPlayer.pageDirectory[0];
							for (int i = 1; i < modPlayer.pageDirectory.Length; i++)
								directory += " > " + modPlayer.pageDirectory[i];
							DrawStringWithBorder(Main.spriteBatch, directory, uiPos + new Vector2(8, 20), Color.White, 0.85f, modPlayer.backgroundUISize.X - 32);
							uiPos = topRight + new Vector2(modPlayer.backgroundUISize.X - 48, 16);
							uiSize.X = 32;
							bool changeColors = false;
							if (Main.mouseX > uiPos.X && Main.mouseX < uiPos.X + uiSize.X && Main.mouseY > uiPos.Y && Main.mouseY < uiPos.Y + uiSize.Y)
							{
								modPlayer.hoveringOverOption = true;
								changeColors = true;
								if (Main.player[Main.myPlayer].GetModPlayer<TRYAGAINLib.LibPlayer>().JustClicked())
                                {
									SoundEngine.PlaySound(SoundID.MenuClose, Main.player[Main.myPlayer].Center);
									modPlayer.UIOpen = false;
								}
							}
							DrawInvBG(Main.spriteBatch, uiPos, uiSize, changeColors ? Color.Red : default(Color));
							DrawStringWithBorder(Main.spriteBatch, "X", uiPos + new Vector2(uiSize.X / 2, uiSize.Y / 2 + 4), changeColors ? Color.White : Color.Red, 1f, 0f, true);
							uiPos = topRight + new Vector2(modPlayer.backgroundUISize.X - 136, 16);
							uiSize.X = 80;
							changeColors = false;
							if (Main.mouseX > uiPos.X && Main.mouseX < uiPos.X + uiSize.X && Main.mouseY > uiPos.Y && Main.mouseY < uiPos.Y + uiSize.Y && !OnPage(modPlayer, default(string)))
							{
								modPlayer.hoveringOverOption = true;
								changeColors = true;
								if (Main.player[Main.myPlayer].GetModPlayer<TRYAGAINLib.LibPlayer>().JustClicked())
								{
									string[] oldPageDirectory = modPlayer.pageDirectory;
									modPlayer.pageDirectory = new string[modPlayer.pageDirectory.Length - 1];
									for (int i = 0; i < modPlayer.pageDirectory.Length; i++)
										modPlayer.pageDirectory[i] = oldPageDirectory[i];
									SoundEngine.PlaySound(SoundID.MenuClose, Main.player[Main.myPlayer].Center);
								}
							}
							DrawInvBG(Main.spriteBatch, uiPos, uiSize, OnPage(modPlayer, default(string)) ? DarkenUI(default(Color), 1.5f) : (changeColors ? BrightenUI(default(Color), 1.5f) : default(Color)));
							DrawStringWithBorder(Main.spriteBatch, "Back", uiPos + new Vector2(uiSize.X / 2, uiSize.Y / 2 + 4), OnPage(modPlayer, default(string)) ? Colors.RarityTrash : (changeColors ? Color.White : new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor)), 1f, 0f, true);
							if (OnPage(modPlayer, default(string)))
							{
								uiPos = new Vector2(topRight.X + 16, uiPos.Y + uiSize.Y + 16);
								uiSize = new Vector2(modPlayer.backgroundUISize.X / 3 - 16, modPlayer.backgroundUISize.Y - 48 - uiSize.Y);
								DrawInvBG(Main.spriteBatch, uiPos, uiSize);
								DrawStringWithBorder(Main.spriteBatch, "Settings", uiPos + new Vector2(uiSize.X / 2, 16), Color.White, 1.5f, 0f, true, false);
								uiPos.X += uiSize.X + 16;
								uiSize += new Vector2(modPlayer.backgroundUISize.X / 3 - 16, -64);
								DrawInvBG(Main.spriteBatch, uiPos, uiSize, DarkenUI(buildABombColor) * TRYAGAINLib.LibUtils.InvBGOpacity);
								DrawInvBG(Main.spriteBatch, uiPos + new Vector2(16, 16), new Vector2(uiSize.X - 32, 48), buildABombColor * TRYAGAINLib.LibUtils.InvBGOpacity);
								DrawStringWithBorder(Main.spriteBatch, "Build-A-Bomb", uiPos + new Vector2((uiSize.X - 32) / 2 + 16, 26), Color.White, 1.5f, 0f, true, false);
								DrawBuildABomb(uiPos, uiSize);
								uiPos.Y += uiSize.Y + 16;
								uiSize.Y = 48;
								DrawInvBG(Main.spriteBatch, uiPos, uiSize);
								uiSize.X = uiSize.X / 6 * 1.5f;
								for (int i = 1; i < 4; i++)
								{
									uiPos.X = MathHelper.Lerp(topRight.X + modPlayer.backgroundUISize.X / 3 + 16 - uiSize.X / 2, topRight.X + modPlayer.backgroundUISize.X - 16 + uiSize.X / 2, (float)i / 4f) - uiSize.X / 2;
									string page = "Challenges";
									if (i == 2)
										page = "Presets";
									if (i == 3)
										page = "Code Keyboard";
									changeColors = false;
									if (Main.mouseX > uiPos.X && Main.mouseX < uiPos.X + uiSize.X && Main.mouseY > uiPos.Y && Main.mouseY < uiPos.Y + uiSize.Y)
									{
										modPlayer.hoveringOverOption = true;
										changeColors = true;
										if (Main.player[Main.myPlayer].GetModPlayer<TRYAGAINLib.LibPlayer>().JustClicked())
										{
											AddPage(modPlayer, page);
											SoundEngine.PlaySound(SoundID.MenuOpen, Main.player[Main.myPlayer].Center);
										}
									}
									DrawInvBG(Main.spriteBatch, uiPos, uiSize, BrightenUI(i == 3 ? codeKeyboardColor : default(Color), (changeColors ? 1.5f : 0.5f) - (i == 3 ? 0.5f : 0f)) * (i == 3 ? TRYAGAINLib.LibUtils.InvBGOpacity : 1f));
									DrawStringWithBorder(Main.spriteBatch, page, uiPos + new Vector2(uiSize.X / 2, uiSize.Y / 2 + 4), Color.White, 1.25f, uiSize.X - 48, true, true);
								}
							}
							else if (OnPage(modPlayer, "Challenges"))
                            {
								uiPos = new Vector2(topRight.X + 16, uiPos.Y + 48);
								uiSize = new Vector2(modPlayer.backgroundUISize.X - 32, 48);
								DrawInvBG(Main.spriteBatch, uiPos, uiSize, default(Color));
								DrawStringWithBorder(Main.spriteBatch, "Challenges", uiPos + uiSize / 2 + new Vector2(0, 6), Color.White, 1.5f, 0f, true, true);
								uiPos.Y += uiSize.Y + 16;
								uiSize.Y = modPlayer.backgroundUISize.Y - 144;
								DrawInvBG(Main.spriteBatch, uiPos, uiSize, default(Color));
								uiPos += new Vector2(16, 16);
								uiSize = new Vector2(uiSize.X - 32, uiSize.Y / 4 - 16);
								Vector2 challengePos = uiPos + new Vector2(uiSize.Y, 0);
								Vector2 challengeSize = uiSize - new Vector2(uiSize.Y, 0);
								int num = 0;
								foreach (Challenge challenge in ClassLoader.Challenges)
                                {
									uiSize = challengeSize;
									uiPos = challengePos + new Vector2(0, (uiSize.Y + 16) * (num % 4));
									if (num / 4 == modPlayer.challengePage)
									DrawInvBG(Main.spriteBatch, uiPos - new Vector2(uiSize.Y, 0), uiSize + new Vector2(uiSize.Y, 0), DarkenUI(default(Color)));
									DrawInvBG(Main.spriteBatch, uiPos - new Vector2(uiSize.Y - 8, -8), new Vector2(uiSize.Y - 16, uiSize.Y - 16), DarkenUI(default(Color), 1.5f));
									Texture2D challengeIcon = ModContent.Request<Texture2D>(challenge.Prerequisites() ? challenge.Texture : "BombtastropheMod/Challenges/PrerequisitesNotMet", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
									if (challengeIcon.Width != 128 && challengeIcon.Height != 64 || challengeIcon == null)
										challengeIcon = ModContent.Request<Texture2D>("BombtastropheMod/Challenges/Placeholder", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
									Main.spriteBatch.Draw(challengeIcon, uiPos - new Vector2(uiSize.Y - 8, -8) + new Vector2(uiSize.Y / 2 - 8, uiSize.Y / 2 - 8), new Rectangle(challenge.Conditions() ? 0 : challengeIcon.Width / 2, 0, challengeIcon.Width / 2, challengeIcon.Height), Color.White, 0f, new Vector2(challengeIcon.Width / 4, challengeIcon.Height / 2), (uiSize.Y - 48) / 64f, SpriteEffects.None, 0f);
									challengeIcon = ModContent.Request<Texture2D>("BombtastropheMod/Challenges/Achievement_Borders", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
									Main.spriteBatch.Draw(challengeIcon, uiPos - new Vector2(uiSize.Y - 8, -8) + new Vector2(uiSize.Y / 2 - 8, uiSize.Y / 2 - 8), null, Color.White, 0f, new Vector2(challengeIcon.Width / 2, challengeIcon.Height / 2), (uiSize.Y - 48) / 64f, SpriteEffects.None, 0f);
									uiPos += new Vector2(0, 8);
									uiSize = new Vector2(ChatManager.GetStringSize(FontAssets.MouseText.Value, challenge.Name, new Vector2(0.75f)).X + 24, 24);
									DrawInvBG(Main.spriteBatch, uiPos, uiSize + new Vector2(32, 0), DarkenUI(default(Color), 1.5f));
									DrawStringWithBorder(Main.spriteBatch, challenge.Name, uiPos + uiSize / 2 + new Vector2(24, 3), Color.White, 0.75f, 0f, true, true);
									DrawInvBG(Main.spriteBatch, uiPos, new Vector2(uiSize.Y, uiSize.Y), DarkenUI(default(Color), 1.5f));
									DrawStringWithBorder(Main.spriteBatch, challenge.Conditions() ? System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Unicode.GetBytes("\u2713")) : "X", uiPos + new Vector2(uiSize.Y / 2, uiSize.Y / 2) + new Vector2(0, 3), challenge.Conditions() ? Color.Lime : Color.Red, 0.75f, 0f, true, true);
									uiPos.X += uiSize.X + 40;
									uiSize.X = challengeSize.X - uiSize.X - 48;
									DrawInvBG(Main.spriteBatch, uiPos, uiSize, DarkenUI(default(Color), 1.5f));
									DrawStringWithBorder(Main.spriteBatch, challenge.Conditions() ? "Unlocked: " + challenge.Reward : "Unlocks: ???", uiPos + new Vector2(8, uiSize.Y / 2 + 3), Color.White, 0.75f, 0f, false, true);
									uiPos = new Vector2(challengePos.X, uiPos.Y + uiSize.Y + 8);
									uiSize = new Vector2(challengeSize.X - 8, challengeSize.Y - 48);
									DrawInvBG(Main.spriteBatch, uiPos, uiSize, DarkenUI(default(Color), 1.5f));
									DrawStringWithBorder(Main.spriteBatch, challenge.Description, uiPos + new Vector2(8, 8), Color.White, 0.75f, 0f, false, false);
									num++;
								}
							}
							else
							{
								uiPos = new Vector2(topRight.X + 16, uiPos.Y + 48);
								uiSize = new Vector2(modPlayer.backgroundUISize.X - 32, modPlayer.backgroundUISize.Y - 80);
								string page = "Continue";
								changeColors = false;
								if (Main.mouseX > uiPos.X && Main.mouseX < uiPos.X + uiSize.X && Main.mouseY > uiPos.Y && Main.mouseY < uiPos.Y + uiSize.Y)
								{
									modPlayer.hoveringOverOption = true;
									changeColors = true;
									if (Main.player[Main.myPlayer].GetModPlayer<TRYAGAINLib.LibPlayer>().JustClicked())
									{
										AddPage(modPlayer, page);
										SoundEngine.PlaySound(SoundID.MenuOpen, Main.player[Main.myPlayer].Center);
									}
								}
								DrawInvBG(Main.spriteBatch, uiPos, uiSize, changeColors ? BrightenUI(default(Color), 1.5f) : default(Color));
								DrawStringWithBorder(Main.spriteBatch, page, uiPos + uiSize / 2 - new Vector2(112, 48), Color.White, 2f, 0f, true, true, true);
							}
							if (Main.mouseX > topRight.X && Main.mouseX < topRight.X + modPlayer.backgroundUISize.X && Main.mouseY > topRight.Y && Main.mouseY < topRight.Y + modPlayer.backgroundUISize.Y)
								Main.player[Main.myPlayer].mouseInterface = true;
						}
						return true;
					},
					InterfaceScaleType.UI)
				);
				layers.Insert(i + 1, new LegacyGameInterfaceLayer(
					"BombtastropheMod: Wrapped Code Event Bar",
					delegate
					{
						if (modPlayer.wrappedCodeAlpha > 0f)
						{
							Texture2D progressBar = Mod.Assets.Request<Texture2D>("WrappedCodeBar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

							float scale = (float)wrappedCodeProgress / 18000f;
							Main.spriteBatch.Draw(progressBar, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2 - (progressBar.Height / 2 - 300)), new Microsoft.Xna.Framework.Rectangle(0, 3 * (progressBar.Height / 5), progressBar.Width, (progressBar.Height / 5)), Color.White * modPlayer.wrappedCodeAlpha, 0f, new Vector2(progressBar.Width / 2, progressBar.Height / 10), 1f, SpriteEffects.None, 0f);
							Main.spriteBatch.Draw(progressBar, new Vector2(Main.screenWidth / 2 - (progressBar.Width / 2 - ((progressBar.Width / 2) * scale)), Main.screenHeight / 2 - (progressBar.Height / 2 - 300)), new Microsoft.Xna.Framework.Rectangle(0, 1 * (progressBar.Height / 5), progressBar.Width, (progressBar.Height / 5)), Color.Lerp(new Color(0, 255, 255), new Color(64, 128, 128), (float)wrappedCodeProgress / 18000f) * modPlayer.wrappedCodeAlpha, 0f, new Vector2(progressBar.Width / 2, progressBar.Height / 10), new Vector2(scale, 1f), SpriteEffects.None, 0f);
							Main.spriteBatch.Draw(progressBar, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2 - (progressBar.Height / 2 - 300)), new Microsoft.Xna.Framework.Rectangle(0, 0, progressBar.Width, (progressBar.Height / 5)), Color.White * modPlayer.wrappedCodeAlpha, 0f, new Vector2(progressBar.Width / 2, progressBar.Height / 10), 1f, SpriteEffects.None, 0f);
							for (int k = 0; k < 2; k++)
								Main.spriteBatch.Draw(progressBar, new Vector2(((Main.screenWidth / 2) + ((float)progressBar.Width * ((float)(k + 1) / 3f))) - 5, Main.screenHeight / 2 - (progressBar.Height / 2 - 300)), new Microsoft.Xna.Framework.Rectangle(0, 2 * (progressBar.Height / 5), progressBar.Width, (progressBar.Height / 5)), Color.White * modPlayer.wrappedCodeAlpha, 0f, new Vector2(progressBar.Width / 2, progressBar.Height / 10), 1f, SpriteEffects.None, 0f);
							Main.spriteBatch.Draw(progressBar, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2 - (progressBar.Height / 2 - 354)), new Microsoft.Xna.Framework.Rectangle(0, 4 * (progressBar.Height / 5), progressBar.Width, (progressBar.Height / 5)), Color.White * modPlayer.wrappedCodeAlpha, 0f, new Vector2(progressBar.Width / 2, progressBar.Height / 10), 1f, SpriteEffects.None, 0f);
							int num = ((wrappedCodeProgress / 6000) + 1);
							for (int k = 0; k < num; k++)
							{
								string bombType = "Grenade";
								if (k == 1)
									bombType = "Bomb";
								Color color = ExplosiveUtils.GetModifierColor("Heat-Seeking");
								if (k == 1)
									color = Color.Lerp(ExplosiveUtils.GetModifierColor("Freezing"), ExplosiveUtils.GetModifierColor("Huge"), 0.5f);
								Vector2 orgin = new Vector2((Main.screenWidth / 2) + (progressBar.Width / 2) - ((25 * (k + 1)) + 32), Main.screenHeight / 2 - (progressBar.Height / 2 - 346));
								scale = 0.75f;
								if (k == 1)
									scale = 0.65f;
								Texture2D explosive = Mod.Assets.Request<Texture2D>("Projectiles/" + bombType, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
								Rectangle textureRect = new Rectangle(0, 0, (explosive.Width), (explosive.Height / Main.projFrames[Mod.Find<ModProjectile>(bombType).Type]));
								if (k != 2)
									Main.spriteBatch.Draw(explosive, orgin, textureRect, color * modPlayer.wrappedCodeAlpha, 0f, new Vector2(explosive.Width / 2, explosive.Height / (2 * Main.projFrames[Mod.Find<ModProjectile>(bombType).Type])), scale, SpriteEffects.None, 0f);
								if (k == 2)
								{
									scale = 0.4f;
									explosive = Mod.Assets.Request<Texture2D>("Projectiles/Present", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
									textureRect = new Rectangle(0, 0, (explosive.Width / 2), (explosive.Height / 6));
									Main.spriteBatch.Draw(explosive, orgin - new Vector2(0, 2), textureRect, Color.White * modPlayer.wrappedCodeAlpha, 0f, new Vector2(explosive.Width / 4, explosive.Height / 12), scale, SpriteEffects.None, 0f);
								}
								else if (k == 0)
								{
									textureRect = new Rectangle(0, 2 * (explosive.Height / Main.projFrames[Mod.Find<ModProjectile>(bombType).Type]), (explosive.Width), (explosive.Height / Main.projFrames[Mod.Find<ModProjectile>(bombType).Type]));
									Main.spriteBatch.Draw(explosive, orgin, textureRect, Color.White * modPlayer.wrappedCodeAlpha, 0f, new Vector2(explosive.Width / 2, explosive.Height / (2 * Main.projFrames[Mod.Find<ModProjectile>(bombType).Type])), scale, SpriteEffects.None, 0f);
									textureRect = new Rectangle(0, 10 * (explosive.Height / Main.projFrames[Mod.Find<ModProjectile>(bombType).Type]), (explosive.Width), (explosive.Height / Main.projFrames[Mod.Find<ModProjectile>(bombType).Type]));
									Main.spriteBatch.Draw(explosive, orgin, textureRect, Color.White * modPlayer.wrappedCodeAlpha, 0f, new Vector2(explosive.Width / 2, explosive.Height / (2 * Main.projFrames[Mod.Find<ModProjectile>(bombType).Type])), scale, SpriteEffects.None, 0f);
								}
								else
                                {
									textureRect = new Rectangle(0, 1 * (explosive.Height / Main.projFrames[Mod.Find<ModProjectile>(bombType).Type]), (explosive.Width), (explosive.Height / Main.projFrames[Mod.Find<ModProjectile>(bombType).Type]));
									Main.spriteBatch.Draw(explosive, orgin, textureRect, Color.White * modPlayer.wrappedCodeAlpha, 0f, new Vector2(explosive.Width / 2, explosive.Height / (2 * Main.projFrames[Mod.Find<ModProjectile>(bombType).Type])), scale, SpriteEffects.None, 0f);
								}
							}
							DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.MouseText.Value, "Wave " + ((wrappedCodeProgress / 6000) + 1) + "   Bomb" + ((wrappedCodeProgress / 6000) > 0 ? "s" : "") + " currently in play:", new Vector2(((Main.screenWidth / 2) - (progressBar.Width / 2)) + 50, Main.screenHeight / 2 - (progressBar.Height / 2 - 334)), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor) * modPlayer.wrappedCodeAlpha, 0f, default(Vector2), 0.75f, SpriteEffects.None, 0f);
						}
						return true;
					},
					InterfaceScaleType.UI)
				);
			}

			void DrawBuildABomb(Vector2 uiPos, Vector2 uiSize)
            {
				uiPos += new Vector2(16, 80);
				uiSize -= new Vector2(32, 96);
				DrawInvBG(Main.spriteBatch, uiPos, uiSize, DarkenUI(buildABombColor, 2f) * TRYAGAINLib.LibUtils.InvBGOpacity);
			}
		}
	}
}