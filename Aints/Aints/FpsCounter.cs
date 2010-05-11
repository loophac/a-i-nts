using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Aints
{
	/// <summary>
	/// got it from here : http://blogs.msdn.com/shawnhar/archive/2007/06/08/displaying-the-framerate.aspx
	/// this draws the fps on the given position, updated once per second
	/// </summary>
	class FpsCounter: DrawableGameComponent
    {
		private Main game;

		private SpriteFont font;
		private Vector2 position;

		private int frameRate = 0;
		private int frameCounter = 0;
		private TimeSpan elapsedTime = TimeSpan.Zero;

		public FpsCounter(Main game, Vector2 position)
            : base(game)
        {
			this.game = game;
			this.position = position;
			game.Components.Add(this);
        }

		protected override void LoadContent()
		{
			font = this.game.Content.Load<SpriteFont>("Font");
		}
		
        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            frameCounter++;

            string fps = frameRate.ToString();

			SpriteBatch spriteBatch = game.SpriteBatch;
			if (spriteBatch != null)
			{
				spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);

				spriteBatch.DrawString(font, fps, position + Vector2.One, Color.Black);
				spriteBatch.DrawString(font, fps, position, Color.White);
				
				spriteBatch.End();
			}
        }
    }
}
