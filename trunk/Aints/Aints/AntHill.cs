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
    public class AntHill:GameObject
    {
        protected int larva;
        protected float lavraPerSec;
        protected float food;
        public float Food
        {
            get { return this.food; }
            set { this.food = value; }
        }
        public AntHill(Main game):base(game,true)
        {
            food = 2000;
            this.game = game;
            Position = new Vector2(50, 50);
        }
        protected override void LoadContent()
        {
            base.LoadContent();
            Sprite = game.Content.Load<Texture2D>("antHill");
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

    }
}
