using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aints.ants
{
	public class AntWorker : Ant
	{
		public AntWorker(Main game, bool active)
			: base(game, active)
		{

		}

        public override void Kill()
        {
            base.Kill();
            game.Reservoir.putBack(this);
        }
	}
}
