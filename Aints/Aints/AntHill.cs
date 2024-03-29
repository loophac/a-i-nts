﻿using System;
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
		protected int larvaSpawn = ConstantsHolder.Singleton.LarvaSpawnCooldown;

        protected float food;
        public float Food
        {
            get { return this.food; }
            set { this.food = value; }
        }

		private String logFile;
		private TextWriter tw;
		private float prevFood;

        public AntHill(Main game, Vector2 position)
			:base(game,true)
        {
			this.scale = 2f;

            food = 2000;
			prevFood = food;
            this.game = game;
            this.Position = position;

			this.logFile = @".\log" + DateTime.Now.Ticks + ".txt";
			tw = new StreamWriter(this.logFile);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Sprite = game.Content.Load<Texture2D>("antHill");
			origin = new Vector2(Sprite.Width, Sprite.Height); //this is dirty, hard coded scale 2
        }

		protected override void UnloadContent()
		{
			base.UnloadContent();
			tw.Close();
		}

        public override void Update(GameTime gameTime)
        {
			//save food evolution in a log file for analysis
			if (this.food != this.prevFood)
			{
                String s = gameTime.TotalGameTime.Ticks + "\t" + this.food.ToString();
				tw.WriteLine( s );
				prevFood = food;
			}

			if (this.larvaSpawn-- < 0)
			{
				if (this.food > ConstantsHolder.Singleton.BirthMinFood)
				{
					this.larvaSpawn = ConstantsHolder.Singleton.LarvaSpawnCooldown;
					game.Reservoir.pickAnt(Position, 0, 0);
					this.food -= ConstantsHolder.Singleton.LarvaCost;
				}
				else
				{
					larvaSpawn = 0;
				}
			}

            base.Update(gameTime);
        }

    }
}
    