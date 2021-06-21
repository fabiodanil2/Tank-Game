using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrabalhoPratico
{
    class TankP
    {
        BasicEffect effect;
        VertexPositionColor[] vertices;
        public Vector3  velocity;
        float posX, posY;
        public Vector3 pos;
        Matrix worldMatrix;
        GraphicsDevice device;
        float ParticuleWidght,ParticuleHeight;
        float FallingV;
        Vector3 Center;
        float random, randomPosicao;
        float time;
        float RectangleWidght, RectangleHeight;
        public TankP(GraphicsDevice device, float widght, float height, Vector3 center, Matrix systemWorld)
        {
            Center = center;
           RectangleWidght = widght;
           RectangleHeight = height;
            this.device = device;
            //vetices que compoem a particula.
            vertices = new VertexPositionColor[2];
            //effect
            effect = new BasicEffect(device);

            worldMatrix = Matrix.Identity;
          
            //define a velocidade a que a particula se desloca.
            FallingV = 0.03f;
            effect.VertexColorEnabled = true;
        }

        public void CreateParticle(GameTime gametime, Vector3 centerPos, float rectangleWidght, float rectangleHeight, Vector3 newDir,  Matrix worldSystem)
        {
            worldMatrix = worldSystem;
            Center = new Vector3(0, 0, 0);
            time += (float)gametime.ElapsedGameTime.TotalMilliseconds;
            //geracao de valores random para definir posicao e magnitude
            randomPosicao = RandomGenarator.getRandomNext();
            random = RandomGenarator.getRandomMinMax();

            ParticuleWidght = random;
            ParticuleHeight = random;

            //para definir a posicao soma-se ao centro o valor da largura/altura do retangulo mais a distancia do ao centro (tanto de altura como largura) para que encontreO  ponto intermiedio entre o centro e o limite exterior.
            posX = Center.X + rectangleWidght * ParticuleWidght;
            posY = Center.Y +  rectangleHeight * ParticuleHeight;
            pos = new Vector3(posX, posY, Center.Z);

            //criaçao dos vertices que compoem a particula, um recebe a posicao calculada o outro é criado um pouco abaixo.
            vertices[0].Position = pos;
            vertices[0].Color = Color.SaddleBrown;
            vertices[1].Position = pos + new Vector3(0, 1f, 0);
            vertices[1].Color = Color.SaddleBrown;

           
            Vector3 dir = RandomGenarator.getRandomNextDouble() * newDir + new Vector3(0, 1, 0);
            velocity = dir;
        }

        public void Update(GameTime gametime)
        {
            effect.World = worldMatrix;
            //worldMatrix = Matrix.Identity;
            time = (float)gametime.ElapsedGameTime.TotalSeconds;

            Vector3 acelaracao = new Vector3(0, -0.98f, 0);
            velocity = velocity + acelaracao * FallingV;
            pos = pos + velocity * time;
           
            vertices[0].Position = pos;
            vertices[1].Position = pos + new Vector3(0, 0.02f, 0);
        }


        public void Draw(GraphicsDevice device, Camera camara)
        {
            effect.TextureEnabled = false;
            effect.VertexColorEnabled = true;
            effect.View = camara.viewMatrix;
            effect.Projection = camara.projectionMatrix; ;

            effect.CurrentTechnique.Passes[0].Apply();         
            device.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
            
        }
    }
}
