using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AUTO_Matic.Scripts.SideScroll;
using AUTO_Matic.SideScroll;

namespace AUTO_Matic
{
    class SideTileMap
    {
        public static int pixelSize = 64;
        public int[,] Map;//local version of the created map
        public int mapWidth;
        public int mapHeight;
        public int worldWidth;
        public int worldHeight;
        public static SideTileMap tileMap = new SideTileMap();
        private static List<GroundTile> groundTiles = new List<GroundTile>();
        private static List<WallTile> wallTiles = new List<WallTile>();
        private static List<PlatformTile> platformTiles = new List<PlatformTile>();
        private static List<BackgroundTile> backgroundTiles = new List<BackgroundTile>();
        private static List<TopDoorTile> topDoorTiles = new List<TopDoorTile>();
        private static List<BottomDoorTile> bottomDoorTiles = new List<BottomDoorTile>();
        public static List<Vector2> enemySpawns = new List<Vector2>(); //List of enemySpawn locations
        public static List<Vector2> playerSpawns = new List<Vector2>();//List of playerSpawn locations 
        private static List<DungeonEntrance> dungeonEntrances = new List<DungeonEntrance>();
        public static List<int> GroundIndexes = new List<int>(); //List of the indexes that contain a Ground tile
        public static List<int> WallTilesIndexes = new List<int>(); //List of indexes that contain the wall tiles
        public static List<int> BackgroundIndexes = new List<int>();//List of indexes that contain the background tiles
        public static List<int> PlatformIndexes = new List<int>();//List of the indexes of the platform tiles
        public static List<RepeatBackground> repeatBG = new List<RepeatBackground>();

        

        //public static List<EmptyTile> EmptyTiles
        //{
        //    get { return emptyTiles; }
        //}

        //Public gets of the private lists above

        public static List<RepeatBackground>  RepeatBG
        {
            get { return repeatBG; }
        }

        public static List<PlatformTile> PlatformTiles
        {
            get { return platformTiles; }
        }

        public static List<DungeonEntrance> DungeonEntrances
        {
            get { return dungeonEntrances; }
        }

        public static List<BackgroundTile> BackgroundTiles
        {
            get { return backgroundTiles; }
        }

        public static List<TopDoorTile> TopDoorTiles
        {
            get { return topDoorTiles; }
        }

        public static List<BottomDoorTile> BottomDoorTiles
        {
            get { return bottomDoorTiles; }
        }

        public static List<GroundTile> GroundTiles
        {
            get { return groundTiles; }
        }

        public static List<WallTile> WallTiles
        {
            get { return wallTiles; }
        }

        public List<GroundTile> GetGroundTiles()
        {
            return groundTiles;
        }

        public List<PlatformTile> GetPlatformTiles()
        {
            return platformTiles;
        }

        //World dim in pixels
        public void SetWorldDims(int width, int height)
        {
            worldWidth = width;
            worldHeight = height;
        }

        //Length of the 2D array
        public void SetDims(int width, int height)
        {
            mapWidth = width;
            mapHeight = height;
        }

        public void SetMap(int[,] map)
        {
            Map = map;
        }

        static public int GetCellByPixelX(int pixelX)
        {
            return pixelX / pixelSize;
        }
        static public int GetCellByPixelY(int pixelY)
        {
            return pixelY / pixelSize;
        }
        static public Vector2 GetWorldDims()
        {
            return new Vector2(tileMap.worldWidth, tileMap.worldHeight);
        }
        public static void Generate(int[,] map, int size, bool isFinal)
        {
          
            //pixelSize = size;
            //Map = map;

            //emptyTiles.Clear();
            backgroundTiles.Clear();
            platformTiles.Clear();
            groundTiles.Clear();
            topDoorTiles.Clear();
            bottomDoorTiles.Clear();
            enemySpawns.Clear();
            playerSpawns.Clear();
            dungeonEntrances.Clear();

            int count = 0;//Repeat background are 4 tiles long so must make every 4
            int count2 = 0;//Repeat is 2 tiles tall so must make every 2
          
            for (int y = 0; y < map.GetLength(0); y++)
            {
                
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    
                    int num = map[y, x];
                    int num2 = 0;
                    if (y != 0)
                        num2 = map[y - 1, x];

                    //if(count == 0 || count >= 4)
                    //{
                    //    if(count2 == 0 || count2 >= 2)
                    //    {
                    //        repeatBG.Add(new RepeatBackground(new Rectangle(x * size, y * size, 256, 128)));
                    //        count = 0;
                    //        count2 = 0;
                    //    }

                    //}

                    if (num == 1)
                    {
                        backgroundTiles.Add(new BackgroundTile(num,new Rectangle(x * size, y * size, size, size)));
                    }
                    else if (num == 2 || num == 5 || num == 6 || num == 13 || num == 14 || num == 15 || num == 16 || num == 17 || num == 18 || num == 19 || num == 20|| num ==21 || num ==22 || num ==23)
                    {
                        //If at the top of the map
                     

                        if (y==0)
                        {
                            groundTiles.Add(new GroundTile(num, new Rectangle(x * size, y * size, size, size)));
                            if (GroundIndexes.Contains(num) == false)
                                GroundIndexes.Add(num);
                        }
                        else if(y > 0 && GroundIndexes.Contains(num2))//If there is a tile above this one
                        {
                            wallTiles.Add(new WallTile(num, new Rectangle(x * size, y * size, size, size)));
                            //Add indexes?
                            if (GroundIndexes.Contains(num) == false)
                                GroundIndexes.Add(num);
                        }
                        else //Make it a ground tile
                        {
                            groundTiles.Add(new GroundTile(num, new Rectangle(x * size, y * size, size, size)));
                            if (GroundIndexes.Contains(num) == false)
                                GroundIndexes.Add(num);
                        }
                      
                       
                    }
                    else if (num == 3 || num == 4 || num == 26) //Platforms
                    {
                        if (num == 26)
                        {
                            //backgroundTiles.Add(new BackgroundTile(1, new Rectangle(x * size, y * size, size, size)));
                        }

                        if (y == 0)//If at the top of the map
                        {
                            platformTiles.Add(new PlatformTile(num, new Rectangle(x * size, y * size, size, size)));
                            if (PlatformIndexes.Contains(num) == false)
                                PlatformIndexes.Add(num);
                        }
                        else if(y > 0 && PlatformIndexes.Contains(num2)) //If there is one above it 
                        {
                            wallTiles.Add(new WallTile(num, new Rectangle(x * size, y * size, size, size)));
                            if (PlatformIndexes.Contains(num) == false)
                                PlatformIndexes.Add(num);
                        }
                        else //Regular platform
                        {
                            platformTiles.Add(new PlatformTile(num, new Rectangle(x * size, y * size, size, size)));
                            if (PlatformIndexes.Contains(num) == false)
                                PlatformIndexes.Add(num);
                        }
                       
                    }
                    //if(num == 1) //Background
                    //{
                    //    backgroundTiles.Add(new BackgroundTile(num, new Rectangle(x * size, y * size, size, size)));
                    //    if (BackgroundIndexes.Contains(num) == false)
                    //        BackgroundIndexes.Add(num);
                    //}
                    else if(num == 9 || num == 12)//Top of doors
                    {
                       // backgroundTiles.Add(new BackgroundTile(1, new Rectangle(x * size, y * size, size, size))); //Place background tile in the spot
                        topDoorTiles.Add(new TopDoorTile(num, new Rectangle(x * size, y * size, size, size)));
                    }
                    else if(num == 8 || num == 11)//Bottom doors
                    {
                        //backgroundTiles.Add(new BackgroundTile(1, new Rectangle(x * size, y * size, size, size)));
                        bottomDoorTiles.Add(new BottomDoorTile(num, new Rectangle(x * size, y * size, size, size)));
                    }
                    else if(num == 25) //Enemy spawns
                    {
                        enemySpawns.Add(new Vector2(x * size, y * size));
                        //backgroundTiles.Add(new BackgroundTile(1, new Rectangle(x * size, y * size, size, size)));
                    }
                    else if(num == 7)//Dungeon entrance
                    {
                        //backgroundTiles.Add(new BackgroundTile(1, new Rectangle(x * size, y * size, size, size)));
                        dungeonEntrances.Add(new DungeonEntrance(num, new Rectangle(x * size, y * size, size, size)));
                    }
                    else if(num == 24)//Player spawn
                    {
                        playerSpawns.Add(new Vector2(x * size, y * size));
                        //backgroundTiles.Add(new BackgroundTile(1, new Rectangle(x * size, y * size, size, size)));
                    }


                    count++;
                }
                count2++;
            }

            tileMap.SetDims(map.GetLength(1), map.GetLength(0));
            tileMap.SetMap(map);
            tileMap.SetWorldDims(map.GetLength(1) * 64, map.GetLength(0) * 64);

            //for(int i = 0; i < map.GetLength(1); i += 4)
            //{
            //    repeatBG.Add(new RepeatBackground(new Rectangle(i * 64, )))
            //}


        }

        public static Vector2 GetNumTilesOfGround(int row, int col) //Will be recieving tile landed on
        {
            int numTilesRight = 0; //Landed on 1 tile
            int numTilesLeft = 0;
            for(int i = col + 1; i < tileMap.mapWidth - 1; i++) //check if there is a tile to the right 
            {
                if (GroundIndexes.Contains(tileMap.getPoint(row, i)) || PlatformIndexes.Contains(tileMap.getPoint(row, i)))
                {
                    numTilesRight++;
                }
                else
                {
                    break;
                }

            }
            for (int i = col - 1; i >= 0; i--)//Check if there is a tile to the left
            {
                //if (tileMap.getPoint(row - 1, i) == 3 || tileMap.getPoint(row - 1, i) == 2 || tileMap.getPoint(row - 1, i) == 0)
                //{
                //    break;
                //}


                if (GroundIndexes.Contains(tileMap.getPoint(row, i)) || PlatformIndexes.Contains(tileMap.getPoint(row, i)))
                {
                    numTilesLeft++;
                }
                else
                {
                    break;
                }

            }
            return new Vector2(numTilesLeft, numTilesRight);
        }

        public static bool CanWalk(int row, int col, string dir)
        {
            switch(dir)
            {
                case "left":
                    if (tileMap.getPoint(row, col - 1) != 3 && tileMap.getPoint(row, col - 1) != 2 && tileMap.getPoint(row, col - 1) != 0)
                        return false;
                    else
                        return true;

                    //break;
                case "right":
                    if (tileMap.getPoint(row, col + 1) != 3 && tileMap.getPoint(row, col + 1) != 2 && tileMap.getPoint(row, col + 1) != 0)
                        return false;
                    else
                        return true;
                    //break;
            }
            return true;
        }
        //public void BuildAndSaveMap(int mapWidth, int mapHeight)

        public static int GetPoint(int row, int col)
        {
            return tileMap.getPoint(row, col);
        }

        public int getPoint(int row, int col)//Returns the index
        {
            if (row >= mapHeight)
                row = mapHeight - 1;
            if (col >= mapWidth)
                col = mapWidth - 1;
            return Map[row, col];
        }



        public static void Draw(SpriteBatch spriteBatch, ContentManager Content, SSCamera ssCamera, bool isFinal = false)
        {

            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.transform);


            //foreach(RepeatBackground tile in repeatBG)
            //{
            //    tile.Draw(spriteBatch);
            //}
            if (!isFinal)
            {
                spriteBatch.Draw(Content.Load<Texture2D>("SideScroll/MapTiles/BG1"),
                    new Rectangle(ssCamera.CameraBounds.X, ssCamera.CameraBounds.Y, ssCamera.CameraBounds.Width, 620), Color.White);
            }


            foreach (BackgroundTile tile in backgroundTiles)
            {
                tile.Draw(spriteBatch);
            }
            foreach (TopDoorTile tile in topDoorTiles)
            {
                tile.Draw(spriteBatch);
            }
            foreach (BottomDoorTile tile in bottomDoorTiles)
            {
                tile.Draw(spriteBatch);
            }
            foreach (WallTile tile in wallTiles)
            {
                tile.Draw(spriteBatch);
            }
            foreach (GroundTile tile in groundTiles)
            {
                tile.Draw(spriteBatch); 
            }
            foreach(PlatformTile tile in platformTiles)
            {
                tile.Draw(spriteBatch);
            }
            foreach (DungeonEntrance tile in dungeonEntrances)
            {
                tile.Draw(spriteBatch);
            }
          
           
            
            // spriteBatch.End();
        }

        public static void LoadMap(string filePath, bool isFinal = false)
        {
            if (System.IO.File.Exists(filePath))
            {
                string mapInfo = File.ReadAllText(filePath);
                int lenghtTilDims = 0;
                int StartWidthIndex = 0;
                int StartHeightIndex = 0;
                int width = 25;
                int height = 0;
                string tHeight = "";

                for (int k = 0; k < mapInfo.Length - 6; k++) //Lets run through the file :)
                {
                    if (mapInfo.Substring(k, 6).Contains("Width:"))//Search for Width: containing 6 characters
                    {
                        width = int.Parse(mapInfo.Substring(k + 6, (mapInfo.Length - (k + 6))));
                        StartWidthIndex = k;

                    }
                }
                //int subLength = mapInfo.Length - StartWidthIndex;
                for (int l = 0; l < mapInfo.Length; l++)
                {
                    if (mapInfo.Substring(l, 7).Contains("Height:"))//Search for Height: containing, 7 characters
                    {
                        int j = 7;
                        while (true)
                        {
                            tHeight += mapInfo.Substring(l + j, 1);
                            try
                            {
                                height = int.Parse(tHeight);
                                StartHeightIndex = l + j;
                                j++;
                            }
                            catch
                            {
                                break;
                            }

                        }

                        break;
                    }
                }
                //int width = int.Parse(mapInfo.Substring(mapInfo.Length -2, 2));
                //int height = int.Parse(mapInfo.Substring(mapInfo.Length - 4, 2));
                int[,] map = new int[height, width];
                int i = 0;
                int x = 0;
                int y = 0;
                int length = mapInfo.Length;

                while (i < length && mapInfo.Substring(i, 1).Contains("H") == false)
                {
                    if (mapInfo.Substring(i, 1) != ",")
                    {
                        int j = i + 1;
                        string num = mapInfo.Substring(i, 1);
                        i++;
                        while (mapInfo.Substring(j, 1) != ",")
                        {
                            num += mapInfo.Substring(j, 1);
                            i++;
                            j++;
                        }
                        map[y, x] = int.Parse(num);
                        x++;
                        if (x >= width)
                        {
                            x = 0;
                            y += 1;
                        }

                        if (y >= height)
                        {
                            break;
                        }
                    }
                    i++;
                }

                tileMap.SetDims(width, height);
                Generate(map, 64, isFinal);

            }
        }
    }    
}

