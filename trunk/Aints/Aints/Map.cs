using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aints
{
    class Map:GameObject
    {
        public const float TOLERANCE_PHERO = 5;
        public Map(Main main)
            : base(main, true)
        {
            DrawOrder = 0;
            UpdateOrder=0;
        }
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected void processPheromones(){
            foreach( Pheromone pheromones in game.PheromonesFood){
                
            }
        }

    }
}
