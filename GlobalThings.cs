using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.DataStructures;

namespace BombtastropheMod
{
	public class BombtastropheGlobalTile : GlobalTile
	{
		public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (type == 28 && Main.tile[i, j].TileFrameX % 36 == 0 && Main.tile[i, j].TileFrameY % 36 == 0)
            {
				foreach (Explosive bomb in BombtastropheConfigClient.Instance.buildABomb)
				{
					string bombType = bomb.type;
					if (bombType == "Mystery Explosive")
						bombType = Main.rand.Next(new string[] { "Grenade", "Bomb", "Dynamite" });
					bombType = bombType.Replace(" ", "");
					string[] modifierType = new string[] { bomb.modifiers[0], bomb.modifiers[1] };
					for (int a = 0; a < modifierType.Length; a++)
					{
						if (modifierType[a] == "Mysterious" || modifierType[a] == "Mysteriously Mysterious")
							modifierType[a] = ExplosiveUtils.RollForModifier(modifierType[a] == "Mysteriously Mysterious");
					}
					float chance = (float)bomb.spawnChance.chance;
					chance *= bomb.spawnChance.chanceMult;
					chance *= bomb.spawnChance.chanceMult2;
					chance *= bomb.spawnChance.chanceMult3;
					if (bomb.enabled && bomb.spawn == "From Breaking Pots" && Main.rand.NextFloat(100f) <= chance)
					{
						Projectile proj = Main.projectile[Projectile.NewProjectile(new EntitySource_TileBreak(i, j), new Vector2(i * 16 + 16, j * 16 + 8), new Vector2(Main.rand.NextFloat(ModContent.GetInstance<BombtastropheConfigClient>().bombSpread / 2f, -ModContent.GetInstance<BombtastropheConfigClient>().bombSpread / 2f), -4f), Mod.Find<ModProjectile>(bombType).Type, 0, 0f, Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16))];
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
		}
	}
}
