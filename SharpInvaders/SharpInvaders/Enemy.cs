// Based on https://mygameprogrammingadventures.blogspot.com/p/monogame-tutorials.html

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SharpInvaders
{
    public class Enemy : GameObject
    {
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
        public Enemy(Texture2D texture, Vector2 position, double moveInterval, double offsetLeft, double offsetRight)
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
                _sinceLastMove += gameTime.ElapsedGameTime.TotalSeconds;
                if (_sinceLastMove > _moveInterval / SpeedMultiplier)
                {
                    Position += Direction * _texture2D.Width / 3;

                    if (Position.X - _offsetLeft < 0 || Position.X + _texture2D.Width * Scale > Settings.Width - _offsetRight)
                    {
                        Position.Y += _texture2D.Height * Scale / 2;
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
            }
        }
    }
}
