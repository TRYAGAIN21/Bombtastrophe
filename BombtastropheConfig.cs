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

namespace BombtastropheMod
{
	[Label("Bombtastrophe Config")]
	public class BombtastropheConfigClient : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;
		public static BombtastropheConfigClient Instance => ModContent.GetInstance<BombtastropheConfigClient>();

		[Header("Important Info")]
		[OptionStrings(new string[] { "Hover over this for info" })]
		[DefaultValue("Hover over this for info")]
		[Label("Reset Info")]
		[Tooltip("If you want to reset all settings, use the 'Reset' preset located at the bottom of this config" +
"\nUsing 'Restore Defaults' will cause Build-A-Bomb to reset and Found Codes' progress to be reset")]
		public string des;
		[OptionStrings(new string[] { "Hover over this for info" })]
		[DefaultValue("Hover over this for info")]
		[Label("Command Info")]
		[Tooltip("If you want a description of a modifier or bomb type, use the 'des' command" +
            "\nCommand Usage: /des <Modifier|Bomb>")]
		public string des2;

		[Header("Main Settings")]

		[OptionStrings(new string[] { "Grenade", "Bomb", "Dynamite", "Mystery Explosive" })]
		[DefaultValue("Grenade")]
		[Label("Explosive Type")]
		public string explosiveType;

		[OptionStrings(new string[] { "None", /*"Sticky",*/ "Bouncy", "Gravity-defying", "Fragmenting", /*"Teleporting",*/ "Stealthy", "Heat-Seeking", "Sandy", "Lingering", "Volatile", "Speedy", "Huge", "Short-fused", "Fiery", /*"Shocking",*/ "Happy", "Heavy", "Chilly", "Mysterious" })]
		[DefaultValue("None")]
		[Label("Modifier Type")]
		public string modifierType;

		[DefaultValue(false)]
		[Label("Explosives Destroy Tiles")]
		[Tooltip("Grenades ARE affected")]
		public bool explosivesDestroy;

		[Label("Spawn Location")]
		public SpawnLocations spawn = new SpawnLocations();

		[Range(0f, 8f)]
		[Increment(.5f)]
		[DefaultValue(2f)]
		[Label("Bomb Spread")]
		[Tooltip("Multiplies the max amount of horizontal speed explosives can spawn with")]
		public float bombSpread;

		[Range(0.25f, 10f)]
		[Increment(0.25f)]
		[DefaultValue(1f)]
		[Label("Bomb Spawn Rate")]
		[Tooltip("The higher the multiplier, the faster bombs spawn")]
		public float bombSpawnRate;

		[Header("Build-A-Bomb")]

		[DefaultValue(false)]
		[Label("Build-A-Bomb Only")]
		[Tooltip("Only Build-A-Bomb explosives will spawn")]
		public bool buildABombOnly;

		[SeparatePage]
		[BackgroundColor(255, 127, 0)]
		[Label("Build-A-Bomb")]
		[Tooltip("Any bombs created in here will be added to the list of spawnable bombs")]
		public List<Explosive> buildABomb = new List<Explosive>();

		[SeparatePage]
		[BackgroundColor(127, 0, 255)]
		[Label("Code Keypad: Found Codes")]
		public FoundCodes codes = new FoundCodes();

		[Header("Misc")]

		[DefaultValue(false)]
		[Label("Explosives Hurt Enemies")]
		public bool explosivesHurtFoes;

		/*[DefaultValue(false)]
		[Label("Projectiles Explode on Death")]
		[Tooltip("Only Enemy Projectiles will Explode")]
		public bool explosiveProjectiles;

		[DefaultValue(false)]
		[Label("Enemies Explode on Death")]
		public bool explosiveKills;

		[DefaultValue(false)]
		[Tooltip("These are smaller then 'Enemies Explode on death' Explosions")]
		[Label("Enemies Create Explosions when hit")]
		public bool explosiveHits;*/

		[Header("Presets")]

		/*[DefaultValue(false)]
		[Label("Seeds of Destruction")]
		public bool SeedsOfDestruction
		{
			get => explosiveType == "Grenade"
				&& modifierType == "Sticky"
				&& explosivesDestroy
				&& bombSpread == 0f
				&& bombSpawnRate == 0.75f
				&& !spawn.player
				&& !spawn.enemy
				&& !spawn.sky
				&& spawn.ground;
			set
			{
				if (value)
				{
					explosiveType = "Grenade";
					modifierType = "Sticky";
					explosivesDestroy = true;
					bombSpread = 0f;
					bombSpawnRate = 0.75f;
					spawn.ground = true;
					spawn.player = (spawn.enemy = (spawn.sky = false));
				}
			}
		}*/

		[DefaultValue(false)]
		[Label("Space Attack")]
		public bool SpaceAttack
		{
			get => explosiveType == "Grenade"
				&& modifierType == "Anti-Gravity"
				&& !explosivesDestroy
				&& bombSpread == 4f
				&& bombSpawnRate == 4.5f
				&& !spawn.player
				&& !spawn.enemy
				&& spawn.sky
				/*&& !spawn.ground*/;
			set
			{
				if (value)
				{
					explosiveType = "Grenade";
					modifierType = "Anti-Gravity";
					explosivesDestroy = false;
					bombSpread = 4f;
					bombSpawnRate = 4.5f;
					spawn.sky = true;
					spawn.player = (spawn.enemy = /*(spawn.ground =*/ false/*)*/);
				}
			}
		}

		[DefaultValue(false)]
		[Label("1.3 Bombtastrophe")]
		[Tooltip("The 1.3 experience...")]
		public bool OnePointThreeBombtastrophe
		{
			get => explosiveType == "Bomb"
				&& modifierType == "Random"
				&& explosivesDestroy
				&& bombSpread == 2f
				&& bombSpawnRate == 5f
				&& spawn.player
				&& spawn.enemy
				&& spawn.sky
				/*&& !spawn.ground*/;
			set
			{
				if (value)
				{
					explosiveType = "Bomb";
					modifierType = "Random";
					explosivesDestroy = true;
					bombSpread = 2f;
					bombSpawnRate = 5f;
					spawn.player = (spawn.enemy = (spawn.sky = true));
					//spawn.ground = false;
				}
			}
		}

		[DefaultValue(false)]
		[Label("Reset")]
		[Tooltip("Acts like the 'Restore Defaults' button, but doesn't erase Build-a-Bomb or your secret code progress!")]
		public bool Reset
		{
			get => false;
			set
			{
				if (value)
				{
					explosiveType = "Grenade";
					modifierType = "None";
					explosivesDestroy = false;
					bombSpread = 2f;
					bombSpawnRate = 1f;
					spawn = new SpawnLocations();
					buildABombOnly = false;
					explosivesHurtFoes = false;
					//explosiveProjectiles = false;
					//explosiveKills = false;
					//explosiveHits = false;
				}
			}
		}
	}

	public class SpawnLocations
	{
		private bool enemyBackingField;
		private bool skyBackingField;
		//private bool groundBackingField;
		private bool playerBackingField;

		[Label("On Player")]
		public bool player
		{
			get => playerBackingField || !enemyBackingField && !skyBackingField /*&& !groundBackingField*/;
			set
			{
				playerBackingField = value;
			}
		}
		[Label("On Enemies")]
		public bool enemy
		{
			get => enemyBackingField;
			set
			{
				if (!enemyBackingField && !skyBackingField /*&& !groundBackingField*/)
					playerBackingField = true;
				enemyBackingField = value;
			}
		}
		[Label("From Sky")]
		public bool sky
		{
			get => skyBackingField;
			set
			{
				if (!enemyBackingField && !skyBackingField /*&& !groundBackingField*/)
					playerBackingField = true;
				skyBackingField = value;
			}
		}
		/*[Label("From Ground")]
		public bool ground
		{
			get => groundBackingField;
			set
			{
				if (!enemyBackingField && !skyBackingField && !groundBackingField)
					playerBackingField = true;
				groundBackingField = value;
			}
        }*/
    }

	public class Code
	{
		[OptionStrings(new string[] { "Hey, you shouldn't be here." })]
		[DefaultValue("Hey, you shouldn't be here.")]
		[Label(" ")]
		public string des;

		[OptionStrings(new string[] { "[c/828282:??????????]" })]
		[DefaultValue("[c/828282:??????????]")]
		[Tooltip("Warning: Modifying this will cause the config to think this code hasn't been found")]
		[Label("Code")]
		public string code;

		public override string ToString() => code;
	}

	public class FoundCodes
	{
		[JsonIgnore]
		[OptionStrings(new string[] { " ", " ", " " })]
		public string notify_Hidden;

		[Label("Codes Found")]
		public CodeUI codeUI = new CodeUI();

		[Header("Settings")]
		[Label("Parts of codes have a chance to appear in death messages (Recommended)")]
		[DefaultValue(true)]
		public bool deathCodes;

		[Label("Prevent already discovered codes from appearing in death messages")]
		[DefaultValue(true)]
		public bool blacklistDiscovered;

		[Header("Code Keypad")]
		[BackgroundColor(127, 0, 255)]
		[OptionStrings(new string[] { "Hover over this for info" })]
		[DefaultValue("Hover over this for info")]
		[Label(" ")]
		[Tooltip("Try for a code in this code keypad to reveal the code here permanently\nNote: Codes inputted in Build-A-Bomb's Code Keypad will not update this")]
		public string des;

		[BackgroundColor(127, 0, 255)]
		[Label("Type code here")]
		[Tooltip("Case insensitive")]
		[DefaultValue("")]
		public string key;

		[BackgroundColor(127, 0, 255)]
		[OptionStrings(new string[] { " ", " ", " " })]
		[Label(" ")]
		[Tooltip("Notifications")]
		public string notify => notify_Hidden;

		[BackgroundColor(127, 0, 255)]
		[Label("Press this to enter code")]
		[Tooltip("A correct code causes it to be added to here\nEvery code is 10 characters long and is case insensitive\nThere are multiple codes, so try to find them all!")]
		public bool keypadEnter
		{
			get => false;
			set
			{
				if (value)
				{
					bool unsuccessful = false;
					string code = key;
					code = code.ToLower();
					if (code.Length != 10f)
					{
						notify_Hidden = "[c/FFFFFF:Code must be 10 characters long]";
						unsuccessful = true;
					}
					else if (code != "1.4release"
						//&& code != "stormnade!"
						//&& code != "rocketsilo"
						//&& code != "betactical"
						//&& code != "protective"
						&& code != "ultramerge"
						&& code != "annihilate"
						//&& code != "overgrowth"
						&& code != "bombmuseum"
						&& code != "dynamight!"
						&& code != "bombaggot!"
						&& code != "xplode-mas"
						&& code != "successful"
						&& code != "secretcode"
						&& code != "thisexists")
					{
						notify_Hidden = "[c/FFFFFF:Invalid Code]";
						unsuccessful = true;
					}
					if (!unsuccessful)
					{
						int count = 0;
						int max = 0;
						/*if (code2.code != "[c/828282:??????????]")
							count++;
						max++;*/
						/*if (code4.code != "[c/828282:??????????]")
							count++;
						max++;
						if (code5.code != "[c/828282:??????????]")
							count++;
						max++;
						if (code6.code != "[c/828282:??????????]")
							count++;
						max++;*/
						if (codeUI.code7.code != "[c/828282:??????????]")
							count++;
						max++;
						/*if (code8.code != "[c/828282:??????????]")
							count++;
						max++;*/
						if (codeUI.code9.code != "[c/828282:??????????]")
							count++;
						max++;
						if (codeUI.code10.code != "[c/828282:??????????]")
							count++;
						max++;
						if (codeUI.code12.code != "[c/828282:??????????]")
							count++;
						max++;
						/*if (code13.code != "[c/828282:??????????]")
							count++;
						max++;*/
						notify_Hidden = "[c/FFFFFF:Code has already been added]";
						if (code == "1.4release" && codeUI.code1.code != code)
						{
							codeUI.code1.code = code;
							notify_Hidden = "[c/FFFFFF:Successful! '" + code + "' added]";
						}
						/*if (code == "betactical" && codeUI.code2.code != code)
						{
							codeUI.code2.code = code;
							notify_Hidden = "[c/FFFFFF:Successful! '" + code + "' added]";
						}*/
						if (code == "ultramerge" && codeUI.code3.code != code)
						{
							if (count < max)
								notify_Hidden = "[c/FFFFFF:You are not yet worthy of this...]";
							else
                            {
								codeUI.code3.code = code;
								notify_Hidden = "[c/FFFFFF:Successful! '" + code + "' added]";
								if (codeUI.code14.code != "[c/828282:??????????]")
									notify_Hidden = "[c/FFFFFF:'" + code + "' added, '" + codeUI.code14.code + "' unlocked xmas stuff]";
							}
						}
						/*if (code == "rocketsilo" && codeUI.code4.code != code)
						{
							codeUI.code4.code = code;
							notify_Hidden = "[c/FFFFFF:Successful! '" + code + "' added]";
						}
						if (code == "protective" && codeUI.code5.code != code)
						{
							codeUI.code5.code = code;
							notify_Hidden = "[c/FFFFFF:Successful! '" + code + "' added]";
						}
						if (code == "stormnade!" && codeUI.code6.code != code)
						{
							codeUI.code6.code = code;
							notify_Hidden = "[c/FFFFFF:Successful! '" + code + "' added]";
						}*/
						if (code == "annihilate" && codeUI.code7.code != code)
						{
							codeUI.code7.code = code;
							notify_Hidden = "[c/FFFFFF:Successful! '" + code + "' added]";
						}
						/*if (code == "overgrowth" && codeUI.code8.code != code)
						{
							codeUI.code8.code = code;
							notify_Hidden = "[c/FFFFFF:Successful! '" + code + "' added]";
						}*/
						if (code == "bombmuseum" && codeUI.code9.code != code)
						{
							codeUI.code9.code = code;
							notify_Hidden = "[c/FFFFFF:Successful! '" + code + "' added]";
						}
						if (code == "dynamight!" && codeUI.code10.code != code)
						{
							codeUI.code10.code = code;
							notify_Hidden = "[c/FFFFFF:Successful! '" + code + "' added]";
						}
						if (code == "bombaggot!" && codeUI.code12.code != code)
						{
							codeUI.code12.code = code;
							notify_Hidden = "[c/FFFFFF:Successful! '" + code + "' added]";
						}
						/*if (code == "parasitic!" && codeUI.code13.code != code)
						{
							codeUI.code13.code = code;
							notify_Hidden = "[c/FFFFFF:Successful! '" + code + "' added]";
						}*/
						if (code == "xplode-mas" && codeUI.code14.code != code)
						{
							codeUI.code14.code = code;
							notify_Hidden = "[c/FFFFFF:'" + code + "' added, xmas for '" + (codeUI.code3.code == "ultramerge" ? codeUI.code3.code : ("]" + codeUI.code3.code + "[c/FFFFFF:")) + "' unlocked]";
						}
						if (code == "successful")
							notify_Hidden = "[c/FFFFFF:'" + key + "' successfully succeeded!]";
						if (code == "secretcode")
						{
							notify_Hidden = "[c/FFFFFF:Yep, sure is a secret code.]";
							if (codeUI.code11.code != code)
                            {
								codeUI.code11.code = code;
								notify_Hidden = "[c/FFFFFF:Yep, sure is a secret code. '" + code + "' added]";
							}
						}
						if (code == "thisexists")
							notify_Hidden = "[c/FFFFFF:It really does exist...]";
					}
				}
			}
		}
    }

	public class CodeUI
	{
		[SeparatePage]
		[BackgroundColor(0, 0, 0)]
		[Label("Code")]
		[Tooltip("This is a super secret code, which do not appear in death messages, you must find out what it is a different way\nHint: Fulfill the dreams of 1000 bombs, but only once you have the knowledge of every message...")]
		public Code code3 = new Code();
		[SeparatePage]
		[BackgroundColor(0, 0, 0)]
		[Label("Code")]
		[Tooltip("This is a super secret code, which do not appear in death messages, you must find out what it is a different way\nHint: Descriptions may contain helpful update info")]
		public Code code1 = new Code();
		/*[SeparatePage]
		[Label("Code")]
		public Code code2 = new Code();*/
		/*[SeparatePage]
		[Label("Code")]
		public Code code4 = new Code();
		[SeparatePage]
		[Label("Code")]
		public Code code5 = new Code();
		[SeparatePage]
		[Label("Code")]
		public Code code6 = new Code();*/
		[SeparatePage]
		[Label("Code")]
		public Code code7 = new Code();
		/*[SeparatePage]
		[Label("Code")]
		public Code code8 = new Code();*/
		[SeparatePage]
		[Label("Code")]
		public Code code9 = new Code();
		[SeparatePage]
		[Label("Code")]
		public Code code10 = new Code();
		[SeparatePage]
		[BackgroundColor(0, 0, 0)]
		[Label("Code")]
		[Tooltip("This is a super secretcode, which do not appear in death messages, you must find out what it is a different way\nHint: There are no typos")]
		public Code code11 = new Code();
		[SeparatePage]
		[Label("Code")]
		public Code code12 = new Code();
		/*[SeparatePage]
		[Label("Code")]
		public Code code13 = new Code();*/
		[SeparatePage]
		[BackgroundColor(0, 0, 0)]
		[Label("Code")]
		[Tooltip("This is a super secret code, which do not appear in death messages, you must find out what it is a different way\nHint: 'Tis the season to be jolly")]
		public Code code14 = new Code();

		public override string ToString()
		{
			int[] count = new int[2];
			int[] max = new int[2];
			if (code1.code != "[c/828282:??????????]")
				count[1]++;
			max[1]++;
			/*if (code2.code != "[c/828282:??????????]")
				count[0]++;
			max[0]++;*/
			if (code3.code == "ultramerge")
				count[1]++;
			max[1]++;
			/*if (code4.code != "[c/828282:??????????]")
				count++;
			max++;
			if (code5.code != "[c/828282:??????????]")
				count++;
			max++;
			if (code6.code != "[c/828282:??????????]")
				count++;
			max++;*/
			if (code7.code != "[c/828282:??????????]")
				count[0]++;
			max[0]++;
			/*if (code8.code != "[c/828282:??????????]")
				count++;
			max++;*/
			if (code9.code != "[c/828282:??????????]")
				count[0]++;
			max[0]++;
			if (code10.code != "[c/828282:??????????]")
				count[0]++;
			max[0]++;
			if (code11.code != "[c/828282:??????????]")
				count[1]++;
			max[1]++;
			if (code12.code != "[c/828282:??????????]")
				count[0]++;
			max[0]++;
			/*if (code13.code != "[c/828282:??????????]")
				count++;
			max++;*/
			if (code14.code != "[c/828282:??????????]")
				count[1]++;
			max[1]++;
			return count[0] + "/" + max[0] + " (" + (Math.Round((((float)count[0]) / (float)max[0]) * 100f)) + "%), Super Secret Codes Found: " + count[1] + "/" + max[1] + " (" + (Math.Round((((float)count[1]) / (float)max[1]) * 100f)) + "%)";
		}
	}

	public class SpawnChance
	{
		[Label("Base Spawn Chance")]
		[Range(1, 100)]
		[Increment(1)]
		[DefaultValue(100)]
		public int chance = 100;

		[Label("Multiplier")]
		[Tooltip("Multiplies spawn chance")]
		[Range(0.1f, 1f)]
		[Increment(0.1f)]
		[DefaultValue(1f)]
		public float chanceMult = 1f;
		[Label("Multiplier")]
		[Tooltip("Multiplies spawn chance")]
		[Range(0.1f, 1f)]
		[Increment(0.1f)]
		[DefaultValue(1f)]
		public float chanceMult2 = 1f;
		[Label("Multiplier")]
		[Tooltip("Multiplies spawn chance")]
		[Range(0.1f, 1f)]
		[Increment(0.1f)]
		[DefaultValue(1f)]
		public float chanceMult3 = 1f;

		public override string ToString()
		{
			float chance = (float)this.chance;
			chance *= chanceMult;
			chance *= chanceMult2;
			chance *= chanceMult3;
			return chance + "% (Click this to modify)";
		}
	}

	public class Explosive
	{
		[JsonIgnore]
		[OptionStrings(new string[] { " ", " ", " " })]
		public string notify_Hidden;

		[DefaultValue(true)]
		[Label("Enabled")]
		public bool enabled;

		[OptionStrings(new string[] { "Grenade", "Bomb", "Dynamite", "Mystery Explosive" })]
		[DefaultValue("Grenade")]
		[Label("Type")]
		public string type;

		[OptionStrings(new string[] { "None", /*"Sticky",*/ "Bouncy", "Gravity-defying", "Fragmenting", /*"Teleporting",*/ "Stealthy", "Heat-Seeking", "Sandy", "Lingering", "Volatile", "Speedy", "Huge", "Short-fused", "Fiery", /*"Shocking",*/ "Happy", "Heavy", "Chilly", "Mysterious" })]
		[Label("Modifier")]
		public string[] modifiers = new string[] { /*"Sticky",*/ "Bouncy", "None" };

		[DefaultValue(false)]
		[Label("Destructive")]
		[Tooltip("Ability to destroy tiles\nGrenades ARE affected")]
		public bool destructive;

		[DefaultValue("On Player")]
		[OptionStrings(new string[] { "On Player", "On Enemies", "From Sky", /*"From Ground"*/ })]
		[Label("Spawn Location")]
		public string spawn;

		[SeparatePage]
		[Label("Spawn Chance")]
		public SpawnChance spawnChance = new SpawnChance();

		[Header("Code Keypad")]
		[JsonIgnore]
		[BackgroundColor(127, 0, 255)]
		[Label("Type Code Here")]
		[Tooltip("Case insensitive")]
		[DefaultValue("")]
		public string key;

		[BackgroundColor(127, 0, 255)]
		[OptionStrings(new string[] { " ", " ", " " })]
		[Label(" ")]
		[Tooltip("Notifications")]
		public string notify => notify_Hidden;

		[BackgroundColor(127, 0, 255)]
		[Label("Press this to enter code")]
		[Tooltip("A correct code in Code Keypad will change this bomb's settings and give it a unique type or modifier\nEvery code is 10 characters long and is case insensitive\nThere are multiple codes, so try to find them all!")]
		public bool keypadEnter
		{
			get => false;
			set
			{
				if (value)
				{
					bool unsuccessful = false;
					string code = key;
					code = code.ToLower();
					if (code == "1.4release")
					{
						if (modifiers[0] != "None" && modifiers[1] == "None")
							modifiers[1] = "Confetti-filled";
						else
							modifiers[0] = "Confetti-filled";
						spawnChance.chance = 100;
						spawnChance.chanceMult = 1f;
						spawnChance.chanceMult2 = 1f;
						spawnChance.chanceMult3 = 1f;
					}
					/*else if (code == "stormnade!")
					{
						type = "Stormnade";
						spawn = "From Sky";
						spawnChance = 100;
						spawnChanceMult[0] = 1f;
						spawnChanceMult[1] = 1f;
						spawnChanceMult[2] = 1f;
					}
					else if (code == "antimatter")
					{
						type = "Antimatter Bomb";
						spawnChance = 1;
						spawnChanceMult[0] = 0.5f;
						spawnChanceMult[1] = 1f;
						spawnChanceMult[2] = 1f;
					}
					else if (code == "rocketsilo")
					{
						type = "Missile";
						spawnChance = 40;
						spawnChanceMult[0] = 1f;
						spawnChanceMult[1] = 1f;
						spawnChanceMult[2] = 1f;
					}
					else if (code == "betactical")
					{
						type = "Nuke";
						spawnChance = 1;
						spawnChanceMult[0] = 0.1f;
						spawnChanceMult[1] = 1f;
						spawnChanceMult[2] = 1f;
					}
					else if (code == "formidable")
					{
						type = "Formidabomb";
						spawnChance = 1;
						spawnChanceMult[0] = 0.1f;
						spawnChanceMult[1] = 0.5f;
						spawnChanceMult[2] = 1f;
					}
					else if (code == "protective")
					{
						if (modifiers[0] != "None" && modifiers[1] == "None")
							modifiers[1] = "Shielded";
						else
							modifiers[0] = "Shielded";
						spawnChance = 20;
						spawnChanceMult[0] = 1f;
						spawnChanceMult[1] = 1f;
						spawnChanceMult[2] = 1f;
					}
					else if (code == "petaxolotl")
					{
						if (modifiers[0] != "None" && modifiers[1] == "None")
							modifiers[1] = "Aquatic";
						else
							modifiers[0] = "Aquatic";
						spawnChance = 60;
						spawnChanceMult[0] = 1f;
						spawnChanceMult[1] = 1f;
						spawnChanceMult[2] = 1f;
					}*/
					else if (code == "annihilate")
					{
						type = "Annihilation Pellet";
						spawnChance.chance = 100;
						spawnChance.chanceMult = 1f;
						spawnChance.chanceMult2 = 1f;
						spawnChance.chanceMult3 = 1f;
					}
					/*else if (code == "overgrowth")
					{
						spawn = "From Shaking Trees";
						spawnChance = 7;
						spawnChanceMult[0] = 1f;
						spawnChanceMult[1] = 1f;
						spawnChanceMult[2] = 1f;
					}*/
					else if (code == "bombmuseum")
					{
						spawn = "From Breaking Pots";
						spawnChance.chance = 16;
						spawnChance.chanceMult = 1f;
						spawnChance.chanceMult2 = 1f;
						spawnChance.chanceMult3 = 1f;
					}
					else if (code == "dynamight!")
					{
						if (modifiers[0] != "None" && modifiers[1] == "None")
							modifiers[1] = "Mighty";
						else
							modifiers[0] = "Mighty";
						spawnChance.chance = 80;
						spawnChance.chanceMult = 1f;
						spawnChance.chanceMult2 = 1f;
						spawnChance.chanceMult3 = 1f;
					}
					else if (code == "bombaggot!")
					{
						type = "Bombaggot";
						spawnChance.chance = 60;
						spawnChance.chanceMult = 1f;
						spawnChance.chanceMult2 = 1f;
						spawnChance.chanceMult3 = 1f;
					}
					else if (code == "ultramerge")
					{
						if (ModContent.GetInstance<BombtastropheConfigClient>().codes.codeUI.code3.code != "ultramerge")
						{
							notify_Hidden = "[c/FFFFFF:You are not yet worthy of this...]";
							unsuccessful = true;
						}
						else if (modifiers[0] == "None" && modifiers[1] == "None")
						{
							modifiers[0] = "Unaltered";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Bouncy" && modifiers[1] == "Bouncy")
						{
							modifiers[0] = "Spring";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Fragmenting" && modifiers[1] == "Fragmenting")
						{
							modifiers[0] = "Cluster";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Stealthy" && modifiers[1] == "Stealthy")
						{
							modifiers[0] = "Invisible";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Sandy" && modifiers[1] == "Sandy")
						{
							modifiers[0] = "Dusty";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Lingering" && modifiers[1] == "Lingering")
						{
							modifiers[0] = "Long-lasting";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Speedy" && modifiers[1] == "Speedy")
						{
							modifiers[0] = "Supersonic";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Huge" && modifiers[1] == "Huge")
						{
							modifiers[0] = "Gigantic";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Shocking" && modifiers[1] == "Shocking")
						{
							modifiers[0] = "Electrocuting";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Fiery" && modifiers[1] == "Fiery")
						{
							modifiers[0] = "Blazing";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Mysterious" && modifiers[1] == "Mysterious")
						{
							modifiers[0] = "Mysteriously Mysterious";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Sticky" && modifiers[1] == "Bouncy" || modifiers[0] == "Bouncy" && modifiers[1] == "Sticky")
						{
							modifiers[0] = "Platform-Sticky Block-Bouncy";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Teleporting" && modifiers[1] == "Stealthy" || modifiers[0] == "Stealthy" && modifiers[1] == "Teleporting")
						{
							modifiers[0] = "Assassinating";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Sandy" && modifiers[1] == "Lingering" || modifiers[0] == "Lingering" && modifiers[1] == "Sandy")
						{
							modifiers[0] = "Messy";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Dusty" && modifiers[1] == "Lingering" || modifiers[0] == "Messy" && modifiers[1] == "Sandy")
						{
							modifiers[0] = "Soiled";
							modifiers[1] = "None";
						}
						/*#region Aquatic Combinations
						else if (modifiers[0] == "Aquatic" && modifiers[1] == "Gravity-defying" || modifiers[0] == "Gravity-defying" && modifiers[1] == "Aquatic")
						{
							modifiers[0] = "Pelican";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Aquatic" && modifiers[1] == "Shocking" || modifiers[0] == "Shocking" && modifiers[1] == "Aquatic" || modifiers[0] == "Electrocuting" && modifiers[1] == "Aquatic")
						{
							modifiers[0] = "Jellyfish";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Aquatic" && modifiers[1] == "Speedy" || modifiers[0] == "Speedy" && modifiers[1] == "Aquatic")
						{
							modifiers[0] = "Eel";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Eel" && modifiers[1] == "Shocking" || modifiers[0] == "Jellyfish" && modifiers[1] == "Speedy")
						{
							modifiers[0] = "Electric Eel";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Pelican" && modifiers[1] == "Fiery")
						{
							modifiers[0] = "Dragon";
							modifiers[1] = "Aquatic";
						}
						else if (modifiers[0] == "Aquatic" && modifiers[1] == "Stealthy" || modifiers[0] == "Stealthy" && modifiers[1] == "Aquatic")
						{
							modifiers[0] = "Submarine";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Submarine" && modifiers[1] == "Heat-Seeking")
						{
							modifiers[0] = "Torpedo";
							modifiers[1] = "None";
						}
						#endregion*/
						/*else if (modifiers[0] == "Fiery" && modifiers[1] == "Gravity-defying" || modifiers[0] == "Gravity-defying" && modifiers[1] == "Fiery")
						{
							modifiers[0] = "Dragon";
							modifiers[1] = "None";
						}*/
						else if (modifiers[0] == "Dragon" && modifiers[1] == "Fragmenting")
						{
							modifiers[0] = "Hydra";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Happy" && modifiers[1] == "Happy")
						{
							modifiers[0] = "Overjoyed";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Short-fused" && modifiers[1] == "Short-fused")
						{
							modifiers[0] = "Super Short-fused";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Heavy" && modifiers[1] == "Heavy")
						{
							modifiers[0] = "Dense";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Stealthy" && modifiers[1] == "Gravity-defying" || modifiers[0] == "Gravity-defying" && modifiers[1] == "Stealthy")
						{
							modifiers[0] = "Light Essence";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Light Essence" && modifiers[1] == "Heat-Seeking")
						{
							modifiers[0] = "Ghostly";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Heat-Seeking" && modifiers[1] == "Lingering" || modifiers[0] == "Lingering" && modifiers[1] == "Heat-Seeking")
						{
							modifiers[0] = "Zombified";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Zombified" && modifiers[1] == "Stealthy")
						{
							modifiers[0] = "Dark Essence";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Dark Essence" && modifiers[1] == "Gravity-defying" || modifiers[0] == "Ghostly" && modifiers[1] == "Lingering")
						{
							modifiers[0] = "Wrathful";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Chilly" && modifiers[1] == "Chilly")
						{
							modifiers[0] = "Freezing";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Freezing" && modifiers[1] == "Lingering")
						{
							modifiers[0] = "Hailstorm";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Chilly" && modifiers[1] == "Sandy" || modifiers[0] == "Sandy" && modifiers[1] == "Chilly")
						{
							modifiers[0] = "Snowy";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Snowy" && modifiers[1] == "Lingering" || modifiers[0] == "Messy" && modifiers[1] == "Chilly" || modifiers[0] == "Soiled" && modifiers[1] == "Chilly")
						{
							modifiers[0] = "Snowstorm";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Snowstorm" && modifiers[1] == "Chilly" || modifiers[0] == "Hailstorm" && modifiers[1] == "Sandy")
						{
							modifiers[0] = "Blizzard";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Blazing" && modifiers[1] == "Short-fused")
						{
							modifiers[0] = "Melting";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Zombified" && modifiers[1] == "Fragmenting")
						{
							modifiers[0] = "Infested";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Volatile" && modifiers[1] == "Volatile" || modifiers[0] == "Fragmenting" && modifiers[1] == "Volatile" || modifiers[0] == "Volatile" && modifiers[1] == "Fragmenting")
						{
							modifiers[0] = "Shattering";
							modifiers[1] = "None";
						}
						else if (modifiers[0] == "Happy" && modifiers[1] == "Chilly" || modifiers[0] == "Chilly" && modifiers[1] == "Happy")
						{
							if (ModContent.GetInstance<BombtastropheConfigClient>().codes.codeUI.code14.code == "[c/828282:??????????]")
                            {
								notify_Hidden = "[c/FFFFFF:Xmas code must be unlocked]";
								unsuccessful = true;
							}
							else
                            {
								modifiers[0] = "Merry";
								modifiers[1] = "None";
							}
						}
						else if (modifiers[0] == "Wrapped" && modifiers[1] == "Heavy" || modifiers[0] == "Heavy" && modifiers[1] == "Wrapped")
						{
							if (ModContent.GetInstance<BombtastropheConfigClient>().codes.codeUI.code14.code == "[c/828282:??????????]")
							{
								notify_Hidden = "[c/FFFFFF:Xmas code must be unlocked]";
								unsuccessful = true;
							}
							else
							{
								modifiers[0] = "Heavily Wrapped";
								modifiers[1] = "None";
							}
						}
						else
						{
							notify_Hidden = "[c/FFFFFF:Needs two compatible modifiers]";
							unsuccessful = true;
						}
					}
					else if (code == "successful")
						notify_Hidden = "[c/FFFFFF:'" + key + "' successfully succeeded!]";
					else if (code == "secretcode")
					{
						notify_Hidden = "[c/FFFFFF:Yep, sure is a secret code.]";
						unsuccessful = true;
					}
					else if (code == "thisexists")
					{
						notify_Hidden = "[c/FFFFFF:It really does exist...]";
						unsuccessful = true;
					}
					else if (code.Length != 10f)
					{
						notify_Hidden = "[c/FFFFFF:Code must be 10 characters long]";
						unsuccessful = true;
					}
					else
                    {
						notify_Hidden = "[c/FFFFFF:Invalid Code]";
						unsuccessful = true;
                    }
					if (!unsuccessful && code != "successful")
						notify_Hidden = "[c/FFFFFF:Successful!]";
				}
			}
		}

		public override string ToString()
		{
			string name = "";
			if (destructive)
				name += "Destructive ";
			if (modifiers[1] != "None")
				name += modifiers[1] + " ";
			if (modifiers[0] != "None")
				name += modifiers[0] + " ";
			name += type;
			if (!enabled)
				name += " (Disabled)";
			return name;
		}

		public override bool Equals(object obj)
		{
			if (obj is Explosive other)
				return type == other.type && modifiers[0] == other.modifiers[0] && modifiers[1] == other.modifiers[1] && destructive == other.destructive && spawn == other.spawn && spawnChance == other.spawnChance && enabled == other.enabled;
			return base.Equals(obj);
		}

		public override int GetHashCode() => new { type, destructive, spawn, spawnChance, enabled }.GetHashCode();
	}

	public static class ExplosiveUtils
	{
		public static string GetExplosiveInfo(string type)
		{
			type = type.ToLower();
			type = type.Replace("_", "");
			type = type.Replace("-", "");
			if (type == "grenade")
				return "Grenade: A small explosive, with average damage, short fuse-length, and produces a small explosion";
			if (type == "bomb")
				return "Bomb: The average explosive, with high damage, average fuse-length, and produces an average sized explosion";
			if (type == "dynamite")
				return "Dynamite: A cylinder-shaped explosive, with very high damage, long fuse-length, and produces a large explosion";
			if (type == "mysteryexplosive")
				return "Mystery Explosive: A mysteriously shaped explosive, sometimes being a grenade, sometimes a bomb, and other times dynamite";
			if (type == "annihilationpellet" || type == "annihilation" || type == "pellet")
				return "Annihilation Pellet: A very small, orb-shaped explosive, with low damage, short-fuse length, and creates a small explosion\n[c/828282:Any modifiers applied to the pellet are converted to range and damage]";
			if (type == "bombaggot")
				return "Bombaggot: A very small, orb-shaped explosive, with average damage, average-fuse length, creates a small explosion, and attempts to readjust aim to hit you\n[c/828282:Any modifiers applied to the maggot are converted to speed and range]";
			if (type == "worldbomb")
				return "World Bomb: Kaboom? Yes " + Main.worldName + ", kaboom.";
			//if (type == "boulder")
			//	return "A boulder-shaped explosive, with boulder damage, boulder fuse-length, produces a larger-than-boulder sized explosion, and acts like a boulder";
			//if (type == "missile")
			//	return "A thin, cylinder explosive, with high damage, average fuse-length, produces a average sized explosion, and is very fast";
			//if (type == "nuke")
			//	return "A really large explosive, with extreme damage, long fuse-length, and produces a colossal explosion";
			//if (type == "antimatterbomb")
			//	return "A plate-shaped explosive, with one-shot potential, long fuse-length, and produces a very large explosion";

			if (type == "none")
				return "None: Your average, run-of-the-mill, unaltered modifier";
			//if (type == "sticky")
			//	return "Sticky: Sticks to platforms and blocks, cancels out Bouncy's effects";
			if (type == "bouncy")
				return "Bouncy: Bounces off blocks"; //return "Bouncy: Bounces off blocks, cancels out Sticky's effects";
			if (type == "spring")
				return "Spring: Bounces off blocks, each bounce higher than the last"; //return "Spring: Bounces off blocks, each bounce higher than the last, and cancels out Sticky's effects";
			if (type == "gravitydefying")
				return "Gravity-defying: Ignores gravity instead of obeying it";
			if (type == "fragmenting")
				return "Fragmenting: Splits into several lower-tier explosives when exploding, grenades split into the fabled Annihilation Pellets";
			if (type == "cluster")
				return "Cluster: Splits into several lower-tier explosives when exploding, which those split into more lower-tiered explosives, repeating until they reach the lowest tier: Annihilation Pellets";
			//if (type == "teleporting")
			//	return "Teleporting: Ignores gravity instead of obeying it";
			if (type == "stealthy")
				return "Stealthy: More transparent than usual";
			if (type == "invisible")
				return "Invisible: Self-explanatory";
			if (type == "heatseeking")
				return "Heat-Seeking: Finds nearby targets and attempts to readjust aim to hit them";
			if (type == "sandy")
				return "Sandy: Explosion produces sand, this is such a good idea";
			if (type == "lingering")
				return "Lingering: Explosion lasts longer than usual";
			if (type == "longlasting")
				return "Long-lasting: Explosion lasts much, much longer than usual";
			if (type == "volatile")
				return "Volatile: Instantly explodes when hitting blocks";
			if (type == "mysterious")
				return "Mysterious: Acts as a random modifier";
			if (type == "mysteriouslymysterious" || type == "mysteriously")
				return "Mysteriously Mysterious: Acts as a random modifier, including secret ones";
			if (type == "confettifilled")
				return "Confetti-filled: Creates confetti when exploding instead of smoke and fire";
			if (type == "heavy")
				return "Heavy: Falls faster than normal";
			if (type == "dense")
				return "Dense: Falls much, much faster than normal";
			if (type == "happy")
				return "Happy: Deals more damage than normal";
			if (type == "overjoyed")
				return "Overjoyed: Deals much, much more damage than normal";
			if (type == "fiery")
				return "Fiery: Explosion also produces lingering flames";
			if (type == "blazing")
				return "Blazing: Leaves a trail of fire wherever it goes, also produces lingering flames when it explodes";
			if (type == "speedy")
				return "Speedy: Moves faster than normal";
			if (type == "supersonic")
				return "Supersonic: Moves much, much faster than normal";
			if (type == "huge")
				return "Huge: Explosion is bigger than normal";
			if (type == "gigantic")
				return "Gigantic: Explosion is much, much bigger than normal";
			if (type == "shortfused")
				return "Short-fused: Takes less time to explode";
			if (type == "supershortfused" || type == "super")
				return "Super Short-fused: Takes much, much less time to explode";
			if (type == "unaltered")
				return "Unaltered: Completely unaltered, no changes here\n[c/828282:I'm joking, it actually makes the bomb appear to be unaltered]";
			if (type == "mighty")
				return "Mighty: Attempts to fly towards you, deals more damage than normal, and explosion is much bigger";
			if (type == "lightessence" || type == "light")
				return "Light Essence: More transparent than usual and ignores gravity\n[c/828282:Perhaps combine it with something else?]";
			if (type == "ghostly")
				return "Ghostly: More transparent than usual, ignores gravity, passes through walls, and attempts to readjust aim to hit you";
			if (type == "zombified")
				return "Zombified: Upon exploding, the explosive is revived in a zombie state and will attempt to readjust aim to hit you while in said zombie state";
			if (type == "darkessence" || type == "dark")
				return "Dark Essence: More transparent than usual, and upon exploding, the explosive is revived in a zombie state and will attempt to readjust aim to hit you while in said zombie state\n[c/828282:Perhaps combine it with something else?]";
			if (type == "wrathful")
				return "Wrathful: Upon exploding, the explosive is revived in a ghostly state, and while in said state the following effects happen: More transparent than usual, ignores gravity, passes through walls, and attempts to readjust aim to hit you";
			if (type == "chilly")
				return "Chilly: Chills anyone who happens to come in contact with it";
			if (type == "freezing")
				return "Freezing: Freezes anyone who happens to come in contact with it";
			if (type == "snowy")
				return "Snowy: Produces snow upon exploding and chills anyone who happens to come in contact with it";
			if (type == "snowstorm")
				return "Snowstorm: Chills anyone who happens to come in contact with it, produces a ton of snow upon exploding and the explosion lasts longer than usual";
			if (type == "blizzard")
				return "Blizzard: Produces a ton of snow upon exploding, constantly produces freezing wind, explosion lasts longer than usual";
			if (type == "hailstorm")
				return "Hailstorm: Freezes anyone who happens to come in contact with it and explosion lasts longer than usual";
			if (type == "melting")
				return "Melting: Leaves a trail of fire wherever it goes, takes less time to explode, also produces lingering flames when it happens to do so";
			if (type == "infested")
				return "Infested: Upon exploding, bombaggots are released, and the explosive is revived in a zombie state and will attempt to readjust aim to hit you while in said zombie state";
			if (type == "merry")
				return "Merry: Deals more damage than normal and chills anyone who happens to come in contact with it";
			if (type == "shattering")
				return "Shattering: Instantly explodes into glass shards when hitting blocks";
			if (type == "wrapped")
				return "Wrapped: Falls down slowly and conceals the bomb, until reaching ground, in which it unwraps and the bomb's fuse is lit";
			if (type == "heavilywrapped")
				return "Heavily Wrapped: Conceals the bomb, until reaching ground, in which it unwraps and the bomb's fuse is lit";
			if (type == "messy")
				return "Messy: Explosion lasts longer than usual and continuously produces sand while exploding, best idea ever";
			if (type == "dusty")
				return "Dusty: Explosion produces a lot of sand, this is such a great idea";
			if (type == "soiled")
				return "Soiled: Explosion lasts longer than usual and continuously produces a ton of sand while exploding, this is the greatest plan";
			return "'" + type + "' is not a modifier or bomb";
		}

		public static Color GetModifierColor(string type, Color? tintColor = null)
		{
			Color color = default(Color);
			if (type == "Sticky")
				return new Color(145, 188, 255);
			if (type == "Bouncy")
				return new Color(254, 149, 210);
			if (type == "Gravity-defying")
				return new Color(201, 161, 247);
			if (type == "Fragmenting")
				return new Color(192, 226, 54);
			if (type == "Teleporting")
				return new Color(255, 95, 252);
			if (type == "Stealthy")
				return new Color(161, 247, 254);
			if (type == "Heat-Seeking")
				return new Color(255, 159, 95);
			if (type == "Sandy")
				return new Color(223, 219, 147);
			if (type == "Lingering")
				return new Color(186, 189, 218);
			if (type == "Speedy")
				return new Color(134, 230, 10);
			if (type == "Huge")
				return new Color(159, 224, 124);
			if (type == "Fiery")
				return new Color(255, 227, 0);
			if (type == "Shocking")
				return new Color(138, 158, 255);
			if (type == "Dusty")
				return new Color(186, 168, 84);
			if (type == "Cluster")
				return new Color(165, 142, 208);
			if (type == "Long-lasting")
				return new Color(230, 222, 10);
			if (type == "Supersonic")
				return new Color(182, 236, 195);
			if (type == "Gigantic")
				return new Color(112, 177, 15);
			if (type == "Electrocuting")
				return new Color(215, 220, 255);
			if (type == "Blazing")
				return new Color(255, (int)Main.masterColor * 200f, 0);
			if (type == "Platform-Sticky Block-Bouncy")
				return Color.Lerp(new Color(145, 188, 255), new Color(254, 149, 210), 0.5f);
			if (type == "Assassinating")
				return new Color(132, 166, 199);
			if (type == "Messy")
				return new Color(169, 125, 93);
			if (type == "Soiled")
				return new Color(114, 81, 56);
			if (type == "Shielded")
				return new Color(136, 217, 234);
			if (type == "Confetti-filled")
				return new Color(255, 170, 253);
			if (type == "Light Essence")
				return new Color(127, 199, 198);
			if (type == "Ghostly")
				return new Color(210, 210, 210);
			if (type == "Zombified 2" || type == "Infested 2")
				return new Color(84, 197, 113);
			if (type == "Dark Essence")
				return new Color(40, 0, 0);
			if (type == "Dark Essence 2")
				return new Color(40, 0, 0);
			if (type == "Wrathful 2")
				return Color.Black;
			if (type == "Infested")
				return new Color(214, 196, 160);
			if (type == "Freezing")
				return new Color(175, 175, 255);
			if (type == "Snowy")
				return new Color(255, 250, 250);
			if (type == "Chilly")
				return new Color(255, 250, 250);
			if (tintColor != null)
				return default(Color);
			return default(Color);
		}

		public static void ExplosionParticles(Projectile projectile, int tier = 1, int dustDivisor = 1, int fireDust = 6, int smokeDust = 31, bool confetti = false, bool smokeClouds = true)
        {
			Vector2 originalSize = new Vector2(projectile.width, projectile.height);
			if (tier == 3)
			{
				if (confetti)
					projectile.Resize(22, 22);
				for (int num776 = 0; num776 < projectile.width / dustDivisor; num776++)
				{
					int num777 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, confetti ? (219 + Main.rand.Next(5)) : smokeDust, 0f, 0f, 100, default(Color), confetti ? 1f : 2f);
					Dust dust217 = Main.dust[num777];
					Dust dust2 = dust217;
					dust2.velocity *= 1.4f;
					if (confetti)
					{
						dust2.fadeIn = 1f;
						dust2.noGravity = true;
					}
				}
				if (confetti)
					projectile.Resize((int)originalSize.X, (int)originalSize.Y);
				for (int num778 = 0; num778 < (projectile.width / 2) / dustDivisor; num778++)
				{
					int num779 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, confetti ? (139 + Main.rand.Next(4)) : fireDust, 0f, 0f, 100, default(Color), confetti ? 1.6f : 3f);
					Main.dust[num779].noGravity = true;
					Dust dust218 = Main.dust[num779];
					Dust dust2 = dust218;
					dust2.velocity *= 5f;
					num779 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, confetti ? (139 + Main.rand.Next(4)) : fireDust, 0f, 0f, 100, default(Color), confetti ? 1.9f : 2f);
					dust218 = Main.dust[num779];
					dust2 = dust218;
					dust2.velocity *= 3f;
				}
				if (smokeClouds)
				{
					for (int num780 = 0; num780 < 2 / dustDivisor; num780++)
					{
						for (int i = 0; i < 4; i++)
						{
							int num775 = Gore.NewGore(projectile.GetSource_FromThis(), projectile.Center, default(Vector2), Main.rand.Next(61, 64));
							Gore gore40 = Main.gore[num775];
							Gore gore2 = gore40;
							gore2.scale *= projectile.scale;
							Main.gore[num775].velocity = ((new Vector2(1f, 1f)).RotatedBy(MathHelper.ToRadians(90f * i))).RotatedByRandom(MathHelper.ToRadians(22.5f));
						}
					}
				}
			}
			else if (tier == 2)
            {
				if (confetti)
					projectile.Resize(22, 22);
				for (int num771 = 0; num771 < (projectile.width / 2) / dustDivisor; num771++)
				{
					int num772 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, confetti ? (219 + Main.rand.Next(5)) : smokeDust, 0f, 0f, 100, default(Color), confetti ? 1f : 1.5f);
					Dust dust215 = Main.dust[num772];
					Dust dust2 = dust215;
					dust2.velocity *= 1.4f;
					if (confetti)
					{
						dust2.fadeIn = 1f;
						dust2.noGravity = true;
					}
				}
				if (confetti)
					projectile.Resize((int)originalSize.X, (int)originalSize.Y);
				for (int num773 = 0; num773 < (projectile.width / 4) / dustDivisor; num773++)
				{
					int num774 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, confetti ? (139 + Main.rand.Next(4)) : fireDust, 0f, 0f, 100, default(Color), confetti ? 1.6f : 2.5f);
					Main.dust[num774].noGravity = true;
					Dust dust216 = Main.dust[num774];
					Dust dust2 = dust216;
					dust2.velocity *= 5f;
					num774 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, confetti ? (139 + Main.rand.Next(4)) : fireDust, 0f, 0f, 100, default(Color), confetti ? 1.9f : 1.5f);
					dust216 = Main.dust[num774];
					dust2 = dust216;
					dust2.velocity *= 3f;
				}
				if (smokeClouds)
				{
					for (int num780 = 0; num780 < 2 / dustDivisor; num780++)
					{
						for (int i = 0; i < 4; i++)
						{
							int num775 = Gore.NewGore(projectile.GetSource_FromThis(), projectile.Center, default(Vector2), Main.rand.Next(61, 64));
							Gore gore40 = Main.gore[num775];
							Gore gore2 = gore40;
							gore2.scale *= projectile.scale;
							Main.gore[num775].velocity = ((new Vector2(1f, 1f)).RotatedBy(MathHelper.ToRadians(90f * i))).RotatedByRandom(MathHelper.ToRadians(22.5f));
						}
					}
				}
			}
			else
            {
				if (confetti)
					projectile.Resize(22, 22);
				for (int num771 = 0; num771 < (projectile.width / 4) / dustDivisor; num771++)
				{
					int num772 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, confetti ? (219 + Main.rand.Next(5)) : smokeDust, 0f, 0f, 100, default(Color), confetti ? 1f : 1.5f);
					Dust dust215 = Main.dust[num772];
					Dust dust2 = dust215;
					dust2.velocity *= 1.4f;
					if (confetti)
					{
						dust2.fadeIn = 1f;
						dust2.noGravity = true;
					}
				}
				if (confetti)
					projectile.Resize((int)originalSize.X, (int)originalSize.Y);
				for (int num773 = 0; num773 < (projectile.width / 8) / dustDivisor; num773++)
				{
					int num774 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, confetti ? (139 + Main.rand.Next(4)) : fireDust, 0f, 0f, 100, default(Color), confetti ? 1.6f : 2.5f);
					Main.dust[num774].noGravity = true;
					Dust dust216 = Main.dust[num774];
					Dust dust2 = dust216;
					dust2.velocity *= 5f;
					num774 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, confetti ? (139 + Main.rand.Next(4)) : fireDust, 0f, 0f, 100, default(Color), confetti ? 1.9f : 1.5f);
					dust216 = Main.dust[num774];
					dust2 = dust216;
					dust2.velocity *= 3f;
				}
				if (smokeClouds)
				{
					for (int i = 0; i < 4 / dustDivisor; i++)
					{
						int num775 = Gore.NewGore(projectile.GetSource_FromThis(), projectile.Center, default(Vector2), Main.rand.Next(61, 64));
						Gore gore40 = Main.gore[num775];
						Gore gore2 = gore40;
						gore2.scale *= projectile.scale;
						Main.gore[num775].velocity = ((new Vector2(1f, 1f)).RotatedBy(MathHelper.ToRadians((90f * dustDivisor) * i))).RotatedByRandom(MathHelper.ToRadians(22.5f));
				}
				}
			}
		}

		public static string RollForModifier(bool secret = false)
		{
			string modifier = Main.rand.Next(new string[] { "None", /*"Sticky",*/ "Bouncy", "Gravity-defying", "Fragmenting", /*"Teleporting",*/ "Stealthy", "Heat-Seeking", "Sandy", "Lingering", "Volatile", "Speedy", "Huge", "Short-fused", "Fiery", /*"Shocking",*/ "Happy", "Heavy", "Chilly" });
			if (secret)
				modifier = Main.rand.Next(new string[] { "None",
					//"Sticky",
					"Bouncy",
					"Gravity-defying",
					"Fragmenting",
					//"Teleporting",
                    "Stealthy",
					"Heat-Seeking",
					"Sandy",
					"Lingering",
					"Volatile",
					"Speedy",
					"Huge",
					"Short-fused",
					"Fiery",
					//"Shocking",
					"Happy",
					"Heavy",
					"Chilly",
					"Mysteriously Mysterious",
					"Unaltered",
					"Spring",
					"Cluster",
					"Invisible",
					"Dusty",
					"Long-lasting",
					"Supersonic",
					"Gigantic",
					//"Electrocuting",
					"Blazing",
					//"Platform-Sticky Block-Bouncy",
                    //"Assassinating",
                    "Messy",
					"Soiled",
					"Confetti-filled",
					//"Shielded",
                    "Overjoyed",
					"Super Short-fused",
					"Dense",
					"Mighty",
					"Freezing",
					"Hailstorm",
					"Snowy",
					"Snowstorm",
				    "Blizzard",
				    "Melting",
					"Infested",
					"Light Essence",
					"Ghostly",
					"Zombified",
					"Dark Essence",
					"Wrathful",
					"Shattering",
					"Merry" });
			return modifier;
		}
    }
}
