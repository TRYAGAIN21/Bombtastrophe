using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using System.Linq;
using System;

namespace BombtastropheMod.Challenges
{
	/*public abstract class Modifier
	{
		public string Name { get; internal set; }
		public string Description { get; internal set; }

		public virtual bool Unlocked() => false;

		public virtual void Defaults() { }
	}*/

	public abstract class Challenge
	{
		public int type { get; internal set; }
		public Mod mod { get; internal set; }

		public virtual string Name => null;
		public virtual string Description => null;
		public virtual string Reward => null;
		public virtual Func<bool> Conditions => () => true;
		public virtual Func<bool> Prerequisites => () => true;
		public virtual string Texture => (base.GetType().Namespace + "." + base.GetType().Name).Replace('.', '/');

		public void Load()
		{
			ClassLoader.AddChallenge(this);
		}
	}

	public class ZeroCasualties : Challenge
	{
		public override string Name => "Zero Casualties";
		public override string Description => "Without leaving space, fully defeat a boss without getting hit by an explosion even once.";
		public override string Reward => "Anti-Gravity Modifier";
		public override Func<bool> Conditions => () => Main.LocalPlayer.GetModPlayer<BombtastrophePlayer>().Unlocked_ZeroCasualties;
	}

	public class PrimevalWays : Challenge
	{
		public override string Name => "Primeval Ways";
		public override string Description => "Defeat the final boss with the '1.3 Bombtastrophe' preset enabled.";
		public override string Reward => "1.3 Random Explosive, 1.3 Random Modifier";
		public override Func<bool> Conditions => () => Main.LocalPlayer.GetModPlayer<BombtastrophePlayer>().Unlocked_PrimevalWays;
		public override string Texture => "BombtastropheMod/Challenges/Placeholder";
	}

	public class Waterproof : Challenge
	{
		public override string Name => "Wait, are they waterproof?";
		public override string Description => "Be blown up by any explosive bearing a lit fuse while underwater.";
		public override string Reward => "Sea Mine Explosive";
		public override Func<bool> Conditions => () => Main.LocalPlayer.GetModPlayer<BombtastrophePlayer>().Unlocked_Waterproof;
		public override string Texture => "BombtastropheMod/Challenges/Placeholder";
	}

	public class BoomBoomPow : Challenge
	{
		public override string Name => "Boom Boom Pow";
		public override string Description => "Blow up 100 enemies with bombs.";
		public override string Reward => "Fragmenting Modifier";
		public override Func<bool> Conditions => () => Main.LocalPlayer.GetModPlayer<BombtastrophePlayer>().Unlocked_BoomBoomPow;
		public override string Texture => "BombtastropheMod/Challenges/Placeholder";
	}
}