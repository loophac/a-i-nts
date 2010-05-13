using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Aints
{
	public class Pools
	{
		private const int POOL_SIZE_ANTS = 100;
		private const int POOL_SIZE_PHEROMONES = 10000;

		private List<Ant> PoolAnts;
		private List<Pheromone> PoolPheromones;
        private Main game;

		public Pools(Main game)
		{
            this.game = game;
			this.PoolPheromones = new List<Pheromone>(POOL_SIZE_PHEROMONES);
			this.PoolAnts = new List<Ant>(POOL_SIZE_ANTS);

			for (int i = 0; i < POOL_SIZE_PHEROMONES; i++)
			{
				PoolPheromones.Add(new Pheromone(game, false));
			}
			for (int i = 0; i < POOL_SIZE_ANTS; i++)
			{
				PoolAnts.Add(new Ant(game, false));
			}
		}

		public Pheromone pickPheromone(TypePheromone type, Vector2 position, float smell)
		{
			if (this.PoolPheromones.Count == 0)
			{
				//TODO : remover quand on aura une idée de bonne taille, et extendre automatiquement
				throw new Exception("Pheromone pool is empty");
			}
			else
			{
				Pheromone p;
				lock (PoolPheromones)
				{
					p = PoolPheromones[PoolPheromones.Count - 1];
					PoolPheromones.RemoveAt(PoolPheromones.Count - 1);
				}
				p.Type = type;
                p.Smell = smell;
				p.Position = position;
				p.Enabled = true;
				p.Visible = true;
				game.Components.Add(p);
				if(type == TypePheromone.food)
					game.PheromonesFood.Add(p);
				else
					game.PheromonesWar.Add(p);
				return p;
			}
		}

		public Ant pickAnt(Vector2 position, float rotation, int team) //type of ant
		{
			if (this.PoolAnts.Count == 0)
			{
				//TODO : remover quand on aura une idée de bonne taille, et extendre automatiquement
				throw new Exception("Ants pool is empty");
			}
			else
			{
				Ant a;
				lock (PoolAnts)
				{ 
					a = PoolAnts[PoolAnts.Count - 1];
					PoolAnts.RemoveAt(PoolAnts.Count - 1);					
				}
                game.Components.Add(a);
				a.Position = position;
				a.Rotation = rotation;
				a.Team = team;
				a.Enabled = true;
				a.Visible = true;
				return a;
			}
		}

		public void putBack(Pheromone p)
		{
			p.Enabled = false;
			p.Visible = false;
			PoolPheromones.Add(p);
		}
		public void putBack(Ant p)
		{
			p.Enabled = false;
			p.Visible = false;
			PoolAnts.Add(p);
		}
	}
}
