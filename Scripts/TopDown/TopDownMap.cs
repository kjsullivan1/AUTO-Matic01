using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_Matic.TopDown
{
    class TopDownMap
    {
        private List<WallTiles> wallTiles = new List<WallTiles>();
        public List<WallTiles> WallTiles
        {
            get { return wallTiles; }
        }

        //public class player to create the player 

        private List<Vector2> enemySpawns = new List<Vector2>();
        public List<Vector2> EnemySpawns
        {
            get { return enemySpawns; }
        }

        private List<FloorTiles> floorTiles = new List<FloorTiles>();
        public List<FloorTiles> FloorTiles
        {
            get { return floorTiles; }
        }

        private int width, height;
        public List<int[,]> xMapDims = new List<int[,]>();
        public List<int[,]> yMapDims = new List<int[,]>();
        public List<int[,]> dMapDims = new List<int[,]>();
        int wallCount = 0;
        int enemyCount = 0;
        int floorCount = 0;
        public List<int> rows = new List<int>();
        public List<int> cols = new List<int>();
        public List<int> WallIndexes = new List<int>();
        public List<int> FloorIndexes = new List<int>();
        public List<int> EnemyIndexes = new List<int>();
        public Rectangle ExitDoor = new Rectangle();

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        //public void Generate(List<int[,]> maps, List<int[,]> yMaps, List<int[,]> dMaps, int size, int screenWidth, int screenHeight, List<Vector2> xPoints, List<Vector2> yPoints, List<Vector2> diagPoints)
        //{
        //    wallTiles.Clear();
        //    enemySpawns.Clear();
        //    floorTiles.Clear();
           
        //    xMapDims.Clear();
        //    yMapDims.Clear();
        //    dMapDims.Clear();
        //    WallIndexes.Clear();
        //    wallCount = 0;
        //    floorCount = 0;
        //    enemyCount = 0;
        //    //Levels created horizontally

        //    for (int i = 0; i < maps.Count; i++)
        //    {
        //        xMapDims.Add(maps[i]);
        //    }
        //    for (int i = 0; i < yMaps.Count; i++)
        //    {
        //        yMapDims.Add(yMaps[i]);
        //    }

        //    for (int i = 0; i < dMaps.Count; i++)
        //    {
        //        dMapDims.Add(dMaps[i]);
        //    }

        //    for (int i = 0; i < maps.Count; i++)
        //    {
        //        rows.Add(maps[i].GetLength(0));
        //        cols.Add(maps[i].GetLength(1));
        //        int levelInX = (int)xPoints[i].X;
        //        for (int y = 0; y < maps[i].GetLength(0); y++)
        //        {
        //            for (int x = 0; x < maps[i].GetLength(1); x++)
        //            {
        //                int num = maps[i][y, x];

        //                if (num == 10 || num == 1 || num == 2 || num == 3|| num == 4 || num == 5 || num == 6 || num == 7 || num == 8 || num == 10)//Walls
        //                {
        //                    wallTiles.Add(new WallTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
        //                    wallTiles[wallCount].mapPoint = new int[] { y,x};
        //                    wallCount++;
        //                    if(WallIndexes.Contains(num) == false)
        //                    {
        //                        WallIndexes.Add(num);
        //                    }
        //                }
        //                if (num == 11) //enemy
        //                {
        //                    if(enemySpawns.Contains(new Vector2((levelInX * screenWidth) + (x * size), y * size)) == false)
        //                    {
        //                        enemySpawns.Add(new Vector2((levelInX * screenWidth) + (x * size), y * size));
        //                    }
                           
        //                    floorTiles.Add(new FloorTiles(9, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
        //                    floorTiles[floorCount].mapPoint = new int[] { y, x };
        //                    floorCount++;
        //                    if (EnemyIndexes.Contains(num) == false)
        //                        EnemyIndexes.Add(num);
        //                    //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
        //                    //skullTiles[skullCount].mapPoint = new int[] { y, x };
        //                    //skullCount++;
        //                }
        //                if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15) //Floors
        //                {
        //                    floorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
        //                    floorTiles[floorCount].mapPoint = new int[] {y, x};
        //                    floorCount++;
        //                    if (FloorIndexes.Contains(num) == false)
        //                        FloorIndexes.Add(num);
        //                }
        //                //if (num == 4)//player 
        //                //{
        //                //    //playerT.Add(new Playert(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
        //                //}
        //                width = (x + 1) * size;
        //                height = (y + 1) * size;

        //            }
        //        }
        //        //width = maps[i].GetLength(1);
        //        //height = maps[i].GetLength(0);
        //    }

        //    for (int i = 1; i < yMaps.Count + 1; i++)
        //    {
        //        rows.Add(yMaps[i - 1].GetLength(0));
        //        cols.Add(yMaps[i - 1].GetLength(1));

        //        int levelInY = (int)yPoints[i].Y;
        //        for (int x = 0; x < cols[i - 1]; x++)
        //        {
        //            for (int y = 0; y < yMaps[i - 1].GetLength(0); y++)
        //            {
        //                int num = yMaps[i - 1][y, x];

        //                if (num == 10 || num == 1 || num == 2 || num == 3 || num == 4 || num == 5 || num == 6 || num == 7 || num == 8 || num == 10)//Walls
        //                {
        //                    wallTiles.Add(new WallTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
        //                    wallTiles[wallCount].mapPoint = new[] { y, x };
        //                    //wallTiles[wallCount].mapPoint = num;
        //                    wallCount++;
        //                    if (WallIndexes.Contains(num) == false)
        //                    {
        //                        WallIndexes.Add(num);
        //                    }
        //                }
        //                if (num == 11) //enemy
        //                {
        //                    if(enemySpawns.Contains(new Vector2(x * size, (y * size) - (levelInY * screenHeight))) == false)
        //                    {
        //                        enemySpawns.Add(new Vector2(x * size, (y * size) - (levelInY * screenHeight)));
        //                    }
                           
        //                    floorTiles.Add(new FloorTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
        //                    floorTiles[floorCount].mapPoint = new int[] { y, x };
        //                    //floorTiles[floorCount].mapPoint = num;
        //                    floorCount++;
        //                    if (EnemyIndexes.Contains(num) == false)
        //                        EnemyIndexes.Add(num);
        //                    //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
        //                    //skullTiles[skullCount].mapPoint = new int[] { y, x };
        //                    //skullCount++;
        //                }
        //                if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15) //Floors
        //                {
        //                    floorTiles.Add(new FloorTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
        //                    floorTiles[floorCount].mapPoint = new int[] { y, x };
        //                    //floorTiles[floorCount].mapPoint = num;
        //                    floorCount++;
        //                    if (FloorIndexes.Contains(num) == false)
        //                        FloorIndexes.Add(num);
        //                }

        //                width = (x + 1) * size;
        //                height = (y + 1) * size;
        //            }
        //        }
        //        //width = yMaps[i].GetLength(1);
        //        //height = yMapDims[i].GetLength(0);
        //    }


        //    for (int i = 0; i < dMaps.Count; i++)
        //    {
        //        rows.Add(dMaps[i].GetLength(0));
        //        cols.Add(dMaps[i].GetLength(1));

        //        for (int x = 0; x < dMaps[i].GetLength(1); x++)
        //        {
        //            for (int y = 0; y < dMaps[i].GetLength(0); y++)
        //            {
        //                int num = dMaps[i][y, x];
        //                int levelInX = (int)diagPoints[i].X;
        //                int levelInY = (int)diagPoints[i].Y;

        //                if (num == 10 || num == 1 || num == 2 || num == 3 || num == 4 || num == 5 || num == 6 || num == 7 || num == 8 || num == 10)//Walls
        //                {
        //                    wallTiles.Add(new WallTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
        //                    wallTiles[wallCount].mapPoint = new int[] { y, x };
        //                    //wallTiles[wallCount].mapPoint = num;
        //                    wallCount++;
        //                    if (WallIndexes.Contains(num) == false)
        //                    {
        //                        WallIndexes.Add(num);
        //                    }
        //                }
        //                if (num == 11) //enemy
        //                {
        //                    if(enemySpawns.Contains(new Vector2((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight))) == false)
        //                    {
        //                        enemySpawns.Add(new Vector2((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight)));
        //                    }
                          
        //                    floorTiles.Add(new FloorTiles(9, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
        //                    floorTiles[floorCount].mapPoint = new int[] { y, x };
        //                    //floorTiles[floorCount].mapPoint = num;
        //                    floorCount++;
        //                    if (EnemyIndexes.Contains(num) == false)
        //                        EnemyIndexes.Add(num);
        //                    //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
        //                    //skullTiles[skullCount].mapPoint = new int[] { y, x };
        //                    //skullCount++;
        //                }
        //                if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15) //Floors
        //                {
        //                    floorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
        //                    floorTiles[floorCount].mapPoint = new int[] { y, x };
        //                    //floorTiles[floorCount].mapPoint = num;
        //                    floorCount++;
        //                    if (FloorIndexes.Contains(num) == false)
        //                        FloorIndexes.Add(num);
        //                }

        //                width = (x + 1) * size;
        //                height = (y + 1) * size;
        //            }
        //        }
        //        //width = dMaps[i].GetLength(1);
        //        //height = dMaps[i].GetLength(0);
        //    }
        //}

        public void Refresh(List<int[,]> maps, List<int[,]> yMaps, List<int[,]> dMaps, int size, int screenWidth, int screenHeight, List<Vector2> xPoints, List<Vector2> yPoints, List<Vector2> diagPoints)
        {
            wallTiles.Clear();
            //enemySpawns.Clear();
            floorTiles.Clear();
            //playerT.Clear();
            xMapDims.Clear();
            yMapDims.Clear();
            dMapDims.Clear();
            wallCount = 0;
            floorCount = 0;
            //skullCount = 0;
            //Levels created horizontally


            for (int i = 0; i < maps.Count; i++)
            {
                xMapDims.Add(maps[i]);
            }
            for (int i = 0; i < yMaps.Count; i++)
            {
                yMapDims.Add(yMaps[i]);
            }

            for (int i = 0; i < dMaps.Count; i++)
            {
                dMapDims.Add(dMaps[i]);
            }

            for (int i = 0; i < maps.Count; i++)
            {





                rows.Add(maps[i].GetLength(0));
                cols.Add(maps[i].GetLength(1));
                int levelInX = (int)xPoints[i].X;
                for (int y = 0; y < maps[i].GetLength(0); y++)
                {
                    for (int x = 0; x < maps[i].GetLength(1); x++)
                    {
                        int num = maps[i][y, x];

                        if (num == 10 || num == 1 || num == 2 || num == 3 || num == 4 || num == 5 || num == 6 || num == 7 || num == 8 || num == 10)//Walls
                        {
                            wallTiles.Add(new WallTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            wallTiles[wallCount].mapPoint = new[] { y, x };
                            wallCount++;
                            if (WallIndexes.Contains(num) == false)
                            {
                                WallIndexes.Add(num);
                            }
                        }
                        if (num == 11) //enemy
                        {
                            if(enemySpawns.Contains(new Vector2((levelInX * screenWidth) + (x * size), y * size)) == false)
                            {
                                enemySpawns.Add(new Vector2((levelInX * screenWidth) + (x * size), y * size));
                            }
                         
                            floorTiles.Add(new FloorTiles(9, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            floorTiles[floorCount].mapPoint = new int[] {y, x};
                            floorCount++;
                            if (EnemyIndexes.Contains(num) == false)
                                EnemyIndexes.Add(num);
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15) //Floors
                        {
                            floorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            floorTiles[floorCount].mapPoint = new int[] { y, x };
                            floorCount++;
                            if (FloorIndexes.Contains(num) == false)
                                FloorIndexes.Add(num);

                            if(num == 12)
                            {
                                ExitDoor = new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size);
                            }
                        }

                        width = (x + 1) * size;
                        height = (y + 1) * size;
                    }
                }
                //width = maps[i].GetLength(1);
                //height = maps[i].GetLength(0);
            }

            for (int i = 1; i < yMaps.Count + 1; i++)
            {
                rows.Add(yMaps[i - 1].GetLength(0));
                cols.Add(yMaps[i - 1].GetLength(1));

                int levelInY = (int)yPoints[i].Y;
                for (int x = 0; x < cols[i - 1]; x++)
                {
                    for (int y = 0; y < yMaps[i - 1].GetLength(0); y++)
                    {
                        int num = yMaps[i - 1][y, x];

                        if (num == 10 || num == 1 || num == 2 || num == 3 || num == 4 || num == 5 || num == 6 || num == 7 || num == 8 || num == 10)//Walls
                        {
                            wallTiles.Add(new WallTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            wallTiles[wallCount].mapPoint = new[] { y, x };
                            //wallTiles[wallCount].mapPoint = num;
                            wallCount++;
                            if (WallIndexes.Contains(num) == false)
                            {
                                WallIndexes.Add(num);
                            }
                        }
                        if (num == 11) //enemy
                        {
                            if(enemySpawns.Contains(new Vector2(x * size, (y * size) - (levelInY * screenHeight))) == false)
                            {
                                enemySpawns.Add(new Vector2(x * size, (y * size) - (levelInY * screenHeight)));
                            }
                           
                            floorTiles.Add(new FloorTiles(9, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            floorTiles[floorCount].mapPoint = new int[] { y, x };
                            //floorTiles[floorCount].mapPoint = num;
                            floorCount++;
                            if (EnemyIndexes.Contains(num) == false)
                                EnemyIndexes.Add(num);
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15) //Floors
                        {
                            floorTiles.Add(new FloorTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            floorTiles[floorCount].mapPoint = new int[] { y, x };
                            //floorTiles[floorCount].mapPoint = num;
                            floorCount++;
                            if (FloorIndexes.Contains(num) == false)
                                FloorIndexes.Add(num);

                            if(num == 12)
                            {
                                ExitDoor = new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size);
                            }
                        }

                        width = (x + 1) * size;
                        height = (y + 1) * size;
                    }
                }
                //width = yMaps[i].GetLength(1);
                //height = yMaps[i].GetLength(0);
            }


            for (int i = 0; i < dMaps.Count; i++)
            {
                rows.Add(dMaps[i].GetLength(0));
                cols.Add(dMaps[i].GetLength(1));

                for (int x = 0; x < dMaps[i].GetLength(1); x++)
                {
                    for (int y = 0; y < dMaps[i].GetLength(0); y++)
                    {
                        int num = dMaps[i][y, x];
                        int levelInX = (int)diagPoints[i].X;
                        int levelInY = (int)diagPoints[i].Y;

                        if (num == 10 || num == 1 || num == 2 || num == 3 || num == 4 || num == 5 || num == 6 || num == 7 || num == 8 || num == 10)//Walls
                        {
                            wallTiles.Add(new WallTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            wallTiles[wallCount].mapPoint = new int[] { y, x };
                            //wallTiles[wallCount].mapPoint = num;
                            wallCount++;
                            if (WallIndexes.Contains(num) == false)
                            {
                                WallIndexes.Add(num);
                            }
                        }
                        if (num == 11) //enemy
                        {
                            if(enemySpawns.Contains(new Vector2((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight))) == false)
                            {
                                enemySpawns.Add(new Vector2((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight)));
                            }
                           
                            floorTiles.Add(new FloorTiles(9, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            floorTiles[floorCount].mapPoint = new int[] { y, x };
                            //floorTiles[floorCount].mapPoint = num;
                            floorCount++;
                            if (EnemyIndexes.Contains(num) == false)
                                EnemyIndexes.Add(num);
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15) //Floors
                        {
                            floorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            floorTiles[floorCount].mapPoint = new int[] { y, x };
                            //floorTiles[floorCount].mapPoint = num;
                            floorCount++;
                            if (FloorIndexes.Contains(num) == false)
                                FloorIndexes.Add(num);

                            ExitDoor = new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size);
                        }

                        width = (x + 1) * size;
                        height = (y + 1) * size;
                    }
                }
                //width = dMaps[i].GetLength(1);
                //height = dMaps[i].GetLength(0);
            }

        }


        public int[,] GenerateMap(string filePath)
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

                return map;

            }
            return new int[15, 25];
        }

        public int GetPoint(int row, int col, int[,] mapDims)
        {
            if (row >= 15)
                row = 14;
            if (col >= 25)
                col = 24;
            return mapDims[row, col];
        }


        public int GetPointLevelX(int row, int col, int lvlNum)
        {
            return xMapDims[lvlNum][row, col];
        }

        public int GetPointLevelY(int row, int col, int lvlNum)
        {
            return yMapDims[lvlNum - 1][row, col];
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (WallTiles tile in wallTiles)
            {
                tile.Draw(spriteBatch);
            }
            //foreach (SkullTiles tile in skullTiles)
            //{
            //    tile.Draw(spriteBatch);
            //}
            foreach (FloorTiles tile in floorTiles)
            {
                tile.Draw(spriteBatch);
            }
        }
    }
}
