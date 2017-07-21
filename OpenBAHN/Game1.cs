using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace OpenBAHN
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D tile;
        Texture2D tile0;
        Texture2D tile1;
        Texture2D tile2;
        Texture2D select;
        int[,][] worldTiles = new int[20, 20][];
        int selectedTileX = 0;
        int selectedTileY = 0;
        int test = 3;
        bool canMove = true;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            tile = Content.Load<Texture2D>(@"tile/id" + test);
            tile0 = Content.Load<Texture2D>(@"tile/id0");
            tile1 = Content.Load<Texture2D>(@"tile/id1");
            tile2 = Content.Load<Texture2D>(@"tile/id2");
            select = Content.Load<Texture2D>(@"tile/marker");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            // moving the cursor
            KeyboardState moveKeyState = Keyboard.GetState();
            if (canMove || moveKeyState.IsKeyDown(Keys.LeftShift)) //moving without Shift is used for precise positioning, with Shift is used for fast moving around world
            {
                if (moveKeyState.IsKeyDown(Keys.Down))
                {
                    selectedTileY += 1;
                    canMove = false;
                }
                if (moveKeyState.IsKeyDown(Keys.Up))
                {
                    selectedTileY -= 1;
                    canMove = false;
                }
                if (moveKeyState.IsKeyDown(Keys.Left))
                {
                    selectedTileX -= 1;
                    canMove = false;
                }
                if (moveKeyState.IsKeyDown(Keys.Right))
                {
                    selectedTileX += 1;
                    canMove = false;
                }
            }
            if (moveKeyState.IsKeyUp(Keys.Down) && moveKeyState.IsKeyUp(Keys.Up) && moveKeyState.IsKeyUp(Keys.Left) && moveKeyState.IsKeyUp(Keys.Right))
            {
                canMove = true;
            }
            // placing an object
            KeyboardState placeKeyState = Keyboard.GetState();
            if (placeKeyState.IsKeyDown(Keys.D0))
            {
                worldTiles[selectedTileX, selectedTileY][0] = 0;
            }
            if (placeKeyState.IsKeyDown(Keys.D1))
            {
                worldTiles[selectedTileX, selectedTileY][0] = 1;
            }
            if (placeKeyState.IsKeyDown(Keys.D2))
            {
                worldTiles[selectedTileX, selectedTileY][0] = 2;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            // draw tiles
            int x = 0;
            int y = 0;
            int id = 0;
            while (y < 24)
            {
                while (x < 20)
                {
                    /*// let's see what ID is an element
                    try
                    {
                        if (worldTiles[x, y][0] == null)
                        {
                            throw new ArgumentNullException();
                        }
                    }
                    catch
                    {
                        id = 0;
                    }
                    finally
                    {
                        //id = worldTiles[x, y][0];
                        id = 1;
                    }
                    /*id = 0;
                    if (worldTiles[x, y][0] == null)
                    {
                        throw new ArgumentNullException();
                        //id = worldTiles[x, y][0];
                    }*/
                    id = 0;
                    spriteBatch.Draw(Content.Load<Texture2D>(@"tile/id" + id), new Rectangle(x * 40, (y * 20) - 20, tile.Width, tile.Height), Color.White);
                    x++;
                }
                x = 0;
                y++;
            }
            // mark selected tile
            spriteBatch.Draw(select, new Rectangle(selectedTileX * 40, selectedTileY * 20, select.Width, select.Height), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
