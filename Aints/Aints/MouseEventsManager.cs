using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Aints
{
	public class MouseEventsManager : DrawableGameComponent
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
		private ButtonState prevButtonLeft = ButtonState.Released;
		private ButtonState prevButtonRight = ButtonState.Released;
		private ButtonState prevButtonMiddle = ButtonState.Released;

		public MouseEventsManager(Main game) : base(game)
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
				&& prevButtonLeft == ButtonState.Released)
			{
				start = new Vector2(current_mouse.X, current_mouse.Y);
			}
			//if button released
			if (current_mouse.LeftButton == ButtonState.Released
				&& prevButtonLeft == ButtonState.Pressed)
			{
				stop = new Vector2(current_mouse.X, current_mouse.Y);
				this.rectangles.Add(buildRectangle(start, stop));
			}

			if (current_mouse.RightButton == ButtonState.Pressed
				&& prevButtonRight == ButtonState.Released)
			{
				game.Foods.Add(new Food(game, new Vector2(current_mouse.X, current_mouse.Y), 100000f));
			}

			if (current_mouse.MiddleButton == ButtonState.Pressed
				&& prevButtonMiddle == ButtonState.Released)
			{
				Vector2 mouse = new Vector2(current_mouse.X, current_mouse.Y);
				for (int i = 0; i < game.PheromonesFood.Values.Count; i++)
				{
					Pheromone p = game.PheromonesFood.Values[i];
					if (Vector2.Distance(p.Position, mouse) < 50)
					{
						p.Kill();
					}
				}

				for (int i = 0; i < game.Foods.Count; i++)
				{
					Food p = game.Foods[i];
					if (Vector2.Distance(p.Position, mouse) < 50)
					{
						p.Kill();
					}
				}
			}

			prevButtonLeft = current_mouse.LeftButton;
			prevButtonRight = current_mouse.RightButton;
			prevButtonMiddle = current_mouse.MiddleButton;
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
