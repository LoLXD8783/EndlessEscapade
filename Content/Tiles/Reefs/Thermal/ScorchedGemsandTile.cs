﻿using EndlessEscapade.Common.Tiles;
using EndlessEscapade.Content.Items.Reefs.Thermal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EndlessEscapade.Content.Tiles.Reefs.Thermal;

public class ScorchedGemsandTile : CompositeTileBase
{
    public override int AtlasWidth { get; } = 4;

    public override int AtlasHeight { get; } = 2;

    public override void SetStaticDefaults() {
        Main.tileMergeDirt[Type] = false;
        Main.tileSolid[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileFrameImportant[Type] = true;

        TileID.Sets.Conversion.Sand[Type] = true;

        AddMapEntry(new Color(106, 96, 95));

        MineResist = 1f;

        HitSound = SoundID.Dig;
        DustType = DustID.Ash;

        ItemDrop = ModContent.ItemType<ScorchedGemsandItem>();
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
        Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

        Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
        Vector2 position = new Vector2(i, j) * 16f - Main.screenPosition + zero;

        Tile tile = Framing.GetTileSafely(i, j);

        Rectangle frame = new(tile.TileFrameX, tile.TileFrameY, 16, 16);

        spriteBatch.Draw(texture, position, frame, Color.White, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
    }
}