using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Aints
{
	public class Cemetery : List<Ant>
	{
		public readonly Vector2 Position;

		public Cemetery(Ant a)
			: base()
		{
			this.Add(a);
			this.Position = a.Position;
		}
	}
}
