using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Aints
{
    class Map:GameComponent
    {
        public const float TOLERANCE_PHERO = 5f;

		private Main game;

        public Map(Main main)
            : base(main)
        {
			//main.Components.Add(this);
			game = main;
        }
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
			if (game.PheromonesFood.list.Count != 0)
			{
				processPheromones(0, game.PheromonesFood.list.Count, game.PheromonesFood);
			}
        }

		protected void processPheromones(int start, int stop, DoubleSortedList list)
		{
			List<Pheromone> middleBand = new List<Pheromone>();
			if (stop - start <= 6)
			{
				for (int i = start; i < stop; i++)
				{
					middleBand.Add(list.ByX.Values[i]);
				}
			}
			else
			{
				float middle = list.ByX.Values[start].Position.X,
					right = list.ByX.Values[stop - 1].Position.X,
					middle_right = middle + TOLERANCE_PHERO,
					middle_left = middle - TOLERANCE_PHERO;

				processPheromones(start, stop / 2, list);
				processPheromones(stop / 2, stop, list);

				foreach (Pheromone pheromone in list.ByY.Values)
				{
					if (middle_left < pheromone.Position.X && pheromone.Position.X < middle_right)
					{
						middleBand.Add(pheromone);
					}
				}
			}

			for (int i = 0; i < middleBand.Count; i++)
			{
				Pheromone p = middleBand[i];
				for (int j = 1; j <= 6; j++)
				{
					if (i + j >= middleBand.Count)
						break;

					Pheromone p2 = middleBand[i + j];
					if (Vector2.Distance(p.Position, p2.Position) < TOLERANCE_PHERO)
					{
						p2.Smell += p.Smell;
						p2.Position = (p2.Position + p.Position) / 2; //ponderer par le smell ?
						p.Kill();
					}
				}
			}
		}
    }
}
