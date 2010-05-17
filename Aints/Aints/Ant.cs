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
    /// props to initialize from GameObject :
    /// behaviours
    /// collisionRadius
    /// position
    /// origin
    /// scale
    /// rotation
    /// velocity
    /// sprite
    /// boundingRectancle
    /// </summary>
	public class Ant : GameObject
	{
		#region props
        protected Behaviours.AntBehaviourFollowPheromone choicePheromones;

		protected int pheromonesTick;
		protected Vector2 previousPosition = new Vector2();

        protected bool goAround;
        public bool GoAround
        {
            get { return goAround; }
            set { goAround = value; }
        }

        protected bool goLeft;
        public bool GoLeft
        {
            get { return goLeft; }
            set { goLeft = value; }
        }
        

		protected float goalLover;
		public float GoalLover
		{
			get { return goalLover; }
		}

		public float InertiaLover
		{
			get { return ConstantsHolder.Singleton._GlobalInertia; }
		}

		protected float pheromoneLover;
		public float PheromoneLover
		{
			get { return pheromoneLover; }
		}

		protected float randomLover;
		public float RandomLover
		{
			get { return randomLover; }
		}

		protected float warLover;
		public float WarLover
		{
			get { return warLover; }
		}

		protected int team;
		public int Team
		{
			get { return this.team; }
			set { this.team = value; }
		}

		protected Vector2 goal;
		public Vector2 Goal
		{
			get { return this.goal; }
			set { this.goal = value; }
		}

		protected AntState state;
		public AntState State
		{
			get { return this.state; }
			set { this.state = value; }
		}

		protected AntHill antHill;
		public AntHill AntHill
		{
			get { return this.antHill; }
			set { this.antHill = value; }
		}

		protected Food food;
		protected Vector2 foodPosition;		
		protected int foodCarried;

		protected float hungry;
		public float Hungry
		{
			get { return this.hungry; }
			set { this.hungry = value; }
		}

		protected int life;
		#endregion

		#region ctor
		public Ant(Main game, bool active)
			: base(game, active)
		{
			hungry = 0;
			life = ConstantsHolder.Singleton.LifeMax;
			pheromonesTick = 0;
            Origin = new Vector2(8, 7);
            choicePheromones = new Aints.Behaviours.AntBehaviourFollowPheromone(game, this, true);
			// make sure it loads and draws
			DrawOrder = 50;
			UpdateOrder = 50;
		}
		#endregion

		#region override
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			CheckState();
			if (Vector2.Distance(AntHill.Position, Position) < ConstantsHolder.Singleton.EatingRadius)
			{
				if (AntHill.Food >= 0)
				{
					AntHill.Food -= Hungry;
					Hungry = 0;
				}
			}
			if (Hungry > ConstantsHolder.Singleton.Starvation)
			{
				Kill();
			}

			foreach (Food possibleFood in game.Foods)
			{
				if (Vector2.Distance(possibleFood.Position, Position) < ConstantsHolder.Singleton.Vision
					&& (food == null || Vector2.Distance(possibleFood.Position, AntHill.Position) < Vector2.Distance(food.Position, AntHill.Position)))
				{
					food = possibleFood;
					foodPosition = possibleFood.Position;
					if (state == AntState.goToFood)
					{
						goal = food.Position;
					}
				}
			}

			this.previousPosition = this.position;
		}
		protected override void LoadContent()
		{
			Sprite = game.Content.Load<Texture2D>("ant");
			this.origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
		}


		protected override void SetupBehaviours()
		{
			base.SetupBehaviours();

			this.behaviours.Add(new Behaviours.AntBehaviourFollowPheromone(game, this,false));
			this.behaviours.Add(new Behaviours.AntBehaviourRandom(game, this));
			this.behaviours.Add(new Behaviours.AntBehaviourFollowGoal(game, this));
			this.behaviours.Add(new Behaviours.AntBehaviourInertia(game, this));
			this.behaviours.Add(new Behaviours.AntHungerBehaviour(game, this));

		}

		public override void Initialize()
		{
			base.Initialize();
			//fixed
			team = -1;
			MaxTurnRadiansPerFrame = 1f;
			maxSpeed = 150f;
			AntHill = game.AntHill;

			ChangeState(AntState.lookForFood);
		}

		public override void DoMovement(Microsoft.Xna.Framework.GameTime gameTime)
		{
			base.DoMovement(gameTime);
			pheromonesTick++;
			if ((foodCarried > 0 
				&& pheromonesTick > ConstantsHolder.Singleton.PheromonesFood
				&& ConstantsHolder.Singleton.PheromonesFood != 0)
				|| (pheromonesTick > ConstantsHolder.Singleton.PheromonesNoFood
				&& ConstantsHolder.Singleton.PheromonesNoFood != 0))
			{
				pheromonesTick = 0;
				dropPheromone(TypePheromone.food);
			}
		}

        public override void CheckCollisions(GameTime gameTime)
        {
            Vector2 curPos = this.position;
            bool hasCollide = false;

            Rectangle vue;
            if (goAround)
            {
                vue = new Rectangle(boundingRectangle.X - ConstantsHolder.Singleton.Vision / 2, boundingRectangle.Y - ConstantsHolder.Singleton.Vision / 2, boundingRectangle.Width + ConstantsHolder.Singleton.Vision, boundingRectangle.Height + ConstantsHolder.Singleton.Vision);
            }
            else
            {
                vue = this.boundingRectangle;
            }

            foreach (Rectangle r in game.Obstacles)
            {
                if (r.Intersects(vue))
                {
                    hasCollide = true;
                    if (r.Intersects(boundingRectangle))
                    {
                        Position -= Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (r.Intersects(this.boundingRectangle))
                        {
                            this.Kill();
                        }
                        else
                        {
                            //each time we collide for the first time we choose a direction
                            //TODO make the random linked to pheromones
                            float angleDirection = (float)Math.Atan2(Velocity.Y, Velocity.X);
                            if (!goAround)
                            {
                                Vector2 Attraction  = choicePheromones.RunBehaviour();
                                Vector2 ifLeft = Velocity.Length() * new Vector2((float)Math.Cos(angleDirection-0.3), (float)Math.Sin(angleDirection-0.3));
                                float scalar1 = Attraction.X*ifLeft.X + Attraction.Y*ifLeft.Y;
                                float scalar2 = Attraction.X * Velocity.X + Attraction.Y * Velocity.Y;


                                double ran = game.Random.NextDouble();
                                bool isLeft = false;
                                if (scalar1 > scalar2)
                                {
                                    isLeft = true;
                                }
                                Math.Exp(ran);
                                double threshold = 0.5 + 0.5 * (1 - Math.Exp(-Math.Abs(scalar1)));
                                if (ran < threshold)
                                {
                                    goLeft = isLeft;
                                }
                                else
                                {
                                    goLeft = !isLeft;
                                }
                            }

                            if (goLeft)
                            {
                                angleDirection += 0.3f;
                            }
                            else
                            {
                                angleDirection -= 0.3f;
                            }
                            Velocity = Velocity.Length() * new Vector2((float)Math.Cos(angleDirection), (float)Math.Sin(angleDirection));
                            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            Rotation = (float)Math.Atan2(Velocity.Y, Velocity.X);
                            if (r.Intersects(this.boundingRectangle))
                            {
                                Position -= Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                        }
                    }

                }
            }
            goAround = hasCollide;
        }		

		protected override void UpdateBoundingRectangle()
		{
			if (this.boundingRectangle.IsEmpty)
			{
				if (this.sprite != null)
				{
					int side = Math.Max(this.sprite.Height, this.sprite.Width);
					this.boundingRectangle = new Rectangle(0, 0, side, side);
				}
				else
				{
					//nothing ?
				}
			}
			else
			{
				this.boundingRectangle.Location = new Point((int)position.X - (int)origin.X, (int)position.Y - (int)origin.Y);
			}
		}

		#endregion

		protected virtual void CheckState()
		{
			//refresh state to take new variables state
			ChangeState(state);

			switch (state)
			{
				case AntState.lookForFood:
					if (food != null)
					{
						ChangeState(AntState.goToFood);
					}
					break;
				case AntState.goToFood:
					if (food.Amount == 0 && Vector2.Distance(foodPosition, Position) < ConstantsHolder.Singleton.Vision)
					{
						food = null;
						ChangeState(AntState.lookForFood);
						break;
					}
					if (Vector2.Distance(foodPosition, Position) < ConstantsHolder.Singleton.EatingRadius)
					{
						foodCarried = ConstantsHolder.Singleton.CarryMax;
						food.Amount -= ConstantsHolder.Singleton.CarryMax;
						ChangeState(AntState.bringBackFood);
					}
					break;
				case AntState.bringBackFood:
					if (Vector2.Distance(AntHill.Position, Position) < ConstantsHolder.Singleton.EatingRadius)
					{
						game.AntHill.Food += foodCarried;
						foodCarried = 0;
						ChangeState(AntState.goToFood);
					}
					break;
			}
		}


		protected virtual void ChangeState(AntState newState)
		{
			state = newState;
			switch (newState)
			{
				case AntState.lookForFood:
					randomLover = ConstantsHolder.Singleton.ScoutRandom;
					warLover = 0.1f;
					goalLover = ConstantsHolder.Singleton.ScoutGoal;
					pheromoneLover = ConstantsHolder.Singleton.ScoutPheromones;
					goal = AntHill.Position;
					break;
				case AntState.bringBackFood:
					randomLover = ConstantsHolder.Singleton.BringFoodRandom;
					warLover = 0.1f;
					goalLover = ConstantsHolder.Singleton.BringFoodGoal;
					pheromoneLover = ConstantsHolder.Singleton.BringFoodPheromones;
					Goal = AntHill.Position;
					break;
				case AntState.goToFood:
					randomLover = ConstantsHolder.Singleton.GoFoodRandom;
					warLover = 0.1f;
					goalLover = ConstantsHolder.Singleton.GoFoodGoal;
					pheromoneLover = ConstantsHolder.Singleton.GoFoodPheromones;
					Goal = foodPosition;
					break;
			}
		}

		protected void dropPheromone(TypePheromone type)
		{
			game.Reservoir.pickPheromone(type, position, Pheromone.SMELL_INIT);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
		}
	}
}
