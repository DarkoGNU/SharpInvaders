﻿// Based on https://drive.google.com/file/d/1cDzo-h-Ww48wxCmX1YpeM6idUpVyxN7U/view

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
        public override Rectangle Bounds => new
        Rectangle(
        (int)Position.X,
        (int)Position.Y,
        (int)(_texture2D.Width * Scale),
        (int)(_texture2D.Height * Scale));
        public override Vector2 Origin => new Vector2(_texture2D.Width / 2, _texture2D.Height /
        2);
        public override Texture2D Texture => _texture2D;
        public Player(Texture2D texture)
        : base(texture)
        {
            Visible = true;
            Enabled = true;
            Direction = Vector2.Zero;
            Scale = Settings.PlayerScale;
            Speed = 480f;
            Position.X = 0;
            Position.Y = (1080 - _texture2D.Height) / 2;
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
        }
        public override void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                Direction = Vector2.Zero;
                KeyboardState state = Keyboard.GetState();
                if (state.IsKeyDown(Keys.Up))
                {
                    Direction.Y = -1;
                }
                else if (state.IsKeyDown(Keys.Down))
                {
                    Direction.Y = 1;
                }
                if (state.IsKeyDown(Keys.Left))
                {
                    Direction.X = -1;
                }
                else if (state.IsKeyDown(Keys.Right))
                {
                    Direction.X = 1;
                }
                if (Direction != Vector2.Zero)
                {
                    Direction.Normalize();
                }
                Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (Position.X < 0)
                {
                    Position.X = 0;
                }
                if (Position.Y < 0)
                {
                    Position.Y = 0;
                }
                if (Position.X + _texture2D.Width * Scale > Settings.Width)

                {
                    Position.X = Settings.Width - _texture2D.Width * Scale;
                }
                if (Position.Y + _texture2D.Height * Scale > Settings.Height)
                {
                    Position.Y = Settings.Height - _texture2D.Height * Scale;
                }
            }
        }
    }
}
