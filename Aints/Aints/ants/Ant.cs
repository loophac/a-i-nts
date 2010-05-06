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
        public const int LIFE_MAX = 100;
        public const int PHEROMONES_PERIOD = 10;
        public const int CARRY_MAX = 10;
        public const int TOLERANCE_VISION = 50;
        public const int TOLERANCE_EAT = 10;
        public const int STARVATION = 20;


        #region props
        protected int pheromonesTick;

        protected float goalLover;
        public float GoalLover
        {
            get { return goalLover; }
        }

        protected float inertiaLover;
        public float InertiaLover
        {
            get { return inertiaLover; }
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
            life = LIFE_MAX;
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
            if (Vector2.Distance(AntHill.Position, Position) < TOLERANCE_EAT)
            {
                if (AntHill.Food >= 0)
                {
                    AntHill.Food -= Hungry;
                    Hungry = 0;
                }
            }
            if (Hungry > STARVATION)
            {
                Kill();
            }

            foreach (Food possibleFood in game.Foods)
            {
                if (Vector2.Distance(possibleFood.Position, Position) < TOLERANCE_VISION && (food == null || Vector2.Distance(possibleFood.Position, AntHill.Position) < Vector2.Distance(food.Position, AntHill.Position)))
                {
                    food = possibleFood;
                    foodPosition = possibleFood.Position;
                    if (state == AntState.goToFood)
                    {
                        goal = food.Position;
                    }
                }
            }
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
            inertiaLover = 1f;
            AntHill = game.AntHill;

            ChangeState(AntState.lookForFood);
        }

        public override void DoMovement(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.DoMovement(gameTime);
            if (pheromonesTick++ > PHEROMONES_PERIOD)
            {
                pheromonesTick = 0;
                dropPheromone(TypePheromone.food);
            }
        }

        #endregion

        protected virtual void CheckState()
        {
            switch (state)
            {
                case AntState.lookForFood:
                    if(food!=null){
                        ChangeState(AntState.goToFood);
                    }
                    break;
                case AntState.goToFood:
                    if (food.Amount == 0 && Vector2.Distance(foodPosition, Position) < TOLERANCE_VISION )
                    {
                        food = null;
                        ChangeState(AntState.lookForFood);
                        break;
                    }
                    if (Vector2.Distance(foodPosition, Position) < TOLERANCE_EAT)
                    {
                        foodCarried = CARRY_MAX;
                        food.Amount -= CARRY_MAX;
                        ChangeState(AntState.bringBackFood);
                    }
                    break;
                case AntState.bringBackFood:
                    if (Vector2.Distance(AntHill.Position, Position) < TOLERANCE_EAT)
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
                    randomLover = 100f;
                    warLover = 0.1f;
                    goalLover = -2f;
                    pheromoneLover = 2f;
                    goal = AntHill.Position;
                    break;
                case AntState.bringBackFood:
                    randomLover = 100f;
                    warLover = 0.1f;
                    goalLover = 50f;
                    pheromoneLover = 1f;
                    Goal = AntHill.Position;
                    break;
                case AntState.goToFood:
                    randomLover = 100f;
                    warLover = 0.1f;
                    goalLover = 50f;
                    pheromoneLover = 1f;
                    Goal = foodPosition;
                    break;
            }
        }

        protected void dropPheromone(TypePheromone type)
        {
            if (foodCarried > 0)
            {
                game.Reservoir.pickPheromone(type, position, 100);
            }
            else
            {
                game.Reservoir.pickPheromone(type, position, 50);
            }
        }
    }
}
