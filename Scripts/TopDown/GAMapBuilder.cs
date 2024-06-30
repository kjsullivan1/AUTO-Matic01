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
        public void Start(int percentAccuracy, TopDownMap mapData, Rectangle currBounds)
        {
            map = new Chromosome(possMaps[rand.Next(0, possMaps.Count)]);
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
            List<Rectangle> mapObject = new List<Rectangle>();

            for(int i =0;i < mapData.WallTiles.Count; i++)
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
                    while (mapData.WallTiles[i].mapPoint[1] + xOffset < tempMap.map.GetLength(1) - 1&& 
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
                    while(mapData.WallTiles[i].mapPoint[0] - yOffset > 0 
                        && mapData.WallIndexes.Contains(tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset,  mapData.WallTiles[i].mapPoint[1]].num) &&
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
                        mapData.WallIndexes.Contains(tempMap.map[mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1]].num)&&
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
                    while(mapData.WallTiles[i].mapPoint[0] + yOffset < tempMap.map.GetLength(0) - 1 
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
            for(int i = 0; i < mapData.EnemySpawns.Count - 1; i++)
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
        List<Rectangle> objectRects;
        string id;

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

        public Gene(int num)
        {
            this.num = num;
            fitness = false;
        }
    }
}
