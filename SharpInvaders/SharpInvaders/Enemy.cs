// Based on https://mygameprogrammingadventures.blogspot.com/p/monogame-tutorials.html

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpInvaders;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Formats.Asn1.AsnWriter;

namespace SharpInvaders
{
    public class Enemy : GameObject
    {

        private readonly double _spawnTime;
        public double SpawnTime => _spawnTime;
        public override Rectangle Bounds => new
        Rectangle(
        (int)Position.X,
        (int)Position.Y,
        (int)(_texture2D.Width * Scale),
        (int)(_texture2D.Height * Scale));
        public override Vector2 Origin => new Vector2(_texture2D.Width / 2, _texture2D.Height /
        2);
        public override Texture2D Texture => _texture2D;
        public Enemy(Texture2D texture, double spawnTime, Vector2 position, float speed)
        : base(texture)
        {
            Direction.Y = 1;
            Speed = speed;
            _spawnTime = spawnTime;
            Position = position;
            Enabled = true;
            Visible = true;
            Scale = Settings.EnemyScale;
        }
        public override void Draw(SpriteBatch batch)
        {
            if (Visible)
            {
                batch.Draw(
                _texture2D,
                Position,
                Source,
                Color.White,
                Rotation,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0.75f);
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (Position.X + _texture2D.Width * Scale < 0)
                {
                    Enabled = false;
                    Visible = false;
                }
            }
        }
    }
}
