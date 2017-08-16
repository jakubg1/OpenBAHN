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
        Int64 tickStart = Environment.TickCount;
        bool canMove = true;
        bool canPlace = true;

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
            // mouse visiblity
            IsMouseVisible = true;
            // delete old log
            System.IO.File.WriteAllText(@"C:\Users\Public\Test\log.txt", "");
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
            if (canPlace)
            {
                if (placeKeyState.IsKeyDown(Keys.S))
                {
                    save();
                    canPlace = false;
                }
                if (placeKeyState.IsKeyDown(Keys.L))
                {
                    load();
                    canPlace = false;
                }
                if (placeKeyState.IsKeyDown(Keys.C))
                {
                    clearWorld();
                    canPlace = false;
                }
                if (placeKeyState.IsKeyDown(Keys.D1))
                {
                    writeTileCurrent(tileList[0]);
                    canPlace = false;
                }
                if (placeKeyState.IsKeyDown(Keys.D2))
                {
                    writeTileCurrent(tileList[1]);
                    canPlace = false;
                }
                if (placeKeyState.IsKeyDown(Keys.D3))
                {
                    writeTileCurrent(tileList[2]);
                    canPlace = false;
                }
                if (placeKeyState.IsKeyDown(Keys.D4))
                {
                    writeTileCurrent(tileList[3]);
                    canPlace = false;
                }
                if (placeKeyState.IsKeyDown(Keys.D5))
                {
                    writeTileCurrent(tileList[4]);
                    canPlace = false;
                }
                if (placeKeyState.IsKeyDown(Keys.D6))
                {
                    writeTileCurrent(tileList[5]);
                    canPlace = false;
                }
                if (placeKeyState.IsKeyDown(Keys.D7))
                {
                    writeTileCurrent(tileList[6]);
                    canPlace = false;
                }
                if (placeKeyState.IsKeyDown(Keys.D8))
                {
                    writeTileCurrent(tileList[7]);
                    canPlace = false;
                }
                if (placeKeyState.IsKeyDown(Keys.D9))
                {
                    writeTileCurrent(tileList[8]);
                    canPlace = false;
                }
                if (placeKeyState.IsKeyDown(Keys.D0))
                {
                    writeTileCurrent(tileList[9]);
                    canPlace = false;
                }
            }
            if (placeKeyState.IsKeyUp(Keys.S) && placeKeyState.IsKeyUp(Keys.L) && placeKeyState.IsKeyUp(Keys.C) && placeKeyState.IsKeyUp(Keys.D1) && placeKeyState.IsKeyUp(Keys.D2) && placeKeyState.IsKeyUp(Keys.D3) && placeKeyState.IsKeyUp(Keys.D4) && placeKeyState.IsKeyUp(Keys.D5) && placeKeyState.IsKeyUp(Keys.D6) && placeKeyState.IsKeyUp(Keys.D7) && placeKeyState.IsKeyUp(Keys.D8) && placeKeyState.IsKeyUp(Keys.D9) && placeKeyState.IsKeyUp(Keys.D0))
            {
                canPlace = true;
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
            for (int y = 0; y < 24; y++)
            {
                for (int x = 0; x < 20; x++)
                {
                    if (getTileID(x, y) != 0)
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>(@"tile/id" + getTileID(x, y)), new Rectangle(x * 40, (y * 20) - 20, 40, 60), Color.White);
                    }
                }
            }
            // mark selected tile
            spriteBatch.Draw(select, new Rectangle(currentTile[0] * 40, currentTile[1] * 20, select.Width, select.Height), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        // methods
        void save()
        {
            Int64 tick = Environment.TickCount;
            writeLog("Saving started");
            string composition = "";
            foreach (List<int> composition2 in worldTiles)
            {
                foreach (int parameter in composition2)
                {
                    composition += Convert.ToString(parameter) + ",";
                    writeLog("Parameter saved: " + parameter);
                }
                composition += "\n";
            }
            System.IO.File.WriteAllText(@"C:\Users\Public\Test\WriteLines.txt", composition);
            tick = (tick - Environment.TickCount) * -1;
            writeLog("World saved successfully in " + tick + " milliseconds.");
        }

        void load()
        {
            Int64 tick = Environment.TickCount;
            writeLog("Loading started");
            clearWorld(); // first, we clear actual world
            string[] composition = System.IO.File.ReadAllLines(@"C:\Users\Public\Test\WriteLines.txt"); // open a file and store every line in one item in array
            writeLog("Read from file successful");
            foreach (string composition2 in composition) // for each item in array
            {
                string composition3 = ""; // we use it for compose a number from characters
                List<int> parameters = new List<int>(); // this list will contain data that we will write in global tiles list
                foreach (char letter in composition2) // for each letter in string like that: "13,5,3,"
                {
                    if (letter != ',') // it's not new parameter so we continue composing a number
                    {
                        composition3 += letter; // composing
                    }
                    if (letter == ',') // after comma we go to new parameter, so we add composed number to the list of parameters; it's important to place a comma after last parameter
                    {
                        try
                        {
                            parameters.Add(Convert.ToInt32(composition3)); // adding
                            writeLog("Parsed a parameter: " + composition3);
                        }
                        catch (FormatException)
                        {
                            writeLog("Can not convert this parameter: " + composition3 + ", skipping...");
                        }
                        finally
                        {
                            composition3 = ""; // don't forget to clear composed number; now we are ready to compose a new parameter
                        }
                    }
                }
                writeLog("Started additional parameters parsing.");
                if (parameters.Count >= 3)
                {
                    int[] composition4 = new int[parameters.Count - 3]; // we create a new array for the rest - parameters
                    for (int i = 0; i < parameters.Count - 3; i++)
                    {
                        composition4[i] = parameters[i + 3];
                    }
                    writeLog("Parameters parsed: " + composition4.Length);
                    writeTile(parameters[0], parameters[1], parameters[2], composition4); // after that, we are ready to add a new tile to the world...
                }
                else
                {
                    writeLog("Error, not enough parameters. To avoid exception, a tile was not added :(");
                }
                parameters.Clear(); // and clear the list for next tile
            }
            tick = (tick - Environment.TickCount) * -1;
            writeLog("World loaded successfully in " + tick + " milliseconds.");
        }

        void writeLog(string text)
        {
            using (System.IO.StreamWriter log = new System.IO.StreamWriter(@"C:\Users\Public\Test\log.txt", true))
            {
                Int64 tick = Environment.TickCount - tickStart;
                log.WriteLine("Tick " + tick + ": " + text);
            }
        }

        void clearWorld()
        {
            writeLog("World clearing started");
            worldTiles.Clear();
            writeLog("World cleared");
        }

        void writeTile(int x, int y, int id, int[] parameters = null)
        {
            // deleting a tile first to don't have 2 elements in one tile
            deleteTile(x, y);
            // when ID is 0 then we only delete a tile so we skip code below
            if (id != 0)
            {
                List<int> composition = new List<int>();
                // adding three obligatory parameters
                composition.Add(x);
                composition.Add(y);
                composition.Add(id);
                // next, a number of optional parameters
                parameters = parameters ?? new int[0];
                foreach (int parameter in parameters)
                {
                    composition.Add(parameter);
                }
                // adding to a global list
                worldTiles.Add(composition);
            }
            writeLog("Tile placed in X: " + x + " Y: " + y + " ID: " + id + " Parameters: " + intArrayToString(parameters));
        }

        void writeTileCurrent(int id, int[] parameters = null)
        {
            writeTile(currentTile[0], currentTile[1], id, parameters);
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

        int getTileParametersQuantity(int x, int y)
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

        string intArrayToString(int[] parameters)
        {
            parameters = parameters ?? new int[0]; // to not cause a crash
            string returnValue = "";
            int iterationsLeft = parameters.Length;
            if (iterationsLeft > 0)
            {
                foreach (int parameter in parameters)
                {
                    iterationsLeft--;
                    returnValue += parameter; // note that conversion go automatically
                    if (iterationsLeft > 0)
                    {
                        returnValue += ", ";
                    }
                }
                return returnValue;
            }
            else
            {
                return "none";
            }
        }
    }
}
