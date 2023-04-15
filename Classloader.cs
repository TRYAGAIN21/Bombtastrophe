using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BombtastropheMod.Challenges;
using System.Collections.Generic;
using System;

namespace BombtastropheMod
{
	public class ClassLoader
	{
		public static readonly List<Challenge> Challenges = new List<Challenge>();

		public static void AddChallenge(Challenge challenge)
		{
			challenge.type = Challenges.Count;
			Challenges.Add(challenge);
		}

		public static void Autoload(Mod mod)
		{
			foreach (var type in mod.GetType().Assembly.GetTypes())
			{
				if (type.IsAbstract || !typeof(Challenge).IsAssignableFrom(type))
					continue;
				((Challenge)Activator.CreateInstance(type)).Load();
			}
		}
	}
}