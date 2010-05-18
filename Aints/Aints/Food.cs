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
    public class Food : GameObject
    {
        #region props
        protected float amount;
        public float Amount
        {
            get { return this.amount; }
            set { this.amount = value; }
        }
        #endregion

        #region ctor
        public Food(Main game,Vector2 position, float amount):base(game,true)
        {
            this.game = game;
            this.Position = position;
            this.Amount = amount;
            // make sure it loads and draws
            DrawOrder = 49;
            UpdateOrder = 51;
        }
        #endregion

        #region override
        protected override void LoadContent()
        {
            base.LoadContent();
            Sprite = game.Content.Load<Texture2D>("food");
			origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Amount <= 0)
            {
                Amount = 0;
                Kill();
            }
        }

		public override void Draw(GameTime gameTime)
		{
			SpriteBatch spriteBatch = game.SpriteBatch;
			if (Sprite != null)
			{
				spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
				spriteBatch.Draw(Sprite, Position, null, Color.Red, Rotation, Origin, Scale, SpriteEffects.None, 1f);
				spriteBatch.End();
			}
		}

        public override void Kill()
        {
            base.Kill();
            game.Foods.Remove(this);
        }
        #endregion
    }
}
