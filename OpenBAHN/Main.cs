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
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D none;
        Texture2D select;

        public Main()
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
            // mouse visiblity
            IsMouseVisible = true;

            // delete old log
            System.IO.File.WriteAllText(@"C:\Users\Public\Test\log.txt", "");
            Log.writeLog("Current version: " + Data.currentVersion);
            base.Initialize();

            // make rectangles
            for (int i = 0; i < 20; i++) // x
            {
                for (int j = 0; j < 24; j++) // y
                {
                    Data.markGraphicsSquares[i, j] = new Rectangle(i * 40, j * 20, 40, 20);
                    Data.tileGraphicsSquares[i, j] = new Rectangle(i * 40, (j - 1) * 20, 40, 60);
                }
            }
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
            // below very important function that allows to draw without texture
            none = new Texture2D(GraphicsDevice, 1, 1);
            none.SetData<Color>(new Color[] {Color.White});


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
            {
                Log.writeLog("Program closed without error.");
                this.Exit();
            }

            // TODO: Add your update logic here
            // moving the cursor
            KeyboardState moveKeyState = Keyboard.GetState();
            if (Data.canMoveKeyBoard || moveKeyState.IsKeyDown(Keys.LeftShift)) //moving without Shift is used for precise positioning, with Shift is used for fast moving around world
            {
                if (moveKeyState.IsKeyDown(Keys.Down))
                {
                    Movement.MoveCursor(2);
                    Movement.UpdateCamera();
                    Data.canMoveKeyBoard = false;
                }
                if (moveKeyState.IsKeyDown(Keys.Up))
                {
                    Movement.MoveCursor(0);
                    Movement.UpdateCamera();
                    Data.canMoveKeyBoard = false;
                }
                if (moveKeyState.IsKeyDown(Keys.Left))
                {
                    Movement.MoveCursor(1);
                    Movement.UpdateCamera();
                    Data.canMoveKeyBoard = false;
                }
                if (moveKeyState.IsKeyDown(Keys.Right))
                {
                    Movement.MoveCursor(3);
                    Movement.UpdateCamera();
                    Data.canMoveKeyBoard = false;
                }
            }
            if (moveKeyState.IsKeyUp(Keys.Down) && moveKeyState.IsKeyUp(Keys.Up) && moveKeyState.IsKeyUp(Keys.Left) && moveKeyState.IsKeyUp(Keys.Right))
            {
                Data.canMoveKeyBoard = true;
            }
            // by mouse
            MouseState moveMouseState = Mouse.GetState();
            if (moveMouseState.LeftButton == ButtonState.Pressed)
            {
                Movement.SetCursorPosition(Movement.GetXPosFromMouse(moveMouseState.X), Movement.GetYPosFromMouse(moveMouseState.Y));
                if (Environment.TickCount % 5 == 0)
                {
                    Movement.UpdateCamera();
                }
                if (Data.canMoveMouse)
                {
                    // If we write "Data.tileSel1 = Data.currentTile;" system will set "Data.tileSel1" as reference, not value!
                    Data.tileSel1[0] = Data.currentTile[0];
                    Data.tileSel1[1] = Data.currentTile[1];
                    Data.selectingMode = false;
                    Data.canMoveMouse = false;
                }
            }
            if (moveMouseState.LeftButton == ButtonState.Released)
            {
                Data.canMoveMouse = true;
            }
            // area selecting
            if (moveMouseState.LeftButton == ButtonState.Pressed && Data.currentTile != Data.tileSel1)
            {
                Data.tileSel2 = Data.currentTile;
                Data.selectingMode = true;
            }
            if (Data.tileSel1[0] == Data.tileSel2[0] && Data.tileSel1[1] == Data.tileSel2[1])
            {
                Data.selectingMode = false;
            }

            // placing an object
            KeyboardState placeKeyState = Keyboard.GetState();
            if (Data.canPlace)
            {
                if (placeKeyState.IsKeyDown(Keys.S))
                {
                    File.Save();
                    Data.canPlace = false;
                }
                if (placeKeyState.IsKeyDown(Keys.L))
                {
                    File.Load();
                    Data.canPlace = false;
                }
                if (placeKeyState.IsKeyDown(Keys.C))
                {
                    World.Clear();
                    Data.canPlace = false;
                }
                if (placeKeyState.IsKeyDown(Keys.Delete))
                {
                    if (!Data.selectingMode)
                    {
                        World.DeleteCurrent();
                    }
                    if (Data.selectingMode)
                    {
                        World.DeleteCurrentArea();
                    }
                }

                int i = 0;
                foreach (Keys key in Data.placeKeys)
                {
                    if (placeKeyState.IsKeyDown(key))
                    {
                        World.WriteCurrent(Data.tileList[i]);
                        Data.canPlace = false;
                    }
                    i++;
                }
            }
            if (placeKeyState.IsKeyUp(Keys.S) && placeKeyState.IsKeyUp(Keys.L) && placeKeyState.IsKeyUp(Keys.C) && placeKeyState.IsKeyUp(Keys.D1) && placeKeyState.IsKeyUp(Keys.D2) && placeKeyState.IsKeyUp(Keys.D3) && placeKeyState.IsKeyUp(Keys.D4) && placeKeyState.IsKeyUp(Keys.D5) && placeKeyState.IsKeyUp(Keys.D6) && placeKeyState.IsKeyUp(Keys.D7) && placeKeyState.IsKeyUp(Keys.D8) && placeKeyState.IsKeyUp(Keys.D9) && placeKeyState.IsKeyUp(Keys.D0))
            {
                Data.canPlace = true;
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
            for (int i = 0; i < 20; i++) // x
            {
                for (int j = 0; j < 24; j++) // y
                {
                    int x = i + Data.cameraPosition[0];
                    int y = j + Data.cameraPosition[1];
                    if (World.GetID(x, y) != 0)
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>(@"tile/id" + World.GetID(x, y)), Data.tileGraphicsSquares[i, j], Color.White);
                    }
                }
            }

            // mark selected tile
            int posx, posy, relx, rely;
            posx = Data.currentTile[0];
            posy = Data.currentTile[1];
            relx = posx - Data.cameraPosition[0];
            rely = posy - Data.cameraPosition[1];
            spriteBatch.Draw(select, Data.markGraphicsSquares[relx, rely], Color.White);

            // 3 lines below: for debug purposes; DO NOT DELETE THEM!
            int posx1, posy1, relx1, rely1;
            posx1 = Data.tileSel1[0];
            posy1 = Data.tileSel1[1];
            relx1 = posx1 - Data.cameraPosition[0];
            rely1 = posy1 - Data.cameraPosition[1];
            spriteBatch.Draw(select, Data.markGraphicsSquares[relx, rely], Color.Lime);
            
            int posx2, posy2, relx2, rely2;
            posx2 = Data.tileSel2[0];
            posy2 = Data.tileSel2[1];
            relx2 = posx2 - Data.cameraPosition[0];
            rely2 = posy2 - Data.cameraPosition[1];
            spriteBatch.Draw(select, Data.markGraphicsSquares[relx, rely], Color.Red);

            //spriteBatch.Draw(none, new Rectangle((Data.tileSel1[0] - Data.cameraPosition[0]) * 40, (Data.tileSel1[1] - Data.cameraPosition[1]) * 20, (Math.Abs(Data.tileSel1[0] - Data.tileSel2[0]) + 1) * 40, (Math.Abs(Data.tileSel1[1] - Data.tileSel2[1]) + 1) * 20), Color.Magenta);
            // mark selected area
            if (Data.selectingMode)
            {
                // up
                //spriteBatch.Draw(none, new Rectangle((Data.tileSel1[0] - Data.cameraPosition[0]) * 40, (Data.tileSel1[1] - Data.cameraPosition[1]) * 20, (Math.Abs(Data.tileSel1[0] - Data.tileSel2[0]) + 1) * 40, 2), Color.Magenta);
                // left
                //spriteBatch.Draw(none, new Rectangle((Data.tileSel1[0] - Data.cameraPosition[0]) * 40, (Data.tileSel1[1] - Data.cameraPosition[1]) * 20, 2, (Math.Abs(Data.tileSel1[1] - Data.tileSel2[1]) + 1) * 20), Color.Magenta);
                // bottom
                //spriteBatch.Draw(none, new Rectangle((Data.tileSel1[0] - Data.cameraPosition[0]) * 40, (((Data.tileSel1[1] - Data.cameraPosition[1]) + (Math.Abs(Data.tileSel1[1] - Data.tileSel2[1]) + 1)) * 20) - 2, (Math.Abs(Data.tileSel1[0] - Data.tileSel2[0]) + 1) * 40, 2), Color.Magenta);
                // right
                //spriteBatch.Draw(none, new Rectangle((((Data.tileSel1[0] - Data.cameraPosition[0]) + (Math.Abs(Data.tileSel1[0] - Data.tileSel2[0]) + 1)) * 40) - 2, (Data.tileSel1[1] - Data.cameraPosition[1]) * 20, 2, (Math.Abs(Data.tileSel1[1] - Data.tileSel2[1]) + 1) * 20), Color.Magenta);
                int[] pixelUpLeft = Graphics.getTilePixelPosition(relx1, rely1, false, false);
                int[] pixelUpRight = Graphics.getTilePixelPosition(relx2, rely1, true, false);
                int[] pixelDownLeft = Graphics.getTilePixelPosition(relx1, rely2, false, true);
                int[] pixelDownRight = Graphics.getTilePixelPosition(relx2, rely2, true, true);
                Rectangle up = new Rectangle(pixelUpLeft[0], pixelUpLeft[1], pixelUpRight[0] - pixelUpLeft[0], 2);
                Rectangle bottom = new Rectangle(pixelDownLeft[0], pixelDownLeft[1] - 1, pixelDownRight[0] - pixelDownLeft[0], 2);
                Rectangle left = new Rectangle(pixelUpLeft[0], pixelUpLeft[1], 2, pixelDownLeft[1] - pixelUpLeft[1]);
                Rectangle right = new Rectangle(pixelUpRight[0] - 1, pixelUpRight[1], 2, pixelDownRight[1] - pixelUpRight[1]);
                spriteBatch.Draw(none, up, Color.Magenta);
                spriteBatch.Draw(none, bottom, Color.Magenta);
                spriteBatch.Draw(none, left, Color.Magenta);
                spriteBatch.Draw(none, right, Color.Magenta);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
    public class Data
    {
        public static List<List<int>> worldTiles = new List<List<int>>();
        // element 0: X
        // element 1: Y
        // element 2: ID
        // further elements: parameters (vary by tile type)
        public static Keys[] placeKeys = new Keys[] { Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0 };
        public static int[] tileList = { 0, 1, 2, 3, 4, 8, 9, 10, 11, 12 };
        public static int[] currentTile = { 0, 0 };
        public static int[] tileSel1 = { 0, 0 };
        public static int[] tileSel2 = { 0, 0 };
        public static bool selectingMode = false;
        public static int[] cameraPosition = { 0, 0 };
        public static bool canMoveKeyBoard = true;
        public static bool canMoveMouse = true;
        public static bool canPlace = true;
        public static Int64 tickStart = Environment.TickCount;
        public const string currentVersion = "Alpha0.0.2"; // remember to change every release!
        public static Rectangle[,] markGraphicsSquares = new Rectangle[20, 24];
        public static Rectangle[,] tileGraphicsSquares = new Rectangle[20, 24];
    }
    public class File
    {
        public static void Save()
        {
            Int64 tick = Environment.TickCount;
            Log.writeLog("Saving started");
            string composition = "";
            composition += Data.currentVersion + "\n#\n"; // version
            foreach (List<int> composition2 in Data.worldTiles) // tracks
            {
                foreach (int parameter in composition2)
                {
                    composition += Convert.ToString(parameter) + ",";
                    Log.writeLog("Parameter saved: " + parameter);
                }
                composition += "\n";
            }
            System.IO.File.WriteAllText(@"C:\Users\Public\Test\WriteLines.txt", composition);
            tick = (tick - Environment.TickCount) * -1;
            Log.writeLog("World saved successfully in " + tick + " milliseconds.");
        }

        public static void Load()
        {
            Int64 tick = Environment.TickCount;
            Log.writeLog("Loading started");
            World.Clear(); // first, we clear current world
            string[] composition = System.IO.File.ReadAllLines(@"C:\Users\Public\Test\WriteLines.txt"); // open a file and store every line in one item in array
            Log.writeLog("Read from file successful");
            int loadingStage = 0; // 0 - file format version, 1 - tracks, 2 - track scripts, 3 - trains, 4 - train scripts
            bool abort = false; // if true there is error and loading is aborted
            foreach (string line in composition) // for each item in array
            {
                if (line == "#") // a hash ends a segment of loaded file
                {
                    loadingStage++;
                    Log.writeLog("Ended parsing a segment of data loading. New stage: " + loadingStage);
                }
                else // we do this because when it's a new segment it parses this hash so it can do unexpected behavior
                {
                    if (loadingStage == 0)
                    {
                        Log.writeLog("File version: " + line);
                        if (line == Data.currentVersion)
                        {
                            Log.writeLog("Versions match!");
                        }
                        else
                        {
                            Log.writeLog("Versions does not match! Loading version list to check that this save is not saved in newer version...");
                            string[] versions = System.IO.File.ReadAllLines(@"C:\Users\Public\Test\versions.txt"); // it works as above
                            foreach (string version in versions)
                            {
                                if (line == version)
                                {
                                    Log.writeLog("This file was saved in older version. Let's check data systems...");
                                    // Code for checking data systems will be written in far future.
                                    Log.writeLog("Data systems are the same in both versions. Continue loading.");
                                    break;
                                }
                                Log.writeLog("It seems that this file was saved in newer version. File not loaded due to data corruption risk.");
                                abort = true;
                            }
                        }
                    }
                    if (loadingStage == 1)
                    {
                        string composedNumber = ""; // we use it for compose a number from characters
                        List<int> parameters = new List<int>(); // this list will contain data that we will write in global tiles list
                        foreach (char letter in line) // for each letter in string like that: "13,5,3,"
                        {
                            if (letter != ',') // it's not new parameter so we continue composing a number
                            {
                                composedNumber += letter; // composing
                            }
                            if (letter == ',') // after comma we go to new parameter, so we add composed number to the list of parameters; it's important to place a comma after last parameter
                            {
                                try
                                {
                                    parameters.Add(Convert.ToInt32(composedNumber)); // adding
                                    Log.writeLog("Parsed a parameter: " + composedNumber);
                                }
                                catch (FormatException)
                                {
                                    Log.writeLog("Can not convert this parameter: " + composedNumber + ", skipping...");
                                }
                                finally
                                {
                                    composedNumber = ""; // don't forget to clear composed number; now we are ready to compose a new parameter
                                }
                            }
                        }
                        Log.writeLog("Started additional parameters parsing.");
                        if (parameters.Count >= 3) // a X, Y and ID values
                        {
                            int[] composedParameters = new int[parameters.Count - 3]; // we create a new array for the rest - parameters
                            for (int i = 0; i < parameters.Count - 3; i++)
                            {
                                composedParameters[i] = parameters[i + 3];
                            }
                            Log.writeLog("Parameters parsed: " + composedParameters.Length);
                            World.Write(parameters[0], parameters[1], parameters[2], composedParameters); // after that, we are ready to add a new tile to the world...
                        }
                        else
                        {
                            Log.writeLog("Error, not enough parameters. To avoid exception, a tile was not added :(");
                        }
                        parameters.Clear(); // and clear the list for next tile
                    }
                }
                if (abort)
                {
                    break;
                }
            }
            tick = (tick - Environment.TickCount) * -1;
            if (abort)
            {
                Log.writeLog("World was not loaded successfully. Time wasted: " + tick + "ms.");
            }
            else
            {
                Log.writeLog("World loaded successfully in " + tick + " milliseconds.");
            }
        }
    }
    public class Movement
    {
        public static void MoveCursor(int direction) // 0: up 1: left 2: down 3: right
        {
            switch (direction)
            {
                case 0:
                    Data.currentTile[1]--;
                    break;
                case 1:
                    Data.currentTile[0]--;
                    break;
                case 2:
                    Data.currentTile[1]++;
                    break;
                case 3:
                    Data.currentTile[0]++;
                    break;
            }
        }
        public static int[] GetPosFromMouse(int x, int y)
        {
            int[] returnValue = new int[2];
            int returnX = Convert.ToInt32(Math.Floor(Convert.ToDouble(x) / 40)) + Data.cameraPosition[0];
            int returnY = Convert.ToInt32(Math.Floor(Convert.ToDouble(y) / 20)) + Data.cameraPosition[1];
            returnValue[0] = returnX;
            returnValue[1] = returnY;
            return returnValue;
        }
        public static int GetXPosFromMouse(int x)
        {
            return Convert.ToInt32(Math.Floor(Convert.ToDouble(x) / 40)) + Data.cameraPosition[0];
        }
        public static int GetYPosFromMouse(int y)
        {
            return Convert.ToInt32(Math.Floor(Convert.ToDouble(y) / 20)) + Data.cameraPosition[1];
        }
        public static void SetCursorPosition(int x, int y)
        {
            Data.currentTile[0] = x;
            Data.currentTile[1] = y;
        }
        public static void UpdateCamera()
        {
            int xMin = Data.cameraPosition[0];
            int yMin = Data.cameraPosition[1];
            int xMax = Data.cameraPosition[0] + 19;
            int yMax = Data.cameraPosition[1] + 23;
            if (Data.currentTile[1] < yMin) // up
            {
                MoveCamera(0);
            }
            if (Data.currentTile[0] < xMin) // left
            {
                MoveCamera(1);
            }
            if (Data.currentTile[1] > yMax) // down
            {
                MoveCamera(2);
            }
            if (Data.currentTile[0] > xMax) // right
            {
                MoveCamera(3);
            }
        }
        static void MoveCamera(int direction) // 0: up 1: left 2: down 3: right
        {
            switch (direction)
            {
                case 0:
                    Data.cameraPosition[1]--;
                    break;
                case 1:
                    Data.cameraPosition[0]--;
                    break;
                case 2:
                    Data.cameraPosition[1]++;
                    break;
                case 3:
                    Data.cameraPosition[0]++;
                    break;
            }
        }
    }
    public class Log
    {
        public static void writeLog(string text)
        {
            using (System.IO.StreamWriter log = new System.IO.StreamWriter(@"C:\Users\Public\Test\log.txt", true))
            {
                Int64 tick = Environment.TickCount - Data.tickStart;
                log.WriteLine("Tick " + tick + ": " + text);
            }
        }
    }
    public class World
    {
        public static void Clear()
        {
            Log.writeLog("World clearing started");
            Data.worldTiles.Clear();
            Log.writeLog("World cleared");
        }

        public static void Write(int x, int y, int id, int[] parameters = null)
        {
            // deleting a tile first to don't have 2 elements in one tile
            Delete(x, y);
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
                Data.worldTiles.Add(composition);
            }
            Log.writeLog("Tile placed in X: " + x + " Y: " + y + " ID: " + id + " Parameters: " + Conversion.intArrayToString(parameters));
        }

        public static void WriteCurrent(int id, int[] parameters = null)
        {
            Write(Data.currentTile[0], Data.currentTile[1], id, parameters);
        }

        public static void Delete(int x, int y)
        {
            int setx = x;
            int sety = y;
            if (GetListItem(setx, sety) != -1)
            {
                Data.worldTiles.RemoveAt(GetListItem(setx, sety));
            }
        }

        public static void DeleteArea(int x1, int y1, int x2, int y2)
        {
            // swap values if they are in wrong order
            if (x1 > x2)
            {
                int temp = x2;
                x2 = x1;
                x1 = temp;
            }
            if (y1 > y2)
            {
                int temp = y2;
                y2 = y1;
                y1 = temp;
            }
            // delete every tile in area
            for (int y = 0; y <= y2 - y1; y++)
            {
                for (int x = 0; x <= x2 - x1; x++)
                {
                    Delete(x1 + x, y1 + y);
                }
            }
        }

        public static void DeleteCurrent()
        {
            Delete(Data.currentTile[0], Data.currentTile[1]);
        }

        public static void DeleteCurrentArea()
        {
            DeleteArea(Data.tileSel1[0], Data.tileSel1[1], Data.tileSel2[0], Data.tileSel2[1]);
        }

        public static int GetID(int x, int y)
        {
            return GetParameter(x, y, 0);
        }

        public static int GetParameter(int x, int y, int parameter)
        {
            int returnValue = 0; // in case when there isn't any tile in that position
            foreach (List<int> composition in Data.worldTiles)
            {
                if (composition[0] == x && composition[1] == y)
                {
                    returnValue = composition[parameter + 2];
                }
            }
            return returnValue;
        }

        public static int GetParametersQuantity(int x, int y)
        {
            if (GetListItem(x, y) != -1)
            {
                return Data.worldTiles[GetListItem(x, y)].Count - 3;
            }
            else
            {
                return -1;
            }
        }

        public static int GetListItem(int x, int y)
        {
            int returnValue = 0;
            foreach (List<int> composition in Data.worldTiles)
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
    public class Graphics
    {
        public static int[] getTilePixelPosition(int x, int y, bool lastPixelX, bool lastPixelY, int offsetX = 0, int offsetY = 0)
        {
            int returnX = x * 40;
            int returnY = y * 20;
            if (lastPixelX)
            {
                returnX += 39 - offsetX;
            }
            else
            {
                returnX += offsetX;
            }
            if (lastPixelY)
            {
                returnY += 19 - offsetY;
            }
            else
            {
                returnX += offsetY;
            }
            return new int[] { returnX, returnY };
        }
    }
    public class Conversion
    {
        public static string intArrayToString(int[] parameters)
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
