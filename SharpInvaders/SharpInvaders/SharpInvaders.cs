// Based on https://mygameprogrammingadventures.blogspot.com/p/monogame-tutorials.html

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace SharpInvaders
{
    public class SharpInvaders : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static readonly Random Random = new Random();
        private readonly List<GameObject> _gameObjects = new List<GameObject>();
        private readonly List<GameObject> _playerBullets = new List<GameObject>();
        private readonly List<Texture2D> _enemyShipTextures = new List<Texture2D>();
        private readonly Queue<Enemy> _enemies = new Queue<Enemy>();
        private Player _player;
        private KeyboardState _keyboardState;
        private KeyboardState _lastKeyboardState;
        private Texture2D _bulletTexture;
        private double _timer;

        public SharpInvaders()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _graphics.PreferredBackBufferWidth = Settings.Width;
            _graphics.PreferredBackBufferHeight = Settings.Height;
            _graphics.ApplyChanges();

            LoadContent();
            ResetGame();
        }
        private void ResetGame()
        {

            _enemies.Clear();
            _gameObjects.Clear();
            _timer = 0;
            for (int i = 0; i < 100; i++)
            {
                _enemies.Enqueue(
                new Enemy(
                _enemyShipTextures[0],
                2 + i,
                new Vector2(Random.Next(0, Settings.Width), -(_enemyShipTextures[0].Height * Settings.EnemyScale)),
                Settings.EnemySpeed));
            }

            _player.Position.X = Settings.Width / 2 - _player.Texture.Width / 2;
            _player.Position.Y = Settings.Height - 2 * _player.Texture.Height;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _player = new Player(Content.Load<Texture2D>("player"));
            _bulletTexture = Content.Load<Texture2D>("bullet");
            _enemyShipTextures.Add(Content.Load<Texture2D>("green"));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            _lastKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();
            _player.Update(gameTime);
            _timer += gameTime.ElapsedGameTime.TotalSeconds;
            if (_enemies.Count > 0 && _enemies.Peek().SpawnTime <= _timer)
            {
                _gameObjects.Add(_enemies.Dequeue());
            }
            if (_keyboardState.IsKeyDown(Keys.Space) && _lastKeyboardState.IsKeyUp(Keys.Space))
            {
                Bullet b = new Bullet(_bulletTexture)
                {
                    Position = new Vector2(
                _player.Position.X + _player.Texture.Width * _player.Scale / 2 - _bulletTexture.Width / 2,
                _player.Position.Y + (_player.Texture.Height * _player.Scale -
                _bulletTexture.Height) / 2)
                };
                _playerBullets.Add(b);
            }
            foreach (GameObject o in _gameObjects)

            {
                if (_player.BoundingBoxCollide(o))
                {
                    ResetGame();
                    break;
                }
                o.Update(gameTime);
            }
            foreach (GameObject o in _playerBullets)
            {
                foreach (GameObject e in _gameObjects)
                {
                    if (e is Enemy)
                    {
                        if (o.BoundingBoxCollide(e))
                        {
                            e.Enabled = false;
                            e.Visible = false;
                            o.Enabled = false;
                            o.Visible = false;
                        }
                    }
                }
                o.Update(gameTime);
            }
            _playerBullets.RemoveAll(b => b.Enabled == false || b.Visible == false);
            _gameObjects.RemoveAll(g => g.Enabled == false || g.Visible == false);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(
            SpriteSortMode.BackToFront,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            null,
            null,
            null,
            null);
            _player.Draw(_spriteBatch);
            foreach (GameObject o in _gameObjects)
            {
                o.Draw(_spriteBatch);
            }
            foreach (GameObject o in _playerBullets)
            {
                o.Draw(_spriteBatch);
            }
            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
