﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_Matic
{
    class SideTileMap
    {
        public static int pixelSize = 64;
        public int[,] Map;
        public int mapWidth;
        public int mapHeight;
        public static SideTileMap tileMap = new SideTileMap();
        private static List<GroundTile> groundTiles = new List<GroundTile>();
        private static List<PlatformTile> platformTiles = new List<PlatformTile>();
        private static List<BackgroundTile> backgroundTiles = new List<BackgroundTile>();
        private static List<TopDoorTile> topDoorTiles = new List<TopDoorTile>();
        private static List<BottomDoorTile> bottomDoorTiles = new List<BottomDoorTile>();
        public static List<Vector2> enemySpawns = new List<Vector2>();
        public static List<Vector2> playerSpawns = new List<Vector2>();
        private static List<DungeonEntrance> dungeonEntrances = new List<DungeonEntrance>();
        public static List<int> GroundIndexes = new List<int>();
        public static List<int> BackgroundIndexes = new List<int>();
        public static List<int> PlatformIndexes = new List<int>();

        //public static List<EmptyTile> EmptyTiles
        //{
        //    get { return emptyTiles; }
        //}

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

        public static void Generate(int[,] map, int size)
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
          
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    int num = map[y, x];

                    if (num == 2 || num == 5 || num == 6 || num == 13 || num == 14 || num == 15 || num == 16 || num == 17 || num == 18 || num == 19 || num == 20|| num ==21 || num ==22 || num ==23)
                    {
                        groundTiles.Add(new GroundTile(num, new Rectangle(x * size, y * size, size, size)));
                        if (GroundIndexes.Contains(num) == false)
                            GroundIndexes.Add(num);

                    }
                    if (num == 3 || num == 4)
                    {
                   
                        platformTiles.Add(new PlatformTile(num, new Rectangle(x * size, y * size, size, size)));
                        if (PlatformIndexes.Contains(num) == false)
                            PlatformIndexes.Add(num);
                    }
                    if(num == 1)
                    {
                        backgroundTiles.Add(new BackgroundTile(num, new Rectangle(x * size, y * size, size, size)));
                        if (BackgroundIndexes.Contains(num) == false)
                            BackgroundIndexes.Add(num);
                    }
                    if(num == 9 || num == 12)
                    {
                        backgroundTiles.Add(new BackgroundTile(1, new Rectangle(x * size, y * size, size, size)));
                        topDoorTiles.Add(new TopDoorTile(num, new Rectangle(x * size, y * size, size, size)));
                    }
                    if(num == 8 || num == 11)
                    {
                        backgroundTiles.Add(new BackgroundTile(1, new Rectangle(x * size, y * size, size, size)));
                        bottomDoorTiles.Add(new BottomDoorTile(num, new Rectangle(x * size, y * size, size, size)));
                    }
                    if(num == 25)
                    {
                        enemySpawns.Add(new Vector2(x * size, y * size));
                        backgroundTiles.Add(new BackgroundTile(1, new Rectangle(x * size, y * size, size, size)));
                    }
                    if(num == 7 || num == 8)
                    {
                        backgroundTiles.Add(new BackgroundTile(1, new Rectangle(x * size, y * size, size, size)));
                        dungeonEntrances.Add(new DungeonEntrance(num, new Rectangle(x * size, y * size, size, size)));
                    }
                    if(num == 24)
                    {
                        playerSpawns.Add(new Vector2(x * size, y * size));
                        backgroundTiles.Add(new BackgroundTile(1, new Rectangle(x * size, y * size, size, size)));
                    }
                }
            }

            tileMap.SetDims(map.GetLength(1), map.GetLength(0));
            tileMap.SetMap(map);


        }

        public static Vector2 GetNumTilesOfGround(int row, int col) //Will be recieving tile landed on
        {
            int numTilesRight = 0; //Landed on 1 tile
            int numTilesLeft = 0;
            for(int i = col + 1; i < tileMap.mapWidth - 1; i++)
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
            for (int i = col - 1; i >= 0; i--)
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

        public int getPoint(int row, int col)
        {
            if (row >= mapHeight)
                row = mapHeight - 1;
            if (col >= mapWidth)
                col = mapWidth - 1;
            return Map[row, col];
        }



        public static void Draw(SpriteBatch spriteBatch)
        {

            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.transform);

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

        public static void LoadMap(string filePath)
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
                Generate(map, 64);

            }
        }
    }    
}
