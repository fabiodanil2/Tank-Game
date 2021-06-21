  using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabalhoPratico
{
    class Terreno
    {
        Color[] cor;
        BasicEffect effect;
        float escalaVertical = (1f / 20f);
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
         public float[] alturaMapa;
        int dimAltura;
        int dimLargura;
        int nPixels;
        Vector3[] normalArray;     
        GraphicsDevice device;


        public Terreno(GraphicsDevice device, Texture2D imageJpg, Texture2D terra)
        {

            effect = new BasicEffect(device);
            // Calcula a aspectRatio, a view matrix e a projeção
            float aspectRatio = (float)device.Viewport.Width /
            device.Viewport.Height;
            effect.View = Matrix.CreateLookAt(
            new Vector3(1.0f, 2.0f, 2.0f),
            Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 0.1f, 1000.0f);
            effect.LightingEnabled = true;
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Texture = terra; // criar efeito textura
            effect.DiffuseColor = new Vector3(0.7f, 0.7f, 0.7f); //kd cor difusa do material
            effect.SpecularColor = new Vector3(0.01f, 0.01f, 0.01f);  //KS propriedade do material
            effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);//La cor ambiente




            effect.DirectionalLight0.Enabled = true; //valores random
            effect.DirectionalLight0.DiffuseColor = new Vector3(0.8f, 0.8f, 0.8f);// ID cor
            effect.DirectionalLight0.Direction = new Vector3(-1.0f, -1.0f, -1.0f);//normal e angulo
            effect.DirectionalLight0.Direction.Normalize();// normalizar vetores
            effect.SpecularPower = 128.0f;//S
            effect.DirectionalLight0.SpecularColor = new Vector3(1.0f, 1.0f, 1.0f);


            cor = new Color[imageJpg.Width * imageJpg.Height];
            imageJpg.GetData<Color>(cor);
            alturaMapa = new float[imageJpg.Width * imageJpg.Height];
            for (int i = 0; i < imageJpg.Width * imageJpg.Height; i++)
                alturaMapa[i] = cor[i].R * escalaVertical;
            dimAltura = imageJpg.Width;
            dimLargura = imageJpg.Height;
            this.device = device;
            Indices(device);
            Vertices(device);

        }

        //surafece follow
        public float altura(float X, float Z)
        {
            int Xa = (int)X;
            int Za = (int)Z;
            float Ya = alturaMapa[Za * dimLargura + Xa];

            int Xb = Xa + 1;
            int Zb = Za;
            float Yb = alturaMapa[Zb * dimLargura + Xb];

            int Xc = Xa;
            int Zc = Za + 1;
            float Yc = alturaMapa[Zc * dimLargura + Xc];

            int Xd = Xa + 1;
            int Zd = Za + 1;
            float Yd = alturaMapa[Zd * dimLargura + Xd];

            float da = X - Xa;
            float db = Xb - X;
            float dc = X - Xc;
            float dd = Xd - X;

            float dab = Z - Za;
            float dcd = Zc - Z;

            float yab = da * Yb + db * Ya;
            float ycd = dc * Yd + dd * Yc;

            return dab * ycd + dcd * yab;
        }

        public Vector3 Normal(float X, float Z)
        {
            int Xa = (int)X;
            int Za = (int)Z;
            Vector3 Ya = normalArray[Za * dimLargura + Xa];

            int Xb = Xa + 1;
            int Zb = Za;
            Vector3 Yb = normalArray[Zb * dimLargura + Xb];

            int Xc = Xa;
            int Zc = Za + 1;
            Vector3 Yc = normalArray[Zc * dimLargura + Xc];

            int Xd = Xa + 1;
            int Zd = Za + 1;
            Vector3 Yd = normalArray[Zd * dimLargura + Xd];

            float da = X - Xa;
            float db = Xb - X;
            float dc = X - Xc;
            float dd = Xd - X;

            float dab = Z - Za;
            float dcd = Zc - Z;

            Vector3 yab = da * Yb + db * Ya;
            Vector3 ycd = dc * Yd + dd * Yc;

            return dab * ycd + dcd * yab;
        }

        public void Indices(GraphicsDevice graphicsDevice)
        {

            short[] indices;

            nPixels = 0;
            indices = new short[2 * (dimAltura) * (dimLargura - 1)];
            for (int x = 0; x < dimLargura - 1; x++)
            {
                for (int z = 0; z < dimAltura; z++)
                {
                    indices[nPixels] = (short)(x + z * dimLargura);
                    indices[nPixels + 1] = (short)(x + z * dimLargura + 1);
                    nPixels = nPixels + 2;
                }
            }
            indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), 2 * (dimAltura) * (dimLargura - 1), BufferUsage.None);
            indexBuffer.SetData<short>(indices);
        }
        public void Vertices(GraphicsDevice graphicsDevice)
        {
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[dimAltura * dimLargura];

            for (int i = 0; i < dimAltura * dimLargura; i++)
            {

                vertices[i] = new VertexPositionNormalTexture(new Vector3(i % dimAltura, alturaMapa[i], i / dimAltura), Vector3.Up, new Vector2((i % dimAltura) % 2, (i / dimAltura) % 2));

            }

            normalArray = new Vector3[dimAltura * dimLargura];


            VertexPositionNormalTexture vertice;
            int x = 0;
            int z = 0;
            //Linha de cima
            for (x = 1; x < dimAltura - 1; x++)
            {

                vertice = vertices[x];

                //Descobre os vizinhos do vertices escolhido
                VertexPositionNormalTexture verticeEsquerda = vertices[x - 1];
                VertexPositionNormalTexture verticeEsquerdaBaixo = vertices[x + dimAltura - 1];
                VertexPositionNormalTexture verticeBaixo = vertices[x + dimAltura];
                VertexPositionNormalTexture verticeBaixoDireita = vertices[x + dimAltura + 1];
                VertexPositionNormalTexture verticeDireita = vertices[x + 1];

                //calcula aqual o vetor pretendido
                Vector3 VectorEsquerda = verticeEsquerda.Position - vertice.Position;
                Vector3 VectorEsquerdaBaixo = verticeEsquerdaBaixo.Position - vertice.Position;
                Vector3 VectorBaixo = verticeBaixo.Position - vertice.Position;
                Vector3 VectorBaixoDireita = verticeBaixoDireita.Position - vertice.Position;
                Vector3 VectorDireita = verticeDireita.Position - vertice.Position;

                //Calcula o produto escalar entre vetores
                Vector3 normal1 = Vector3.Cross(VectorEsquerda, VectorEsquerdaBaixo);
                Vector3 normal2 = Vector3.Cross(VectorEsquerdaBaixo, VectorBaixo);
                Vector3 normal3 = Vector3.Cross(VectorBaixo, VectorBaixoDireita);
                Vector3 normal4 = Vector3.Cross(VectorBaixoDireita, VectorDireita);

                Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2) + Vector3.Normalize(normal3) + Vector3.Normalize(normal4)) / 4;
                Normal.Normalize();
                vertices[x].Normal = Normal;
                normalArray[x] = Normal;
            }
            //linha da esquerda
            for (z = 1; z < dimLargura - 1; z++)

            {
                vertice = vertices[z * dimAltura];
                //Descobre os vizinhos do vertices escolhido
                VertexPositionNormalTexture verticeBaixo = vertices[(z * dimAltura) + dimAltura];
                VertexPositionNormalTexture verticeDireitaBaixo = vertices[(z * dimAltura) + dimAltura + 1];
                VertexPositionNormalTexture verticeDireita = vertices[(z * dimAltura) + 1];
                VertexPositionNormalTexture verticeCimaDireita = vertices[(z * dimAltura) - dimAltura + 1];
                VertexPositionNormalTexture verticeCima = vertices[(z * dimAltura) - dimAltura];

                //calcula aqual o vetor pretendido
                Vector3 VectorBaixo = verticeBaixo.Position - vertice.Position;
                Vector3 VectorDireitaBaixo = verticeDireitaBaixo.Position - vertice.Position;
                Vector3 VectorDireita = verticeDireita.Position - vertice.Position;
                Vector3 VectorCimaDireita = verticeCimaDireita.Position - vertice.Position;
                Vector3 VectorCima = verticeCima.Position - vertice.Position;

                //Calcula o produto escalar entre vetores
                Vector3 normal1 = Vector3.Cross(VectorBaixo, VectorDireitaBaixo);
                Vector3 normal2 = Vector3.Cross(VectorDireitaBaixo, VectorDireita);
                Vector3 normal3 = Vector3.Cross(VectorDireita, VectorCimaDireita);
                Vector3 normal4 = Vector3.Cross(VectorCimaDireita, VectorCima);

                Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2) + Vector3.Normalize(normal3) + Vector3.Normalize(normal4)) / 4;
                Normal.Normalize();
                vertices[z * dimAltura].Normal = Normal;
                normalArray[z * dimAltura] = Normal;

            }


            //linha inferior
            for (x = 1; x < dimAltura - 1; x++)
            {
                vertice = vertices[(dimAltura - 1) * dimAltura + x];

                //Descobre os vizinhos do vertices escolhido
                VertexPositionNormalTexture verticeDireita = vertices[(dimAltura - 1) * dimAltura + x + 1];
                VertexPositionNormalTexture verticeDireitaCima = vertices[(dimAltura - 1) * dimAltura + x + 1 - dimAltura];
                VertexPositionNormalTexture verticeCima = vertices[(dimAltura - 1) * dimAltura + x - dimAltura];
                VertexPositionNormalTexture verticeCimaEsquerda = vertices[(dimAltura - 1) * dimAltura + x - dimAltura - 1];
                VertexPositionNormalTexture verticeEsquerda = vertices[(dimAltura - 1) * dimAltura + x - 1];

                //calcula aqual o vetor pretendido
                Vector3 VectorDireita = verticeDireita.Position - vertice.Position;
                Vector3 VectorDireitaCima = verticeDireitaCima.Position - vertice.Position;
                Vector3 VectorCima = verticeCima.Position - vertice.Position;
                Vector3 VectorEsquerdaCima = verticeCimaEsquerda.Position - vertice.Position;
                Vector3 VectorEsquerda = verticeEsquerda.Position - vertice.Position;

                // Calcula o produto escalar entre vetores
                Vector3 normal1 = Vector3.Cross(VectorDireita, VectorDireitaCima);
                Vector3 normal2 = Vector3.Cross(VectorDireitaCima, VectorCima);
                Vector3 normal3 = Vector3.Cross(VectorCima, VectorEsquerdaCima);
                Vector3 normal4 = Vector3.Cross(VectorEsquerdaCima, VectorEsquerda);

                Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2) + Vector3.Normalize(normal3) + Vector3.Normalize(normal4)) / 4;
                Normal.Normalize();
                vertices[(dimAltura - 1) * dimAltura + x].Normal = Normal;
                normalArray[(dimAltura - 1) * dimAltura + x] = Normal;
            }
            //linha direita

            for (z = 2; z < dimAltura - 1; z++)
            {
                vertice = vertices[(dimAltura - 1) * z];

                //Descobre os vizinhos do vertices escolhido
                VertexPositionNormalTexture verticeCima = vertices[(dimAltura - 1) * z - dimAltura];
                VertexPositionNormalTexture verticeEsquerdaCima = vertices[(dimAltura - 1) * z - dimAltura - 1];
                VertexPositionNormalTexture verticeEsquerda = vertices[(dimAltura - 1) * z - 1];
                VertexPositionNormalTexture verticeBaixoEsquerda = vertices[(dimAltura - 1) * z + dimAltura - 1];
                VertexPositionNormalTexture verticeBaixo = vertices[(dimAltura - 1) * z + dimAltura];

                //calcula aqual o vetor pretendido
                Vector3 VectorCima = verticeCima.Position - vertice.Position;
                Vector3 VectorEsquerdaCima = verticeEsquerdaCima.Position - vertice.Position;
                Vector3 VectorEsquerda = verticeEsquerda.Position - vertice.Position;
                Vector3 VectorBaixoEsquerda = verticeBaixoEsquerda.Position - vertice.Position;
                Vector3 VectorBaixo = verticeBaixo.Position - vertice.Position;

                //Calcula o produto escalar entre vetores
                Vector3 normal1 = Vector3.Cross(VectorCima, VectorEsquerdaCima);
                Vector3 normal2 = Vector3.Cross(VectorEsquerdaCima, VectorEsquerda);
                Vector3 normal3 = Vector3.Cross(VectorEsquerda, VectorBaixoEsquerda);
                Vector3 normal4 = Vector3.Cross(VectorBaixoEsquerda, VectorBaixo);

                Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2) + Vector3.Normalize(normal3) + Vector3.Normalize(normal4)) / 4;
                Normal.Normalize();
                vertices[(dimAltura - 1) * z].Normal = Normal;
                normalArray[(dimAltura - 1) * z] = Normal;

            }

            //centro
            for (x = 1; x < dimAltura - 1; x++)
            {
                for (z = 1; z < dimAltura - 1; z++)
                {
                    vertice = vertices[x * dimAltura + z];

                    //Descobre os vizinhos do vertices escolhido
                    VertexPositionNormalTexture verticeCima = vertices[(x * dimAltura + z) - dimAltura];
                    VertexPositionNormalTexture verticeEsquerdaCima = vertices[(x * dimAltura + z) - dimAltura - 1];
                    VertexPositionNormalTexture verticeEsquerda = vertices[(x * dimAltura + z) - 1];
                    VertexPositionNormalTexture verticeBaixoEsquerda = vertices[(x * dimAltura + z) + dimAltura - 1];
                    VertexPositionNormalTexture verticeBaixo = vertices[(x * dimAltura + z) + dimAltura];
                    VertexPositionNormalTexture verticeBaixoDireita = vertices[(x * dimAltura + z) + dimAltura + 1];
                    VertexPositionNormalTexture verticeDireita = vertices[(x * dimAltura + z) + 1];
                    VertexPositionNormalTexture verticeCimaDireita = vertices[(x * dimAltura + z) - dimAltura + 1];

                    //calcula aqual o vetor pretendido
                    Vector3 VectorCima = verticeCima.Position - vertice.Position;
                    Vector3 VectorEsquerdaCima = verticeEsquerdaCima.Position - vertice.Position;
                    Vector3 VectorEsquerda = verticeEsquerda.Position - vertice.Position;
                    Vector3 VectorBaixoEsquerda = verticeBaixoEsquerda.Position - vertice.Position;
                    Vector3 VectorBaixo = verticeBaixo.Position - vertice.Position;
                    Vector3 VectorBaixoDireita = verticeBaixoDireita.Position - vertice.Position;
                    Vector3 VectorDireita = verticeDireita.Position - vertice.Position;
                    Vector3 VectorCimaDireita = verticeCimaDireita.Position - vertice.Position;



                    //Calcula o produto escalar entre vetores
                    Vector3 normal1 = Vector3.Cross(VectorCima, VectorEsquerdaCima);
                    Vector3 normal2 = Vector3.Cross(VectorEsquerdaCima, VectorEsquerda);
                    Vector3 normal3 = Vector3.Cross(VectorEsquerda, VectorBaixoEsquerda);
                    Vector3 normal4 = Vector3.Cross(VectorBaixoEsquerda, VectorBaixo);
                    Vector3 normal5 = Vector3.Cross(VectorBaixo, VectorBaixoDireita);
                    Vector3 normal6 = Vector3.Cross(VectorBaixoDireita, VectorDireita);
                    Vector3 normal7 = Vector3.Cross(VectorDireita, VectorCimaDireita);
                    Vector3 normal8 = Vector3.Cross(VectorCimaDireita, VectorCima);

                    Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2) + Vector3.Normalize(normal3) + Vector3.Normalize(normal4)
                        + Vector3.Normalize(normal5) + Vector3.Normalize(normal6) + Vector3.Normalize(normal7) + Vector3.Normalize(normal8)) / 8;
                    Normal.Normalize();
                    vertices[x * dimAltura + z].Normal = Normal;
                    normalArray[x * dimAltura + z] = Normal;

                }

            }
            //cantos


            {
                x = 0;
                z = 0;
                vertice = vertices[x * dimAltura + z];

                VertexPositionNormalTexture verticeBaixo = vertices[(x * dimAltura + z) + dimAltura];
                VertexPositionNormalTexture verticeBaixoDireita = vertices[(x * dimAltura + z) + dimAltura + 1];
                VertexPositionNormalTexture verticeDireita = vertices[(x * dimAltura + z) + 1];

                Vector3 vectorBaixo = verticeBaixo.Position - vertice.Position;
                Vector3 vectorBaixoDireita = verticeBaixoDireita.Position - vertice.Position;
                Vector3 vectorDireita = verticeDireita.Position - vertice.Position;

                Vector3 normal1 = Vector3.Cross(vectorBaixo, vectorBaixoDireita);
                Vector3 normal2 = Vector3.Cross(vectorBaixoDireita, vectorDireita);
                Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2)) / 2;
                Normal.Normalize();
                vertices[x * dimAltura + z].Normal = Normal;
                normalArray[x * dimAltura + z] = Normal;
            }


            {
                x = 0;
                z = 127;
                vertice = vertices[x * dimAltura + z];

                VertexPositionNormalTexture verticeEsquerda = vertices[(x * dimAltura + z) - 1];
                VertexPositionNormalTexture verticeEsquerdaBaixo = vertices[(x * dimAltura + z) + dimAltura - 1];
                VertexPositionNormalTexture verticeBaixo = vertices[(x * dimAltura + z) + dimAltura];

                Vector3 vectorEsquerda = verticeEsquerda.Position - vertice.Position;
                Vector3 vectorBaixoEsquerda = verticeEsquerdaBaixo.Position - vertice.Position;
                Vector3 vectorBaixo = verticeBaixo.Position - vertice.Position;

                Vector3 normal1 = Vector3.Cross(vectorEsquerda, vectorBaixoEsquerda);
                Vector3 normal2 = Vector3.Cross(vectorBaixoEsquerda, vectorBaixo);
                Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2)) / 2;
                Normal.Normalize();
                vertices[x * dimAltura + z].Normal = Normal;
                normalArray[x * dimAltura + z] = Normal;
            }

            {
                x = 127;
                z = 0;
                vertice = vertices[x * dimAltura + z];

                VertexPositionNormalTexture verticeCima = vertices[(x * dimAltura + z) - dimAltura];
                VertexPositionNormalTexture verticeCimaDireita = vertices[(x * dimAltura + z) - dimAltura + 1];
                VertexPositionNormalTexture verticeDireita = vertices[(x * dimAltura + z) + 1];

                Vector3 vectorCima = verticeCima.Position - vertice.Position;
                Vector3 vectorCimaDireita = verticeCimaDireita.Position - vertice.Position;
                Vector3 vectorDireita = verticeDireita.Position - vertice.Position;

                Vector3 normal1 = Vector3.Cross(vectorCima, vectorCimaDireita);
                Vector3 normal2 = Vector3.Cross(vectorCimaDireita, vectorDireita);
                Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2)) / 2;
                Normal.Normalize();
                vertices[x * dimAltura + z].Normal = Normal;
                normalArray[x * dimAltura + z] = Normal;
            }

            {
                x = 127;
                z = 127;
                vertice = vertices[x * dimAltura + z];

                VertexPositionNormalTexture verticeCima = vertices[(x * dimAltura + z) - dimAltura];
                VertexPositionNormalTexture verticeCimaEsquerda = vertices[(x * dimAltura + z) - dimAltura - 1];
                VertexPositionNormalTexture verticeEsquerda = vertices[(x * dimAltura + z) - 1];

                Vector3 vectorCima = verticeCima.Position - vertice.Position;
                Vector3 vectorCimaEsquerda = verticeCimaEsquerda.Position - vertice.Position;
                Vector3 vectorEsquerda = verticeEsquerda.Position - vertice.Position;

                Vector3 normal1 = Vector3.Cross(vectorCima, vectorCimaEsquerda);
                Vector3 normal2 = Vector3.Cross(vectorCimaEsquerda, vectorEsquerda);
                Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2)) / 2;
                Normal.Normalize();
                vertices[x * dimAltura + z].Normal = Normal;
                normalArray[x * dimAltura + z] = Normal;
            }

            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), (dimAltura) * (dimLargura), BufferUsage.None);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);


        }



        public void Draw(GraphicsDevice device, Camera camara)
        {
            effect.View = camara.viewMatrix;
            effect.Projection = camara.projectionMatrix;
            // Indica o efeito para desenhar os eixos
            effect.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;
            for (int i = 0; i < dimLargura - 1; i++)
            {
                device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, i * 2 * dimAltura, 2 * dimAltura - 2);
            }


        }
    }
}
