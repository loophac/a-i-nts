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
using Aints.Behaviours;


namespace Aints
{
    public abstract class GameObject : Microsoft.Xna.Framework.DrawableGameComponent
    {
		#region props
		protected Main game;

		protected Texture2D sprite;
		public Texture2D Sprite
		{
			get { return sprite; }
        			set
			{
				sprite = value;
			}
		}

        protected List<Behaviour> behaviours;

		protected float collisionRadius;
		public virtual float CollisionRadius
		{
			get { return this.collisionRadius; }
			set { this.collisionRadius = value; }
		}

		protected Vector2 position;
		public Vector2 Position
		{
			get { return this.position; }
			set
			{
				this.position = value;
				UpdateBoundingRectangle();
			}
		}

		protected Vector2 origin;
		public Vector2 Origin
		{
			get { return this.origin; }
			set
			{
				// update the position for the new origin
				this.Position += value - this.Origin;
				//set the origin
				this.origin = value;
			}
		}

		protected float scale = 1.0f;
		public float Scale  
		{
			get { return this.scale; }
			set
			{
				this.scale = value;
			}
		}

		protected float rotation;
		public float Rotation
		{
			get { return this.rotation; }
			set { this.rotation = value; }
		}

		protected Vector2 velocity;
		public Vector2 Velocity
		{
			get { return this.velocity; }
			set { this.velocity = value; }
		}

        protected float maxSpeed;

		protected Rectangle boundingRectangle = new Rectangle();
		public Rectangle BoundingRectangle
		{
			get { return this.boundingRectangle; }
		}

        protected float maxTurnRadiansPerSec;
        public float MaxTurnRadiansPerFrame
        {
            get { return maxTurnRadiansPerSec; }
            set { this.maxTurnRadiansPerSec = value; }
        }

		#endregion

		#region ctor
		public GameObject(Main game, bool active)
			:base(game)
		{
			this.game = game;
            if (active)
            {
                game.Components.Add(this);
            }
			SetupBehaviours();
            maxSpeed = 100f;

			this.Enabled = active;
			this.Visible = active;
		} 
		#endregion

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateBehaviour(gameTime);

            DoMovement(gameTime);

            CheckCollisions(gameTime);
        }

        protected void UpdateBehaviour(GameTime gameTime)
        {
            if (behaviours.Count > 0)
            {
                Vector2 newVelocity = Vector2.Zero;

                foreach (Behaviours.Behaviour behaviour in behaviours)
                {
                    newVelocity += behaviour.RunBehaviour();
                }

               // clamp turn rate
                float oldAngle = (float)Math.Atan2(Velocity.Y, Velocity.X);
				Rotation = (float)Math.Atan2(newVelocity.Y, newVelocity.X);
                Velocity = Vector2.Normalize(new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation))) * Math.Min(newVelocity.Length(),maxSpeed);
                //Velocity = newVelocity;
            }
        }

        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        public virtual void DoMovement(GameTime gameTime)
        {
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

			SpriteBatch spriteBatch = game.SpriteBatch;
            if (Sprite != null)
            {
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                spriteBatch.Draw(Sprite, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 1f);
                spriteBatch.End();
            }
        }

        public virtual void Kill()
        {
            // this should destroy the object and possibly spawn a special graphical effect (eg explosion)
            Enabled = false;
            Visible = false;
            game.Components.Remove(this);
        }

        public virtual void ApplyDamage(float damage)
        {
            // This should apply any damage appropriate to the game object, whether that involves life, energy or whatever
            // Do nothing for a basic game object
        }

		protected virtual void UpdateBoundingRectangle()
		{
		}

        private void SetOriginToCentre()
        {
            this.Origin = GetCenter();
        }

        public Vector2 GetCenter()
        {
            return new Vector2(this.BoundingRectangle.Width / 2, this.BoundingRectangle.Height / 2);
        }

        public virtual void CheckCollisions(GameTime gameTime)
        {

        }

		public bool CollidesWith(GameObject target)
		{
			if (Vector2.Distance(GetCenter(), target.GetCenter()) <= this.CollisionRadius + target.CollisionRadius)
			{
				return true;
			}
			return false;
		}

        protected virtual void SetupBehaviours()
        {
            this.behaviours = new List<Behaviour>();
        }
    }
}