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
        private float _lastShot = 0;
        private Texture2D _bulletTexture;

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

            foreach (GameObject o in Children)
            {
                o.Draw(batch); 
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                Direction = Vector2.Zero;
                KeyboardState state = Keyboard.GetState();

                if (state.IsKeyDown(Keys.Left))
                {
                    Direction.X = -1;
                }
                else if (state.IsKeyDown(Keys.Right))
                {
                    Direction.X = 1;
                }

                Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (Position.X < Settings.PlayerMargin)
                {
                    Position.X = Settings.PlayerMargin;
                }
                else if (Position.X > Settings.Width - _texture2D.Width * Scale - Settings.PlayerMargin)
                {
                    Position.X = Settings.Width - _texture2D.Width * Scale - Settings.PlayerMargin;
                }
            }

            foreach (GameObject o in Children)
            {
                o.Update(gameTime);
            }
        }

        public void Reset(Vector2 position)
        {
            _lastShot = 0;
            Position = position;
        }

        public void Shoot(GameTime gameTime)
        {
            Bullet b = new Bullet(_bulletTexture)
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
