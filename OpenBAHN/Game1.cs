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
        Texture2D select;
        List<List<int>> worldTiles = new List<List<int>>();
        // element 0: X
        // element 1: Y
        // element 2: ID
        // further elements: parameters (vary by tile type)
        int[] tileList = { 0, 1, 2, 3, 4, 8, 9, 10, 11, 12 };
        int[] currentTile = { 0, 0 };
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
                    currentTile[1] += 1;
                    canMove = false;
                }
                if (moveKeyState.IsKeyDown(Keys.Up))
                {
                    currentTile[1] -= 1;
                    canMove = false;
                }
                if (moveKeyState.IsKeyDown(Keys.Left))
                {
                    currentTile[0] -= 1;
                    canMove = false;
                }
                if (moveKeyState.IsKeyDown(Keys.Right))
                {
                    currentTile[0] += 1;
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
                writeTileCurrent(tileList[0]);
            }
            if (placeKeyState.IsKeyDown(Keys.D2))
            {
                writeTileCurrent(tileList[1]);
            }
            if (placeKeyState.IsKeyDown(Keys.D3))
            {
                writeTileCurrent(tileList[2]);
            }
            if (placeKeyState.IsKeyDown(Keys.D4))
            {
                writeTileCurrent(tileList[3]);
            }
            if (placeKeyState.IsKeyDown(Keys.D5))
            {
                writeTileCurrent(tileList[4]);
            }
            if (placeKeyState.IsKeyDown(Keys.D6))
            {
                writeTileCurrent(tileList[5]);
            }
            if (placeKeyState.IsKeyDown(Keys.D7))
            {
                writeTileCurrent(tileList[6]);
            }
            if (placeKeyState.IsKeyDown(Keys.D8))
            {
                writeTileCurrent(tileList[7]);
            }
            if (placeKeyState.IsKeyDown(Keys.D9))
            {
                writeTileCurrent(tileList[8]);
            }
            if (placeKeyState.IsKeyDown(Keys.D0))
            {
                writeTileCurrent(tileList[9]);
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
                    spriteBatch.Draw(Content.Load<Texture2D>(@"tile/id" + id), new Rectangle(x * 40, (y * 20) - 20, 40, 60), Color.White);
                    x++;
                }
                x = 0;
                y++;
            }
            // mark selected tile
            spriteBatch.Draw(select, new Rectangle(currentTile[0] * 40, currentTile[1] * 20, select.Width, select.Height), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        // methods
        void save()
        {
            // System.IO.File.WriteAllBytes(@"C:\Users\Public\TestFolder\WriteLines.txt", worldTiles);
        }

        void writeTile(int x, int y, int ID, int[] parameters = null)
        {
            // deleting a tile first to don't have 2 elements in one tile
            deleteTile(x, y);
            // when ID is 0 then we only delete a tile so we skip code below
            if (ID != 0)
            {
                List<int> composition = new List<int>();
                // adding three obligatory parameters
                composition.Add(x);
                composition.Add(y);
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

        void writeTileCurrent(int ID, int[] parameters = null)
        {
            writeTile(currentTile[0], currentTile[1], ID, parameters);
        }

        void deleteTile(int x, int y)
        {
            int setx = x;
            int sety = y;
            if (getTileItem(setx, sety) != -1)
            {
                worldTiles.RemoveAt(getTileItem(setx, sety));
            }
        }

        void deleteTileCurrent()
        {
            deleteTile(currentTile[0], currentTile[1]);
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

        int getTileParameterNumber(int x, int y)
        {
            if (getTileItem(x, y) != -1)
            {
                return worldTiles[getTileItem(x, y)].Count - 3;
            }
            else
            {
                return -1;
            }
        }

        int getTileItem(int x, int y)
        {
            int returnValue = 0;
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
