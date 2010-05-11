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
	public abstract class Ant : GameObject
	{
		#region props
		protected int pheromonesTick;
		protected Vector2 previousPosition = new Vector2();

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
		}


		protected override void SetupBehaviours()
		{
			base.SetupBehaviours();

			this.behaviours.Add(new Behaviours.AntBehaviourFollowPheromone(game, this));
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
			maxSpeed = 200f;
			AntHill = game.AntHill;

			ChangeState(AntState.lookForFood);
		}

		public override void DoMovement(Microsoft.Xna.Framework.GameTime gameTime)
		{
			base.DoMovement(gameTime);
			pheromonesTick++;
			if ((foodCarried > 0 && pheromonesTick > ConstantsHolder.Singleton.PheromonesFood)
				|| pheromonesTick > ConstantsHolder.Singleton.PheromonesNoFood)
			{
				pheromonesTick = 0;
				dropPheromone(TypePheromone.food);
			}
		}

		public override void CheckCollisions(GameTime gameTime)
		{
			Vector2 curPos = this.position;

			foreach (Rectangle r in game.Obstacles)
			{
				if (r.Intersects(this.boundingRectangle))
				{
					this.Position = new Vector2(previousPosition.X, curPos.Y);
					if (r.Intersects(this.boundingRectangle))
					{
						this.Position = new Vector2(curPos.X, previousPosition.Y);
						if (r.Intersects(this.boundingRectangle))
						{
							this.Position = new Vector2(previousPosition.X, previousPosition.Y);
							if (r.Intersects(this.boundingRectangle))
							{
								this.Kill();
							}
						}
					}
				}
			}
		}

		protected override void UpdateBoundingRectangle()
		{
			if (this.boundingRectangle.IsEmpty && this.sprite != null)
			{
				int side = Math.Max(this.sprite.Height, this.sprite.Width);
				this.boundingRectangle = new Rectangle(0, 0, side, side);
			}
			else
			{
				this.boundingRectangle.Location = new Point((int)position.X, (int)position.Y);
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
						Console.WriteLine(game.AntHill.Food);
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
			game.Reservoir.pickPheromone(type, position, Pheromone.SMELL_MAX);
		}
	}
}
