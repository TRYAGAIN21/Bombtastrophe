using Terraria.ModLoader;

namespace BombtastropheMod
{
	public class BombtastropheMod : Mod
	{
		public override void Load()
		{
			ClassLoader.Autoload(this);
		}
	}
}