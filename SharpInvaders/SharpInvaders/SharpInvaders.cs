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
            Ready = 0,
            Level1 = 1,
            Level2 = 2,
            Level3 = 3,
            Win = 4,
            GameOver = 5,
        };

        private GameState _gameState = GameState.Ready;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static readonly Random Random = new Random();
        private readonly List<Enemy> _enemies = new List<Enemy>();
        private Texture2D _enemyShipTexture;
        private Player _player;
        private Texture2D _enemyBulletTexture;
        private SpriteFont _font;

        private int _originalEnemyCount;
        private int _lastRoundScore = 0;
        private int _score = 0;

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
                                    (maxHorizontal - x) * _enemyShipTexture.Width * Settings.EnemyScale + (maxHorizontal - x) * spacingWidth,
                                    _enemyBulletTexture,
                                    Settings.EnemyShootChance * ((int)_gameState + 1)));
                }
            }

            _originalEnemyCount = _enemies.Count;
            _lastRoundScore = _score;
            _score = 0;

            _player.Reset(new Vector2(Settings.Width / 2 - _player.Texture.Width / 2, Settings.Height - 2 * _player.Texture.Height));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _player = new Player(Content.Load<Texture2D>("player"), Content.Load<Texture2D>("bullet"));
            _enemyBulletTexture = Content.Load<Texture2D>("bullet2");
            _enemyShipTexture = Content.Load<Texture2D>("green");
            _font = Content.Load<SpriteFont>("Font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (_gameState == GameState.Ready || _gameState == GameState.GameOver || _gameState == GameState.Win)
            {
                if (_gameState == GameState.Ready && (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.Right)))
                {
                    _gameState = GameState.Level1;
                }

                return;
            }

            _score = _lastRoundScore + (_originalEnemyCount - _enemies.Count) * 100;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _player.Update(gameTime);

            double enemySpeedMultiplier = 1 + 5 * (_originalEnemyCount - _enemies.Count) / (double)_originalEnemyCount;
            // Debug.WriteLine(enemySpeedMultiplier.ToString());

            foreach (Enemy o in _enemies)
            {
                o.SpeedMultiplier = enemySpeedMultiplier;

                o.Update(gameTime);

                if (o.GameOver(_player))
                {
                    GameOver();
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

            if (_enemies.Count == 0)
            {
                _gameState += 1;


                if (_gameState != GameState.Win)
                {
                    ResetGame();
                }
            }

            base.Update(gameTime);
        }

        private void GameOver()
        {
            _gameState = GameState.GameOver;
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

            _spriteBatch.DrawString(_font, "Score: " + _score, new Vector2(20, 20), Color.White);

            if (_gameState != GameState.Ready && _gameState != GameState.GameOver && _gameState != GameState.Win)
            {
                _spriteBatch.DrawString(_font, "Level: " + (int)_gameState, new Vector2(20, 40), Color.White);
            }
            else if (_gameState == GameState.GameOver)
            {
                _spriteBatch.DrawString(_font, "Game Over!", new Vector2(20, 40), Color.White);
            }
            else if (_gameState == GameState.Win)
            {
                _spriteBatch.DrawString(_font, "You win!", new Vector2(20, 40), Color.White);
            }
            else
            {
                _spriteBatch.DrawString(_font, "Press an arrow key to begin!", new Vector2(20, 40), Color.White);
            }

            if (_gameState != GameState.Ready)
            {
                foreach (GameObject o in _enemies)
                {
                    o.Draw(_spriteBatch);
                }
            }

            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
