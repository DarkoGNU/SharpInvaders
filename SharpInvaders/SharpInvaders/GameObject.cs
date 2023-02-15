// Based on https://mygameprogrammingadventures.blogspot.com/p/monogame-tutorials.html

using System;
using System.Collections.Generic;

using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace SharpInvaders
{
    public abstract class GameObject
    {
        public readonly List<GameObject> Children = new List<GameObject>();

        public Vector2 Position;
        public Vector2 Direction;
        public float Scale = 1;
        public float Rotation = 0;
        protected readonly Texture2D _texture2D;
        protected Color[] ColorData;
        public GameObject(Texture2D texture)
        {
            _texture2D = texture;
            ColorData = new Color[_texture2D.Width * _texture2D.Height];
            _texture2D.GetData(ColorData);
        }
        public virtual Rectangle Source => new Rectangle(0, 0, Texture.Width, Texture.Height);
        public abstract Rectangle Bounds { get; }
        public abstract Vector2 Origin { get; }
        public abstract Texture2D Texture { get; }
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
        public float Speed { get; set; }
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch batch);
        public Matrix Transformation => Matrix.CreateScale(Scale) *
        Matrix.CreateRotationZ(Rotation);
        public bool BoundingBoxCollide(GameObject sprite)
        {
            return Bounds.Intersects(sprite.Bounds);
        }
    }
}
