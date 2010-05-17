using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;

namespace Aints
{
	public class PheroSortedList : SortedList<float, Pheromone>
	{
		private const float smallValue = 0.001f;

		public void Add(Pheromone p)
		{
			lock (this)
			{
				bool added = false;
				while (!added)
				{
					try
					{
						Add(p.Position.X, p);
						added = true;
					}
					catch (ArgumentException)
					{
						p.Position = new Vector2(p.Position.X + smallValue, p.Position.Y);
					}
				}

				int index = IndexOfKey(p.Position.X);
				int i = index - 1;
				while (i >= 0 && Values[i].Position.X > p.Position.X - ConstantsHolder.Singleton.PheroFusionRadius)
				{
					if (Vector2.Distance(p.Position, Values[i].Position) < ConstantsHolder.Singleton.PheroFusionRadius)
					{
						Values[i].Smell += p.Smell;
						p.Kill();
						return;
					}
					i--;
				}
				i = index + 1;
				while (i < Count && Values[i].Position.X < p.Position.X + ConstantsHolder.Singleton.PheroFusionRadius)
				{
					if (Vector2.Distance(p.Position, Values[i].Position) < ConstantsHolder.Singleton.PheroFusionRadius)
					{
						Values[i].Smell += p.Smell;
						p.Kill();
						return;
					}
					i++;
				}
			}
		}

		public bool Remove(Pheromone p)
		{
			return this.Remove(p.Position.X);
		}
	}
}
