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
using System.IO;
using System.Globalization;

namespace Aints
{
    public class AntHill:GameObject
    {
        protected int larva;
        protected float lavraPerSec;
        protected float food;
        public float Food
        {
            get { return this.food; }
            set { this.food = value; }
        }

		private String logFile;
		private TextWriter tw;
		private float prevFood;

        public AntHill(Main game):base(game,true)
        {
            food = 2000;
			prevFood = food;
            this.game = game;
            Position = new Vector2(50, 50);

			this.logFile = @".\log" + DateTime.Now.Ticks + ".txt";
			tw = new StreamWriter(this.logFile);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Sprite = game.Content.Load<Texture2D>("antHill");
        }

		protected override void UnloadContent()
		{
			base.UnloadContent();
			tw.Close();
		}

        public override void Update(GameTime gameTime)
        {
			if (this.food != this.prevFood)
			{
				//we ensure to have a dot to separate the decimal part, it's easier to import
				tw.WriteLine(this.food.ToString("F", new CultureInfo("en-US")) + "\t" + gameTime.TotalGameTime.Ticks);
				prevFood = food;
			}

            base.Update(gameTime);
        }

    }
}
