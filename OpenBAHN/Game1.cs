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
        //int[][] worldTiles = new int[0][];
        List<List<int>> worldTiles = new List<List<int>>();
        // element 0: X
        // element 1: Y
        // element 2: ID
        // further elements: parameters (vary by tile type)
        int[] tileList = { 0, 1, 2, 3, 4, 8, 9, 10, 11, 12 };
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
            if (placeKeyState.IsKeyDown(Keys.D1))
            {
                writeTile(true, 0, 0, tileList[0]);
            }
            if (placeKeyState.IsKeyDown(Keys.D2))
            {
                writeTile(true, 0, 0, tileList[1]);
            }
            if (placeKeyState.IsKeyDown(Keys.D3))
            {
                writeTile(true, 0, 0, tileList[2]);
            }
            if (placeKeyState.IsKeyDown(Keys.D4))
            {
                writeTile(true, 0, 0, tileList[3]);
            }
            if (placeKeyState.IsKeyDown(Keys.D5))
            {
                writeTile(true, 0, 0, tileList[4]);
            }
            if (placeKeyState.IsKeyDown(Keys.D6))
            {
                writeTile(true, 0, 0, tileList[5]);
            }
            if (placeKeyState.IsKeyDown(Keys.D7))
            {
                writeTile(true, 0, 0, tileList[6]);
            }
            if (placeKeyState.IsKeyDown(Keys.D8))
            {
                writeTile(true, 0, 0, tileList[7]);
            }
            if (placeKeyState.IsKeyDown(Keys.D9))
            {
                writeTile(true, 0, 0, tileList[8]);
            }
            if (placeKeyState.IsKeyDown(Keys.D0))
            {
                writeTile(true, 0, 0, tileList[9]);
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
                    id = getTileID(x, y);
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
        // methods
        void writeTile(bool actualPos, int x, int y, int ID, int[] parameters = null)
        {
            // deleting a tile first to don't have 2 elements in one tile
            deleteTile(actualPos, x, y);
            // when ID is 0 then we only delete a tile so we skip code below
            if (ID != 0)
            {
                List<int> composition = new List<int>();
                // adding three obligatory parameters
                if (actualPos)
                {
                    composition.Add(selectedTileX);
                    composition.Add(selectedTileY);
                }
                else
                {
                    composition.Add(x);
                    composition.Add(y);
                }
                composition.Add(ID);
                // next, a number of optional parameters
                parameters = parameters ?? new int[0];
                foreach (int parameter in parameters)
                {
                    composition.Add(parameter);
                }
                // adding to a global list
                worldTiles.Add(composition);
            }
        }
        void deleteTile(bool actualPos, int x, int y)
        {
            int setx = x;
            int sety = y;
            if (actualPos)
            {
                setx = selectedTileX;
                sety = selectedTileY;
            }
            if (getTileItem(setx, sety) != -1)
            {
                worldTiles.RemoveAt(getTileItem(setx, sety));
            }
        }
        int getTileID(int x, int y)
        {
            return getTileParameter(x, y, 0);
        }
        int getTileParameter(int x, int y, int parameter)
        {
            int returnValue = 0; // in case when there isn't any tile in that position
            foreach (List<int> composition in worldTiles)
            {
                if (composition[0] == x && composition[1] == y)
                {
                    returnValue = composition[parameter + 2];
                }
            }
            return returnValue;
        }
        int getTileItem(int x, int y)
        {
            int returnValue = 0; // as above
            foreach (List<int> composition in worldTiles)
            {
                if (composition[0] == x && composition[1] == y)
                {
                    return returnValue;
                }
                returnValue++;
            }
            return -1;
        }
    }
}
