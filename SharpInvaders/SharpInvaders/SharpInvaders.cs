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
        // represents the current state of the game
        private enum GameState
        {
            Ready = 0,
            Level1 = 1,
            Level2 = 2,
            Level3 = 3,
            Win = 4,
            GameOver = 5,
        };

        // holds the current state of the game
        private GameState _gameState = GameState.Ready;

        // used for drawing
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // the names are self explaining - some stuff used in game logic and drawing
        public static readonly Random Random = new Random();
        private readonly List<Enemy> _enemies = new List<Enemy>();
        private Texture2D _enemyShipTexture;
        private Player _player;
        private Texture2D _enemyBulletTexture;
        private SpriteFont _font;

        // some additional variables used in game logic
        private int _originalEnemyCount;
        private int _lastRoundScore = 0;
        private int _score = 0;

        public SharpInvaders()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content"; // we have all the assets in "Content"
            IsMouseVisible = true; // mouse can be visible - we don't mind
        }

        protected override void Initialize()
        {
            base.Initialize();

            // set graphics settings
            _graphics.PreferredBackBufferWidth = Settings.Width;
            _graphics.PreferredBackBufferHeight = Settings.Height;
            _graphics.ApplyChanges();

            // initialize the game
            LoadContent();
            ResetGame();
        }
        private void ResetGame()
        {
            _enemies.Clear(); // remove all enemies

            // variables used to separate the enemies by some distance
            float spacingWidth = (float)(_enemyShipTexture.Width * Settings.EnemyScale * 0.5f);
            float spacingHeight = (float)(_enemyShipTexture.Height * Settings.EnemyScale * 0.5f);

            // maximum values of variables "x" and "y" in loop below
            int maxHorizontal = (int)((Settings.Width - 4 * _enemyShipTexture.Width * Settings.EnemyScale) / (_enemyShipTexture.Width * Settings.EnemyScale + spacingWidth)); // (x + 4) * _enemyShipTextures[0].Width * Settings.EnemyScale + x * spacingWidth < Settings.Width
            int maxVertical = (int)(Settings.Height / (2.5 * (_enemyShipTexture.Height * Settings.EnemyScale + spacingHeight))); // y * _enemyShipTextures[0].Height * Settings.EnemyScale + y * spacingHeight < Settings.Height / 2.5
            // will be used for loop counters and calculating enemy position offsets

            for (int x = 2; x <= maxHorizontal; x++) // calculate over rows
            {
                for (int y = 1; y <= maxVertical; y++) // calculate over columns
                {
                    // black magic
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

            // save some info
            _originalEnemyCount = _enemies.Count;
            _lastRoundScore = _score;
            _score = 0;

            // reset the player
            _player.Reset(new Vector2(Settings.Width / 2 - _player.Texture.Width / 2, Settings.Height - 2 * _player.Texture.Height));
        }

        protected override void LoadContent()
        {
            // new sprite batch :)
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // load textures and create player
            _player = new Player(Content.Load<Texture2D>("player"), Content.Load<Texture2D>("bullet"));
            _enemyBulletTexture = Content.Load<Texture2D>("bullet2");
            _enemyShipTexture = Content.Load<Texture2D>("green");
            _font = Content.Load<SpriteFont>("Font");
        }

        protected override void Update(GameTime gameTime)
        {
            // do some checks before updating
            if (_gameState == GameState.Ready || _gameState == GameState.GameOver || _gameState == GameState.Win)
            {
                // if we're ready, see if we should switch to levl 1
                if (_gameState == GameState.Ready && (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.Right)))
                {
                    _gameState = GameState.Level1;
                }

                // skip code below
                return;
            }

            // calculate current score
            _score = _lastRoundScore + (_originalEnemyCount - _enemies.Count) * 100;

            // see if we should exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // update the player
            _player.Update(gameTime);

            // calculate the multiplier for enemy speed
            double enemySpeedMultiplier = 1 + 5 * (_originalEnemyCount - _enemies.Count) / (double)_originalEnemyCount;
            // Debug.WriteLine(enemySpeedMultiplier.ToString());

            // for each enemy
            foreach (Enemy o in _enemies)
            {
                // set new speed multiplier
                o.SpeedMultiplier = enemySpeedMultiplier;

                // update it
                o.Update(gameTime);

                // see if it killed the player
                if (o.GameOver(_player))
                {
                    GameOver();
                    break;
                }
            }

            // for each player's bullet
            foreach (GameObject o in _player.Children)
            {
                foreach (GameObject e in _enemies)
                {
                    // check if it collides with enemies
                    if (e is Enemy)
                    {
                        // if it collides, hide bullet and enemy
                        if (o.BoundingBoxCollide(e))
                        {
                            e.Enabled = false;
                            e.Visible = false;
                            o.Enabled = false;
                            o.Visible = false;
                        }
                    }
                }

                // update player's bullets
                o.Update(gameTime);
            }

            // remove unneeded enemies
            _enemies.RemoveAll(g => (g.Enabled == false || g.Visible == false) && g.Children.Count == 0);

            // if there are no enemies left - advance to the next level
            if (_enemies.Count == 0)
            {
                _gameState += 1;

                // if the level we've advanced to is not "Win" - reset the game - to properly advance to the next level
                if (_gameState != GameState.Win)
                {
                    ResetGame();
                }
            }

            base.Update(gameTime);
        }

        private void GameOver()
        {
            // simply change the state variable
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

            _player.Draw(_spriteBatch); // draw the player


            // draw the score
            _spriteBatch.DrawString(_font, "Score: " + _score, new Vector2(20, 20), Color.White);

            // draw some level information
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

            // don't draw enemies in ready state
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
