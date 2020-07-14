using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EEMod.Projectiles
{
    public class OceanArrowProjectile : VisualProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Quartz");
        }

        public override void SetDefaults()
        {
            projectile.width = 36;       //projectile width
            projectile.height = 46;  //projectile height
            projectile.friendly = true;      //make that the projectile will not damage you
            projectile.tileCollide = false;   //make that the projectile will be destroed if it hits the terrain
            projectile.penetrate = -1;      //how many npc will penetrate
            projectile.alpha = 255;                               //how many time this projectile has before disepire
            projectile.light = 0.3f;    // projectile light
            projectile.ignoreWater = true;
            projectile.aiStyle = 0;
            projectile.timeLeft = 1000000000;
        }

        public bool visible = true;
        public override void AI()
        {
            projectile.ai[0] += 0.1f;
            projectile.position.X = Main.player[projectile.owner].Center.X - projectile.width / 2 + (float)Math.Sin(projectile.ai[0]) * 10 - 100;
            projectile.position.Y = Main.player[projectile.owner].Center.Y;
            if (!visible)
                projectile.alpha += 5;
            else
                projectile.alpha -= 5;
            if (projectile.alpha < 0)
                projectile.alpha = 0;
            if (projectile.alpha > 255)
                projectile.alpha = 255;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)  //this make the projectile sprite rotate perfectaly around the player
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.Cyan * (float)((double)(255 - projectile.alpha) / (double)255), projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
    }
}
