using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;    
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Aints.Behaviours
{
    class AntHungerBehaviour : Behaviour
    {
        private const float HUNGER_ADDITION = 0.005f;
        public AntHungerBehaviour(Main game, GameObject owner)
        {
            this.game = game;
            this.owner = owner;
        }

        override public Vector2 RunBehaviour()
        {
            ((Ant)owner).Hungry += HUNGER_ADDITION; 
            Vector2 hungerAttraction = ((Ant)owner).AntHill.Position - ((Ant)owner).Position;
            if (hungerAttraction != Vector2.Zero)
            {
                hungerAttraction.Normalize();
            }
            hungerAttraction *= (((Ant)owner).Hungry)/*/ConstantsHolder.Singleton.Starvation*/;
            return hungerAttraction;
        }
    }
}
