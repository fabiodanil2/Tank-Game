using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TrabalhoPratico
{
    class SystemP
    {

        List<TankP>ParticuleList;
        List<TankP> ActiveParticuleList;
        GraphicsDevice device;
        TankP ParticuleTemp;
        public Vector3 CenterPos;
        public BasicEffect effect;
        public Matrix worldMatrix;
        int ParticuleQuant;
        float RectangleHeight, RectangleWidght;
        public bool CreateParticules;

        public SystemP(GraphicsDevice device, Vector3 Center, float Widght, float height, Matrix worldTank)
        {
            ParticuleQuant = 3000;
            CenterPos = Center;
            this.device = device;

            ParticuleList = new List<TankP>(ParticuleQuant);
            ActiveParticuleList = new List<TankP>(ParticuleQuant);

            effect = new BasicEffect(device);
            worldMatrix = Matrix.Identity;

            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;

            RectangleWidght = Widght;
            RectangleHeight = height;
           CreateParticule(ParticuleQuant);

            CreateParticules = true;
        }

        //a lista de particulas nao ativa é preenchida com a quantidade de particulas desejada
        public void CreateParticule(int ParticuleQ)
        {
            for (int i = 0; i < ParticuleQ; i++)
            {
                ParticuleList.Add(new TankP(device, RectangleWidght, RectangleHeight, CenterPos, worldMatrix));
            }
        }

        public void Update(GameTime gametime, Vector3 pos, Vector3 newDir, Tank tank)
        {
            MoveToBack(tank);

            //para cada Update retiram-se x particulas da lista nao ativa e colocam-se as mesmas na lista de particulas ativas.
            for (int i = 0; i < 5; i++)
            {
                if (ActiveParticuleList.Count < ParticuleQuant - 1000)
                {
                    //particula temporaria recebe a primeira particula da lista de nao ativas.
                    ParticuleTemp = ParticuleList.First();
                    //calcula posicao e direcao.
                    
                        ParticuleTemp.CreateParticle(gametime, CenterPos, RectangleWidght, RectangleHeight, newDir,  worldMatrix);
                    //adiciona particula a lista ativa.
                    ActiveParticuleList.Add(ParticuleTemp);
                    //remove da lista nao ativa.
                    ParticuleList.Remove(ParticuleTemp);
                }
            }

            foreach (TankP p in ActiveParticuleList)
            {
                //Update de cada particula da lista ativa.
                p.Update(gametime);
                //se a particula igualar a posicao em Y de 0
                if (p.pos.Y <0f)
                {
                    //...é adicionada á lista nao ativa...
                    ParticuleList.Add(p);
                }
            }
            //... e é removida da lista ativa.
            ActiveParticuleList.RemoveAll(particula => particula.pos.Y <0f);
        }


        private void MoveToBack(Tank tank)
        {
            Vector3 offset = new Vector3(-0.6f, 0.05f, -0.7f);
            Matrix rotation = Matrix.CreateTranslation(offset) * tank.Rotation;// gero o pó atras do tanque
            Vector3 transformOffset = Vector3.Transform(offset, rotation);
           
            worldMatrix = rotation;
            worldMatrix.Translation = transformOffset + tank.pos;
        }


        public void Draw(GraphicsDevice device, Camera camera)
        {
            //cada particula na lista ativa é desenhada.
            foreach (TankP p in ActiveParticuleList)
            {
                p.Draw(device, camera);
            }
           
        }

       

    }
}
