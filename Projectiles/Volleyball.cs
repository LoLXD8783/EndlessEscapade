using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace EEMod.Projectiles
{
    public class Volleyball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Volleyball");
        }

        public override void SetDefaults()
        {
            projectile.width = 54;
            projectile.height = 52;
            projectile.alpha = 0;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.scale *= 0.7f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Bounce(projectile.modProjectile, oldVelocity, .7f);
            return false;
        }
        Vector2 SavedVel;
        public void Bounce(ModProjectile modProj, Vector2 oldVelocity, float bouncyness = 1f)
        {
            Projectile projectile = modProj.projectile;
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X * bouncyness;

            if (projectile.velocity.Y != oldVelocity.Y)
                projectile.velocity.Y = -oldVelocity.Y * bouncyness;
        }
        int frames = 11;
        int frame;
        float ree = 0;
        public static int GetPlayer(Vector2 center, int[] playersToExclude = default, bool activeOnly = true, float distance = -1, Func<Player, bool> CanAdd = null)
        {
            int currentPlayer = -1;
            for (int i = 0; i < Main.player.Length; i++)
            {
                Player player = Main.player[i];
                if ((!activeOnly || (player.active && !player.dead)) && (distance == -1f || player.Distance(center) < distance))
                {
                    bool add = true;
                    if (playersToExclude != default)
                    {
                        foreach (int m in playersToExclude)
                        {
                            if (m == player.whoAmI) { add = false; break; }
                        }
                    }
                    if (add)
                    {
                        distance = player.Distance(center);
                        currentPlayer = i;
                    }
                }
            }
            return currentPlayer;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
                writer.WriteVector2(SavedVel);
                writer.WriteVector2(mouseHitBoxVec);
                writer.Write(frame);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SavedVel = reader.ReadVector2();
            mouseHitBoxVec = reader.ReadVector2();
            frame = reader.ReadInt32();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player chosenPlayer = Main.player[GetPlayer(projectile.Center)];
            Texture2D volleyArrow = TextureCache.VArrow;
            Main.spriteBatch.Draw(volleyArrow, projectile.Center - Main.screenPosition, new Rectangle(0, (volleyArrow.Height / frames) * (11 - frame), volleyArrow.Width, volleyArrow.Height / frames), Color.White * ree, new Vector2(mouseHitBoxVec.X - chosenPlayer.Center.X, mouseHitBoxVec.Y - chosenPlayer.Center.Y).ToRotation() + MathHelper.Pi/2, new Rectangle(0, 0, volleyArrow.Width, volleyArrow.Height).Size() / 2, 1, SpriteEffects.None, 0);
            return true;
        }
        public static Vector2 mouseHitBoxVec;
        public override void AI()
        {
            Player chosenPlayer = Main.player[GetPlayer(projectile.Center)];
            EEPlayer modPlayer = chosenPlayer.GetModPlayer<EEPlayer>();
            if (Main.myPlayer == GetPlayer(projectile.Center))
            {
                mouseHitBoxVec = new Vector2(Main.mouseX + (int)Main.screenPosition.X, Main.mouseY + (int)Main.screenPosition.Y);
                projectile.netUpdate = true;
            }
            Rectangle mouseHitBox = new Rectangle((int)mouseHitBoxVec.X - 6, (int)mouseHitBoxVec.Y - 6, 12, 12);
            Rectangle projectileHitBox = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
            Rectangle playerHitBox = new Rectangle((int)chosenPlayer.position.X - 30, (int)chosenPlayer.position.Y - 30, chosenPlayer.width + 30, chosenPlayer.height + 30);
            frame = (int)(modPlayer.powerLevel * (11f/ modPlayer.maxPowerLevel));
            projectile.timeLeft = 100;
            projectile.velocity.Y += 0.2f;
            if(projectile.velocity.Y > 10)
            {
                projectile.velocity.Y = 10;
            }
            projectile.velocity.X *= 0.98f;
            
            if(playerHitBox.Intersects(projectileHitBox))
            {
                if (projectile.ai[0] == 1 && chosenPlayer.controlUseItem)
                {
                    SavedVel = projectile.velocity = Vector2.Normalize(new Vector2(mouseHitBoxVec.X - chosenPlayer.Center.X, mouseHitBoxVec.Y - chosenPlayer.Center.Y)) * 10;
                    projectile.ai[0] = 0;
                    projectile.netUpdate = true;
                }
            }
           /* if (chosenPlayer.controlUseItem && mouseHitBox.Intersects(projectileHitBox) && projectile.ai[0] != 2)
            {
                projectile.ai[0] = 0;
                projectile.velocity.Y = 0;
                Vector2 lastPosX = projectile.Center;
                projectile.Center = new Vector2(mouseHitBoxVec.X, mouseHitBoxVec.Y);
                Vector2 newPosX = projectile.Center;
                SavedVel = newPosX - lastPosX;
            }*/
            else if(projectile.ai[0] == 0)
            {
                ree = 0;
                projectile.velocity = SavedVel;
                projectile.ai[0] = 1;
                projectile.ai[1] = 0;
                projectile.netUpdate = true;
            }
            if (chosenPlayer.controlUp && mouseHitBox.Intersects(projectileHitBox))
            {
                projectile.ai[0] = 2;
                projectile.netUpdate = true;
            }
            if (projectile.ai[0] == 2)
            {
                if (chosenPlayer.controlUseItem)
                {
                    ree += 0.01f;
                    if(ree > 1)
                    {
                        ree = 1;
                    }
                    SavedVel = Vector2.Normalize(new Vector2(mouseHitBoxVec.X - chosenPlayer.Center.X, mouseHitBoxVec.Y - chosenPlayer.Center.Y)) * modPlayer.powerLevel;
                    projectile.ai[1] = 1;
                }
                if(projectile.ai[1] == 1 && !chosenPlayer.controlUseItem)
                {
                    projectile.ai[0] = 0;
                }
                projectile.Center = chosenPlayer.Center + new Vector2((chosenPlayer.direction * 10) - 10, -30);
                projectile.netUpdate = true;
            }
            projectile.rotation += projectile.velocity.X/16f;
        }
    }
}
