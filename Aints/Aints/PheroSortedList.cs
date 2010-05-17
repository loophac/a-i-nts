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
		public void Add(Pheromone p)
		{
			lock (this)
			{
				Add(p.Position.X, p);
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
