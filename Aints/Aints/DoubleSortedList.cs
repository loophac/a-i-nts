using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Aints
{
	public class DoubleSortedList
	{
		private SortedList<float, Pheromone> byY;
		public SortedList<float, Pheromone> ByY { get { return byY; } }
		private SortedList<float, Pheromone> byX;
		public SortedList<float, Pheromone> ByX { get { return byX; } }

		public IList<Pheromone> list { get { return this.byX.Values; } }

		public DoubleSortedList()
		{
			byY = new SortedList<float, Pheromone>();
			byX = new SortedList<float, Pheromone>();
		}

		public DoubleSortedList(int size)
		{
			byY = new SortedList<float, Pheromone>(size);
			byX = new SortedList<float, Pheromone>(size);
		}

		public void Add(Pheromone p)
		{
            //TODO make something coherant here 
            try
            {
                byX.Add(p.Position.X, p);
                
                    byY.Add(p.Position.Y, p);
                }
                catch(ArgumentException) { }
		}

		public bool Remove(Pheromone p)
		{
			//FIXME : this won't work because we take the barycenter, so X may change
			return byX.Remove(p.Position.X) && byY.Remove(p.Position.X);
		}

		//public void split(out DoubleSortedList a, out DoubleSortedList b)
		//{
		//    int size = byX.Count / 2 + byX.Count % 2;
		//    a = new DoubleSortedList(size);
		//    b = new DoubleSortedList(size);

		//    //addrange ?

		//}
	}
}
