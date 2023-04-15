using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BombtastropheMod
{
	public class CheckDes : ModCommand
	{
		public override CommandType Type
			=> CommandType.Chat;

		public override string Command
			=> "des";

		public override string Usage
			=> "/des <Modifier|Bomb>";

		public override string Description
			=> "Shows the description of a modifier or bomb";

		public override void Action(CommandCaller caller, string input, string[] args) 
		{
			Main.NewText(ExplosiveUtils.GetExplosiveInfo(args[0]), 255, 255, 255);
		}
	}

	public class eventCommand : ModCommand
	{
		public override CommandType Type
			=> CommandType.World;

		public override string Command
			=> "event";

		public override string Usage
			=> "/event <percentage>";

		public override string Description
			=> "Debug Command";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (!float.TryParse(args[0], out float percentage))
				BombtastropheSystem.wrappedCodeProgress = 1;
			BombtastropheSystem.wrappedCodeProgress = (int)(18000f * percentage);
		}
	}
}