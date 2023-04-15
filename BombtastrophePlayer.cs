using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using BombtastropheMod.Projectiles;
using Terraria.Utilities;
using Terraria.Audio;

namespace BombtastropheMod
{
	public class BombtastrophePlayer : ModPlayer
	{
		public bool UIOpen;
		public string[] pageDirectory = new string[1];
		public int OptionHover_FrameCounter;
		public bool hoveringOverOption;
		public Vector2 backgroundUISize;

		public bool Unlocked_ZeroCasualties;
		public bool Unlocked_PrimevalWays;
		public bool Unlocked_Waterproof;
		public bool Unlocked_BoomBoomPow;
		public bool Unlocked_BombShelter;

		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (BombtastropheSystem.OpenUI.JustPressed)
			{
				if (Main.playerInventory && Main.myPlayer == Player.whoAmI)
					Main.playerInventory = false;
				UIOpen = !UIOpen;
				if (UIOpen)
					SoundEngine.PlaySound(SoundID.MenuOpen, Player.Center);
				else
					SoundEngine.PlaySound(SoundID.MenuClose, Player.Center);
			}
		}

		private Projectile ExplosionHitBy;
		public int bombsKilledBy;
		public int angelTimer;
		public float wrappedCodeAlpha;
		public override void ResetEffects()
        {
			if (!UIOpen)
            {
				pageDirectory = new string[1];
				backgroundUISize = default(Vector2);
			}
			ExplosionHitBy = null;
			pageDirectory[0] = "Bombtastrophe Menu";
			OptionHover_FrameCounter = hoveringOverOption ? Math.Min(OptionHover_FrameCounter + 1, 2) : 0;
			if (OptionHover_FrameCounter == 1)
				SoundEngine.PlaySound(SoundID.MenuTick, Player.Center);
			hoveringOverOption = false;
		}

		public override void SaveData(TagCompound tag)
		{
			tag["bombsKilledBy"] = bombsKilledBy;
			tag["angelTimer"] = angelTimer;
		}

		public override void LoadData(TagCompound tag)
		{
			bombsKilledBy = tag.GetInt("bombsKilledBy");
			if (tag.GetInt("angelTimer") > 0)
				angelTimer = 1;
		}

		private int bombSpawnTimer;
		public override void PostUpdateMiscEffects()
		{
			if (angelTimer > 0)
            {
				if (angelTimer == 300)
					Projectile.NewProjectile(Player.GetSource_FromThis(), new Vector2(Player.Center.X, Player.Center.Y - 600), Vector2.Zero, ModContent.ProjectileType<DivineBeing>(), 0, 0f, Player.whoAmI);
				angelTimer++;
				if (angelTimer > 4200)
					angelTimer = 0;
				return;
			}
			if (BombtastropheSystem.wrappedCodeProgress > 0)
			{
				Texture2D progressBar = Mod.Assets.Request<Texture2D>("WrappedCodeBar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				Vector2 barPos = new Vector2((Main.screenWidth / 2) - (progressBar.Width / 2), Main.screenHeight / 2 - (progressBar.Height / 2 - 300));
				if (wrappedCodeAlpha > 0.4f && Main.mouseX >= barPos.X && Main.mouseX <= barPos.X + progressBar.Width && Main.mouseY >= barPos.Y && Main.mouseY <= barPos.Y + (progressBar.Height / 5))
					wrappedCodeAlpha -= 0.6f / 15f;
				else
					wrappedCodeAlpha += 0.6f / 15f;
				wrappedCodeAlpha = Math.Max(wrappedCodeAlpha, 0.4f);
				wrappedCodeAlpha = Math.Min(wrappedCodeAlpha, 1f);
				return;
			}
			wrappedCodeAlpha = Math.Max(wrappedCodeAlpha - (0.6f / 15f), 0f);
			int spawnTimer = (int)(300f / BombtastropheConfigClient.Instance.bombSpawnRate);
			bombSpawnTimer++;
			if (bombSpawnTimer > spawnTimer)
			{
				bombSpawnTimer = 0;
				if (!BombtastropheConfigClient.Instance.buildABombOnly)
				{
					if (BombtastropheConfigClient.Instance.spawn.player)
						SpawnBomb(0);
					if (BombtastropheConfigClient.Instance.spawn.enemy)
						SpawnBomb(1);
					if (BombtastropheConfigClient.Instance.spawn.sky)
						for (int i = 0; i < 3; i++)
							SpawnBomb(2);
					//if (BombtastropheConfigClient.Instance.spawn.ground)
					//	for (int i = 0; i < 2; i++)
					//		SpawnBomb(3);
				}
				foreach (Explosive bomb in BombtastropheConfigClient.Instance.buildABomb)
				{
					if (bomb.enabled)
					{
						float chance = (float)bomb.spawnChance.chance;
						chance *= bomb.spawnChance.chanceMult;
						chance *= bomb.spawnChance.chanceMult2;
						chance *= bomb.spawnChance.chanceMult3;
						if (bomb.spawn == "On Player" && Main.rand.NextFloat(100f) <= chance)
							SpawnBuildABomb(0, bomb);
						if (bomb.spawn == "On Enemies")
							SpawnBuildABomb(1, bomb);
						if (bomb.spawn == "From Sky" && Main.rand.NextFloat(100f) <= chance)
							SpawnBuildABomb(2, bomb);
						//if (bomb.spawn == "From Ground" && Main.rand.NextFloat(100f) <= chance)
						//	SpawnBuildABomb(3, bomb);
					}
				}
			}
		}

		public override void ModifyHitNPCWithProj(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			/*if (ModContent.GetInstance<BombtastropheConfigClient>().explosiveHits && !(target.life <= 0))
			{
				Projectile proj = Main.projectile[Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Explosion>(), 0, 0f, Player.whoAmI)];
				proj.width = (proj.height = (target.width + target.width / 2));
				if (target.height > target.width)
					proj.width = (proj.height = (target.Height + target.height / 2));
			}*/
		}

		public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
			if (proj.type == ModContent.ProjectileType<Grenade>()
				|| proj.type == ModContent.ProjectileType<Bomb>()
				|| proj.type == ModContent.ProjectileType<Dynamite>()
				|| proj.type == ModContent.ProjectileType<AnnihilationPellet>()
				|| proj.type == ModContent.ProjectileType<Bombaggot>())
				ExplosionHitBy = proj;
		}

		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (angelTimer > 0)
            {
				Player.statLife = Player.statLifeMax2;
				return false;
			}
			if (Player.whoAmI == Main.myPlayer)
            {
				bombsKilledBy++;
				if (ExplosionHitBy != null)
				{
					int count = 0;
					int max = 0;
					//if (BombtastropheConfigClient.Instance.codes.codeUI.code2.code != "[c/828282:??????????]")
					//	count++;
					//max++;
					if (BombtastropheConfigClient.Instance.codes.codeUI.code3.code == "ultramerge")
						count++;
					max++;
					//if (BombtastropheConfigClient.Instance.codes.codeUI.code4.code != "[c/828282:??????????]")
					//	count++;
					//max++;
					//if (BombtastropheConfigClient.Instance.codes.codeUI.code5.code != "[c/828282:??????????]")
					//	count++;
					//max++;
					//if (BombtastropheConfigClient.Instance.codes.codeUI.code6.code != "[c/828282:??????????]")
					//	count++;
					//max++;
					if (BombtastropheConfigClient.Instance.codes.codeUI.code7.code != "[c/828282:??????????]")
						count++;
					max++;
					//if (BombtastropheConfigClient.Instance.codes.codeUI.code8.code != "[c/828282:??????????]")
					//	count++;
					//max++;
					if (BombtastropheConfigClient.Instance.codes.codeUI.code9.code != "[c/828282:??????????]")
						count++;
					max++;
					if (BombtastropheConfigClient.Instance.codes.codeUI.code10.code != "[c/828282:??????????]")
						count++;
					max++;
					if (BombtastropheConfigClient.Instance.codes.codeUI.code12.code != "[c/828282:??????????]")
						count++;
					max++;
					//if (BombtastropheConfigClient.Instance.codes.codeUI.code13.code != "[c/828282:??????????]")
					//	count++;
					//max++;
					if (count == max - 1)
                    {
						if (bombsKilledBy >= 1000)
                        {
							angelTimer = 1;
							bombsKilledBy = 0;
							Main.NewText("Something divine is approaching...", 255, 255, 0);
							Player.statLife = Player.statLifeMax2;
							return false;
						}
						else
							damageSource = PlayerDeathReason.ByCustomReason(Player.name + Main.rand.Next(new string[] { " did not see the bloody bombs", " was not a good demolitionist", " got caught in the blast wave", " did not account for the fact that explosives could roll", ("'s limbs were scattered across " + Main.worldName), " should have chosen the bullet hole", " did not factor in the blast radius", " forgot the rope coil and gel", " made a big mistake", (" likes hugging " + ExplosionHitBy.Name), (" attempted to pick up a " + ExplosionHitBy.Name), " blew up" }));
					}
					else if ((Main.rand.Next(20) == 0 || bombsKilledBy >= 20) && BombtastropheConfigClient.Instance.codes.deathCodes && (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && count != max))
					{
						var code = new WeightedRandom<string>();
						if (Main.rand.Next(5) == 0)
						{
							if (Main.rand.Next(10) == 0)
							{
								//if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code2.code == "[c/828282:??????????]")
								//	code.Add("betactical");
								//if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code4.code == "[c/828282:??????????]")
								//	code.Add("rocketsilo");
								//if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code5.code == "[c/828282:??????????]")
								//	code.Add("protective");
								//if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code6.code == "[c/828282:??????????]")
								//	code.Add("stormnade!");
								if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code7.code == "[c/828282:??????????]")
									code.Add("annihilate");
								//if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code8.code == "[c/828282:??????????]")
								//	code.Add("overgrowth");
								if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code9.code == "[c/828282:??????????]")
									code.Add("bombmuseum");
								if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code10.code == "[c/828282:??????????]")
									code.Add("dynamight!");
								if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code12.code == "[c/828282:??????????]")
									code.Add("bombaggot!");
								//if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code13.code == "[c/828282:??????????]")
								//	code.Add("parasitic!");
							}
							else
                            {
								string[] list = new string[] { "betac", "tical" };
								/*if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code2.code == "[c/828282:??????????]")
									for (int i = 0; i < list.Length; i++)
										code.Add(list[i]);*/
								/*list = new string[] { "rocke", "tsilo" };
								if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code4.code == "[c/828282:??????????]")
									for (int i = 0; i < list.Length; i++)
										code.Add(list[i]);
								list = new string[] { "prote", "ctive" };
								if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code5.code == "[c/828282:??????????]")
									for (int i = 0; i < list.Length; i++)
										code.Add(list[i]);
								list = new string[] { "storm", "nade!" };
								if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code6.code == "[c/828282:??????????]")
									for (int i = 0; i < list.Length; i++)
										code.Add(list[i]);*/
								list = new string[] { "annih", "ilate" };
								if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code7.code == "[c/828282:??????????]")
									for (int i = 0; i < list.Length; i++)
										code.Add(list[i]);
								/*list = new string[] { "overg", "rowth" };
								if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code8.code == "[c/828282:??????????]")
									for (int i = 0; i < list.Length; i++)
										code.Add(list[i]);*/
								list = new string[] { "bombm", "useum" };
								if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code9.code == "[c/828282:??????????]")
									for (int i = 0; i < list.Length; i++)
										code.Add(list[i]);
								list = new string[] { "dynam", "ight!" };
								if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code10.code == "[c/828282:??????????]")
									for (int i = 0; i < list.Length; i++)
										code.Add(list[i]);
								list = new string[] { "bomba", "ggot!" };
								if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code12.code == "[c/828282:??????????]")
									for (int i = 0; i < list.Length; i++)
										code.Add(list[i]);
								/*list = new string[] { "paras", "itic!" };
								if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code13.code == "[c/828282:??????????]")
									for (int i = 0; i < list.Length; i++)
										code.Add(list[i]);*/
							}
                        }
						else
                        {
							string[] list = new string[] { "be", "ta", "ct", "ic", "al" };
							/*if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code2.code == "[c/828282:??????????]")
								for (int i = 0; i < list.Length; i++)
									code.Add(list[i]);*/
							/*list = new string[] { "ro", "ck", "et", "si", "lo" };
							if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code4.code == "[c/828282:??????????]")
								for (int i = 0; i < list.Length; i++)
									code.Add(list[i]);
							list = new string[] { "pr", "ot", "ec", "ti", "ve" };
							if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code5.code == "[c/828282:??????????]")
								for (int i = 0; i < list.Length; i++)
									code.Add(list[i]);
							list = new string[] { "st", "or", "mn", "ad", "e!" };
							if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code6.code == "[c/828282:??????????]")
								for (int i = 0; i < list.Length; i++)
									code.Add(list[i]);*/
							list = new string[] { "an", "ni", "hi", "la", "te" };
							if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code7.code == "[c/828282:??????????]")
								for (int i = 0; i < list.Length; i++)
									code.Add(list[i]);
							/*list = new string[] { "ov", "er", "gr", "ow", "th" };
							if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code8.code == "[c/828282:??????????]")
								for (int i = 0; i < list.Length; i++)
									code.Add(list[i]);*/
							list = new string[] { "bo", "mb", "mu", "se", "um" };
							if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code9.code == "[c/828282:??????????]")
								for (int i = 0; i < list.Length; i++)
									code.Add(list[i]);
							list = new string[] { "dy", "na", "mi", "gh", "t!" };
							if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code10.code == "[c/828282:??????????]")
								for (int i = 0; i < list.Length; i++)
									code.Add(list[i]);
							list = new string[] { "bo", "mb", "ag", "go", "t!" };
							if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code12.code == "[c/828282:??????????]")
								for (int i = 0; i < list.Length; i++)
									code.Add(list[i]);
							/*list = new string[] { "pa", "ra", "si", "ti", "c!" };
							if (!BombtastropheConfigClient.Instance.codes.blacklistDiscovered || BombtastropheConfigClient.Instance.codes.blacklistDiscovered && BombtastropheConfigClient.Instance.codes.codeUI.code13.code == "[c/828282:??????????]")
								for (int i = 0; i < list.Length; i++)
									code.Add(list[i]);*/
						}
						string codeGiven = code;
						damageSource = PlayerDeathReason.ByCustomReason(codeGiven.Length == 2f
							? (Main.rand.Next(new string[] { "While in " + Main.rand.Next(new string[] { "bomb heaven", "the bomb underworld" }) + ", a random bomb said '[c/00FF00:" + codeGiven + "]' to " + Player.name + " and refused to elaborate",
							Player.name + " blew up, causing " + (Player.Male ? "him" : "her") + " to end up in " + Main.rand.Next(new string[] { "bomb heaven", "the bomb underworld" }) + ", and while there, " + (Player.Male ? "" : "s") + "he overheard a bomb saying '[c/00FF00:" + codeGiven  + "]'",
							Player.name + " found " + (Player.Male ? "him" : "her") + "self in " + Main.rand.Next(new string[] { "bomb heaven", "the bomb underworld" }) + " and found '[c/00FF00:" + codeGiven + "]' etched into the plaque of a fountain",
							Player.name + " woke up in " + Main.rand.Next(new string[] { "bomb heaven and saw a cloud", "the bomb underworld and saw smoke" }) + " in the shape of '[c/00FF00:" + codeGiven  + "]'",
							"While in " + Main.rand.Next(new string[] { "bomb heaven", "the bomb underworld" } ) +", '[c/00FF00:" + codeGiven  + "]' kept appearing whenever " + Player.name + " blinked" } ))
							: (codeGiven.Length == 5f
							? (Main.rand.Next(new string[] { Player.name + " went to " + Main.rand.Next(new string[] { "bomb heaven", "the bomb underworld" }) + " and found a forgotten book about legendary explosives and found '[c/00FF00:" + codeGiven + "]' written in a hidden page",
							Player.name + " ended up in " + Main.rand.Next(new string[] { "bomb heaven's highest cloud and found a wise angel", "the bomb underworld's deepest layer and found a wise demon" }) + ", who told " + Player.name + " that '[c/00FF00:" + codeGiven + "]' may be able to unlock ancient secrets" } ))
							: (Player.name + " discovered an ancient bomb with '[c/00FF00:" + codeGiven + "]' inscribed on it while in " + Main.rand.Next(new string[] { "bomb heaven", "the bomb underworld" }))));
						bombsKilledBy = 0;
					}
					else
						damageSource = PlayerDeathReason.ByCustomReason(Player.name + Main.rand.Next(new string[] { " did not see the bloody bombs", " was not a good demolitionist", " got caught in the blast wave", " did not account for the fact that explosives could roll", ("'s limbs were scattered across " + Main.worldName), " should have chosen the bullet hole", " did not factor in the blast radius", " forgot the rope coil and gel", " made a big mistake", (" likes hugging " + ExplosionHitBy.Name), (" attempted to pick up a " + ExplosionHitBy.Name), " blew up" }));
				}
			}
			return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
		}

		private void SpawnBomb(int spawnType)
        {
			string bombType = ModContent.GetInstance<BombtastropheConfigClient>().explosiveType;
			if (bombType == "Mystery Explosive")
				bombType = Main.rand.Next(new string[] { "Grenade", "Bomb", "Dynamite" });
			string modifierType = ModContent.GetInstance<BombtastropheConfigClient>().modifierType;
			if (modifierType == "Mysterious")
				modifierType = ExplosiveUtils.RollForModifier();
			else if (modifierType == "Random")
				modifierType = Main.rand.Next(new string[] { "None", /*"Sticky",*/ "Bouncy" });
			if (spawnType == 0)
			{
				Projectile proj = Main.projectile[Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, new Vector2(Main.rand.NextFloat(ModContent.GetInstance<BombtastropheConfigClient>().bombSpread, -ModContent.GetInstance<BombtastropheConfigClient>().bombSpread), -8f), Mod.Find<ModProjectile>(bombType).Type, 0, 0f, Player.whoAmI)];
				if (proj.ModProjectile is Projectiles.Explosive explosive)
				{
					explosive.modifiers[0] = modifierType;
					explosive.modifiers[1] = "None";
					if (ModContent.GetInstance<BombtastropheConfigClient>().explosivesDestroy)
						explosive.destructive = true;
				}
			}
			if (spawnType == 1)
			{
				int numOfEnemies = 0;
				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].type != 488)
						numOfEnemies++;
				}
				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].type != 488 && Main.rand.NextFloat(100f) <= (100f / (((float)numOfEnemies) / 2f)))
					{
						Projectile proj = Main.projectile[Projectile.NewProjectile(Main.npc[i].GetSource_FromThis(), Main.npc[i].Center, new Vector2(Main.rand.NextFloat(ModContent.GetInstance<BombtastropheConfigClient>().bombSpread, -ModContent.GetInstance<BombtastropheConfigClient>().bombSpread), -8f), Mod.Find<ModProjectile>(bombType).Type, 0, 0f, Player.whoAmI)];
						if (proj.ModProjectile is Projectiles.Explosive explosive)
						{
							explosive.modifiers[0] = modifierType;
							explosive.modifiers[1] = "None";
							if (ModContent.GetInstance<BombtastropheConfigClient>().explosivesDestroy)
								explosive.destructive = true;
						}
					}
				}
			}
			if (spawnType == 2)
			{
				//if (Main.rand.Next((int)(500f * BombtastropheConfigClient.Instance.bombSpawnRate) + 1) == 0)
				//	bombType = "WrappedCode";
				Projectile proj = Main.projectile[Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + new Vector2(Main.rand.NextFloat(-800f, 800f), Main.rand.NextFloat(-900f, -800f)), new Vector2(Main.rand.NextFloat(ModContent.GetInstance<BombtastropheConfigClient>().bombSpread, -ModContent.GetInstance<BombtastropheConfigClient>().bombSpread), 8f), Mod.Find<ModProjectile>(bombType).Type, 0, 0f, Player.whoAmI)];
				if (proj.ModProjectile is Projectiles.Explosive explosive)
				{
					explosive.modifiers[0] = modifierType;
					explosive.modifiers[1] = "None";
					if (ModContent.GetInstance<BombtastropheConfigClient>().explosivesDestroy)
						explosive.destructive = true;
				}
			}
			/*if (spawnType == 3)
            {
				Vector2 screenDimensions = new Vector2(124, 70);
				Vector2 tilePos = new Vector2((float)(Player.Center.X / 16) + (float)(screenDimensions.X / 2) / 16, (float)(Player.Center.Y / 16) + (screenDimensions.Y / 2));
				if (tilePos.X < 0)
                {
					screenDimensions.X += tilePos.X;
					tilePos.X = 0;
				}
				if (tilePos.X > Main.maxTilesX)
				{
					screenDimensions.X -= tilePos.X - Main.maxTilesX;
					tilePos.X = Main.maxTilesX;
				}
				if (tilePos.Y < 0)
					tilePos.Y = 0;
				if (tilePos.Y > Main.maxTilesY)
					tilePos.Y = Main.maxTilesY;
				bool GoUp = true;
				for (int i = 0; i < screenDimensions.Y; i++)
                {
					Tile tile = Main.tile[(int)tilePos.X, (int)tilePos.Y];
					if (tile != null && tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
						tilePos.Y -= 16;
					else if (tilePos.Y >= (float)(Player.Center.Y / 16) || Main.rand.Next(4) == 0)
                    {
						spawnPos = tilePos * new Vector2(16, 16);
						break;
					}
				}
            }*/
		}


		private void SpawnBuildABomb(int spawnType, Explosive bomb)
		{
			string bombType = bomb.type;
			if (bombType == "Mystery Explosive") 
				bombType = Main.rand.Next(new string[] { "Grenade", "Bomb", "Dynamite" });
			bombType = bombType.Replace(" ", "");
			string[] modifierType = new string[] { bomb.modifiers[0], bomb.modifiers[1] };
			for (int i = 0; i < modifierType.Length; i++)
            {
				if (modifierType[i] == "Mysterious" || modifierType[i] == "Mysteriously Mysterious")
					modifierType[i] = ExplosiveUtils.RollForModifier(modifierType[i] == "Mysteriously Mysterious");
			}
			if (spawnType == 0)
            {
				Projectile proj = Main.projectile[Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, new Vector2(Main.rand.NextFloat(ModContent.GetInstance<BombtastropheConfigClient>().bombSpread, -ModContent.GetInstance<BombtastropheConfigClient>().bombSpread), -8f), Mod.Find<ModProjectile>(bombType).Type, 0, 0f, Player.whoAmI)];
				if (proj.ModProjectile is Projectiles.Explosive explosive)
				{
					explosive.modifiers[0] = modifierType[0];
					explosive.modifiers[1] = modifierType[1];
					if (explosive.destructive)
						explosive.destructive = true;
				}
			}
			if (spawnType == 1)
			{
				float chance = (float)bomb.spawnChance.chance;
				chance *= bomb.spawnChance.chanceMult;
				chance *= bomb.spawnChance.chanceMult2;
				chance *= bomb.spawnChance.chanceMult3;
				int numOfEnemies = 0;
				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].type != 488)
						numOfEnemies++;
				}
				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].type != 488 && Main.rand.NextFloat(100f) <= chance / (((float)numOfEnemies) / 2f))
                    {
						Projectile proj = Main.projectile[Projectile.NewProjectile(Main.npc[i].GetSource_FromThis(), Main.npc[i].Center, new Vector2(Main.rand.NextFloat(ModContent.GetInstance<BombtastropheConfigClient>().bombSpread, -ModContent.GetInstance<BombtastropheConfigClient>().bombSpread), -8f), Mod.Find<ModProjectile>(bombType).Type, 0, 0f, Player.whoAmI)];
						if (proj.ModProjectile is Projectiles.Explosive explosive)
						{
							explosive.modifiers[0] = modifierType[0];
							explosive.modifiers[1] = modifierType[1];
							if (explosive.destructive)
								explosive.destructive = true;
						}
					}
				}
			}
			if (spawnType == 2)
            {
				//if (Main.rand.Next(((int)(500f * BombtastropheConfigClient.Instance.bombSpawnRate) * BombtastropheConfigClient.Instance.buildABomb.Count) + 1) == 0)
				//	bombType = "WrappedCode";
				Projectile proj = Main.projectile[Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + new Vector2(Main.rand.NextFloat(-800f, 800f), Main.rand.NextFloat(-900f, -800f)), new Vector2(Main.rand.NextFloat(ModContent.GetInstance<BombtastropheConfigClient>().bombSpread, -ModContent.GetInstance<BombtastropheConfigClient>().bombSpread), 8f), Mod.Find<ModProjectile>(bombType).Type, 0, 0f, Player.whoAmI)];
				if (proj.ModProjectile is Projectiles.Explosive explosive)
				{
					explosive.modifiers[0] = modifierType[0];
					explosive.modifiers[1] = modifierType[1];
					if (explosive.destructive)
						explosive.destructive = true;
				}
			}
			/*if (spawnType == 3)
			{
				Vector2 screenDimensions = new Vector2(124, 70);
				Vector2 tilePos = new Vector2((float)(Player.Center.X / 16) + (float)(screenDimensions.X / 2) / 16, (float)(Player.Center.Y / 16) + (screenDimensions.Y / 2));
				if (tilePos.X < 0)
				{
					screenDimensions.X += tilePos.X;
					tilePos.X = 0;
				}
				if (tilePos.X > Main.maxTilesX)
				{
					screenDimensions.X -= tilePos.X - Main.maxTilesX;
					tilePos.X = Main.maxTilesX;
				}
				if (tilePos.Y < 0)
					tilePos.Y = 0;
				if (tilePos.Y > Main.maxTilesY)
					tilePos.Y = Main.maxTilesY;
				bool GoUp = true;
				for (int i = 0; i < screenDimensions.Y; i++)
				{
					Tile tile = Main.tile[(int)tilePos.X, (int)tilePos.Y];
					if (tile != null && tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
						tilePos.Y -= 16;
					else if (tilePos.Y >= (float)(Player.Center.Y / 16) || Main.rand.Next(4) == 0)
					{
						spawnPos = tilePos * new Vector2(16, 16);
						break;
					}
				}
			}*/
		}
	}
}