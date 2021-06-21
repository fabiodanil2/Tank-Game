using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace TrabalhoPratico
{
    class BulletGenarator2
    {

        static List<Bullet> BulletsActive;
        static List<Bullet> BulletsNotActive;
        static List<Bullet> CopyBulletsActive;
        static int NumberOfBullets;
        static Tank2 tank2;
        static ContentManager content;
        static Bullet bulletTemp;
        static Vector3 posBullet, dirBullet, pos;



        static public void Initialize(ContentManager cont, Tank2 tank2)
        {
            BulletGenarator2.tank2 = tank2;
            BulletsActive = new List<Bullet>(500);
            BulletsNotActive = new List<Bullet>(500);
            content = cont;
            NumberOfBullets = 500;
            CopyBulletsActive = BulletsActive;

            for (int i = 0; i < NumberOfBullets; i++)
            {
                BulletsNotActive.Add(new Bullet(content));
            }
        }

        static public void Pos_Dir_Bullets()
        {
            Vector3 offset = new Vector3(0, 1.45f, 1.5f);
            Matrix rotacao = Matrix.CreateRotationX(tank2.cannonAngle) * Matrix.CreateRotationY(tank2.turrentAngle) * tank2.rotation;

            offset = Vector3.Transform(offset, rotacao);
            dirBullet = Vector3.Transform(new Vector3(0, 0, 1), rotacao);
            posBullet = tank2.pos + offset;
        }

        static public void Shoot()
        {
            Pos_Dir_Bullets();
            bulletTemp = BulletsNotActive.First();

            bulletTemp.pos = posBullet;
            bulletTemp.dir = dirBullet;

            BulletsActive.Add(bulletTemp);
            BulletsNotActive.Remove(bulletTemp);
        }

        static public void Remove(Bullet bullet)
        {
            BulletsActive.Remove(bullet);
            BulletsNotActive.Add(bullet);
        }

        static public void UpdateBalas(GameTime gameTime, Terreno terreno)
        {
            //copiaBalasAtivas = balasAtivas.ToList();
            foreach (Bullet bullet in BulletsActive)
            {
                bullet.Update(gameTime);
            }
            pos.Y = terreno.altura(posBullet.X, posBullet.Z);
            BulletsActive.RemoveAll(b => b.pos.Y < pos.Y);
            BulletsActive.RemoveAll(b => b.bulletDestroid == true);
        }

        static public void DrawBullet(GraphicsDevice device, Camera camara)
        {
            foreach (Bullet bullet in BulletsActive)
            {
                bullet.Draw(device, camara);
            }
        }

        static public List<Bullet>GetListOfBulletsActive()
        {
            return BulletsActive;
        }
    }

}
