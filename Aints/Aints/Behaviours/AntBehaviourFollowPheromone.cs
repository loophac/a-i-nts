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
    public class AntBehaviourFollowPheromone : Behaviour
    {
        protected const float TOLERANCE=200;
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
			PheroSortedList scanPheromones ;
            
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
            float smellXmin = owner.Position.X - TOLERANCE;
            float smellXmax = owner.Position.X + TOLERANCE;
            float smellYmin = owner.Position.Y - TOLERANCE;
            float smellYmax = owner.Position.Y + TOLERANCE;


            Pheromone indexPhero = new Pheromone(game,false);
            indexPhero.Position = new Vector2(smellXmin,0);
            int index;
            try
            {
                scanPheromones.Add(smellXmin, indexPhero);
                index = scanPheromones.IndexOfValue(indexPhero);
            }catch(ArgumentException e){
                index = scanPheromones.IndexOfKey(smellXmin);
            }

            List<Pheromone> selected = new List<Pheromone>();

                for (int i = index; i < scanPheromones.Values.Count; i++)
                {
                    if (scanPheromones.Values[i].Position.X < smellXmax)
                    {
                        break;
                    }
                    if (scanPheromones.Values[i].Position.Y > smellYmin && scanPheromones.Values[i].Position.Y < smellYmax)
                    {
                        selected.Add(scanPheromones.Values[i]);
                    }
                }
            
            scanPheromones.Remove(indexPhero);
            foreach (Pheromone pheromone in selected)
            {
                attractionPhero = (position - pheromone.Position);
                float distance = attractionPhero.Length();
                float scallarProduct = attractionPhero.X * owner.Velocity.X + attractionPhero.Y * owner.Velocity.Y;
                if (totalAngle || scallarProduct > 0)
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
