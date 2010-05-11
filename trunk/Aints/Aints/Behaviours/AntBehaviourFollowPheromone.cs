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
        public AntBehaviourFollowPheromone(Main game, GameObject owner)
        {
            this.game = game;
            this.owner = owner;
        }

        public Vector2 pheromonesAttraction(Vector2 position, TypePheromone type)
        {
            Vector2 attraction = Vector2.Zero;
            Vector2 attractionPhero;
            List<Pheromone> scanPheromones;
            if (type == TypePheromone.food)
            {
                scanPheromones = game.PheromonesFood;
            }
            else if (type == TypePheromone.war)
            {
                scanPheromones = game.PheromonesWar;
            }
            else
            {
                return attraction;
            }
            foreach (Pheromone pheromone in scanPheromones)
            {
                attractionPhero = (position - pheromone.Position);
                attractionPhero.Normalize();
                attractionPhero *= (float)(Main.G_PHEROMONES * pheromone.Smell / Math.Pow(Vector2.Distance(pheromone.Position, position), 3));
                attraction += attractionPhero;
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
