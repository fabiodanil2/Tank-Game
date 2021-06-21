using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrabalhoPratico
{
    class Bullet
    {
        Model bulletModel;
        public Matrix world;
        public Vector3 pos, dir;
        float velocity;
        float time;
        public BoundingSphere boundingSphere;
        public bool bulletDestroid=false;

        public Bullet( ContentManager content)
        {
                   
            LoadContent(content);      
            boundingSphere = new BoundingSphere();
            boundingSphere.Radius = 0.3f;//raio da BoundingSphere
        }

        public void LoadContent(ContentManager content)
        {
            velocity = 0.1f;
            bulletModel = content.Load<Model>("Sphere");
        }

        public void Update(GameTime gameTime)
        {
            boundingSphere.Center = pos;
            time += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 4096f;

            pos += (Vector3.Normalize(dir) * velocity);
            pos.Y -= 0.098f * (time * time);// formula como calcyla a pos na vertical dependendo da gravidade e do tempo
            world = Matrix.CreateScale(0.1f) * Matrix.CreateTranslation(pos);
        }
        public void Draw(GraphicsDevice device, Camera camara)
        {
            bulletModel.Root.Transform = world;   
            
            // Draw the model.
            foreach (ModelMesh mesh in bulletModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = bulletModel.Root.Transform;
                    effect.View = camara.viewMatrix;
                    effect.Projection = camara.projectionMatrix;
                    
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}
