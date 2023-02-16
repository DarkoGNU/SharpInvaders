// Based on https://mygameprogrammingadventures.blogspot.com/p/monogame-tutorials.html

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SharpInvaders
{
    public class Bullet : GameObject
    {
        public override Rectangle Bounds => new
        Rectangle(
        (int)Position.X,
        (int)Position.Y,
        (int)(_texture2D.Width * Scale),
        (int)(_texture2D.Height * Scale));
        public override Vector2 Origin => new Vector2(_texture2D.Width / 2, _texture2D.Height /
        2);
        public override Texture2D Texture => _texture2D;
        public Bullet(Texture2D texture, Vector2 direction, float speed)
        : base(texture)
        {
            Speed = speed;
            Direction = direction;
            Enabled = true;
            Visible = true;
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
                if (Position.Y > Settings.Height || Position.Y < 0 - _texture2D.Height * Scale)
                {
                    Enabled = false;
                    Visible = false;
                }
            }
        }
    }
}
