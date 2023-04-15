using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BombtastropheMod
{
	public class NiceList : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Santamite's Nice List");
			Tooltip.SetDefault("A fragment of Santamite's nice list..." +
                "\nX" +
				"\nPlutonium Bomb" +
                "\nLandmine" +
                "\nOffensive Grenade" +
                "\nDynamite" +
                "\nElectromagnetic Bomb" +
                "\n- (Unnamed)" +
                "\nMOAB" +
                "\nAtomic Bomb" +
                "\nStun Grenade");
			Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 46;
			Item.height = 36;
			Item.rare = -11;
			Item.value = Item.sellPrice(0, 2, 0, 0);
		}
	}
}