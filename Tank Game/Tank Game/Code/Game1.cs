using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TrabalhoPratico
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D imagemJpg;
        Texture2D dirt;
        Terreno floor;
        Camera camera;
        Tank tank;
        Tank2 tank2;
        int Life1 = 100;
        int Life2 = 100;
        List<Bullet> bullet;
        List<Bullet> bullet2;
        public const int tileSize = 64; //the tilesize, do not change
        
        public int Width, Height;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1360;
            graphics.PreferredBackBufferHeight = 700;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            base.Initialize();
            floor = new Terreno(GraphicsDevice, imagemJpg, dirt);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);
            imagemJpg = Content.Load<Texture2D>("lh3d1");
            dirt = Content.Load<Texture2D>("3");
            camera = new Camera(GraphicsDevice, floor);
            tank = new Tank(GraphicsDevice, new Vector3(60f, 0f, 60f), this);
            tank2 = new Tank2(GraphicsDevice, Content.Load<Model>("tank1"), new Vector3(70f, 0f, 70f));
            BulletGenarator.Initialize(Content, tank);
            BulletGenarator2.Initialize(Content, tank2);



        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            KeyboardState kb = Keyboard.GetState();
            KeyboardState kc = Keyboard.GetState();
            camera.Update(kb, GraphicsDevice, floor,gameTime,tank);
            tank.Update( kc, floor, gameTime);
            tank2.Update(kb, floor, gameTime, tank, tank2);
            BulletGenarator.UpdateBullets(gameTime,floor);
            BulletGenarator2.UpdateBalas(gameTime, floor);
            bullet = BulletGenarator.GetListOfBulletsActive();
            bullet2 = BulletGenarator2.GetListOfBulletsActive();

            float dis = (tank.boundingSphere.Center - tank2.boundingSphere.Center).Length();
                
            if (dis < tank.boundingSphere.Radius + tank2.boundingSphere.Radius)
            {
  
                tank.pos = tank.NextPos;
                tank2.pos = tank2.NextPos;
            }
           

            foreach (Bullet bala in bullet)
            {

                float d = (bala.boundingSphere.Center - tank2.boundingSphere.Center).Length();
               

                if (d < bala.boundingSphere.Radius + tank2.boundingSphere.Radius)
                {
                    bala.bulletDestroid = true;
                    Life1 -= 10 ;
                }
               


            }

            foreach (Bullet balas2 in bullet2)
            {
                float b = (balas2.boundingSphere.Center - tank.boundingSphere.Center).Length();
                if (b < balas2.boundingSphere.Radius + tank2.boundingSphere.Radius)
                {
                    balas2.bulletDestroid = true;
                    Life2 -= 10;
                }
            }
            if (Life1==0)
            {
                Exit();
            }
            if (Life2 == 0)
            {
                Exit();
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {



                GraphicsDevice.Clear(Color.Blue);

                // TODO: Add your drawing code here
                floor.Draw(GraphicsDevice, camera);
                tank.Draw(GraphicsDevice, camera);
                tank2.Draw(GraphicsDevice, camera);
                BulletGenarator.DrawBullet(GraphicsDevice, camera);
            BulletGenarator2.DrawBullet(GraphicsDevice, camera);

            base.Draw(gameTime);
            }
        }
    }

