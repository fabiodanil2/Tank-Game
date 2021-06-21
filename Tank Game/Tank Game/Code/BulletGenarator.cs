using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace TrabalhoPratico
{
    class BulletGenarator
    {

        static List<Bullet> BulletsActive;//balas ativas
        static List<Bullet> BulletsNotActive;//balas nao ativas
        static List<Bullet> CopyBulletsActive;// lista de balas que serve para auxiliar o ciclo
        static int NumberOfBullets;
        static Tank tank1;
        static ContentManager content;
        static Bullet bulletTemp;
        static Vector3 posBullet, dirBullet, pos;



        static public void Initialize(ContentManager cont, Tank tank)
        {
            tank1 = tank;
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
            Vector3 offset = new Vector3(0, 1.45f, 1.5f);// de onde a bala sai/é criada
            Matrix rotacao = Matrix.CreateRotationX(tank1.cannonAngle) * Matrix.CreateRotationY(tank1.turrentAngle) * tank1.Rotation;

            offset = Vector3.Transform(offset, rotacao);
            dirBullet = Vector3.Transform(new Vector3(0, 0, 1), rotacao);//serve para determinar para onde é lançada a bala 
            posBullet = tank1.pos + offset;//determina a pos da bala
        }

        static public void Shoot()
        {
            Pos_Dir_Bullets();
            bulletTemp = BulletsNotActive.First();//recebe a primeira bala da lista não ativa

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

        static public void UpdateBullets(GameTime gameTime, Terreno terreno)
        {
            //copiaBalasAtivas = balasAtivas.ToList();
            foreach (Bullet bullet in BulletsActive)
            {
                bullet.Update(gameTime);
            }
            pos.Y = terreno.altura(posBullet.X, posBullet.Z);// serve para indicar a posição do chão
            BulletsActive.RemoveAll(b => b.pos.Y < pos.Y);// verifica se a bala passou o chao
            BulletsActive.RemoveAll(b => b.bulletDestroid == true);
        }

        static public void DrawBullet(GraphicsDevice device, Camera camara)
        {
            foreach (Bullet bullet in BulletsActive)
            {
                bullet.Draw(device, camara);
            }
        }

        static public List<Bullet> GetListOfBulletsActive()
        {
            return BulletsActive;
        }
    }

}
