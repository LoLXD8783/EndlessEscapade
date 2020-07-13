using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using EEMod.Items.Placeables;

namespace EEMod.Tiles
{
    public class AtlanteanSlabTile : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlendAll[Type] = true;

            AddMapEntry(new Color(36, 53, 135));

            dustType = 154;
            drop = ModContent.ItemType<AtlanteanSlab>();
            soundStyle = 1;
            mineResist = 1f;
            minPick = 0;
        }
    }
}
