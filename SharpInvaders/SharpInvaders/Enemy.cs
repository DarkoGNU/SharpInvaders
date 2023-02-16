// Based on https://mygameprogrammingadventures.blogspot.com/p/monogame-tutorials.html

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SharpInvaders
{
    public class Enemy : GameObject
    {
        private static Random random = new Random();
        private double _shootChance;
        private Texture2D _bulletTexture;

        public double SpeedMultiplier { get; set; } = 1;

        private double _moveInterval;
        private double _sinceLastMove = 0;
        private double _offsetLeft;
        private double _offsetRight;

        public override Rectangle Bounds => new
        Rectangle(
        (int)Position.X,
        (int)Position.Y,
        (int)(_texture2D.Width * Scale),
        (int)(_texture2D.Height * Scale));
        public override Vector2 Origin => new Vector2(_texture2D.Width / 2, _texture2D.Height /
        2);
        public override Texture2D Texture => _texture2D;
        public Enemy(Texture2D texture, Vector2 position, double moveInterval, double offsetLeft, double offsetRight, Texture2D bulletTexture, double shootChance)
        : base(texture)
        {
            Direction.X = -1;
            Position = position;
            Enabled = true;
            Visible = true;
            Scale = Settings.EnemyScale;
            _moveInterval = moveInterval;
            _offsetLeft = offsetLeft;
            _offsetRight = offsetRight;
            _bulletTexture = bulletTexture;
            _shootChance = shootChance;
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

            foreach (GameObject o in Children)
            {
                o.Draw(batch);
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                _sinceLastMove += gameTime.ElapsedGameTime.TotalSeconds;
                if (_sinceLastMove > _moveInterval / SpeedMultiplier)
                {
                    Position += Direction * _texture2D.Width / 3;

                    if (Position.X - _offsetLeft < 0 || Position.X + _texture2D.Width * Scale > Settings.Width - _offsetRight)
                    {
                        Position.Y += _texture2D.Height * Scale;
                        Direction.X *= -1;

                        Position += Direction * _texture2D.Width / 3;
                        Position.X += Direction.X * 10;
                    }

                    if (Position.Y > Settings.Height)
                    {
                        Enabled = false;
                        Visible = false;
                    }

                    _sinceLastMove = 0;
                }

                Shoot();
            }

            foreach (GameObject o in Children)
            {
                o.Update(gameTime);
            }

            Children.RemoveAll(o => o.Enabled == false || o.Visible == false);
        }

        public bool GameOver()
        {
            return Position.Y + Texture.Height * Scale > Settings.Height;
        }

        private void Shoot()
        {
            if (random.NextDouble() > _shootChance || !Enabled)
            {
                return;
            }

            Bullet b = new Bullet(_bulletTexture, new Vector2(0, 1))
            {
                Position = new Vector2(
                Position.X + Texture.Width * Scale / 2 - _bulletTexture.Width / 2,
                Position.Y + (Texture.Height * Scale -
                _bulletTexture.Height) / 2)
            };

            Children.Add(b);
        }
    }
}
