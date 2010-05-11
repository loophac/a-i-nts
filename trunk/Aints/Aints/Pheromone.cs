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
    public class Pheromone : GameObject
    {
		public const float SMELL_MAX = 100;
		public const float SMELL_HALF = SMELL_MAX/2f;
        private const float CONDITION_DISPARITION = 20f;
        private const float EVAPORATION = 0.995f;
		
		#region props
		protected float smell;
        public float Smell
		{
            get{return smell;}
            set { smell = value; }
        }

        protected TypePheromone type;
		public TypePheromone Type
		{
			get { return this.type; }
			set { this.type = value; }
		}
		#endregion

		#region ctor
        public Pheromone(Main game, bool active)
            : base(game, active)
		{
            // make sure it loads and draws
            DrawOrder = 49;
            UpdateOrder = 50;
		}
		#endregion

        #region override
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            smell *= EVAPORATION;
            if (smell < CONDITION_DISPARITION)
            {
                Kill();
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Sprite = game.Content.Load<Texture2D>("pheromone");
        }

        public override void Kill()
        {
            base.Kill();
            game.Reservoir.putBack(this);
        }

		public override void Draw(GameTime gameTime)
		{
			byte alpha = (byte)(smell * 2.5f);
			SpriteBatch spriteBatch = game.SpriteBatch;
			if (Sprite != null)
			{
				spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
				spriteBatch.Draw(Sprite, Position, null, new Color(Color.White, alpha), Rotation, Origin, Scale, SpriteEffects.None, 1f);
				spriteBatch.End();
			}
		}
        #endregion
    }
}
