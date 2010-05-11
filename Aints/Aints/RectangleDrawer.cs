using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Aints
{
	public class RectangleDrawer : DrawableGameComponent
	{
		private Main game;

		private Texture2D dummy;
		private List<Rectangle> rectangles = new List<Rectangle>();
		public List<Rectangle> Rectangles
		{
			get { return this.rectangles; }
		}
		private Vector2 start, stop;

		//assume we start the game with the button released
		private ButtonState prevButtonState = ButtonState.Released;

		public RectangleDrawer(Main game) : base(game)
		{
			this.game = game;
			game.Components.Add(this);

			this.DrawOrder = 100;
		}

		protected override void LoadContent()
		{
			this.dummy = game.Content.Load<Texture2D>("dot");
		}

		public override void Update(GameTime gameTime)
		{
			MouseState current_mouse = Mouse.GetState();

			//if button pressed
			if (current_mouse.LeftButton == ButtonState.Pressed
				&& prevButtonState == ButtonState.Released)
			{
				start = new Vector2(current_mouse.X, current_mouse.Y);
			}

			//if button released
			if (current_mouse.LeftButton == ButtonState.Released
				&& prevButtonState == ButtonState.Pressed)
			{
				stop = new Vector2(current_mouse.X, current_mouse.Y);
				this.rectangles.Add(buildRectangle(start, stop));
			}

			prevButtonState = current_mouse.LeftButton;
		}

		public override void Draw(GameTime gameTime)
		{
			SpriteBatch spriteBatch = game.SpriteBatch;
			if (spriteBatch != null)
			{
				spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);

				foreach(Rectangle r in rectangles)
				{
					spriteBatch.Draw(dummy, r, Color.DarkSlateGray);
				}

				spriteBatch.End();
			}
		}

		private Rectangle buildRectangle(Vector2 a, Vector2 b)
		{
			int x, y, w, h;
			x = (int)Math.Min(a.X, b.X);
			y = (int)Math.Min(a.Y, b.Y);
			w = (int)Math.Abs(a.X - b.X);
			h = (int)Math.Abs(a.Y - b.Y);

			return new Rectangle(x, y, w, h);
		}
	}
}
