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
    class AntBehaviourInertia : Behaviour
    {
        public AntBehaviourInertia(Main game, GameObject owner)
        {
            this.game = game;
            this.owner = owner;
        }


        override public Vector2 RunBehaviour()
        {
            Vector2 ret = ((Ant)owner).InertiaLover * owner.Velocity;
			ret.Normalize();
			return ret;
        }
    }
}
