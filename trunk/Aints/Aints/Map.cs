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

namespace Aints
{   
    public class Map : GameObject
    {
        public Map(Main main)
            : base(main,true)
        {
            DrawOrder = 0;
        }

        #region override
        protected override void LoadContent()
        {
            Sprite = game.Content.Load<Texture2D>("map2-2");
        }
        #endregion
    }
}
