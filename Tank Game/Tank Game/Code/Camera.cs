using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabalhoPratico
{
    class Camera
    {
        public Matrix viewMatrix;
        Vector3 pos; 

        float yaw;
        float pitch;



        Vector3 normal;
        public Matrix projectionMatrix; //

        float fieldOfView;
        float aspectRadio;
        float nearPlane;
        float farPlane;
        int c = 3;




        public Camera(GraphicsDevice device, Terreno terreno)
        {
            //inicia os valores default
            pos = new Vector3(20.0f, 5.0f, 20.0f);// pos da camara
            yaw = 0.0f; // inicializamos a zero
            pitch = 0.0f;
            Vector3 dirDefault = new Vector3(0.0f, 0.0f, -1.0f); // vetor da direçao da camara
            Matrix rotacionCamara = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0.0f);//rodar a direçao atarves de uma matriz de rotaçao a pala do yaw w do pitch


            Vector3 dir = Vector3.Transform(dirDefault, rotacionCamara);
            Vector3 target = pos + dir;
            normal = Vector3.Up;

            viewMatrix = Matrix.CreateLookAt(pos, target, normal);
            fieldOfView = MathHelper.ToRadians(45.0f);
            aspectRadio = (float)device.Viewport.Width / device.Viewport.Height;
            nearPlane = 0.1f;
            farPlane = 1000.0f;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRadio, nearPlane, farPlane);


        }

        public void Update(KeyboardState kb, GraphicsDevice device, Terreno terreno, GameTime gameTime, Tank tank)
        {

            MouseState ms = Mouse.GetState();
            Vector2 center = new Vector2(device.Viewport.Width / 2, device.Viewport.Height / 2);
            float delta_X = ms.X - center.X;
            float delta_Y = ms.Y - center.Y;

            yaw += -(delta_X) * MathHelper.ToRadians(0.5f);// sensibilidade do rato 0.1f graus por pixel

            pitch += -(delta_Y) * MathHelper.ToRadians(0.5f);


            Vector3 dirDefault = new Vector3(0f, 0f, -2.0f); // vetor da direçao da camara
            Matrix rotacaoCamara = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0.0f);//rodar a direçao atarves de uma matriz de rotaçao a pala do yaw w do pitch
            Vector3 dir = Vector3.Transform(dirDefault, rotacaoCamara);
            Vector3 novaPos = pos;

            if (kb.IsKeyDown(Keys.F1))
            {
                c = 1;
            }
            else if (kb.IsKeyDown(Keys.F2))
            {
                c = 2;
            }
            else if (kb.IsKeyDown(Keys.F3))
            {
                c = 3;
            }

            switch (c)
            {


                case 1:

                    Vector3 Right = Vector3.Cross(dir, Vector3.Up); //produto escalar de 2 vetores
                    Right.Normalize();// normalizar o vetor para que tenha o tamanho 1
                                      //andar para cima e baixo
                    if (kb.IsKeyDown(Keys.NumPad8))
                        pos = pos + dir * 0.1f; // a velocidade que se prentedner mover a camara no espaço3d em metros por segundo
                    if (kb.IsKeyDown(Keys.NumPad5))
                        pos = pos - dir * 0.1f;
                    if (kb.IsKeyDown(Keys.NumPad4))
                        pos = pos - Right * 0.1f;
                    if (kb.IsKeyDown(Keys.NumPad6))
                        pos = pos + Right * 0.1f;

                    if ((pos.X > 128 || pos.X < 0) || (pos.Z > 128 || pos.Z < 0))
                    {
                        pos.Y = 0f;
                    }
                    else
                    {

                        pos.Y = 1.5f + terreno.altura(pos.X, pos.Z);


                        Vector3 Target = pos + dir;
                        viewMatrix = Matrix.CreateLookAt(pos, Target, normal);
                        Mouse.SetPosition((int)center.X, (int)center.Y);//
                    }

                    break;

                case 2:
                     Right = Vector3.Cross(dir, Vector3.Up); //produto escalar de 2 vetores
                    Right.Normalize();// normalizar o vetor para que tenha o tamanho 1
                    

                    if (kb.IsKeyDown(Keys.NumPad2))
                        pos = pos + dir * 0.1f; // a velocidade que se prentedner mover a camara no espaço3d em metros por segundo
                    if (kb.IsKeyDown(Keys.NumPad0))
                        pos = pos - dir * 0.1f;
                    if (kb.IsKeyDown(Keys.NumPad1))
                        pos = pos - Right * 0.1f;
                    if (kb.IsKeyDown(Keys.NumPad3))
                        pos = pos + Right * 0.1f;

                    
                    Vector3 right1 = Vector3.Cross(dir, Vector3.Up); //produto externo de 2 vetores
                    right1.Normalize();// normalizar o vetor para que tenha o tamanho 1
                                       //andar para cima e baixo
                                       //if (kb.IsKeyDown(Keys.W))
                

                    

                    Vector3 target1 = pos + dir;
                    viewMatrix = Matrix.CreateLookAt(pos, target1, normal);
                    Mouse.SetPosition((int)center.X, (int)center.Y);

                    break;

                case 3:
                    Vector3 tankpos = tank.boneTransform[tank.turrentBone.Index].Translation;//O tankpos vai buscar a posição do bone da turrent. 

                    Matrix rotacion = Matrix.CreateRotationX(tank.cannonAngle) * Matrix.CreateRotationY(tank.turrentAngle) * tank.Rotation;
                    
                    Vector3 dircamera = tank.boneTransform[tank.turrentBone.Index].Backward;
                    
                    dircamera.Normalize();

                    Vector3 target2 = tankpos + dircamera;

                    tankpos = tankpos - dircamera*10;

                    viewMatrix = Matrix.CreateLookAt(tankpos, target2, tank.Normal);
                    Mouse.SetPosition((int)center.X, (int)center.Y);


                    break;

            }


        }
    }
}
