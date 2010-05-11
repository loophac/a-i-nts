using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aints.ants
{
	public class AntWarrior : Ant
	{
		public AntWarrior (Main game, bool active)
			: base(game, active)
		{
			this.scale = 1.2f;
		}


        public override void Kill()
        {
            base.Kill();
            game.Reservoir.putBack(this);
        }
	}
}
