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
    class AntBehaviourFollowGoal : Behaviour
    {
        public AntBehaviourFollowGoal(Main game, GameObject owner)
        {
            this.game = game;
            this.owner = owner;
        }


        override public Vector2 RunBehaviour()
        {
            if (((Ant)owner).Goal != null)
            {
                
                    Vector2 follow = (((Ant)owner).Goal - owner.Position);
                    if (follow != Vector2.Zero)
                    {
                        follow.Normalize();
                    }
                if (!((Ant)owner).GoAround)
                {
                    return ((Ant)owner).GoalLover * follow;
                }
                else
                {
                    return ((Ant)owner).GoalLover * 0.5f* follow+owner.Velocity;
                }
            }
            else
            {
                return Vector2.Zero;
            }
        }
    }
}
