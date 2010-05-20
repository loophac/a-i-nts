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

		protected Cemetery cemeteryBest;
        protected Cemetery cemeteryToClean;
        protected Vector2 cemeteryToCleanPos;

        protected int cemeterySmell;
        protected Ant cadaver;
        protected AntState previousState;
        protected int leftToClean;

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
            choicePheromones = new Aints.Behaviours.AntBehaviourFollowPheromone(game, this, false);
			// make sure it loads and draws
			DrawOrder = 50;
			UpdateOrder = 50;
		}
		#endregion

		#region override
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //the ant have a certain probability to die each frame
            double ran = game.Random.NextDouble();
            if (ran < 0.5f / ConstantsHolder.Singleton.HalfLife)
            {
                Kill();
            }
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

            //find closest food point
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

			//find bigest cemetery around
            for (int i = 0; i < game.Cemeteries.Count; i++)
            {
                Cemetery possibleCemetery = game.Cemeteries[i];
				//within view range
                if (Vector2.Distance(possibleCemetery.Position, Position) < ConstantsHolder.Singleton.Vision 
					&& possibleCemetery.Count!=0)
                {
                    //find bigest cemetery
                    if ( Vector2.Distance(possibleCemetery.Position,antHill.Position)>=ConstantsHolder.Singleton.SainityDistance 
						&& (cemeteryBest == null || possibleCemetery.Count > cemeterySmell))
                    {
                        //if the ant had a previous best cemetery, it decided to move it in the new one
                        bool needToClean = false;
                        if (cemeteryBest != null 
							&& cemeteryBest != possibleCemetery 
							&& state != AntState.goToCorpse 
							&& state != AntState.transportCorpse)
                        {
                            cemeteryToClean = cemeteryBest;
                            cemeteryToCleanPos = cemeteryToClean.Position;
                            previousState = state;
                            needToClean = true;
                        }
                        //update the biggest known cemetery
                        cemeteryBest = possibleCemetery;
                        cemeterySmell = possibleCemetery.Count;
                        if (state == AntState.transportCorpse)
                        {
                            goal = possibleCemetery.Position;
                            goalLover = ConstantsHolder.Singleton.TransportGoal;
                        }
                        //start to clean the old cemetery
                        if (needToClean)
                        {
                            ChangeState(AntState.backToClean);
                        }
                    }
					////with a decent distance to the hill
					//if (Vector2.Distance(possibleCemetery.Position, antHill.Position) >= ConstantsHolder.Singleton.SainityDistance 
					//    && (cemeteryBest == null || possibleCemetery.Count > cemeterySmell))
					//{
					//    bool needToClean = false;
					//    if (cemeteryBest != null 
					//        && cemeteryBest != possibleCemetery 
					//        && state != AntState.goToCorpse 
					//        && state != AntState.transportCorpse)
					//    {
					//        cemeteryToClean = cemeteryBest;
					//        cemeteryToCleanPos = cemeteryToClean.Position;
					//        previousState = state;
					//        needToClean = true;
					//    }
					//    cemeteryBest = possibleCemetery;
					//    cemeterySmell = possibleCemetery.Count;
					//    if (state == AntState.transportCorpse)
					//    {
					//        goal = possibleCemetery.Position;
					//        goalLover = ConstantsHolder.Singleton.TransportGoal;
					//    }
					//    if (needToClean)
					//    {
					//        ChangeState(AntState.backToClean);
					//    }
					//}

                    //Update the size of the biggest known cemetery
                    if (possibleCemetery == cemeteryBest)
                    {
                        cemeterySmell = possibleCemetery.Count;
                    }
                    //if the cemetery seen is not the biggest the ant start to clean it
                    //if the ant is not already carrying a corpse a going to a corpse
                    else if (state != AntState.goToCorpse && state != AntState.transportCorpse  )
                    {
                        cadaver = possibleCemetery.Last();
                        //handle a little bug that sometime the cemetery have one null cadaver instead of nothing
                        //it would be maybe cleaner to prevent that
                        if (cadaver != null)
                        {
                            cemeteryToClean = possibleCemetery;
                            cemeteryToCleanPos = cemeteryToClean.Position;
                            previousState = state;
                            //remember the size to know if there is a need to come back to clean
                            leftToClean = cemeteryToClean.Count;
                            ChangeState(AntState.goToCorpse);
                        }
                        else
                        {
							//we don't keep empty cemeteries
                            game.Cemeteries.Remove(possibleCemetery);
                            if (state == AntState.backToClean)
                            {
                                cemeteryToClean = null;
                                ChangeState(previousState);
                            }
                        }
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
				|| (pheromonesTick > ConstantsHolder.Singleton.PheromonesGoFood
				&& ConstantsHolder.Singleton.PheromonesGoFood != 0
				&& this.state == AntState.goToFood))
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
                                Vector2 ifLeft = Velocity.Length() * new Vector2((float)Math.Cos(angleDirection-0.5), (float)Math.Sin(angleDirection-0.5));
                                float scalar1 = Attraction.X*ifLeft.X + Attraction.Y*ifLeft.Y;
                                float scalar2 = Attraction.X * Velocity.X + Attraction.Y * Velocity.Y;


                                double ran = game.Random.NextDouble();
                                bool isLeft = false;
                                if (scalar1 > scalar2)
                                {
                                    isLeft = true;
                                }
                                Math.Exp(ran);
                                double threshold = 0.5 + 0.5 * (1 - Math.Exp(-Math.Abs(scalar1/1000)));
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

        public override void Kill()
        {
            Enabled = false;
            Sprite = game.Content.Load<Texture2D>("ant_dead");

			bool stored = false;
			foreach (Cemetery c in game.Cemeteries)
			{
				if (Vector2.Distance(this.Position, c.Position) < ConstantsHolder.Singleton.CemeteryFusionRadius)
				{
					c.Add(this);
					stored = true;
					break;
				}
			}
			if (!stored)
			{
				game.Cemeteries.Add(new Cemetery(this));
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
                case AntState.transportCorpse:
                    cadaver.Position = position;
                    //if the ant don't know a cemetery it will just put the corpse at a sanitary distance of the anthill
                    if (cemeteryBest == null)
                    {
                        if (Vector2.Distance(position, antHill.Position) > ConstantsHolder.Singleton.SainityDistance)
                        {
                            //double tap!
                            cadaver.Kill();
                            if (leftToClean > 0)
                            {
                                ChangeState(AntState.backToClean);
                            }
                            else
                            {
                                ChangeState(previousState);
                            }
                        }
                    }
                    else
                    {
                        // the ant reaches the biggest known cemetery it put the cadaver in the cemetery
                        if (Vector2.Distance(goal, Position) < ConstantsHolder.Singleton.EatingRadius)
                        {
                            //if the best known have desapeared the ant just put the corpse at a sanitary distance from the anthill
                            if (cemeteryBest.Count == 0)
                            {
                                cemeteryBest = null;
                                //double tap!
                                cadaver.Kill();
                            }
                            else
                            {
                                cemeteryBest.Add(cadaver);
                            }
                            cadaver = null;
                            //go back to clean the previous cemetery if it is not known as empty
                            if (leftToClean > 0)
                            {
                                ChangeState(AntState.backToClean);
                            }
                            else
                            {
                                ChangeState(previousState);
                            }
                            break;
                        }   
                    }
                    
                    break;
                case AntState.goToCorpse:
                    if (Vector2.Distance(goal, Position) < ConstantsHolder.Singleton.EatingRadius)
                    {
                        //take a cadaver in a cemetery when it reach it   
                        if (cemeteryToClean.Contains(cadaver))
                        {
                            cemeteryToClean.Remove(cadaver);
                            leftToClean = cemeteryToClean.Count;
                            //if taking the corpse clean the cemetery it will be destroyed
                            if (cemeteryToClean.Count == 0)
                            {
                                game.Cemeteries.Remove(cemeteryToClean);
                                cemeteryToClean = null;
                                
                            }
                            
                            ChangeState(AntState.transportCorpse);
                        }
                        //if the corpse have been taken by an other ant, it return to normal activity
                        else
                        {
                            ChangeState(previousState);
                        }
                        
                    }
                    break;
                //go back the previous cemetery to clean it
                case AntState.backToClean:
                    //if the cemetery is empty the ant remove it and go back to its activity
                    //if it's full the ant will automatically change state to goforCorpse from the cemetery scan in the update function
                    if (Vector2.Distance(goal, Position) < ConstantsHolder.Singleton.Vision)
                    {
                        if (cemeteryToClean == null || cemeteryToClean.Count == 0)
                        {
                            leftToClean = 0;
                            cemeteryToClean = null;
                            if (previousState == AntState.backToClean)
                            {
                                previousState = AntState.lookForFood;
                            }
                            ChangeState(previousState);
                        }
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
                case AntState.goToCorpse:
                    randomLover = ConstantsHolder.Singleton.GoFoodRandom;
                    warLover = 0.1f;
                    goalLover = ConstantsHolder.Singleton.GoFoodGoal;
                    pheromoneLover = ConstantsHolder.Singleton.TransportPheromones;
                    Goal = cadaver.Position;
                    break;
                case AntState.backToClean:
                    randomLover = ConstantsHolder.Singleton.GoFoodRandom;
                    warLover = 0.1f;
                    goalLover = ConstantsHolder.Singleton.GoFoodGoal;
                    pheromoneLover = ConstantsHolder.Singleton.TransportPheromones;
                    Goal = cemeteryToCleanPos;
                    break;
                case AntState.transportCorpse:
                    randomLover = ConstantsHolder.Singleton.TransportRandom;
                    warLover = 0.1f;
                    if (cemeteryBest == null)
                    {
                        goalLover = -ConstantsHolder.Singleton.TransportGoal;
                        Goal = antHill.Position;
                    }
                    else
                    {
                        goalLover = ConstantsHolder.Singleton.TransportGoal;
                        Goal = cemeteryBest.Position;
                    }
                    pheromoneLover = ConstantsHolder.Singleton.GoFoodPheromones;
                    
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
