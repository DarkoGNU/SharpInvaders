// Based on https://drive.google.com/file/d/1cDzo-h-Ww48wxCmX1YpeM6idUpVyxN7U/view

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpInvaders;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Formats.Asn1.AsnWriter;

namespace SharpInvaders
{
    public class Player : GameObject
    {
        private double _sinceLastShot = Settings.PlayerShootInterval; // how much time passed since last shot
        private Texture2D _bulletTexture; // texture used for bullet

        public override Rectangle Bounds => new
        Rectangle(
        (int)Position.X,
        (int)Position.Y,
        (int)(_texture2D.Width * Scale),
        (int)(_texture2D.Height * Scale));
        public override Vector2 Origin => new Vector2(_texture2D.Width / 2, _texture2D.Height /
        2);
        public override Texture2D Texture => _texture2D;
        public Player(Texture2D texture, Texture2D bulletTexture)
        : base(texture)
        {
            Visible = true;
            Enabled = true;
            Direction = Vector2.Zero;
            Scale = Settings.PlayerScale;
            Speed = Settings.PlayerSpeed;
            Position.X = Settings.Width / 2 - Texture.Width / 2;
            Position.Y = Settings.Height - 2 * Texture.Height;
            _bulletTexture = bulletTexture;
        }

        public override void Draw(SpriteBatch batch)
        {
            // Draw the player if it's visible
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
                0.5f);
            }

            // Draw the player's bullets
            foreach (GameObject o in Children)
            {
                o.Draw(batch);
            }
        }
        public override void Update(GameTime gameTime)
        {
            // Update the player if it's enabled
            if (Enabled)
            {
                Direction = Vector2.Zero; // Direction is zero at first
                KeyboardState state = Keyboard.GetState();

                // Determine direction based on key pressed
                if (state.IsKeyDown(Keys.Left))
                {
                    Direction.X = -1;
                }
                else if (state.IsKeyDown(Keys.Right))
                {
                    Direction.X = 1;
                }

                // Update position based on direction
                Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Make player unable to go outside screen
                if (Position.X < Settings.PlayerMargin)
                {
                    Position.X = Settings.PlayerMargin;
                }
                else if (Position.X > Settings.Width - _texture2D.Width * Scale - Settings.PlayerMargin)
                {
                    Position.X = Settings.Width - _texture2D.Width * Scale - Settings.PlayerMargin;
                }

                // Try to shoot
                _sinceLastShot += gameTime.ElapsedGameTime.TotalSeconds;
                Shoot();
            }

            // Update player's bullets
            foreach (GameObject o in Children)
            {
                o.Update(gameTime);
            }

            // Remove unneeded bullets
            Children.RemoveAll(o => o.Enabled == false || o.Visible == false);
        }

        public void Reset(Vector2 position)
        {
            // Reset the player
            _sinceLastShot = Settings.PlayerShootInterval;
            Position = position;
        }

        private void Shoot()
        {
            // Shoot if enough time passed
            if (_sinceLastShot < Settings.PlayerShootInterval || !Enabled)
            {
                return;
            }

            // Reset the timer
            _sinceLastShot = 0;

            // Create a new bullet
            Bullet b = new Bullet(_bulletTexture, new Vector2(0, -1), Settings.BulletSpeed)
            {
                Position = new Vector2(
                Position.X + Texture.Width * Scale / 2 - _bulletTexture.Width / 2,
                Position.Y + (Texture.Height * Scale -
                _bulletTexture.Height) / 2)
            };

            // Add the new bullet to children
            Children.Add(b);
        }
    }
}
