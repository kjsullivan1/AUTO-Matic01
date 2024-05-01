using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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

        private List<EnemySpawn> enemySpawns = new List<EnemySpawn>();
        public List<EnemySpawn> EnemySpawns
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

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public void Generate(List<int[,]> maps, List<int[,]> yMaps, List<int[,]> dMaps, int size, int screenWidth, int screenHeight, List<Vector2> xPoints, List<Vector2> yPoints, List<Vector2> diagPoints)
        {
            wallTiles.Clear();
            enemySpawns.Clear();
            floorTiles.Clear();
           
            xMapDims.Clear();
            yMapDims.Clear();
            dMapDims.Clear();
            wallCount = 0;
            floorCount = 0;
            enemyCount = 0;
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

                        if (num == 2)//Walls
                        {
                            wallTiles.Add(new WallTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            wallTiles[wallCount].index = num;
                            wallCount++;
                        }
                        if (num == 3) //enemy
                        {
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        if (num == 1) //Floors
                        {
                            floorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            floorTiles[floorCount].index = num;
                            floorCount++;
                        }
                        if (num == 4)//player 
                        {
                            //playerT.Add(new Playert(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
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

                        if (num == 2)
                        {
                            wallTiles.Add(new WallTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            wallTiles[wallCount].index = num;
                            wallCount++;
                        }
                        if (num == 3)//enemies
                        {
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        if (num == 1)
                        {
                            floorTiles.Add(new FloorTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            //floorTiles[floorCount].mapPoint = new int[] { y, x };
                            floorTiles[floorCount].index = num;
                            floorCount++;
                        }
                        if (num == 4)//player
                        {
                            //playerT.Add(new Playert(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                        }

                        width = (x + 1) * size;
                        height = (y + 1) * size;
                    }
                }
                //width = yMaps[i].GetLength(1);
                //height = yMapDims[i].GetLength(0);
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

                        if (num == 2)
                        {
                            wallTiles.Add(new WallTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            //wallTiles[wallCount].mapPoint = new int[] { y, x };
                            wallTiles[wallCount].index = num;
                            wallCount++;
                        }
                        if (num == 3)//enemy
                        {
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        if (num == 1)
                        {
                            floorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            //floorTiles[floorCount].mapPoint = new int[] { y, x };
                            floorTiles[floorCount].index = num;
                            floorCount++;
                        }
                        if (num == 4)//player
                        {
                            //playerT.Add(new Playert(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                        }

                        width = (x + 1) * size;
                        height = (y + 1) * size;
                    }
                }
                //width = dMaps[i].GetLength(1);
                //height = dMaps[i].GetLength(0);
            }
        }

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

                        if (num == 2)
                        {
                            wallTiles.Add(new WallTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                           // wallTiles[wallCount].mapPoint = new int[] { y, x };
                            wallTiles[wallCount].index = num;
                            wallCount++;
                        }
                        if (num == 3) //enemy
                        {
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        if (num == 1)
                        {
                            floorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //floorTiles[floorCount].mapPoint = new int[] { y, x };
                            floorTiles[floorCount].index = num;
                            floorCount++;
                        }
                        if (num == 4)//player
                        {
                            //playerT.Add(new Playert(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
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

                        if (num == 2)
                        {
                            wallTiles.Add(new WallTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            //wallTiles[wallCount].mapPoint = new int[] { y, x };
                            wallTiles[wallCount].index = num;
                            wallCount++;
                        }
                        if (num == 3)//enemy
                        {
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        if (num == 1)
                        {
                            floorTiles.Add(new FloorTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            //floorTiles[floorCount].mapPoint = new int[] { y, x };
                            floorTiles[floorCount].index = num;
                            floorCount++;
                        }
                        if (num == 4)//player
                        {
                            //playerT.Add(new Playert(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
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

                        if (num == 2)
                        {
                            wallTiles.Add(new WallTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            //wallTiles[wallCount].mapPoint = new int[] { y, x };
                            wallTiles[wallCount].index = num;
                            wallCount++;
                        }
                        if (num == 3)//enemy
                        {
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        if (num == 1)
                        {
                            floorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            //floorTiles[floorCount].mapPoint = new int[] { y, x };
                            floorTiles[floorCount].index = num;
                            floorCount++;
                        }
                        if (num == 4)//player
                        {
                            //playerT.Add(new Playert(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                        }

                        width = (x + 1) * size;
                        height = (y + 1) * size;
                    }
                }
                //width = dMaps[i].GetLength(1);
                //height = dMaps[i].GetLength(0);
            }

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
