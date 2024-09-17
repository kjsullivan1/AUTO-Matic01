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

        private List<FloorTiles> doorTiles = new List<FloorTiles>();
        public List<FloorTiles> DoorTiles
        {
            get { return doorTiles; }
        }
        private List<SlamTiles> slamTiles = new List<SlamTiles>();
        public List<SlamTiles> SlamTiles
        {
            get { return slamTiles; }
        }

        private List<EnvironmentTile> environmentTiles = new List<EnvironmentTile>();
        public List<EnvironmentTile> EnvironmentTiles
        {
            get { return environmentTiles; }
        }
        public void AddEnvironmentTile(EnvironmentTile tile)
        {
            environmentTiles.Add(tile);
        }

        
        private int width, height;
        public List<int[,]> xMapDims = new List<int[,]>();
        public List<int[,]> yMapDims = new List<int[,]>();
        public List<int[,]> dMapDims = new List<int[,]>();
        public List<EnemySpawn> enemySpawnPoints = new List<EnemySpawn>();
        int wallCount = 0;
        int enemyCount = 0;
        int floorCount = 0;
        public List<int> rows = new List<int>();
        public List<int> cols = new List<int>();
        public List<int> WallIndexes = new List<int>();
        public List<int> FloorIndexes = new List<int>();
        public List<int> EnemyIndexes = new List<int>();
        public List<int> SlamIndexes = new List<int>();
        public Rectangle ExitDoor = new Rectangle();

        public int levelInX = 0;
        public int levelInY = 0;
        public Vector2 ScreenSize = Vector2.Zero;

        public bool setBarrier = true;
        bool refresh = false;
        public bool publicRefresh = false;

        Rectangle currBounds = Rectangle.Empty;
        bool isRight = false, isLeft = false, isUp = false, isDown = false;

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

       

        public void Refresh(List<int[,]> maps, List<int[,]> yMaps, List<int[,]> dMaps, int size, 
            int screenWidth, int screenHeight, List<Vector2> xPoints, List<Vector2> yPoints, List<Vector2> diagPoints, int numLevel)
        {
            wallTiles.Clear();
            //enemySpawns.Clear();
            enemySpawnPoints.Clear();
            floorTiles.Clear();
            //playerT.Clear();
            xMapDims.Clear();
            yMapDims.Clear();
            dMapDims.Clear();
            doorTiles.Clear();
            wallCount = 0;
            floorCount = 0;
            //skullCount = 0;
            //Levels created horizontally
            ScreenSize = new Vector2(screenWidth, screenHeight);

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

            switch(numLevel)
            {
                case 0:
                    BuildDungeon0(maps, yMaps, dMaps, size, screenWidth, screenHeight, xPoints, yPoints, diagPoints);
                    break;
                case 1:
                    BuildDungeon1(maps, yMaps, dMaps, size, screenWidth, screenHeight, xPoints, yPoints, diagPoints);
                    break;
                case 2:
                    BuildDungeon2(maps, yMaps, dMaps, size, screenWidth, screenHeight, xPoints, yPoints, diagPoints);
                    break;
                case 3:
                    BuildDungeon3(maps, yMaps, dMaps, size, screenWidth, screenHeight, xPoints, yPoints, diagPoints);
                    break;


            }
            if (setBarrier)
                SetBarrier();
            if (publicRefresh == false && currBounds != Rectangle.Empty)
                SetBarrierSide();
            //else if (setBarrier == false)
            //    setBarrier = true;
           

        }

        private void BuildDungeon0(List<int[,]> maps, List<int[,]> yMaps, List<int[,]> dMaps, int size, int screenWidth, int screenHeight, List<Vector2> xPoints, List<Vector2> yPoints, List<Vector2> diagPoints)
        {
            #region NonDiagMaps
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
                            if (enemySpawns.Contains(new Vector2((levelInX * screenWidth) + (x * size), y * size)) == false)
                            {
                                enemySpawns.Add(new Vector2((levelInX * screenWidth) + (x * size), y * size));
                            }

                            floorTiles.Add(new FloorTiles(9, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                            enemySpawnPoints.Add(new EnemySpawn(new int[] { y, x }, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            floorCount++;
                            if (EnemyIndexes.Contains(num) == false)
                                EnemyIndexes.Add(num);
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15 || num == 16 || num == 17 || num == 18 || num == 19) //Floors
                        {
                            floorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                            floorCount++;
                            if (FloorIndexes.Contains(num) == false)
                                FloorIndexes.Add(num);

                            if (num == 12)
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

            for (int i = 0; i < yMaps.Count; i++)
            {
                rows.Add(yMaps[i].GetLength(0));
                cols.Add(yMaps[i].GetLength(1));

                int levelInY = (int)yPoints[i].Y;
                for (int x = 0; x < cols[i]; x++)
                {
                    for (int y = 0; y < yMaps[i].GetLength(0); y++)
                    {
                        int num = yMaps[i][y, x];

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
                            if (enemySpawns.Contains(new Vector2(x * size, (y * size) - (levelInY * screenHeight))) == false)
                            {
                                enemySpawns.Add(new Vector2(x * size, (y * size) - (levelInY * screenHeight)));
                            }

                            floorTiles.Add(new FloorTiles(9, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                            enemySpawnPoints.Add(new EnemySpawn(new int[] { y, x }, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            //floorTiles[floorTiles.Count - 1].mapPoint = num;
                            floorCount++;
                            if (EnemyIndexes.Contains(num) == false)
                                EnemyIndexes.Add(num);
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15 || num == 16 || num == 17 || num == 18 || num == 19) //Floors
                        {
                            floorTiles.Add(new FloorTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                            //floorTiles[floorTiles.Count - 1].mapPoint = num;
                            floorCount++;
                            if (FloorIndexes.Contains(num) == false)
                                FloorIndexes.Add(num);

                            if (num == 12)
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

            #endregion
            for (int i = 0; i < dMaps.Count; i++)
            {
                rows.Add(dMaps[i].GetLength(0));
                cols.Add(dMaps[i].GetLength(1));

                for (int y = 0; y < dMaps[i].GetLength(0); y++)
                {
                    for (int x = 0; x < dMaps[i].GetLength(1); x++)
                    {
                        int num = dMaps[i][y, x];
                        levelInX = (int)diagPoints[i].X;
                        levelInY = (int)diagPoints[i].Y;


                        if (num == 0)
                        {
                            slamTiles.Add(new TopDown.SlamTiles(9, new Rectangle((levelInX * screenWidth) + (x * size),
                                (y * size) - (levelInY * screenHeight), size, size)));
                            slamTiles[slamTiles.Count - 1].mapPoint = new int[] { y, x };
                            if (SlamIndexes.Contains(num) == false)
                                SlamIndexes.Add(num);
                        }

                        else if (num == 10 || num == 1 || num == 2 || num == 3 || num == 4 || num == 5 || num == 6 || num == 7 || num == 8 || num == 10)//Walls
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
                        else if (num == 11) //enemy
                        {
                            if (enemySpawns.Contains(new Vector2((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight))) == false)
                            {
                                enemySpawns.Add(new Vector2((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight)));
                                enemySpawnPoints.Add(new EnemySpawn(new int[] { y, x }, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            }

                            floorTiles.Add(new FloorTiles(9, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };

                            //floorTiles[floorTiles.Count - 1].mapPoint = num;
                            floorCount++;
                            if (EnemyIndexes.Contains(num) == false)
                                EnemyIndexes.Add(num);
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        else if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15 || num == 16 || num == 17 || num == 18 || num == 19) //Floors
                        {
                            floorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                            //floorTiles[floorTiles.Count - 1].mapPoint = num;
                            floorCount++;
                            if (FloorIndexes.Contains(num) == false)
                                FloorIndexes.Add(num);

                            switch (num)
                            {
                                case 9:
                                    floorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    if (FloorIndexes.Contains(num) == false)
                                        FloorIndexes.Add(num);
                                    floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;
                                default:
                                    doorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    doorTiles[doorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;

                            }

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
        private void BuildDungeon1(List<int[,]> maps, List<int[,]> yMaps, List<int[,]> dMaps, int size, int screenWidth, int screenHeight, List<Vector2> xPoints, List<Vector2> yPoints, List<Vector2> diagPoints)
        {
            #region NonDiagMaps
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

                        if (num == 10 || num == 1 || num == 2 || num == 3 || num == 4 || num == 5 || num == 6 || num == 7 || num == 8)//Walls
                        {
                            //wallTiles.Add(new WallTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));

                            switch(num)
                            {
                                case 10:
                                    wallTiles.Add(new WallTiles(28, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 1:
                                    wallTiles.Add(new WallTiles(21, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 2:
                                    wallTiles.Add(new WallTiles(29, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 3:
                                    wallTiles.Add(new WallTiles(20, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 4:
                                    wallTiles.Add(new WallTiles(22, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 5:
                                    wallTiles.Add(new WallTiles(23, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 6:
                                    wallTiles.Add(new WallTiles(24, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 7:
                                    wallTiles.Add(new WallTiles(25, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 8:
                                    wallTiles.Add(new WallTiles(26, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                default:
                                    wallTiles.Add(new WallTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;  

                            }

                            wallTiles[wallCount].mapPoint = new[] { y, x };
                            wallCount++;
                            if (WallIndexes.Contains(num) == false)
                            {
                                WallIndexes.Add(num);
                            }
                        }
                        if (num == 11) //enemy
                        {
                            if (enemySpawns.Contains(new Vector2((levelInX * screenWidth) + (x * size), y * size)) == false)
                            {
                                enemySpawns.Add(new Vector2((levelInX * screenWidth) + (x * size), y * size));
                            }

                            floorTiles.Add(new FloorTiles(27, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                            enemySpawnPoints.Add(new EnemySpawn(new int[] { y, x }, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            floorCount++;
                            if (EnemyIndexes.Contains(num) == false)
                                EnemyIndexes.Add(num);
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15 || num == 16 || num == 17 || num == 18 || num == 19) //Floors
                        {
                            switch(num)
                            {
                                case 9:
                                    floorTiles.Add(new FloorTiles(27, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    if (FloorIndexes.Contains(num) == false)
                                        FloorIndexes.Add(num);
                                    floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;
                                default:
                                    doorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    doorTiles[doorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;

                            }
                            //floorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                          
                            floorCount++;
       

                            if (num == 12)
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

            for (int i = 0; i < yMaps.Count; i++)
            {
                rows.Add(yMaps[i].GetLength(0));
                cols.Add(yMaps[i].GetLength(1));

                int levelInY = (int)yPoints[i].Y;
                for (int x = 0; x < cols[i]; x++)
                {
                    for (int y = 0; y < yMaps[i].GetLength(0); y++)
                    {
                        int num = yMaps[i][y, x];

                        if (num == 10 || num == 1 || num == 2 || num == 3 || num == 4 || num == 5 || num == 6 || num == 7 || num == 8 || num == 10)//Walls
                        {
                            //wallTiles.Add(new WallTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            switch (num)
                            {
                                case 10:
                                    wallTiles.Add(new WallTiles(28, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 1:
                                    wallTiles.Add(new WallTiles(21, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 2:
                                    wallTiles.Add(new WallTiles(29, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 3:
                                    wallTiles.Add(new WallTiles(20, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 4:
                                    wallTiles.Add(new WallTiles(22, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 5:
                                    wallTiles.Add(new WallTiles(23, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 6:
                                    wallTiles.Add(new WallTiles(24, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 7:
                                    wallTiles.Add(new WallTiles(25, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 8:
                                    wallTiles.Add(new WallTiles(26, new Rectangle( (x * size), (y * size), size, size)));
                                    break;
                                default:
                                    wallTiles.Add(new WallTiles(num, new Rectangle( (x * size), (y * size), size, size)));
                                    break;

                            }
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
                            if (enemySpawns.Contains(new Vector2(x * size, (y * size) - (levelInY * screenHeight))) == false)
                            {
                                enemySpawns.Add(new Vector2(x * size, (y * size) - (levelInY * screenHeight)));
                            }

                            floorTiles.Add(new FloorTiles(27, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                            enemySpawnPoints.Add(new EnemySpawn(new int[] { y, x }, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            //floorTiles[floorTiles.Count - 1].mapPoint = num;
                            floorCount++;
                            if (EnemyIndexes.Contains(num) == false)
                                EnemyIndexes.Add(num);
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15 || num == 16 || num == 17 || num == 18 || num == 19
                            || num == 20 || num == 21 || num == 22 || num == 23) //Floors
                        {
                            switch (num)
                            {
                                case 9:
                                    floorTiles.Add(new FloorTiles(27, new Rectangle( (x * size), (y * size), size, size)));
                                    if (FloorIndexes.Contains(num) == false)
                                        FloorIndexes.Add(num);

                                    floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;
                                default:
                                    doorTiles.Add(new FloorTiles(num, new Rectangle((x * size), (y * size), size, size)));
                                    doorTiles[doorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;

                            }
                            //floorTiles.Add(new FloorTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                          
                            //floorTiles[floorTiles.Count - 1].mapPoint = num;
                            floorCount++;
                           

                            if (num == 12)
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

            #endregion
            for (int i = 0; i < dMaps.Count; i++)
            {
                rows.Add(dMaps[i].GetLength(0));
                cols.Add(dMaps[i].GetLength(1));

                for (int y = 0; y < dMaps[i].GetLength(0); y++)
                {
                    for (int x = 0; x < dMaps[i].GetLength(1); x++)
                    {
                        int num = dMaps[i][y, x];
                        levelInX = (int)diagPoints[i].X;
                        levelInY = (int)diagPoints[i].Y;


                        if (num == 0)
                        {
                           
                            slamTiles.Add(new TopDown.SlamTiles(27, new Rectangle((levelInX * screenWidth) + (x * size),
                                (y * size) - (levelInY * screenHeight), size, size)));
                            slamTiles[slamTiles.Count - 1].mapPoint = new int[] { y, x };
                            if (SlamIndexes.Contains(num) == false)
                                SlamIndexes.Add(num);
                            floorTiles.Add(new FloorTiles(27, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                        }

                        else if (num == 10 || num == 1 || num == 2 || num == 3 || num == 4 || num == 5 || num == 6 || num == 7 || num == 8 || num == 10)//Walls
                        {

                            switch (num)
                            {
                                case 10:
                                    wallTiles.Add(new WallTiles(28, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 1:
                                    wallTiles.Add(new WallTiles(21, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 2:
                                    wallTiles.Add(new WallTiles(29, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 3:
                                    wallTiles.Add(new WallTiles(20, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 4:
                                    wallTiles.Add(new WallTiles(22, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 5:
                                    wallTiles.Add(new WallTiles(23, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 6:
                                    wallTiles.Add(new WallTiles(24, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 7:
                                    wallTiles.Add(new WallTiles(25, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 8:
                                    wallTiles.Add(new WallTiles(26, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                default:
                                    wallTiles.Add(new WallTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;

                            }
                            wallTiles[wallCount].mapPoint = new int[] { y, x };
                            //wallTiles[wallCount].mapPoint = num;
                            wallCount++;
                            if (WallIndexes.Contains(num) == false)
                            {
                                WallIndexes.Add(num);
                            }
                        }
                        else if (num == 11) //enemy
                        {
                            if (enemySpawns.Contains(new Vector2((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight))) == false)
                            {
                                enemySpawns.Add(new Vector2((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight)));
                                enemySpawnPoints.Add(new EnemySpawn(new int[] { y, x }, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            }

                            floorTiles.Add(new FloorTiles(27, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };

                            //floorTiles[floorTiles.Count - 1].mapPoint = num;
                            floorCount++;
                            if (EnemyIndexes.Contains(num) == false)
                                EnemyIndexes.Add(num);
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        else if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15 || num == 16 || num == 17 || num == 18 || num == 19
                            || num == 20 || num == 21 || num == 22 || num == 23) //Floors
                        {
                            switch (num)
                            {
                                case 9:
                                    floorTiles.Add(new FloorTiles(27, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    if (FloorIndexes.Contains(num) == false)
                                        FloorIndexes.Add(num);

                                    floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;
                                default:
                                    doorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    doorTiles[doorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;

                            }
                            //floorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
 
                            //floorTiles[floorTiles.Count - 1].mapPoint = num;
                            floorCount++;
                          

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
        private void BuildDungeon2(List<int[,]> maps, List<int[,]> yMaps, List<int[,]> dMaps, int size, int screenWidth, int screenHeight, List<Vector2> xPoints, List<Vector2> yPoints, List<Vector2> diagPoints)
        {
            #region NonDiagMaps
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

                        if (num == 10 || num == 1 || num == 2 || num == 3 || num == 4 || num == 5 || num == 6 || num == 7 || num == 8)//Walls
                        {
                            //wallTiles.Add(new WallTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));

                            switch (num)
                            {
                                case 10:
                                    wallTiles.Add(new WallTiles(39, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 1:
                                    wallTiles.Add(new WallTiles(32, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 2:
                                    wallTiles.Add(new WallTiles(30, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 3:
                                    wallTiles.Add(new WallTiles(31, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 4:
                                    wallTiles.Add(new WallTiles(33, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 5:
                                    wallTiles.Add(new WallTiles(34, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 6:
                                    wallTiles.Add(new WallTiles(35, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 7:
                                    wallTiles.Add(new WallTiles(36, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 8:
                                    wallTiles.Add(new WallTiles(37, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                default:
                                    wallTiles.Add(new WallTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;

                            }

                            wallTiles[wallCount].mapPoint = new[] { y, x };
                            wallCount++;
                            if (WallIndexes.Contains(num) == false)
                            {
                                WallIndexes.Add(num);
                            }
                        }
                        if (num == 11) //enemy
                        {
                            if (enemySpawns.Contains(new Vector2((levelInX * screenWidth) + (x * size), y * size)) == false)
                            {
                                enemySpawns.Add(new Vector2((levelInX * screenWidth) + (x * size), y * size));
                            }

                            floorTiles.Add(new FloorTiles(38, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                            enemySpawnPoints.Add(new EnemySpawn(new int[] { y, x }, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            floorCount++;
                            if (EnemyIndexes.Contains(num) == false)
                                EnemyIndexes.Add(num);
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15 || num == 16 || num == 17 || num == 18 || num == 19) //Floors
                        {
                            switch (num)
                            {
                                case 9:
                                    floorTiles.Add(new FloorTiles(38, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    if (FloorIndexes.Contains(num) == false)
                                        FloorIndexes.Add(num);
                                    floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;
                                default:
                                    doorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    doorTiles[doorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;

                            }
                            //floorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                           
                            floorCount++;
                          

                            if (num == 12)
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

            for (int i = 0; i < yMaps.Count; i++)
            {
                rows.Add(yMaps[i].GetLength(0));
                cols.Add(yMaps[i].GetLength(1));

                int levelInY = (int)yPoints[i].Y;
                for (int x = 0; x < cols[i]; x++)
                {
                    for (int y = 0; y < yMaps[i].GetLength(0); y++)
                    {
                        int num = yMaps[i][y, x];

                        if (num == 10 || num == 1 || num == 2 || num == 3 || num == 4 || num == 5 || num == 6 || num == 7 || num == 8 || num == 10)//Walls
                        {
                            //wallTiles.Add(new WallTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            switch (num)
                            {
                                case 10:
                                    wallTiles.Add(new WallTiles(39, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 1:
                                    wallTiles.Add(new WallTiles(32, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 2:
                                    wallTiles.Add(new WallTiles(30, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 3:
                                    wallTiles.Add(new WallTiles(31, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 4:
                                    wallTiles.Add(new WallTiles(33, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 5:
                                    wallTiles.Add(new WallTiles(34, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 6:
                                    wallTiles.Add(new WallTiles(35, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 7:
                                    wallTiles.Add(new WallTiles(36, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 8:
                                    wallTiles.Add(new WallTiles(37, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                default:
                                    wallTiles.Add(new WallTiles(num, new Rectangle((x * size), (y * size), size, size)));
                                    break;

                            }
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
                            if (enemySpawns.Contains(new Vector2(x * size, (y * size) - (levelInY * screenHeight))) == false)
                            {
                                enemySpawns.Add(new Vector2(x * size, (y * size) - (levelInY * screenHeight)));
                            }

                            floorTiles.Add(new FloorTiles(38, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                            enemySpawnPoints.Add(new EnemySpawn(new int[] { y, x }, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            //floorTiles[floorTiles.Count - 1].mapPoint = num;
                            floorCount++;
                            if (EnemyIndexes.Contains(num) == false)
                                EnemyIndexes.Add(num);
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15 || num == 16 || num == 17 || num == 18 || num == 19) //Floors
                        {
                            switch (num)
                            {
                                case 9:
                                    floorTiles.Add(new FloorTiles(38, new Rectangle((x * size), (y * size), size, size)));
                                    if (FloorIndexes.Contains(num) == false)
                                        FloorIndexes.Add(num);
                                    floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;
                             
                                default:
                                    floorTiles.Add(new FloorTiles(num, new Rectangle((x * size), (y * size), size, size)));
                                    doorTiles[doorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;

                            }
                            //floorTiles.Add(new FloorTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
            
                            //floorTiles[floorTiles.Count - 1].mapPoint = num;
                            floorCount++;
        

                            if (num == 12)
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
            #endregion

            for (int i = 0; i < dMaps.Count; i++)
            {
                rows.Add(dMaps[i].GetLength(0));
                cols.Add(dMaps[i].GetLength(1));

                for (int y = 0; y < dMaps[i].GetLength(0); y++)
                {
                    for (int x = 0; x < dMaps[i].GetLength(1); x++)
                    {
                        int num = dMaps[i][y, x];
                        levelInX = (int)diagPoints[i].X;
                        levelInY = (int)diagPoints[i].Y;


                        if (num == 0)
                        {
                            slamTiles.Add(new TopDown.SlamTiles(9, new Rectangle((levelInX * screenWidth) + (x * size),
                                (y * size) - (levelInY * screenHeight), size, size)));
                            slamTiles[slamTiles.Count - 1].mapPoint = new int[] { y, x };
                            if (SlamIndexes.Contains(num) == false)
                                SlamIndexes.Add(num);
                        }

                        else if (num == 10 || num == 1 || num == 2 || num == 3 || num == 4 || num == 5 || num == 6 || num == 7 || num == 8 || num == 10)//Walls
                        {

                            switch (num)
                            {
                                case 10:
                                    wallTiles.Add(new WallTiles(39, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 1:
                                    wallTiles.Add(new WallTiles(32, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 2:
                                    wallTiles.Add(new WallTiles(30, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 3:
                                    wallTiles.Add(new WallTiles(31, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 4:
                                    wallTiles.Add(new WallTiles(33, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 5:
                                    wallTiles.Add(new WallTiles(34, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 6:
                                    wallTiles.Add(new WallTiles(35, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 7:
                                    wallTiles.Add(new WallTiles(36, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 8:
                                    wallTiles.Add(new WallTiles(37, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                default:
                                    wallTiles.Add(new WallTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;

                            }
                            wallTiles[wallCount].mapPoint = new int[] { y, x };
                            //wallTiles[wallCount].mapPoint = num;
                            wallCount++;
                            if (WallIndexes.Contains(num) == false)
                            {
                                WallIndexes.Add(num);
                            }
                        }
                        else if (num == 11) //enemy
                        {
                            if (enemySpawns.Contains(new Vector2((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight))) == false)
                            {
                                enemySpawns.Add(new Vector2((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight)));
                                enemySpawnPoints.Add(new EnemySpawn(new int[] { y, x }, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            }

                            floorTiles.Add(new FloorTiles(38, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };

                            //floorTiles[floorTiles.Count - 1].mapPoint = num;
                            floorCount++;
                            if (EnemyIndexes.Contains(num) == false)
                                EnemyIndexes.Add(num);
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        else if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15 || num == 16 || num == 17 || num == 18 || num == 19) //Floors
                        {
                            switch (num)
                            {
                                case 9:
                                    floorTiles.Add(new FloorTiles(38, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    if (FloorIndexes.Contains(num) == false)
                                        FloorIndexes.Add(num);
                                   floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;
                          
                                default:
                                    doorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    doorTiles[doorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;

                            }
                            //floorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                           
                            //floorTiles[floorTiles.Count - 1].mapPoint = num;
                            floorCount++;
                           

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
        private void BuildDungeon3(List<int[,]> maps, List<int[,]> yMaps, List<int[,]> dMaps, int size, int screenWidth, int screenHeight, List<Vector2> xPoints, List<Vector2> yPoints, List<Vector2> diagPoints)
        {
            #region NonDiagMaps
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

                        if (num == 10 || num == 1 || num == 2 || num == 3 || num == 4 || num == 5 || num == 6 || num == 7 || num == 8)//Walls
                        {
                            //wallTiles.Add(new WallTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));

                            switch (num)
                            {
                                case 10:
                                    wallTiles.Add(new WallTiles(48, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 1:
                                    wallTiles.Add(new WallTiles(41, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 2:
                                    wallTiles.Add(new WallTiles(43, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 3:
                                    wallTiles.Add(new WallTiles(40, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 4:
                                    wallTiles.Add(new WallTiles(42, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 5:
                                    wallTiles.Add(new WallTiles(46, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 6:
                                    wallTiles.Add(new WallTiles(47, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 7:
                                    wallTiles.Add(new WallTiles(44, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                case 8:
                                    wallTiles.Add(new WallTiles(45, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;
                                default:
                                    wallTiles.Add(new WallTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    break;

                            }

                            wallTiles[wallCount].mapPoint = new[] { y, x };
                            wallCount++;
                            if (WallIndexes.Contains(num) == false)
                            {
                                WallIndexes.Add(num);
                            }
                        }
                        if (num == 11) //enemy
                        {
                            if (enemySpawns.Contains(new Vector2((levelInX * screenWidth) + (x * size), y * size)) == false)
                            {
                                enemySpawns.Add(new Vector2((levelInX * screenWidth) + (x * size), y * size));
                            }

                            floorTiles.Add(new FloorTiles(49, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                            enemySpawnPoints.Add(new EnemySpawn(new int[] { y, x }, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            floorCount++;
                            if (EnemyIndexes.Contains(num) == false)
                                EnemyIndexes.Add(num);
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15 || num == 16 || num == 17 || num == 18 || num == 19) //Floors
                        {
                            switch (num)
                            {
                                case 9:
                                    floorTiles.Add(new FloorTiles(49, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    if (FloorIndexes.Contains(num) == false)
                                        FloorIndexes.Add(num);
                                    floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;
                                default:
                                    doorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                                    doorTiles[doorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;

                            }
                            //floorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                          
                            floorCount++;
                            

                            if (num == 12)
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

            for (int i = 0; i < yMaps.Count; i++)
            {
                rows.Add(yMaps[i].GetLength(0));
                cols.Add(yMaps[i].GetLength(1));

                int levelInY = (int)yPoints[i].Y;
                for (int x = 0; x < cols[i]; x++)
                {
                    for (int y = 0; y < yMaps[i].GetLength(0); y++)
                    {
                        int num = yMaps[i][y, x];

                        if (num == 10 || num == 1 || num == 2 || num == 3 || num == 4 || num == 5 || num == 6 || num == 7 || num == 8 || num == 10)//Walls
                        {
                            //wallTiles.Add(new WallTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            switch (num)
                            {
                                case 10:
                                    wallTiles.Add(new WallTiles(48, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 1:
                                    wallTiles.Add(new WallTiles(41, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 2:
                                    wallTiles.Add(new WallTiles(43, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 3:
                                    wallTiles.Add(new WallTiles(40, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 4:
                                    wallTiles.Add(new WallTiles(42, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 5:
                                    wallTiles.Add(new WallTiles(46, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 6:
                                    wallTiles.Add(new WallTiles(47, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 7:
                                    wallTiles.Add(new WallTiles(44, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                case 8:
                                    wallTiles.Add(new WallTiles(45, new Rectangle((x * size), (y * size), size, size)));
                                    break;
                                default:
                                    wallTiles.Add(new WallTiles(num, new Rectangle((x * size), (y * size), size, size)));
                                    break;

                            }
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
                            if (enemySpawns.Contains(new Vector2(x * size, (y * size) - (levelInY * screenHeight))) == false)
                            {
                                enemySpawns.Add(new Vector2(x * size, (y * size) - (levelInY * screenHeight)));
                            }

                            floorTiles.Add(new FloorTiles(49, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                            enemySpawnPoints.Add(new EnemySpawn(new int[] { y, x }, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            //floorTiles[floorTiles.Count - 1].mapPoint = num;
                            floorCount++;
                            if (EnemyIndexes.Contains(num) == false)
                                EnemyIndexes.Add(num);
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15 || num == 16 || num == 17 || num == 18 || num == 19) //Floors
                        {
                            switch (num)
                            {
                                case 9:
                                    floorTiles.Add(new FloorTiles(49, new Rectangle((x * size), (y * size), size, size)));
                                    if (FloorIndexes.Contains(num) == false)
                                        FloorIndexes.Add(num);

                                    floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;
                                default:
                                    doorTiles.Add(new FloorTiles(num, new Rectangle((x * size), (y * size), size, size)));
                                    doorTiles[doorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;

                            }
                            //floorTiles.Add(new FloorTiles(num, new Rectangle((x * size), (y * size) - (levelInY * screenHeight), size, size)));
                          
                            //floorTiles[floorTiles.Count - 1].mapPoint = num;
                            floorCount++;
                          

                            if (num == 12)
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
            #endregion

            for (int i = 0; i < dMaps.Count; i++)
            {
                rows.Add(dMaps[i].GetLength(0));
                cols.Add(dMaps[i].GetLength(1));

                for (int y = 0; y < dMaps[i].GetLength(0); y++)
                {
                    for (int x = 0; x < dMaps[i].GetLength(1); x++)
                    {
                        int num = dMaps[i][y, x];
                        levelInX = (int)diagPoints[i].X;
                        levelInY = (int)diagPoints[i].Y;


                        if (num == 0)
                        {
                            slamTiles.Add(new TopDown.SlamTiles(9, new Rectangle((levelInX * screenWidth) + (x * size),
                                (y * size) - (levelInY * screenHeight), size, size)));
                            slamTiles[slamTiles.Count - 1].mapPoint = new int[] { y, x };
                            if (SlamIndexes.Contains(num) == false)
                                SlamIndexes.Add(num);
                        }

                        else if (num == 10 || num == 1 || num == 2 || num == 3 || num == 4 || num == 5 || num == 6 || num == 7 || num == 8 || num == 10)//Walls
                        {

                            switch (num)
                            {
                                case 10:
                                    wallTiles.Add(new WallTiles(48, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 1:
                                    wallTiles.Add(new WallTiles(41, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 2:
                                    wallTiles.Add(new WallTiles(43, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 3:
                                    wallTiles.Add(new WallTiles(40, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 4:
                                    wallTiles.Add(new WallTiles(42, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 5:
                                    wallTiles.Add(new WallTiles(46, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 6:
                                    wallTiles.Add(new WallTiles(47, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 7:
                                    wallTiles.Add(new WallTiles(44, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                case 8:
                                    wallTiles.Add(new WallTiles(45, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;
                                default:
                                    wallTiles.Add(new WallTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    break;

                            }
                            wallTiles[wallCount].mapPoint = new int[] { y, x };
                            //wallTiles[wallCount].mapPoint = num;
                            wallCount++;
                            if (WallIndexes.Contains(num) == false)
                            {
                                WallIndexes.Add(num);
                            }
                        }
                        else if (num == 11) //enemy
                        {
                            if (enemySpawns.Contains(new Vector2((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight))) == false)
                            {
                                enemySpawns.Add(new Vector2((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight)));
                                enemySpawnPoints.Add(new EnemySpawn(new int[] { y, x }, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            }

                            floorTiles.Add(new FloorTiles(49, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                            floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };

                            //floorTiles[floorTiles.Count - 1].mapPoint = num;
                            floorCount++;
                            if (EnemyIndexes.Contains(num) == false)
                                EnemyIndexes.Add(num);
                            //skullTiles.Add(new SkullTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size), size, size)));
                            //skullTiles[skullCount].mapPoint = new int[] { y, x };
                            //skullCount++;
                        }
                        else if (num == 9 || num == 12 || num == 13 || num == 14 || num == 15 || num == 16 || num == 17 || num == 18 || num == 19) //Floors
                        {
                            switch (num)
                            {
                                case 9:
                                    floorTiles.Add(new FloorTiles(49, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    if (FloorIndexes.Contains(num) == false)
                                        FloorIndexes.Add(num);
                                    floorTiles[floorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;
                                default:
                                    doorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                                    doorTiles[doorTiles.Count - 1].mapPoint = new int[] { y, x };
                                    break;

                            }
                            //floorTiles.Add(new FloorTiles(num, new Rectangle((levelInX * screenWidth) + (x * size), (y * size) - (levelInY * screenHeight), size, size)));
                      
                            //floorTiles[floorTiles.Count - 1].mapPoint = num;
                            floorCount++;
                           

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

        public void SetBarrier()
        {
            foreach(FloorTiles tile in doorTiles)
            {
                wallTiles.Add(new TopDown.WallTiles(tile.Rectangle));
                wallTiles[wallTiles.Count - 1].mapPoint = tile.mapPoint;
            }

            setBarrier = true;
        }

        public void SetBarrierValues(Rectangle bounds, bool isRight, bool isLeft, bool isUp, bool isDown)
        {
            currBounds = bounds;
            this.isRight = isRight;
            this.isLeft = isLeft;
            this.isUp = isUp;
            this.isDown = isDown;

            refresh = true;
        }

        private void SetBarrierSide()
        {
            if(isRight)
            {
                foreach(FloorTiles tile in doorTiles)
                {
                    if(tile.Rectangle.Right == currBounds.Right)
                    {
                        wallTiles.Add(new TopDown.WallTiles(tile.Rectangle));
                        wallTiles[wallTiles.Count - 1].mapPoint = tile.mapPoint;
                    }
                }
            }
            else if(isLeft)
            {
                foreach (FloorTiles tile in doorTiles)
                {
                    if (tile.Rectangle.Left == currBounds.Left)
                    {
                        wallTiles.Add(new TopDown.WallTiles(tile.Rectangle));
                        wallTiles[wallTiles.Count - 1].mapPoint = tile.mapPoint;
                    }
                }
            }
            else if(isDown)
            {
                foreach (FloorTiles tile in doorTiles)
                {
                    if (tile.Rectangle.Bottom == currBounds.Bottom)
                    {
                        wallTiles.Add(new TopDown.WallTiles(tile.Rectangle));
                        wallTiles[wallTiles.Count - 1].mapPoint = tile.mapPoint;
                    }
                }
            }
            else if(isUp)
            {
                foreach (FloorTiles tile in doorTiles)
                {
                    if (tile.Rectangle.Top == currBounds.Top)
                    {
                        wallTiles.Add(new TopDown.WallTiles(tile.Rectangle));
                        wallTiles[wallTiles.Count - 1].mapPoint = tile.mapPoint;
                    }
                }
            }
            publicRefresh = true;
            //refresh = false;
            //setBarrier = true;
        }

        public void RemoveBarrier()
        {
            for(int i = WallTiles.Count - 1; i >= 0; i--)
            {
                for(int j = doorTiles.Count -1; j >= 0; j--)
                {
                    if(wallTiles[i].Rectangle == doorTiles[j].Rectangle)
                    {
                        wallTiles.RemoveAt(i);
                        break;
                    }
                }
            }

            setBarrier = false;
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
            foreach (FloorTiles tile in floorTiles)
            {
                tile.Draw(spriteBatch);
            }
            foreach (FloorTiles tile in doorTiles)
            {
                tile.Draw(spriteBatch);
            }
            foreach (SlamTiles tile in slamTiles)
            {
                tile.Draw(spriteBatch);
            }
            foreach (WallTiles tile in wallTiles)
            {
                tile.Draw(spriteBatch);
            }
            foreach (EnvironmentTile tile in environmentTiles)
            {
                tile.Draw(spriteBatch, tile.direction);
            }
  
   
            //foreach (SkullTiles tile in skullTiles)
            //{
            //    tile.Draw(spriteBatch);
            //}
            
        }
    }
}
