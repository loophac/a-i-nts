using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Aints
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
	{
		public const float G_PHEROMONES = 10f;
        public const int NUMBER_FOOD = 20;
        public const int NUMBER_ANTS = 100;

		#region props
		protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

		protected Random random = new Random();
		public Random Random
		{
			get { return random; }
		}

		protected PheroSortedList pheromonesFood = new PheroSortedList();
		public PheroSortedList PheromonesFood
		{
			get { return pheromonesFood; }
		}

		protected PheroSortedList pheromonesWar = new PheroSortedList();
		public PheroSortedList PheromonesWar
		{
			get { return pheromonesWar; }
		}

		protected Pools reservoir;
		public Pools Reservoir
		{
			get { return this.reservoir; }
		}

        protected AntHill antHill;
        public AntHill AntHill
        {
            get { return this.antHill; }
        }

        protected List<Food> foods = new List<Food>();
        public List<Food> Foods
        {
            get { return this.foods; }
            set { this.foods = value; }
        }

		private MouseEventsManager rd;
		public List<Rectangle> Obstacles
		{
			get { return this.rd.Rectangles; }
		}

        private List<Cemetery> cemeteries;
        public List<Cemetery> Cemeteries
        {
            get { return this.Cemeteries; }
            set { this.Cemeteries = value; }
        }

		private Texture2D backGround;
		#endregion

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
		{
			//fit to the bg
			graphics.PreferredBackBufferWidth = 1024;
			graphics.PreferredBackBufferHeight = 768;
			//graphics.IsFullScreen = true;
			graphics.ApplyChanges();

            Content.RootDirectory = "Content";
			new FpsCounter(this, new Vector2(graphics.GraphicsDevice.Viewport.Width - 25, 5)); //displays FPS

			ConstantsForm c = new ConstantsForm();
			c.Show();

			rd = new MouseEventsManager(this);
			Vector2 startingPoint = new Vector2(Random.Next(GraphicsDevice.Viewport.Width), Random.Next(GraphicsDevice.Viewport.Height));
			antHill = new AntHill(this, startingPoint);
			this.reservoir = new Pools(this);
            this.Cemeteries = new List<Cemetery>() ;

			//ajout des fourmis
            for (int i = 0; i < NUMBER_ANTS; i++)
            {
				reservoir.pickAnt(startingPoint, 0f, 0);
            }

            //ajout de la bouffe
            for (int i = 0; i < NUMBER_FOOD; i++)
            {
                Foods.Add(new Food(this, new Vector2(Random.Next(GraphicsDevice.Viewport.Width),Random.Next(GraphicsDevice.Viewport.Height)),500));
            }

			this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

			backGround = Content.Load<Texture2D>("map");
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
			base.UnloadContent(); //this is necessary for the log files
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LimeGreen);

			spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
			spriteBatch.Draw(backGround, Vector2.Zero, Color.White);
			spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
