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
        // Children (e.g. bullets)
        public readonly List<GameObject> Children = new List<GameObject>();

        public Vector2 Position; // position of the object
        public Vector2 Direction; // direction of the object's movement
        public float Scale = 1; // scale - used to scale the texture, for drawing and game logic
        public float Rotation = 0; // not implemented - should the object (texture) be rotated - not implemented
        protected readonly Texture2D _texture2D; // texture used by object
        protected Color[] ColorData; // unused - color data (for pixel perfect collision detection) - unused
        public GameObject(Texture2D texture) // constructor
        {
            _texture2D = texture;
            ColorData = new Color[_texture2D.Width * _texture2D.Height];
            _texture2D.GetData(ColorData);
        }
        public virtual Rectangle Source => new Rectangle(0, 0, Texture.Width, Texture.Height); // unused - leftover from original code
        public abstract Rectangle Bounds { get; } // used for checking collisions
        public abstract Vector2 Origin { get; } // unused - leftover from original code
        public abstract Texture2D Texture { get; } // texture of this object
        public bool Enabled { get; set; } // is the object enabled
        public bool Visible { get; set; } // is the object visible
        public float Speed { get; set; } // speed of the object
        public abstract void Update(GameTime gameTime); // updates the object
        public abstract void Draw(SpriteBatch batch); // draws the object
        public Matrix Transformation => Matrix.CreateScale(Scale) *
        Matrix.CreateRotationZ(Rotation); // unused - useful for pixel perfect collision detection (unimplemented) - unused
        public bool BoundingBoxCollide(GameObject sprite)
        {
            // Check collisions - use Xna's Rectangle to do it
            return Bounds.Intersects(sprite.Bounds);
        }
    }
}
