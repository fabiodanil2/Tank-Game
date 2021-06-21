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
    class Tank2
    {
        Model myModel;
        Bullet Bullet;
        SystemP ParticuleSystem;

        public Vector3 pos, NextPos, dir;

        float scale;
        float gameTime, gameTimePassed;
        public float velocity = 8f;
        public float turrentAngle = 0.0f;// angulo de rotaçao
        public float cannonAngle = 0.0f; // angulo do canhao
        float angle = 0.0f;
        float leftFrontWheelRotation = 0.0f;// angulo das rodas
        float rightFrontWheelRotation = 0.0f;
        float leftBackWheelRotation = 0.0f;
        float rightBackWheelRotation = 0.0f;

        

        ModelBone LeftBackWheelBone;
        ModelBone RightBackWheelBone;
        ModelBone LeftFrontWheelBone;
        ModelBone RightFrontWheelBone;
        ModelBone LeftSteerBone;
        ModelBone RightSteerBone;
        ModelBone turrentBone;
        ModelBone cannonBone;
        Matrix leftBackWheelBoneTransform;
        Matrix rightBackWheelBoneTransform;
        Matrix leftFrontWheelBoneTransform;
        Matrix rightFrontWheelBoneTransform;
        public Matrix rotation;
        Matrix cannonTransform;
        Matrix turrentTransform;
        public Matrix world;
        Matrix[] boneTransform;

        bool playerControl = true;

        public BoundingSphere boundingSphere;







        public Tank2(GraphicsDevice device, Model m, Vector3 InitialPos)
        {
            myModel = m;
            scale = 0.004f;
            //posiçao inicial do tanque
            pos = InitialPos;



            //load das "imagens"
            LeftBackWheelBone = myModel.Bones["l_back_wheel_geo"];
            RightBackWheelBone = myModel.Bones["r_back_wheel_geo"];
            LeftFrontWheelBone = myModel.Bones["l_front_wheel_geo"];
            RightFrontWheelBone = myModel.Bones["r_front_wheel_geo"];
            LeftSteerBone = myModel.Bones["l_steer_geo"];
            RightSteerBone = myModel.Bones["r_steer_geo"];
            turrentBone = myModel.Bones["turret_geo"];
            cannonBone = myModel.Bones["canon_geo"];


            turrentTransform = turrentBone.Transform;
            cannonTransform = cannonBone.Transform;

            //transform dos bones das rodas
            leftBackWheelBoneTransform = LeftBackWheelBone.Transform;
            rightBackWheelBoneTransform = RightBackWheelBone.Transform;
            leftFrontWheelBoneTransform = LeftFrontWheelBone.Transform;
            rightFrontWheelBoneTransform = RightFrontWheelBone.Transform;


            boneTransform = new Matrix[myModel.Bones.Count];
            myModel.Root.Transform = Matrix.CreateScale(scale);

            //particulas
            ParticuleSystem = new SystemP(device, pos, 2.8f, 0.5f, world);



        }

        public void Update(KeyboardState kb, Terreno terreno, GameTime gametime, Tank tank, Tank2 tank2)
        {
            boundingSphere = new BoundingSphere();
            boundingSphere.Radius = 2f;
            boundingSphere.Center = pos;

            if (Bullet != null)
                Bullet.Update(gametime);

            if (kb.IsKeyDown(Keys.F9))
            {
                playerControl = false;
                velocity = 0;
            }


            if (playerControl)
            {



                if (kb.IsKeyDown(Keys.Up))
                {
                    if (cannonAngle > -0.8f)
                        cannonAngle -= 0.01f;
                }

                if (kb.IsKeyDown(Keys.Right))
                    turrentAngle -= MathHelper.ToRadians(1.0f);

                if (kb.IsKeyDown(Keys.Left))
                    turrentAngle += MathHelper.ToRadians(1.0f);

                if (kb.IsKeyDown(Keys.Down))
                {
                    if (cannonAngle < 0.2f)
                        cannonAngle += 0.01f;
                }

                if (kb.IsKeyDown(Keys.M))
                {
                    gameTime += (float)gametime.ElapsedGameTime.TotalSeconds;
                    if (gameTime - gameTimePassed > 0.5f)
                    {
                        BulletGenarator2.Shoot();
                        gameTimePassed = gameTime;

                    }
                }


                Matrix translation = Matrix.CreateTranslation(pos);
                Matrix Scale = Matrix.CreateScale(scale);
                rotation = Matrix.CreateRotationY(angle);
                dir = Vector3.Transform(Vector3.UnitZ, rotation);//o canhao so mexe em altura e em relacao a turret                
                NextPos = pos;

                if (kb.IsKeyDown(Keys.K))
                {
                    pos = pos + dir * velocity * (float)gametime.ElapsedGameTime.TotalSeconds;
                    leftFrontWheelRotation -= MathHelper.ToRadians(5.0f);
                    rightFrontWheelRotation -= MathHelper.ToRadians(5.0f);
                    leftBackWheelRotation -= MathHelper.ToRadians(5.0f);
                    rightBackWheelRotation -= MathHelper.ToRadians(5.0f);
                }

                if (kb.IsKeyDown(Keys.I))
                {
                    pos = pos - dir * velocity * (float)gametime.ElapsedGameTime.TotalSeconds;

                    leftFrontWheelRotation += MathHelper.ToRadians(5.0f);
                    rightFrontWheelRotation += MathHelper.ToRadians(5.0f);
                    leftBackWheelRotation += MathHelper.ToRadians(5.0f);
                    rightBackWheelRotation += MathHelper.ToRadians(5.0f);
                }

                if (kb.IsKeyDown(Keys.J))
                {
                    angle = angle + 0.05f;     //speed rotation of the tank
                    leftFrontWheelRotation += MathHelper.ToRadians(5.0f);
                    leftBackWheelRotation += MathHelper.ToRadians(5.0f);
                }

                if (kb.IsKeyDown(Keys.L))
                {
                    angle = angle - 0.05f;       //speed rotation of the tank

                    rightFrontWheelRotation += MathHelper.ToRadians(5.0f);
                    rightBackWheelRotation += MathHelper.ToRadians(5.0f);
                }



                //Nao deixa o tank sair do mapa
                if ((pos.X > 126 || pos.X < 0) || (pos.Z > 126 || pos.Z < 0))
                    pos = NextPos;


                //altura a que o tanque esta doo terreno
                pos.Y = terreno.altura(pos.X, pos.Z);

                Vector3 Normal = terreno.Normal(pos.X, pos.Z);
                Vector3 Right = Vector3.Cross(dir, Normal);
                Right.Normalize();
                dir = Vector3.Cross(Normal, Right);


                rotation.Forward = dir;
                rotation.Up = Normal;
                rotation.Right = Right;


                LeftFrontWheelBone.Transform = Matrix.CreateRotationX(leftFrontWheelRotation) * leftFrontWheelBoneTransform;
                RightFrontWheelBone.Transform = Matrix.CreateRotationX(rightFrontWheelRotation) * rightFrontWheelBoneTransform;
                LeftBackWheelBone.Transform = Matrix.CreateRotationX(leftBackWheelRotation) * leftBackWheelBoneTransform;
                RightBackWheelBone.Transform = Matrix.CreateRotationX(rightBackWheelRotation) * rightBackWheelBoneTransform;

                turrentBone.Transform = Matrix.CreateRotationY(turrentAngle) * turrentTransform;
                cannonBone.Transform = Matrix.CreateRotationX(cannonAngle) * cannonTransform;

                myModel.CopyAbsoluteBoneTransformsTo(boneTransform);
                myModel.Root.Transform = Scale * rotation * translation;


            }
            else
            {
                boidControl(tank, terreno, gametime);
                gameTime += (float)gametime.ElapsedGameTime.TotalSeconds;
                if (gameTime - gameTimePassed > 0.5f)
                {
                    BulletGenarator2.Shoot();
                    gameTimePassed = gameTime;


                }

            }

        }

        private void boidControl(Tank tank, Terreno terreno, GameTime gametime)
        {
            Vector3 Vseek, A;
            float MaxA = 5f;
            float MaxSpeed = 5f;

            Vseek = tank.pos - pos;
            Vseek.Normalize();
            Vseek *= MaxSpeed;
            dir.Normalize();

            Vector3 v = dir * velocity;
            A = (Vseek - v);
            A.Normalize();
            A *= MaxA;

            v = v + A * (float)gametime.ElapsedGameTime.TotalSeconds;
            velocity = v.Length();
            dir = v;
            dir.Normalize();

            NextPos = pos;

            pos = pos + dir * velocity * (float)gametime.ElapsedGameTime.TotalSeconds;

            if ((pos.X > 126 || pos.X < 0) || (pos.Z > 126 || pos.Z < 0))
                pos = NextPos;


            pos.Y = terreno.altura(pos.X, pos.Z);

            Vector3 Normal = terreno.Normal(pos.X, pos.Z);
            Vector3 Right = Vector3.Cross(dir, Normal);
            Right.Normalize();
            dir = Vector3.Cross(Normal, Right);
            dir.Normalize();

            rotation.Forward = -dir;
            rotation.Up = Normal;
            rotation.Right = -Right;

            leftFrontWheelRotation += MathHelper.ToRadians(5.0f);
            rightFrontWheelRotation += MathHelper.ToRadians(5.0f);
            leftBackWheelRotation += MathHelper.ToRadians(5.0f);
            rightBackWheelRotation += MathHelper.ToRadians(5.0f);

            LeftFrontWheelBone.Transform = Matrix.CreateRotationX(leftFrontWheelRotation) * leftFrontWheelBoneTransform;
            RightFrontWheelBone.Transform = Matrix.CreateRotationX(rightFrontWheelRotation) * rightFrontWheelBoneTransform;
            LeftBackWheelBone.Transform = Matrix.CreateRotationX(leftBackWheelRotation) * leftBackWheelBoneTransform;
            RightBackWheelBone.Transform = Matrix.CreateRotationX(rightBackWheelRotation) * rightBackWheelBoneTransform;

            Matrix translation = Matrix.CreateTranslation(pos);
            Matrix escala = Matrix.CreateScale(scale);

            myModel.CopyAbsoluteBoneTransformsTo(boneTransform);
            myModel.Root.Transform = escala * rotation * translation;

        }


        public void Draw(GraphicsDevice device, Camera camara)
        {

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransform[mesh.ParentBone.Index];// saber qual e o bone 
                    effect.View = camara.viewMatrix;
                    effect.Projection = camara.projectionMatrix;
                    effect.EnableDefaultLighting();
                }
                // Draw each mesh of the model
                mesh.Draw();
            }
            if (Bullet != null)
                Bullet.Draw(device, camara);

        }
    }
}



