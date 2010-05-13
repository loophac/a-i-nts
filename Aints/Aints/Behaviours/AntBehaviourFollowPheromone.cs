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

namespace Aints.Behaviours
{
    class AntBehaviourFollowPheromone : Behaviour
    {
        protected const float TOLERANCE=100;
        protected bool totalAngle;
        public AntBehaviourFollowPheromone(Main game, GameObject owner, bool totalAngle)
        {
            this.game = game;
            this.owner = owner;
            this.totalAngle = totalAngle;
        }

        public Vector2 pheromonesAttraction(Vector2 position, TypePheromone type)
        {
            Vector2 attraction = Vector2.Zero;
            Vector2 attractionPhero;
			IList<Pheromone> scanPheromones ;
            
            if (type == TypePheromone.food)
            {
                scanPheromones = game.PheromonesFood.list;
            }
            else if (type == TypePheromone.war)
            {
                scanPheromones = game.PheromonesWar.list;
            }
            else
            {
                return attraction;
            }
            float smellXmin = owner.Position.X - TOLERANCE;
            float smellXmax = owner.Position.X + TOLERANCE;
            float smellYmin = owner.Position.Y - TOLERANCE;
            float smellYmax = owner.Position.Y + TOLERANCE;

            IEnumerable<Pheromone> selected = scanPheromones.TakeWhile(pheromone => pheromone.Position.X > smellXmin && pheromone.Position.X <smellXmax);
            selected = selected.TakeWhile(pheromone => pheromone.Position.Y >smellYmin && pheromone.Position.Y < smellYmax);
            foreach (Pheromone pheromone in selected)
            {
                attractionPhero = (position - pheromone.Position);
                float distance = attractionPhero.Length();
                float scallarProduct = attractionPhero.X * owner.Velocity.X + attractionPhero.Y * owner.Velocity.Y;
                if ((totalAngle || scallarProduct > 0) && distance <TOLERANCE)
                {
                    attractionPhero.Normalize();
                    attractionPhero *=(Main.G_PHEROMONES * pheromone.Smell / (distance*distance*distance));                
                    attraction += attractionPhero;
                }
			}
            return attraction;
        }

        override public Vector2 RunBehaviour()
        {
            Vector2 warAttraction = pheromonesAttraction(owner.Position,TypePheromone.war);
            Vector2 foodAttraction = pheromonesAttraction(owner.Position, TypePheromone.food);
            Vector2 attraction = ((Ant)owner).WarLover*warAttraction + (1-((Ant)owner).WarLover)*foodAttraction;
			Vector2 ret = ((Ant)owner).PheromoneLover * attraction;
			//ret.Normalize();
			return ret;
        }
    }
}
