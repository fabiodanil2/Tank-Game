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
    class Tank
    {
        Model myModel;
        Bullet Bullet;
        SystemP ParticuleSystem;

        public Vector3 pos, Normal, Right, NextPos, dir;

        float scale;
        float gameTime, gameTimePassed;
        float timer;
        public float velocity = 8f;
        public float cannonAngle;
        public float turrentAngle;
        public float angle = 0.0f;
        float leftFrontWheelRotation = 0.0f;// angulo das rodas
        float rightFrontWheelRotation = 0.0f;
        float leftBackWheelRotation = 0.0f;
        float rightBackWheelRotation = 0.0f;

        // model das todas
        ModelBone LeftBackWheelBone;
        ModelBone RightBackWheelBone;
        ModelBone LeftFrontWheelBone;
        ModelBone RightFrontWheelBone;
        public ModelBone turrentBone;
        public ModelBone cannonBone;

        Matrix cannonTransform;
        Matrix turrentTransform;
        public Matrix[] boneTransform;
        Matrix leftBackWheelBoneTransform;
        Matrix rightBackWheelBoneTransform;
        Matrix leftFrontWheelBoneTransform;
        Matrix rightFrontWheelBoneTransform;
        public Matrix world;
        public Matrix Rotation;

        public BoundingSphere boundingSphere;

        bool ParticuleOn;
  

        public Tank(GraphicsDevice device, Vector3 InitialPos, Game1 game)
        {

            myModel = game.Content.Load<Model>("tank");
            scale = 0.004f;//tamanho do tanque
            //posiçao inicial do tanque
            pos = InitialPos;




            //load das "imagens"
            LeftBackWheelBone = myModel.Bones["l_back_wheel_geo"];
            RightBackWheelBone = myModel.Bones["r_back_wheel_geo"];
            LeftFrontWheelBone = myModel.Bones["l_front_wheel_geo"];
            RightFrontWheelBone = myModel.Bones["r_front_wheel_geo"];

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
            ParticuleSystem = new SystemP(device, pos, 2.3f, 0.2f, world);

        }

        public void Update(KeyboardState kc, Terreno terreno, GameTime gametime)
        {

            if (Bullet != null)
                Bullet.Update(gametime);


            if (kc.IsKeyDown(Keys.T))
            {
                if (cannonAngle > -0.8f)
                    cannonAngle -= 0.01f;
            }

            if (kc.IsKeyDown(Keys.H))
                turrentAngle -= MathHelper.ToRadians(1.0f);

            if (kc.IsKeyDown(Keys.F))
                turrentAngle += MathHelper.ToRadians(1.0f);

            if (kc.IsKeyDown(Keys.G))
            {
                if (cannonAngle < 0.2f)
                    cannonAngle += 0.01f;
            }


            timer += (float)gametime.ElapsedGameTime.TotalSeconds;

            if (timer > 1)
            {
                if (kc.IsKeyDown(Keys.Space))
                {
                    gameTime += (float)gametime.ElapsedGameTime.TotalSeconds;
                    if (gameTime - gameTimePassed > 0.5f)
                    {
                        BulletGenarator.Shoot();
                        gameTimePassed = gameTime;
                        timer = 0;

                    }

                }

            }






            Matrix translation = Matrix.CreateTranslation(pos);


            Matrix Scale = Matrix.CreateScale(scale);
            Rotation = Matrix.CreateRotationY(angle);

            dir = Vector3.Transform(-Vector3.UnitZ, Rotation);//o canhao so mexe em altura e em relacao a turret           
            NextPos = pos;

            if (kc.IsKeyDown(Keys.S))
            {
                pos = pos + dir * velocity * (float)gametime.ElapsedGameTime.TotalSeconds;
                // rotaçao das rodas e velocidade
                leftFrontWheelRotation -= MathHelper.ToRadians(5.0f);
                rightFrontWheelRotation -= MathHelper.ToRadians(5.0f);
                leftBackWheelRotation -= MathHelper.ToRadians(5.0f);
                rightBackWheelRotation -= MathHelper.ToRadians(5.0f);
            }

            if (kc.IsKeyDown(Keys.W))
            {
                ParticuleOn = true;
                 pos = pos - dir * velocity * (float)gametime.ElapsedGameTime.TotalSeconds;
                leftFrontWheelRotation += MathHelper.ToRadians(5.0f);
                rightFrontWheelRotation += MathHelper.ToRadians(5.0f);
                leftBackWheelRotation += MathHelper.ToRadians(5.0f);
                rightBackWheelRotation += MathHelper.ToRadians(5.0f);



            }


            if (kc.IsKeyDown(Keys.A))
            {
                angle = angle + 0.05f;     //speed rotation of the tank
                leftFrontWheelRotation += MathHelper.ToRadians(5.0f);
                leftBackWheelRotation += MathHelper.ToRadians(5.0f);
            }

            if (kc.IsKeyDown(Keys.D))
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

            Normal = terreno.Normal(pos.X, pos.Z);
            Right = Vector3.Cross(dir, Normal);
            Right.Normalize();
            dir = Vector3.Cross(Normal, Right);

            Rotation = Matrix.Identity;
            Rotation.Forward = dir;
            Rotation.Up = Normal;
            Rotation.Right = Right;




            if (ParticuleOn)
                ParticuleSystem.Update(gametime, pos, Vector3.Cross(Normal, Right), this);





            turrentBone.Transform = Matrix.CreateRotationY(turrentAngle) * turrentTransform;
            cannonBone.Transform = Matrix.CreateRotationX(cannonAngle) * cannonTransform;

            // rotaçao das rodas  e posiçao
            LeftFrontWheelBone.Transform = Matrix.CreateRotationX(leftFrontWheelRotation) * leftFrontWheelBoneTransform;
            RightFrontWheelBone.Transform = Matrix.CreateRotationX(rightFrontWheelRotation) * rightFrontWheelBoneTransform;
            LeftBackWheelBone.Transform = Matrix.CreateRotationX(leftBackWheelRotation) * leftBackWheelBoneTransform;
            RightBackWheelBone.Transform = Matrix.CreateRotationX(rightBackWheelRotation) * rightBackWheelBoneTransform;

            myModel.CopyAbsoluteBoneTransformsTo(boneTransform);
            myModel.Root.Transform = Scale * Rotation * translation;

            boundingSphere = new BoundingSphere();
            boundingSphere.Radius = 2f;
            boundingSphere.Center = pos;

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

            ParticuleSystem.Draw(device, camara);
        }


    }
}



