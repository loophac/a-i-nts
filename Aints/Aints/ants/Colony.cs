using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aints.ants;

namespace Aints
{
	class Colony : GameObject
	{
		#region props
		public readonly int team; 
		#endregion

		#region ctor
		public Colony(Main game, int team)
			:base(game, true)
		{
			this.team = team;
		} 
		#endregion

		private void antSpawn(TypeAnt type)
		{
			Ant a;
			//switch (type)
			//{
			//    case TypeAnt.war: a = new AntWarrior(game)
			//}
		}
	}
}
