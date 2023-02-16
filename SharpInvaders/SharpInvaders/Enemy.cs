// Based on https://mygameprogrammingadventures.blogspot.com/p/monogame-tutorials.html

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace SharpInvaders
{
    // Represents an enemy
    public class Enemy : GameObject
    {
        private static Random random = new Random(); // random number generator
        private double _shootChance; // chance of shooting each update
        private Texture2D _bulletTexture; // texture used for bullet

        public double SpeedMultiplier { get; set; } = 1; // used to make the enemy faster

        private double _moveInterval; // used to calculate enemy speed
        private double _sinceLastMove = 0; // used to determine when to move
        private double _offsetLeft; // used to determine when to change direction
        private double _offsetRight; // used to determine when to change direction

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
            // Draw the enemy if it's visible
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

            // Draw its children (bullets)
            foreach (GameObject o in Children)
            {
                o.Draw(batch);
            }
        }
        public override void Update(GameTime gameTime)
        {
            // Update the enemy if it's enabled
            if (Enabled)
            {
                // Increment time since last move
                _sinceLastMove += gameTime.ElapsedGameTime.TotalSeconds;
                // And check if we should move
                if (_sinceLastMove > _moveInterval / SpeedMultiplier)
                {
                    // Increment the position
                    Position += Direction * _texture2D.Width / 3;

                    // Check if direction should be changed
                    if (Position.X - _offsetLeft < 0 || Position.X + _texture2D.Width * Scale > Settings.Width - _offsetRight)
                    {
                        // Go down and change direction
                        Position.Y += _texture2D.Height * Scale;
                        Direction.X *= -1;

                        // Go a tiny bit backwards (to avoid triggering this "if" statement on next iteration)
                        Position += Direction * _texture2D.Width / 3;
                        Position.X += Direction.X * 10;
                    }

                    // Enemy reached the bottom
                    if (Position.Y > Settings.Height)
                    {
                        Enabled = false;
                        Visible = false;
                    }

                    // Reset time since last move
                    _sinceLastMove = 0;
                }

                // Try to shoot
                Shoot();
            }

            // Update the children (bullets)
            foreach (GameObject o in Children)
            {
                o.Update(gameTime);
            }

            // Remove unneeded bullets
            Children.RemoveAll(o => o.Enabled == false || o.Visible == false);
        }

        public bool GameOver(GameObject o)
        {
            // Iterate over bullets
            foreach (GameObject b in Children)
            {
                // If argument collides with the bullet, it's game over
                if (b.BoundingBoxCollide(o))
                {
                    return true;
                }
            }

            // Check if the enemy went off screen
            return Position.Y + Texture.Height * Scale > Settings.Height
                || o.BoundingBoxCollide(this);
        }

        private void Shoot()
        {
            // Calculate chance of shooting
            if (random.NextDouble() > _shootChance || !Enabled)
            {
                // Return if we don't shoot
                return;
            }

            // Create a new bullet
            Bullet b = new Bullet(_bulletTexture, new Vector2(0, 1), Settings.EnemyBulletSpeed)
            {
                Position = new Vector2(
                Position.X + Texture.Width * Scale / 2 - _bulletTexture.Width / 2,
                Position.Y + (Texture.Height * Scale -
                _bulletTexture.Height) / 2)
            };

            // Add to children
            Children.Add(b);
        }
    }
}
