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
        protected const float TOLERANCE=10; 
        public AntBehaviourFollowPheromone(Main game, GameObject owner)
        {
            this.game = game;
            this.owner = owner;
        }

        public Vector2 pheromonesAttraction(Vector2 position, TypePheromone type)
        {
            Vector2 attraction = Vector2.Zero;
            Vector2 attractionPhero;
			IList<Pheromone> scanPheromones;
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
            foreach (Pheromone pheromone in scanPheromones)
            {
                attractionPhero = (position - pheromone.Position);
                float distance = attractionPhero.Length();
                float scallarProduct = attractionPhero.X * owner.Velocity.X + attractionPhero.Y * owner.Velocity.Y;
                if (scallarProduct > 0 && distance >TOLERANCE)
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
