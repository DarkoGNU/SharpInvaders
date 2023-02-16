// Based on https://mygameprogrammingadventures.blogspot.com/p/monogame-tutorials.html

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;

namespace SharpInvaders
{
    public class SharpInvaders : Game
    {
        private enum GameState
        {
            Ready,
            Level1,
            Level2,
            Level3,
            GameOver,
        }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static readonly Random Random = new Random();
        private readonly List<Enemy> _enemies = new List<Enemy>();
        private  Texture2D _enemyShipTexture;
        private Player _player;
        private Texture2D _bulletTexture;

        private int _originalEnemyCount;

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

            float spacingWidth = (float)(_enemyShipTexture.Width * Settings.EnemyScale * 0.5f);
            float spacingHeight = (float)(_enemyShipTexture.Height * Settings.EnemyScale * 0.5f);

            int maxHorizontal = (int)((Settings.Width - 4 * _enemyShipTexture.Width * Settings.EnemyScale) / (_enemyShipTexture.Width * Settings.EnemyScale + spacingWidth)); // (x + 4) * _enemyShipTextures[0].Width * Settings.EnemyScale + x * spacingWidth < Settings.Width
            int maxVertical = (int)(Settings.Height / (2.5 * (_enemyShipTexture.Height * Settings.EnemyScale + spacingHeight))); // y * _enemyShipTextures[0].Height * Settings.EnemyScale + y * spacingHeight < Settings.Height / 2.5

            for (int x = 2; x <= maxHorizontal; x++)
            {
                for (int y = 1; y <= maxVertical; y++)
                {
                    _enemies.Add(
                                    new Enemy(
                                    _enemyShipTexture,
                                    new Vector2(x * _enemyShipTexture.Width * Settings.EnemyScale + x * spacingWidth, y * _enemyShipTexture.Height * Settings.EnemyScale + y * spacingHeight),
                                    Settings.EnemyMoveInterval,
                                    (x - 2.5) * _enemyShipTexture.Width * Settings.EnemyScale + x * spacingWidth,
                                    (maxHorizontal - x) * _enemyShipTexture.Width * Settings.EnemyScale + (maxHorizontal - x) * spacingWidth));
                }
            }

            _originalEnemyCount = _enemies.Count;

            _player.Reset(new Vector2(Settings.Width / 2 - _player.Texture.Width / 2, Settings.Height - 2 * _player.Texture.Height));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _player = new Player(Content.Load<Texture2D>("player"), Content.Load<Texture2D>("bullet"));
            _bulletTexture = Content.Load<Texture2D>("bullet");
            _enemyShipTexture = Content.Load<Texture2D>("green");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _player.Update(gameTime);

            double enemySpeedMultiplier = 1 + 9 * (_originalEnemyCount - _enemies.Count) / (double)_originalEnemyCount;
            Debug.WriteLine(enemySpeedMultiplier.ToString());

            foreach (Enemy o in _enemies)
            {
                o.SpeedMultiplier = enemySpeedMultiplier;

                if (_player.BoundingBoxCollide(o))
                {
                    ResetGame();
                    break;
                }

                o.Update(gameTime);

                if (o.GameOver())
                {
                    ResetGame();
                    break;
                }
            }

            foreach (GameObject o in _player.Children)
            {
                foreach (GameObject e in _enemies)
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

            _enemies.RemoveAll(g => (g.Enabled == false || g.Visible == false) && g.Children.Count == 0);

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

            foreach (GameObject o in _enemies)
            {
                o.Draw(_spriteBatch);
            }

            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
