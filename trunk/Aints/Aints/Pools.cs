using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aints.ants;
using Microsoft.Xna.Framework;

namespace Aints
{
	public class Pools
	{
		private const int POOL_SIZE_ANTS = 100;
		private const int POOL_SIZE_PHEROMONES = 10000;

		private List<AntWarrior> PoolWarriors;
		private List<AntWorker> PoolWorkers;
		private List<Pheromone> PoolPheromones;
        private Main game;

		public Pools(Main game)
		{
            this.game = game;
			this.PoolPheromones = new List<Pheromone>(POOL_SIZE_PHEROMONES);
			this.PoolWarriors = new List<AntWarrior>(POOL_SIZE_ANTS);
			this.PoolWorkers = new List<AntWorker>(POOL_SIZE_ANTS);

			for (int i = 0; i < POOL_SIZE_PHEROMONES; i++)
			{
				PoolPheromones.Add(new Pheromone(game, false));
			}
			for (int i = 0; i < POOL_SIZE_ANTS; i++)
			{
				PoolWarriors.Add(new AntWarrior(game, false));
				PoolWorkers.Add(new AntWorker(game, false));
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
                game.Components.Add(p);
                ((Main)game).PheromonesFood.Add(p);
				p.Type = type;
                p.Smell = smell;
				p.Position = position;
				p.Enabled = true;
                p.Visible = true;
				return p;
			}
		}

		public AntWarrior pickWarrior(Vector2 position, float rotation, int team)
		{
			if (this.PoolWarriors.Count == 0)
			{
				//TODO : remover quand on aura une idée de bonne taille, et extendre automatiquement
				throw new Exception("Warriors pool is empty");
			}
			else
			{
				AntWarrior a;
				lock (PoolWarriors)
				{
					a = PoolWarriors[PoolWarriors.Count - 1];
					PoolWarriors.RemoveAt(PoolWarriors.Count - 1);
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

		public AntWorker pickWorker(Vector2 position, float rotation, int team)
		{
			if (this.PoolWarriors.Count == 0)
			{
				//TODO : remover quand on aura une idée de bonne taille, et extendre automatiquement
				throw new Exception("Warriors pool is empty");
			}
			else
			{
				AntWorker a;
				lock (PoolWorkers)
				{ 
					a = PoolWorkers[PoolWorkers.Count - 1];
					PoolWorkers.RemoveAt(PoolWorkers.Count - 1);					
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
		public void putBack(AntWarrior p)
		{
			p.Enabled = false;
			p.Visible = false;
			PoolWarriors.Add(p);
		}
		public void putBack(AntWorker p)
		{
			p.Enabled = false;
			p.Visible = false;
			PoolWorkers.Add(p);
		}
	}
}
