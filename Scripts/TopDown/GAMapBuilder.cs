using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AUTO_Matic.Scripts.TopDown;
using AUTO_Matic.TopDown;
using Microsoft.Xna.Framework;

namespace AUTO_Matic.Scripts.TopDown
{
    class GAMapBuilder
    {

        public GAMapBuilder(List<int[,]> possMaps)
        {
            this.possMaps = possMaps;
        }

        List<int[,]> possMaps = new List<int[,]>();
        Random rand = new Random();
        Chromosome map;
        int accuracy;
        int max;

        List<MapObject> mapObjects = new List<MapObject>();
        //List<List<int[,]>> population = new List<List<int[,]>>();


        //Different types of changes: Add walls, Change enemy spawn, remove walls, add enemy spawn

        public int[,] ChooseMap()
        {
            map = new Chromosome(possMaps[rand.Next(0, possMaps.Count)]);

            return ConvertMap(map);
        }

        public int[,] Start(int percentAccuracy, TopDownMap mapData, Rectangle currBounds)
        {
            
            accuracy = percentAccuracy;
            max = map.map.GetLength(0) * map.map.GetLength(1);

            Chromosome tempMap = map;
            // List<Rectangle> tempRects = new List<Rectangle>();
            //for(int y = 0; y < tempMap.map.GetLength(0); y++)
            //{
            //    for(int x = 0; x < tempMap.map.GetLength(1); x++)
            //    {

            //    }
            //}
            GetMapObjects(mapData, currBounds, tempMap);

            //Remove everything to test
            foreach(MapObject obj in mapObjects)
            {
                for(int i = 0; i < obj.objectRects.Count; i++)
                {

                    foreach (WallTiles tile1 in mapData.WallTiles)
                    {
                        if (tile1.Rectangle == obj.objectRects[i])
                        {
                            //tile = tile1;
                            //tempMap.map[tile1.mapPoint[0], tile1.mapPoint[1]].num = 9;
                            //break;
                        }
                    }

                    #region Old Tests
                    //if (mapData.WallIndexes.Contains(tempMap.map[Math.Abs(obj.objectRects[i].Y / (64 * levelInY)), obj.objectRects[i].X / (64 * levelInX)].num)
                    //    || mapData.EnemyIndexes.Contains(tempMap.map[Math.Abs(obj.objectRects[i].Y / (64 * levelInY)), obj.objectRects[i].X / (64 * levelInX)].num))
                    //{
                    //    tempMap.map[Math.Abs(obj.objectRects[i].Y / (64 * levelInY)), obj.objectRects[i].X / (64 * levelInX)].num = mapData.FloorIndexes[1];
                    //}
                    // WallTiles tile = new WallTiles(10, obj.objectRects[i]);

                    //if(tile.mapPoint != null)
                    //{

                    //}

                    //if(obj.id == "Wall" && mapData.WallTiles.Contains(new WallTiles(10, obj.objectRects[i])))
                    //{
                    //     = new WallTiles(10, obj.objectRects[i]);
                    //    tempMap.map[mapData.WallTiles..mapPoint[0],
                    //        mapData.WallTiles.Find(new WallTiles(10, obj.objectRects[i])).mapPoint[1]] = 9;
                    //}

                    //int y = obj.objectRects[i].Y / (64 * levelInY);
                    //int x = obj.objectRects[i].X / (64 * levelInX);
                    //tempMap.map[Math.Abs(y), x].num = 10;
                    #endregion
                }
            }

            int[,] testMap = ConvertMap(tempMap);

            return ConvertMap(tempMap);

            //return mp;
        }

        private static int[,] ConvertMap(Chromosome tempMap)
        {
            int[,] mp = new int[tempMap.map.GetLength(0), tempMap.map.GetLength(1)];

            for (int y = 0; y < tempMap.map.GetLength(0); y++)
            {
                for (int x = 0; x < tempMap.map.GetLength(1); x++)
                {
                    mp[y, x] = tempMap.map[y, x].num;
                }
            }

            return mp;
        }

        private void GetMapObjects(TopDownMap mapData, Rectangle currBounds, Chromosome tempMap)
        {
            List<Rectangle> mapObject = new List<Rectangle>();

            for (int i = 0; i < mapData.WallTiles.Count; i++)
            {
                if (currBounds.Contains(mapData.WallTiles[i].Rectangle) && mapData.WallTiles[i].mapPoint[0] != 0 && mapData.WallTiles[i].mapPoint[1] != 0
                    && mapData.WallTiles[i].mapPoint[0] != tempMap.map.GetLength(0) - 1 && mapData.WallTiles[i].mapPoint[1] != tempMap.map.GetLength(1) - 1 &&
                    tempMap.map[mapData.WallTiles[i].mapPoint[0], mapData.WallTiles[i].mapPoint[1]].fitness)
                {
                    mapObject = new List<Rectangle>();
                    mapObject.Add(mapData.WallTiles[i].Rectangle);
                    tempMap.map[mapData.WallTiles[i].mapPoint[0], mapData.WallTiles[i].mapPoint[1]].fitness = false;

                    //bool stop = false;


                    #region Find Map Object
                    //Right and Left
                    //Right
                    int xOffset = 1;
                    while (mapData.WallTiles[i].mapPoint[1] + xOffset < tempMap.map.GetLength(1) - 1 &&
                        mapData.WallIndexes.Contains(tempMap.map[mapData.WallTiles[i].mapPoint[0], mapData.WallTiles[i].mapPoint[1] + xOffset].num) &&
                        tempMap.map[mapData.WallTiles[i].mapPoint[0], mapData.WallTiles[i].mapPoint[1] + xOffset].fitness)
                    {
                        mapObject.Add(new Rectangle(mapData.WallTiles[i].Rectangle.X + (64 * xOffset), mapData.WallTiles[i].Rectangle.Y,
                            mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height));

                        tempMap.map[mapData.WallTiles[i].mapPoint[0], mapData.WallTiles[i].mapPoint[1] + xOffset].fitness = false;
                        xOffset += 1;
                    }
                    //Left
                    xOffset = 1;
                    while (mapData.WallTiles[i].mapPoint[1] - xOffset > 0 &&
                        mapData.WallIndexes.Contains(tempMap.map[mapData.WallTiles[i].mapPoint[0], mapData.WallTiles[i].mapPoint[1] - xOffset].num) &&
                        tempMap.map[mapData.WallTiles[i].mapPoint[0], mapData.WallTiles[i].mapPoint[1] - xOffset].fitness)
                    {
                        mapObject.Add(new Rectangle(mapData.WallTiles[i].Rectangle.X - (64 * xOffset), mapData.WallTiles[i].Rectangle.Y,
                          mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height));


                        tempMap.map[mapData.WallTiles[i].mapPoint[0], mapData.WallTiles[i].mapPoint[1] - xOffset].fitness = false;
                        xOffset += 1;
                    }
                    //Up and Down
                    //Up
                    int yOffset = 1;
                    while (mapData.WallTiles[i].mapPoint[0] - yOffset > 0
                        && mapData.WallIndexes.Contains(tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1]].num) &&
                        tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1]].fitness)
                    {
                        mapObject.Add(new Rectangle(mapData.WallTiles[i].Rectangle.X, mapData.WallTiles[i].Rectangle.Y - (64 * yOffset),
                            mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height));

                        tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1]].fitness = false;
                        yOffset += 1;
                    }
                    //Down
                    yOffset = 1;
                    while (mapData.WallTiles[i].mapPoint[0] + yOffset < tempMap.map.GetLength(0) - 1 &&
                        mapData.WallIndexes.Contains(tempMap.map[mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1]].num) &&
                        tempMap.map[mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1]].fitness)
                    {
                        mapObject.Add(new Rectangle(mapData.WallTiles[i].Rectangle.X, mapData.WallTiles[i].Rectangle.Y + (64 * yOffset),
                           mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height));

                        tempMap.map[mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1]].fitness = false;
                        yOffset += 1;
                    }

                    //Diags
                    xOffset = 1;
                    yOffset = 1;

                    //Down and Right
                    while (mapData.WallTiles[i].mapPoint[0] + yOffset < tempMap.map.GetLength(0) - 1
                        && mapData.WallTiles[i].mapPoint[1] + xOffset < tempMap.map.GetLength(1) - 1 &&
                        mapData.WallIndexes.Contains(tempMap.map[mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1] + xOffset].num) &&
                        tempMap.map[mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1] + xOffset].fitness)
                    {
                        mapObject.Add(new Rectangle(mapData.WallTiles[i].Rectangle.X + (64 * xOffset), mapData.WallTiles[i].Rectangle.Y + (64 * yOffset),
                          mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height));

                        tempMap.map[mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1] + xOffset].fitness = false;
                        yOffset += 1;
                        xOffset += 1;
                    }

                    xOffset = 1;
                    yOffset = 1;

                    //Up and Right
                    while (mapData.WallTiles[i].mapPoint[1] + xOffset < tempMap.map.GetLength(1) - 1 &&
                       mapData.WallTiles[i].mapPoint[0] - yOffset > 0 &&
                       mapData.WallIndexes.Contains(tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1] + xOffset].num) &&
                       tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1] + xOffset].fitness)
                    {
                        mapObject.Add(new Rectangle(mapData.WallTiles[i].Rectangle.X + (64 * xOffset), mapData.WallTiles[i].Rectangle.Y - (64 * yOffset),
                         mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height));

                        tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1] + xOffset].fitness = false;
                        yOffset += 1;
                        xOffset += 1;
                    }

                    xOffset = 1;
                    yOffset = 1;

                    //Down and Left
                    while (mapData.WallTiles[i].mapPoint[0] + yOffset < tempMap.map.GetLength(0) - 1 &&
                        mapData.WallTiles[i].mapPoint[1] - xOffset > 0 &&
                        mapData.WallIndexes.Contains(tempMap.map[mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1] - xOffset].num) &&
                        tempMap.map[mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1] - xOffset].fitness)
                    {
                        mapObject.Add(new Rectangle(mapData.WallTiles[i].Rectangle.X - (64 * xOffset), mapData.WallTiles[i].Rectangle.Y + (64 * yOffset),
                          mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height));

                        tempMap.map[mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1] - xOffset].fitness = false;
                        yOffset += 1;
                        xOffset += 1;
                    }

                    xOffset = 1;
                    yOffset = 1;

                    //Up and Left
                    while (
                        mapData.WallTiles[i].mapPoint[0] - yOffset > 0 && mapData.WallTiles[i].mapPoint[1] - xOffset > 0 &&
                        mapData.WallIndexes.Contains(tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1] - xOffset].num) &&
                        tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1] - xOffset].fitness)
                    {
                        mapObject.Add(new Rectangle(mapData.WallTiles[i].Rectangle.X - (64 * xOffset), mapData.WallTiles[i].Rectangle.Y - (64 * yOffset),
                          mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height));

                        tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1] - xOffset].fitness = false;
                        yOffset += 1;
                        xOffset += 1;
                    }
                    #endregion

                    if (mapObjects.Contains(new MapObject(mapObject, "Wall")) == false && mapObject.Count >= 2)
                        mapObjects.Add(new MapObject(mapObject, "Wall"));
                    //mapObjects.Add(mapObject);


                }
            }

            List<Rectangle> enemyLoc = new List<Rectangle>();
            for (int i = 0; i < mapData.EnemySpawns.Count; i++)
            {
                enemyLoc = new List<Rectangle>();
                if (currBounds.Contains(mapData.EnemySpawns[i]))
                {
                    enemyLoc.Add(new Rectangle(mapData.EnemySpawns[i].ToPoint(), new Point(64, 64)));
                    mapObjects.Add(new MapObject(enemyLoc, "Enemy"));
                }

            }
        }
    }
    
    class MapObject
    {
       // int index;
        public List<Rectangle> objectRects;
        public string id;
      

        public MapObject(List<Rectangle> rects, string id)
        {
            //this.index = index;
            objectRects = rects;
            this.id = id;
        }
    }

    class Chromosome
    {
        public Gene[,] map;
        public int fitness;
        List<int> possNums;
        int minNum;
        int maxNum;

        public Chromosome(int[,] map)
        {
            Gene[,] tempMap = new Gene[map.GetLength(0), map.GetLength(1)];
            possNums = new List<int>();
            for(int y = 0; y < map.GetLength(0); y++)
            {
                for(int x = 0; x < map.GetLength(1); x++)
                {
                    if(!possNums.Contains(map[y,x]))
                    {
                        possNums.Add(map[y, x]);
                    }
                    tempMap[y, x].num = map[y, x];
                    tempMap[y, x].fitness = true;
                    tempMap[y, x].mapPoint = new int[] { y, x };
                }
            }

            minNum = possNums[0];
            maxNum = possNums[possNums.Count - 1];

            for(int i = 0; i < possNums.Count; i++)
            {
                if(minNum > possNums[i])
                {
                    minNum = possNums[i];
                }
                if(maxNum < possNums[i])
                {
                    maxNum = possNums[i];
                }
            }
            this.map = tempMap;
            fitness = 0;
            
        }

        //Map build will need the indexes of the used tiles. 
        public Chromosome CreateChromosome(int[,] map)
        {
            return new Chromosome(map);
        }
        
    }

    struct Gene
    {
        public int num;
        public bool fitness;
        public int[] mapPoint;

        public Gene(int num, int[] point)
        {
            this.num = num;
            fitness = false;
            mapPoint = point;
        }
    }
}
