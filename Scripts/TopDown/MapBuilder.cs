﻿using AUTO_Matic.TopDown;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace AUTO_Matic.Scripts.TopDown
{
    namespace AUTO_Matic.Scripts.TopDown
    {
        class MapBuilder
        {
            public MapBuilder(List<int[,]> possMaps)
            {
                this.possMaps = possMaps;
            }

            List<int[,]> possMaps = new List<int[,]>();
            Random rand = new Random();
            CellMap map;
            int accuracy;
            int max;
            string environmentDirection = "";

            List<MapObject> mapObjects = new List<MapObject>();
            int numEnvironment = 0;
            int numEnemies = 0;
            enum MapEffectObjects { AddWall, AddEnemy, RemoveEnemy, RemoveWall, SpeedBoost, DamageOverTime }
            MapEffectObjects mapObjectEffect;
            public void ChooseEffectObject()
            {
                bool selected = false;

                while(!selected)
                {
                    int num = rand.Next(0, 101);

                    if (num < 25 && numEnvironment < 8)
                    {
                        mapObjectEffect = MapEffectObjects.SpeedBoost;
                        GetDirection();
                        selected = true;
                        numEnvironment++;
                    }
                    else if (num >= 25 && num < 50 && numEnvironment < 8)
                    {
                        mapObjectEffect = MapEffectObjects.DamageOverTime;
                        GetDirection();
                        selected = true;
                        numEnvironment++;
                    }
                    else if (num >= 50 && num < 62 && numEnemies > 4)
                    {
                        mapObjectEffect = MapEffectObjects.RemoveEnemy;
                        selected = true;
                      
                    }
                    else if (num >= 62 && num < 75)
                    {
                        mapObjectEffect = MapEffectObjects.RemoveWall;
                        selected = true;
                    }
                    else if (num >= 75 && num < 87 && numEnemies < 7)
                    {
                        mapObjectEffect = MapEffectObjects.AddEnemy;
                        selected = true;
                    
                    }
                    else
                    {
                        mapObjectEffect = MapEffectObjects.AddWall;
                        selected = true;
                    }
                }
              
            }

            public void GetDirection()
            {
                int num = rand.Next(0, 4);

                switch(num)
                {
                    case 0:
                        environmentDirection = "right";
                        break;
                    case 1:
                        environmentDirection = "left";
                        break;
                    case 2:
                        environmentDirection = "up";
                        break;
                    case 3:
                        environmentDirection = "down";
                        break;
                }
            }

            enum MapEffectLayout { Horizontal, Vertical, Diag, None }
            MapEffectLayout mapLayoutEffect;
            public void ChooseLayoutEffect()
            {
                int num = rand.Next(0, 4);
                switch (num)
                {
                    case 0:
                        mapLayoutEffect = MapEffectLayout.Horizontal;
                        break;
                    case 1:
                        mapLayoutEffect = MapEffectLayout.Vertical;
                        break;
                    case 2:
                        mapLayoutEffect = MapEffectLayout.Diag;
                        break;
                    case 3:
                        mapLayoutEffect = MapEffectLayout.None;
                        break;
                }
            }


            public void ChooseMapEffects()
            {
                ChooseEffectObject();
                ChooseLayoutEffect();
            }
            //Different types of changes: Add walls, Change enemy spawn, remove walls, add enemy spawn

            public int[,] ChooseMap()
            {
                map = new CellMap(possMaps[rand.Next(0, possMaps.Count)]);

                return ConvertMap(map);
            }

            public int[,] Start(int percentAccuracy, TopDownMap mapData, Rectangle currBounds)
            {

                accuracy = percentAccuracy;
                max = map.map.GetLength(0) * map.map.GetLength(1);

                CellMap tempMap = map;
                numEnvironment = 0;
                GetMapObjects(mapData, currBounds, tempMap);

                //Choose the effects of the map
                ChooseMapEffects();

                int numWalls = 0;
                for (int k = 0; k < mapObjects.Count; k++)
                {
                    if (mapObjects[k].id == "Wall")
                    {
                        numWalls++;
                    }
                }
                for (int i = mapData.enemySpawnPoints.Count - 1; i >= 0; i--)
                {
                    if (!currBounds.Contains(mapData.enemySpawnPoints[i].Rectangle))
                    {
                        mapData.enemySpawnPoints.Remove(mapData.enemySpawnPoints[i]);
                    }
                }
                int affectedObjects = rand.Next(5, 20);
                numEnemies = mapData.enemySpawnPoints.Count;

                //Effect objects
             

                #region MapLayoutEffect
                //mapLayoutEffect = MapEffectLayout.Diag;
                for (int y = 0; y < tempMap.map.GetLength(0); y++)
                {
                    for (int x = 0; x < tempMap.map.GetLength(1); x++)
                    {
                        tempMap.map[y, x].fitness = true;
                    }
                }

                //mapLayoutEffect = MapEffectLayout.Diag;
                //Effect layout
                switch (mapLayoutEffect)
                {
                    case MapEffectLayout.Horizontal: //Flip the map horizontally (Flip the x)
                        int middleX = tempMap.map.GetLength(1) / 2;

                        for(int y = 1; y < tempMap.map.GetLength(0) - 1; y++)
                        {
                            for(int x = 1; x < tempMap.map.GetLength(1) - 1; x++)
                            {
                                if(mapData.WallIndexes.Contains(tempMap.map[y,x].num) && tempMap.map[y,x].fitness)
                                {
                                    tempMap.map[y, x].num = 9;
                                    tempMap.map[y, x].fitness = false;
                                   // tempMap.map[Math.Abs(x - tempMap.map.GetLength(0)), Math.Abs(y - tempMap.map.GetLength(1))].num = 0;
                                    int test = (tempMap.map.GetLength(1) - 1) - x;
                                    tempMap.map[y, test].num = 0;
                                }
                                //else if(tempMap.map[y,x].fitness == false && tempMap.map[y,x].num == 0)
                                //{
                                //    //int test = (tempMap.map.GetLength(1)) - x;
                                //    tempMap.map[y, x].num = 10;
                                //}
                                if (mapData.EnemyIndexes.Contains(tempMap.map[y,x].num) && tempMap.map[y,x].fitness)
                                {
                                    tempMap.map[y, x].num = 9;
                                    mapData.EnemySpawns.Clear();
                                    tempMap.map[y, x].fitness = false;
                                    tempMap.map[y, (tempMap.map.GetLength(1) - 1) - x].num = -1;
                                    //tempMap.map[y, x].num = 9;
                                    //tempMap.map[Math.Abs(x - tempMap.map.GetLength(0)), Math.Abs(y - tempMap.map.GetLength(1))].num = 11;
                                }
                                //else if(tempMap.map[y,x].fitness == false && tempMap.map[y,x].num == -1)
                                //{
                                //    tempMap.map[y,x]
                                //}
                            }
                        }
                        for (int y = 1; y < tempMap.map.GetLength(0) - 1; y++)
                        {
                            for (int x = 1; x < tempMap.map.GetLength(1) - 1; x++)
                            {
                                //if (mapData.WallIndexes.Contains(tempMap.map[y, x].num) && tempMap.map[y, x].fitness)
                                //{
                                //    tempMap.map[y, x].num = 9;
                                //    tempMap.map[y, x].fitness = false;
                                //    tempMap.map[Math.Abs(x - tempMap.map.GetLength(0)), Math.Abs(y - tempMap.map.GetLength(1))].num = 0;
                                //    int test = (tempMap.map.GetLength(1)) - x;
                                //    tempMap.map[y, test].num = 10;
                                //}
                                if (tempMap.map[y, x].num == 0)
                                {
                                    //int test = (tempMap.map.GetLength(1)) - x;
                                    tempMap.map[y, x].num = 10;
                                }
                                //if (mapData.EnemyIndexes.Contains(tempMap.map[y, x].num) && tempMap.map[y, x].fitness)
                                //{
                                //    tempMap.map[y, x].num = 9;
                                //    mapData.EnemySpawns.Clear();
                                //    tempMap.map[y, x].fitness = false;
                                //    tempMap.map[y, (tempMap.map.GetLength(1)) - x].num = -1;
                                //    //tempMap.map[y, x].num = 9;
                                //    //tempMap.map[Math.Abs(x - tempMap.map.GetLength(0)), Math.Abs(y - tempMap.map.GetLength(1))].num = 11;
                                //}
                                else if (tempMap.map[y, x].num == -1)
                                {
                                    tempMap.map[y, x].num = 11;
                                }
                            }
                        }
                        //foreach (MapObject obj in mapObjects)
                        //{
                        //    for (int i = 0; i < obj.objectRects.Count; i++)
                        //    {
                        //        tempMap.map[obj.objectRects[i].mapPoint[0], obj.objectRects[i].mapPoint[1]].num = 9;


                        //        if (obj.id == "Wall")
                        //        {
                        //            tempMap.map[obj.objectRects[i].mapPoint[0],
                        //           (tempMap.map.GetLength(1) - obj.objectRects[i].mapPoint[1])].num = 0;
                        //            mapData.WallTiles.Remove(new WallTiles(10, obj.objectRects[i].rect));
                        //        }
                        //        else if (obj.id == "Enemy")
                        //        {
                        //            tempMap.map[obj.objectRects[i].mapPoint[0],
                        //          (tempMap.map.GetLength(1) - obj.objectRects[i].mapPoint[1])].num = 11;
                        //            mapData.EnemySpawns.Remove(new Vector2(obj.objectRects[i].rect.X,
                        //   obj.objectRects[i].rect.Y));
                        //            //break;
                        //        }

                        //        //tempMap.map[obj.objectRects[]
                        //    }
                        //}

                        break;
                    case MapEffectLayout.Vertical:
                        for (int y = 1; y < tempMap.map.GetLength(0) - 1; y++)
                        {
                            for (int x = 1; x < tempMap.map.GetLength(1) - 1; x++)
                            {
                                if (mapData.WallIndexes.Contains(tempMap.map[y, x].num) && tempMap.map[y, x].fitness)
                                {
                                    tempMap.map[y, x].num = 9;
                                    tempMap.map[y, x].fitness = false;
                                    // tempMap.map[Math.Abs(x - tempMap.map.GetLength(0)), Math.Abs(y - tempMap.map.GetLength(1))].num = 0;
                                    //int test = (tempMap.map.GetLength(1)) - x;
                                    tempMap.map[(tempMap.map.GetLength(0) - 1) - y, x].num = 0;
                                }
                                //else if(tempMap.map[y,x].fitness == false && tempMap.map[y,x].num == 0)
                                //{
                                //    //int test = (tempMap.map.GetLength(1)) - x;
                                //    tempMap.map[y, x].num = 10;
                                //}
                                if (mapData.EnemyIndexes.Contains(tempMap.map[y, x].num) && tempMap.map[y, x].fitness)
                                {
                                    tempMap.map[y, x].num = 9;
                                    mapData.EnemySpawns.Clear();
                                    tempMap.map[y, x].fitness = false;
                                    tempMap.map[(tempMap.map.GetLength(0) - 1) - y, x].num = -1;
                                    //tempMap.map[y, x].num = 9;
                                    //tempMap.map[Math.Abs(x - tempMap.map.GetLength(0)), Math.Abs(y - tempMap.map.GetLength(1))].num = 11;
                                }
                                //else if(tempMap.map[y,x].fitness == false && tempMap.map[y,x].num == -1)
                                //{
                                //    tempMap.map[y,x]
                                //}
                            }
                        }
                        for (int y = 1; y < tempMap.map.GetLength(0) - 1; y++)
                        {
                            for (int x = 1; x < tempMap.map.GetLength(1) - 1; x++)
                            {
                               
                                if (tempMap.map[y, x].num == 0)
                                {
                                    //int test = (tempMap.map.GetLength(1)) - x;
                                    tempMap.map[y, x].num = 10;
                                }
                               
                                else if (tempMap.map[y, x].num == -1)
                                {
                                    tempMap.map[y, x].num = 11;
                                }
                            }
                        }
                        break;

                    case MapEffectLayout.Diag:

                        for (int y = 1; y < tempMap.map.GetLength(0) - 1; y++)
                        {
                            for (int x = 1; x < tempMap.map.GetLength(1) - 1; x++)
                            {
                                if (mapData.WallIndexes.Contains(tempMap.map[y, x].num) && tempMap.map[y, x].fitness)
                                {
                                    tempMap.map[y, x].num = 9;
                                    tempMap.map[y, x].fitness = false;
                                    // tempMap.map[Math.Abs(x - tempMap.map.GetLength(0)), Math.Abs(y - tempMap.map.GetLength(1))].num = 0;
                                    //int test = (tempMap.map.GetLength(1)) - x;
                                    tempMap.map[(tempMap.map.GetLength(0) - 1) - y, (tempMap.map.GetLength(1) - 1) - x].num = 0;
                                }
                                //else if(tempMap.map[y,x].fitness == false && tempMap.map[y,x].num == 0)
                                //{
                                //    //int test = (tempMap.map.GetLength(1)) - x;
                                //    tempMap.map[y, x].num = 10;
                                //}
                                if (mapData.EnemyIndexes.Contains(tempMap.map[y, x].num) && tempMap.map[y, x].fitness)
                                {
                                    tempMap.map[y, x].num = 9;
                                    mapData.EnemySpawns.Clear();
                                    tempMap.map[y, x].fitness = false;
                                    tempMap.map[(tempMap.map.GetLength(0) - 1) - y, (tempMap.map.GetLength(1) - 1) - x].num = -1;
                                    //tempMap.map[y, x].num = 9;
                                    //tempMap.map[Math.Abs(x - tempMap.map.GetLength(0)), Math.Abs(y - tempMap.map.GetLength(1))].num = 11;
                                }
                                //else if(tempMap.map[y,x].fitness == false && tempMap.map[y,x].num == -1)
                                //{
                                //    tempMap.map[y,x]
                                //}
                            }
                        }
                        for (int y = 1; y < tempMap.map.GetLength(0) - 1; y++)
                        {
                            for (int x = 1; x < tempMap.map.GetLength(1) - 1; x++)
                            {

                                if (tempMap.map[y, x].num == 0)
                                {
                                    //int test = (tempMap.map.GetLength(1)) - x;
                                    tempMap.map[y, x].num = 10;
                                }

                                else if (tempMap.map[y, x].num == -1)
                                {
                                    tempMap.map[y, x].num = 11;
                                }
                            }
                        }
                        break;
                    case MapEffectLayout.None:
                        break;
                }
                #endregion
                for (int i = 0; i < affectedObjects; i++)
                {
                    EffectObjects(mapData, tempMap, numWalls);
                    ChooseEffectObject();
                }
                //Remove everything to test
                //foreach (MapObject obj in mapObjects)
                //{
                //    for (int j = 0; j < obj.objectRects.Count; j++)
                //    {

                //        foreach (WallTiles tile1 in mapData.WallTiles)
                //        {
                //            if (tile1.Rectangle == obj.objectRects[j].rect)
                //            {
                //                //tile = tile1;
                //                //tempMap.map[tile1.mapPoint[0], tile1.mapPoint[1]].num = 9;
                //                //break;
                //            }
                //        }

                //        #region Old Tests
                //        //if (mapData.WallIndexes.Contains(tempMap.map[Math.Abs(obj.objectRects[i].Y / (64 * levelInY)), obj.objectRects[i].X / (64 * levelInX)].num)
                //        //    || mapData.EnemyIndexes.Contains(tempMap.map[Math.Abs(obj.objectRects[i].Y / (64 * levelInY)), obj.objectRects[i].X / (64 * levelInX)].num))
                //        //{
                //        //    tempMap.map[Math.Abs(obj.objectRects[i].Y / (64 * levelInY)), obj.objectRects[i].X / (64 * levelInX)].num = mapData.FloorIndexes[1];
                //        //}
                //        // WallTiles tile = new WallTiles(10, obj.objectRects[i]);

                //        //if(tile.mapPoint != null)
                //        //{

                //        //}

                //        //if(obj.id == "Wall" && mapData.WallTiles.Contains(new WallTiles(10, obj.objectRects[i])))
                //        //{
                //        //     = new WallTiles(10, obj.objectRects[i]);
                //        //    tempMap.map[mapData.WallTiles..mapPoint[0],
                //        //        mapData.WallTiles.Find(new WallTiles(10, obj.objectRects[i])).mapPoint[1]] = 9;
                //        //}

                //        //int y = obj.objectRects[i].Y / (64 * levelInY);
                //        //int x = obj.objectRects[i].X / (64 * levelInX);
                //        //tempMap.map[Math.Abs(y), x].num = 10;
                //        #endregion
                //    }
                //}

                //int[,] testMap = ConvertMap(tempMap);

                return ConvertMap(tempMap);

                //return mp;
            }

            private void EffectObjects(TopDownMap mapData, CellMap tempMap, int numWalls)
            {
                switch (mapObjectEffect)
                {
                    #region AddWall
                    case MapEffectObjects.AddWall:

                        int num = rand.Next(0, numWalls);

                        switch (mapObjects[num].wallId) //"Diag", "LShape", "Horizontal", "Vertical"
                        {
                            case "Horizontal":
                                if (rand.Next(0, 101) < 50) //Choose right or left... less than 50 is left and > is right
                                {
                                    Object FarLeft = mapObjects[num].objectRects[0];
                                    for (int k = 0; k < mapObjects[num].objectRects.Count; k++)
                                    {
                                        if (FarLeft.rect.X > mapObjects[num].objectRects[k].rect.X)
                                        {
                                            FarLeft = mapObjects[num].objectRects[k];
                                        }
                                    }

                                    if (mapData.FloorIndexes.Contains(tempMap.map[FarLeft.mapPoint[0],
                                       FarLeft.mapPoint[1] - 1].num))
                                    {
                                        tempMap.map[FarLeft.mapPoint[0],
                                        FarLeft.mapPoint[1] - 1].num = 10;
                                    }
                                }
                                else
                                {
                                    Object farRight = mapObjects[num].objectRects[0];
                                    for (int k = 0; k < mapObjects[num].objectRects.Count; k++)
                                    {
                                        if (farRight.rect.X < mapObjects[num].objectRects[k].rect.X)
                                        {
                                            farRight = mapObjects[num].objectRects[k];
                                        }
                                    }

                                    if (mapData.FloorIndexes.Contains(tempMap.map[farRight.mapPoint[0],
                                       farRight.mapPoint[1] + 1].num))
                                    {
                                        tempMap.map[farRight.mapPoint[0],
                                        farRight.mapPoint[1] + 1].num = 10;
                                    }
                                }
                                break;
                            case "Vertical":
                                if (rand.Next(0, 101) < 50) //Choose up or down... less than 50 is up and > is down
                                {
                                    Object farUp = mapObjects[num].objectRects[0];
                                    for (int k = 0; k < mapObjects[num].objectRects.Count; k++)
                                    {
                                        if (farUp.rect.Y > mapObjects[num].objectRects[k].rect.Y)
                                        {
                                            farUp = mapObjects[num].objectRects[k];
                                        }
                                    }

                                    if (mapData.FloorIndexes.Contains(tempMap.map[farUp.mapPoint[0] - 1,
                                       farUp.mapPoint[1]].num))
                                    {
                                        tempMap.map[farUp.mapPoint[0] - 1,
                                        farUp.mapPoint[1]].num = 10;
                                    }
                                }
                                else
                                {
                                    Object farDown = mapObjects[num].objectRects[0];
                                    for (int k = 0; k < mapObjects[num].objectRects.Count; k++)
                                    {
                                        if (farDown.rect.Y < mapObjects[num].objectRects[k].rect.Y)
                                        {
                                            farDown = mapObjects[num].objectRects[k];
                                        }
                                    }

                                    if (mapData.FloorIndexes.Contains(tempMap.map[farDown.mapPoint[0] + 1,
                                       farDown.mapPoint[1]].num))
                                    {
                                        tempMap.map[farDown.mapPoint[0] + 1,
                                        farDown.mapPoint[1]].num = 10;
                                    }
                                }
                                break;
                            case "Diag":
                                int num1 = rand.Next(0, 101);
                                if (num1 < 50) //Chooses to add a diag block 1/4 chance of being in same dir, 1/4 not happening
                                {
                                    Object chosenObj;
                                    if (rand.Next(0, 2) < 1)
                                    {
                                        chosenObj = mapObjects[num].objectRects[0];//Far left
                                        for (int k = 0; k < mapObjects[num].objectRects.Count; k++)
                                        {
                                            if (chosenObj.rect.X > mapObjects[num].objectRects[k].rect.X)
                                            {
                                                chosenObj = mapObjects[num].objectRects[k];
                                            }
                                        }
                                    }
                                    else
                                    {
                                        chosenObj = mapObjects[num].objectRects[0];// Far Right
                                        for (int k = 0; k < mapObjects[num].objectRects.Count; k++)
                                        {
                                            if (chosenObj.rect.X < mapObjects[num].objectRects[k].rect.X)
                                            {
                                                chosenObj = mapObjects[num].objectRects[k];
                                            }
                                        }
                                    }

                                    if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj.mapPoint[0] - 1,
                                       chosenObj.mapPoint[1] - 1].num))
                                    {
                                        tempMap.map[chosenObj.mapPoint[0] - 1,
                                        chosenObj.mapPoint[1] - 1].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj.mapPoint[0] + 1,
                                       chosenObj.mapPoint[1] - 1].num))
                                    {
                                        tempMap.map[chosenObj.mapPoint[0] + 1,
                                        chosenObj.mapPoint[1] - 1].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj.mapPoint[0] - 1,
                                      chosenObj.mapPoint[1] + 1].num))
                                    {
                                        tempMap.map[chosenObj.mapPoint[0] - 1,
                                        chosenObj.mapPoint[1] + 1].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj.mapPoint[0] + 1,
                                      chosenObj.mapPoint[1] + 1].num))
                                    {
                                        tempMap.map[chosenObj.mapPoint[0] + 1,
                                        chosenObj.mapPoint[1] + 1].num = 10;
                                    }
                                }
                                else
                                {
                                    Object chosenObj1;
                                    if (rand.Next(0, 2) < 1)
                                    {
                                        chosenObj1 = mapObjects[num].objectRects[0];//Far up
                                        for (int k = 0; k < mapObjects[num].objectRects.Count; k++)
                                        {
                                            if (chosenObj1.rect.Y > mapObjects[num].objectRects[k].rect.Y)
                                            {
                                                chosenObj1 = mapObjects[num].objectRects[k];
                                            }
                                        }
                                    }
                                    else
                                    {
                                        chosenObj1 = mapObjects[num].objectRects[0];// Far down
                                        for (int k = 0; k < mapObjects[num].objectRects.Count; k++)
                                        {
                                            if (chosenObj1.rect.Y < mapObjects[num].objectRects[k].rect.Y)
                                            {
                                                chosenObj1 = mapObjects[num].objectRects[k];
                                            }
                                        }
                                    }

                                    if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj1.mapPoint[0] - 1,
                                       chosenObj1.mapPoint[1] - 1].num))
                                    {
                                        tempMap.map[chosenObj1.mapPoint[0] - 1,
                                        chosenObj1.mapPoint[1] - 1].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj1.mapPoint[0] + 1,
                                       chosenObj1.mapPoint[1] - 1].num))
                                    {
                                        tempMap.map[chosenObj1.mapPoint[0] + 1,
                                        chosenObj1.mapPoint[1] - 1].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj1.mapPoint[0] - 1,
                                      chosenObj1.mapPoint[1] + 1].num))
                                    {
                                        tempMap.map[chosenObj1.mapPoint[0] - 1,
                                        chosenObj1.mapPoint[1] + 1].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj1.mapPoint[0] + 1,
                                      chosenObj1.mapPoint[1] + 1].num))
                                    {
                                        tempMap.map[chosenObj1.mapPoint[0] + 1,
                                        chosenObj1.mapPoint[1] + 1].num = 10;
                                    }
                                }

                                break;
                            case "LShape":

                                Object chosenObj2;
                                int num2 = rand.Next(0, 4);
                                if (num2 == 0)
                                {
                                    chosenObj2 = mapObjects[num].objectRects[0];//Far up and right
                                    for (int k = 0; k < mapObjects[num].objectRects.Count; k++)
                                    {
                                        if (chosenObj2.rect.Y > mapObjects[num].objectRects[k].rect.Y ||
                                            chosenObj2.rect.X < mapObjects[num].objectRects[k].rect.X)
                                        {
                                            chosenObj2 = mapObjects[num].objectRects[k];
                                        }
                                    }

                                    if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0] - 1,
                                       chosenObj2.mapPoint[1]].num))
                                    {
                                        tempMap.map[chosenObj2.mapPoint[0] - 1,
                                        chosenObj2.mapPoint[1]].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0] + 1,
                                       chosenObj2.mapPoint[1]].num))
                                    {
                                        tempMap.map[chosenObj2.mapPoint[0] + 1,
                                        chosenObj2.mapPoint[1]].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0],
                                      chosenObj2.mapPoint[1] + 1].num))
                                    {
                                        tempMap.map[chosenObj2.mapPoint[0],
                                        chosenObj2.mapPoint[1] + 1].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0],
                                      chosenObj2.mapPoint[1] - 1].num))
                                    {
                                        tempMap.map[chosenObj2.mapPoint[0],
                                        chosenObj2.mapPoint[1] - 1].num = 10;
                                    }
                                }
                                else if (num == 1)
                                {
                                    chosenObj2 = mapObjects[num].objectRects[0];// Far down and right
                                    for (int k = 0; k < mapObjects[num].objectRects.Count; k++)
                                    {
                                        if (chosenObj2.rect.Y < mapObjects[num].objectRects[k].rect.Y ||
                                             chosenObj2.rect.X < mapObjects[num].objectRects[k].rect.X)
                                        {
                                            chosenObj2 = mapObjects[num].objectRects[k];
                                        }
                                    }

                                    if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0] - 1,
                                      chosenObj2.mapPoint[1]].num))
                                    {
                                        tempMap.map[chosenObj2.mapPoint[0] - 1,
                                        chosenObj2.mapPoint[1]].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0] + 1,
                                       chosenObj2.mapPoint[1]].num))
                                    {
                                        tempMap.map[chosenObj2.mapPoint[0] + 1,
                                        chosenObj2.mapPoint[1]].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0],
                                      chosenObj2.mapPoint[1] + 1].num))
                                    {
                                        tempMap.map[chosenObj2.mapPoint[0],
                                        chosenObj2.mapPoint[1] + 1].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0],
                                      chosenObj2.mapPoint[1] - 1].num))
                                    {
                                        tempMap.map[chosenObj2.mapPoint[0],
                                        chosenObj2.mapPoint[1] - 1].num = 10;
                                    }
                                }
                                else if (num == 2)
                                {
                                    chosenObj2 = mapObjects[num].objectRects[0];// Far up and left
                                    for (int k = 0; k < mapObjects[num].objectRects.Count; k++)
                                    {
                                        if (chosenObj2.rect.Y > mapObjects[num].objectRects[k].rect.Y ||
                                             chosenObj2.rect.X > mapObjects[num].objectRects[k].rect.X)
                                        {
                                            chosenObj2 = mapObjects[num].objectRects[k];
                                        }
                                    }

                                    if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0] - 1,
                                      chosenObj2.mapPoint[1]].num))
                                    {
                                        tempMap.map[chosenObj2.mapPoint[0] - 1,
                                        chosenObj2.mapPoint[1]].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0] + 1,
                                       chosenObj2.mapPoint[1]].num))
                                    {
                                        tempMap.map[chosenObj2.mapPoint[0] + 1,
                                        chosenObj2.mapPoint[1]].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0],
                                      chosenObj2.mapPoint[1] + 1].num))
                                    {
                                        tempMap.map[chosenObj2.mapPoint[0],
                                        chosenObj2.mapPoint[1] + 1].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0],
                                      chosenObj2.mapPoint[1] - 1].num))
                                    {
                                        tempMap.map[chosenObj2.mapPoint[0],
                                        chosenObj2.mapPoint[1] - 1].num = 10;
                                    }
                                }
                                else if (num == 3)
                                {
                                    chosenObj2 = mapObjects[num].objectRects[0];// Far down and left
                                    for (int k = 0; k < mapObjects[num].objectRects.Count; k++)
                                    {
                                        if (chosenObj2.rect.Y < mapObjects[num].objectRects[k].rect.Y ||
                                             chosenObj2.rect.X > mapObjects[num].objectRects[k].rect.X)
                                        {
                                            chosenObj2 = mapObjects[num].objectRects[k];
                                        }
                                    }

                                    if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0] - 1,
                                      chosenObj2.mapPoint[1]].num))
                                    {
                                        tempMap.map[chosenObj2.mapPoint[0] - 1,
                                        chosenObj2.mapPoint[1]].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0] + 1,
                                       chosenObj2.mapPoint[1]].num))
                                    {
                                        tempMap.map[chosenObj2.mapPoint[0] + 1,
                                        chosenObj2.mapPoint[1]].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0],
                                      chosenObj2.mapPoint[1] + 1].num))
                                    {
                                        tempMap.map[chosenObj2.mapPoint[0],
                                        chosenObj2.mapPoint[1] + 1].num = 10;
                                    }
                                    else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0],
                                      chosenObj2.mapPoint[1] - 1].num))
                                    {
                                        tempMap.map[chosenObj2.mapPoint[0],
                                        chosenObj2.mapPoint[1] - 1].num = 10;
                                    }
                                }
                                break;
                        }


                        break;
                    #endregion
                    #region AddEnemy
                    case MapEffectObjects.AddEnemy:
                        int pickNum = rand.Next(0, numWalls);

                        int wallNum = rand.Next(0, mapObjects[pickNum].objectRects.Count);

                        int randNum = rand.Next(0, 4);

                        int enemyType = 11;

                        if (randNum == 0) //Add enemy right
                        {
                            if(mapData.FloorIndexes.Contains(tempMap.map[mapObjects[pickNum].objectRects[wallNum].mapPoint[0], 
                                mapObjects[pickNum].objectRects[wallNum].mapPoint[1] + 2].num) && 
                                !mapData.EnemyIndexes.Contains(tempMap.map[mapObjects[pickNum].objectRects[wallNum].mapPoint[0],
                                mapObjects[pickNum].objectRects[wallNum].mapPoint[1] + 2].num))
                            {
                                tempMap.map[mapObjects[pickNum].objectRects[wallNum].mapPoint[0],
                                mapObjects[pickNum].objectRects[wallNum].mapPoint[1] + 2].num = enemyType;

                                numEnemies++;
                            }
                        }
                        else if (randNum == 1) //Add enemy left
                        {
                            if (mapData.FloorIndexes.Contains(tempMap.map[mapObjects[pickNum].objectRects[wallNum].mapPoint[0],
                                mapObjects[pickNum].objectRects[wallNum].mapPoint[1] - 2].num) &&
                                !mapData.EnemyIndexes.Contains(tempMap.map[mapObjects[pickNum].objectRects[wallNum].mapPoint[0],
                                mapObjects[pickNum].objectRects[wallNum].mapPoint[1] - 2].num))
                            {
                                tempMap.map[mapObjects[pickNum].objectRects[wallNum].mapPoint[0],
                                mapObjects[pickNum].objectRects[wallNum].mapPoint[1] - 2].num = enemyType;

                                numEnemies++;
                            }
                        }
                        else if (randNum == 2)//Add enemy up
                        {
                            if (mapData.FloorIndexes.Contains(tempMap.map[mapObjects[pickNum].objectRects[wallNum].mapPoint[0] - 2,
                                mapObjects[pickNum].objectRects[wallNum].mapPoint[1]].num) &&
                                !mapData.EnemyIndexes.Contains(tempMap.map[mapObjects[pickNum].objectRects[wallNum].mapPoint[0] - 2,
                                mapObjects[pickNum].objectRects[wallNum].mapPoint[1]].num))
                            {
                                tempMap.map[mapObjects[pickNum].objectRects[wallNum].mapPoint[0] - 2,
                                mapObjects[pickNum].objectRects[wallNum].mapPoint[1]].num = enemyType;

                                numEnemies++;
                            }
                        }
                        else if(randNum == 3)//Add enemy Down
                        {
                            if (mapData.FloorIndexes.Contains(tempMap.map[mapObjects[pickNum].objectRects[wallNum].mapPoint[0] + 2,
                                mapObjects[pickNum].objectRects[wallNum].mapPoint[1]].num) &&
                                !mapData.EnemyIndexes.Contains(tempMap.map[mapObjects[pickNum].objectRects[wallNum].mapPoint[0] + 2,
                                mapObjects[pickNum].objectRects[wallNum].mapPoint[1]].num))
                            {
                                tempMap.map[mapObjects[pickNum].objectRects[wallNum].mapPoint[0] + 2,
                                mapObjects[pickNum].objectRects[wallNum].mapPoint[1]].num = enemyType;

                                numEnemies++;
                            }
                        }
                        break;
                    #endregion
                    #region RemoveEnemy
                    case MapEffectObjects.RemoveEnemy:
                        
                        int enemyPick = rand.Next(0, mapData.enemySpawnPoints.Count);

                        tempMap.map[mapData.enemySpawnPoints[enemyPick].mapPoint[0],
                            mapData.enemySpawnPoints[enemyPick].mapPoint[1]].num = 9;
                        mapData.EnemySpawns.Remove(new Vector2(mapData.enemySpawnPoints[enemyPick].Rectangle.X,
                            mapData.enemySpawnPoints[enemyPick].Rectangle.Y));
                        numEnemies--;
                        break;
                    #endregion
                    #region RemoveWall
                    case MapEffectObjects.RemoveWall:
                        int pick = rand.Next(0, numWalls);

                        int wall = rand.Next(0, mapObjects[pick].objectRects.Count);

                        tempMap.map[mapObjects[pick].objectRects[wall].mapPoint[0], 
                            mapObjects[pick].objectRects[wall].mapPoint[1]].num = 9;
                        mapData.WallTiles.Remove(new WallTiles(10, mapObjects[pick].objectRects[wall].rect));
                        break;
                    #endregion
                    #region SpeedBoost
                    case MapEffectObjects.SpeedBoost:

                        EnvironmentTile environmentTile = new EnvironmentTile(10, Rectangle.Empty, "");
                        int[] selectedPoint;

                        bool picked = false;
                        while(!picked)
                        {
                            Cell selectedCell = tempMap.map[rand.Next(4, tempMap.map.GetLength(0) - 4), rand.Next(4, tempMap.map.GetLength(1) - 4)];
                            if (mapData.FloorIndexes.Contains(selectedCell.num)) //If random selection is a floor tile
                            {

                                int emptySpaces = 0;
                                switch (environmentDirection)
                                {
                                    case "right":
                                        for (int i = 1; i <= 3; i++)
                                        {
                                            if (mapData.FloorIndexes.Contains(tempMap.map[selectedCell.mapPoint[0], selectedCell.mapPoint[1] + i].num) || mapData.EnemyIndexes.Contains(tempMap.map[selectedCell.mapPoint[0], selectedCell.mapPoint[1] + i].num) ||
                                                tempMap.map[selectedCell.mapPoint[0], selectedCell.mapPoint[1]].num == 62 || tempMap.map[selectedCell.mapPoint[0], selectedCell.mapPoint[1] + i].num == 59 ||
                                                tempMap.map[selectedCell.mapPoint[0], selectedCell.mapPoint[1]].num == 60 || tempMap.map[selectedCell.mapPoint[0], selectedCell.mapPoint[1] + i].num == 63)
                                            {
                                                emptySpaces++;
                                            }
                                        }
                                        break;
                                    case "left":
                                        for (int i = 1; i <= 3; i++)
                                        {
                                            if (mapData.FloorIndexes.Contains(tempMap.map[selectedCell.mapPoint[0], selectedCell.mapPoint[1] - i].num) || mapData.EnemyIndexes.Contains(tempMap.map[selectedCell.mapPoint[0], selectedCell.mapPoint[1] - i].num) ||
                                                tempMap.map[selectedCell.mapPoint[0], selectedCell.mapPoint[1]].num == 62 || tempMap.map[selectedCell.mapPoint[0], selectedCell.mapPoint[1] - i].num == 59 ||
                                                tempMap.map[selectedCell.mapPoint[0], selectedCell.mapPoint[1]].num == 60 || tempMap.map[selectedCell.mapPoint[0], selectedCell.mapPoint[1] - i].num == 63)
                                            {
                                                emptySpaces++;
                                            }
                                        }
                                        break;
                                    case "up":
                                        for (int i = 1; i <= 3; i++)
                                        {
                                            if (mapData.FloorIndexes.Contains(tempMap.map[selectedCell.mapPoint[0] - i, selectedCell.mapPoint[1]].num) || mapData.EnemyIndexes.Contains(tempMap.map[selectedCell.mapPoint[0] - i, selectedCell.mapPoint[1]].num) ||
                                                tempMap.map[selectedCell.mapPoint[0] - i, selectedCell.mapPoint[1]].num == 62 || tempMap.map[selectedCell.mapPoint[0] - i, selectedCell.mapPoint[1]].num == 59 ||
                                                tempMap.map[selectedCell.mapPoint[0] - i, selectedCell.mapPoint[1]].num == 60 || tempMap.map[selectedCell.mapPoint[0] - i, selectedCell.mapPoint[1]].num == 63)
                                            {
                                                emptySpaces++;
                                            }
                                        }
                                        break;
                                    case "down":
                                        for (int i = 1; i <= 3; i++)
                                        {
                                            if (mapData.FloorIndexes.Contains(tempMap.map[selectedCell.mapPoint[0] + i, selectedCell.mapPoint[1]].num) || mapData.EnemyIndexes.Contains(tempMap.map[selectedCell.mapPoint[0] + i, selectedCell.mapPoint[1]].num) || 
                                                tempMap.map[selectedCell.mapPoint[0] + i, selectedCell.mapPoint[1]].num == 62 || tempMap.map[selectedCell.mapPoint[0] + i, selectedCell.mapPoint[1]].num == 59 ||
                                                tempMap.map[selectedCell.mapPoint[0] + i, selectedCell.mapPoint[1]].num == 60 || tempMap.map[selectedCell.mapPoint[0] + i, selectedCell.mapPoint[1]].num == 63)
                                            {
                                                emptySpaces++;
                                            }
                                        }
                                        break;
                                }
                                if(emptySpaces == 3)
                                {
                                    picked = true;
                                    selectedPoint = selectedCell.mapPoint;
                                    if (environmentDirection == "left" || environmentDirection == "right")
                                    {
                                        environmentTile = new EnvironmentTile(62,
                                            new Rectangle((int)(mapData.ScreenSize.X * mapData.levelInX) + (selectedPoint[1] * 64),
                                           (selectedPoint[0] * 64) - (int)(mapData.ScreenSize.Y * mapData.levelInY), 64, 64), environmentDirection);//62 is the speedBoost
                                        environmentTile.mapPoint = selectedPoint;
                                        mapData.dMapDims[mapData.dMapDims.Count - 1][selectedPoint[0], selectedPoint[1]] = 62;
                                    }
                                    else if (environmentDirection == "up" || environmentDirection == "down")
                                    {
                                        environmentTile = new EnvironmentTile(59,
                                             new Rectangle((int)(mapData.ScreenSize.X * mapData.levelInX) + (selectedPoint[1] * 64),
                                           (selectedPoint[0] * 64) - (int)(mapData.ScreenSize.Y * mapData.levelInY), 64, 64), environmentDirection);//59 is the speedBoost
                                        environmentTile.mapPoint = selectedPoint;
                                        mapData.dMapDims[mapData.dMapDims.Count - 1][selectedPoint[0], selectedPoint[1]] = 59;
                                    }
                                }
                            }
                        }

                        mapData.AddEnvironmentTile(environmentTile);
                        break;
                    #endregion
                    #region DamageOverTime
                    case MapEffectObjects.DamageOverTime:
                        EnvironmentTile environmentTile1 = new EnvironmentTile(10, Rectangle.Empty, "");
                        int[] selectedPoint1;

                        bool picked1 = false;
                        while (!picked1)
                        {
                            Cell selectedCell = tempMap.map[rand.Next(1, tempMap.map.GetLength(0) - 1), rand.Next(1, tempMap.map.GetLength(1) - 1)];
                            
                            if (mapData.FloorIndexes.Contains(selectedCell.num)) //If random selection is a floor tile
                            {

                                    picked1 = true;
                                    selectedPoint1 = selectedCell.mapPoint;
                                    if (environmentDirection == "left" || environmentDirection == "right")
                                    {
                                        environmentTile1 = new EnvironmentTile(63,
                                           new Rectangle((int)(mapData.ScreenSize.X * mapData.levelInX) + (selectedPoint1[1] * 64),
                                           (selectedPoint1[0] * 64) - (int)(mapData.ScreenSize.Y * mapData.levelInY), 64, 64), environmentDirection);//62 is the speedBoost
                                        environmentTile1.mapPoint = selectedPoint1;
                                        mapData.dMapDims[mapData.dMapDims.Count - 1][selectedPoint1[0], selectedPoint1[1]] = 63;
                                    }
                                    else if (environmentDirection == "up" || environmentDirection == "down")
                                    {
                                        environmentTile1 = new EnvironmentTile(60,
                                            new Rectangle((int)(mapData.ScreenSize.X * mapData.levelInX) + (selectedPoint1[1] * 64),
                                           (selectedPoint1[0] * 64) - (int)(mapData.ScreenSize.Y * mapData.levelInY), 64, 64), environmentDirection);//59 is the speedBoost
                                        environmentTile1.mapPoint = selectedPoint1;
                                        mapData.dMapDims[mapData.dMapDims.Count - 1][selectedPoint1[0], selectedPoint1[1]] = 60;
                                    }
                                
                            }
                        }
                       
                        mapData.AddEnvironmentTile(environmentTile1);
                        break;
                        #endregion
                }
            }

            private static int[,] ConvertMap(CellMap tempMap)
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

            private void GetMapObjects(TopDownMap mapData, Rectangle currBounds, CellMap tempMap)
            {
                List<Object> mapObject = new List<Object>();
                bool up = false, left = false, right = false, down = false, diag = false;
                for (int i = 0; i < mapData.WallTiles.Count; i++)
                {
                    if (currBounds.Contains(mapData.WallTiles[i].Rectangle) && mapData.WallTiles[i].mapPoint[0] != 0 && mapData.WallTiles[i].mapPoint[1] != 0
                        && mapData.WallTiles[i].mapPoint[0] != tempMap.map.GetLength(0) - 1 && mapData.WallTiles[i].mapPoint[1] != tempMap.map.GetLength(1) - 1 &&
                        tempMap.map[mapData.WallTiles[i].mapPoint[0], mapData.WallTiles[i].mapPoint[1]].fitness)
                    {
                        mapObject = new List<Object>();
                        mapObject.Add(new Object(mapData.WallTiles[i].Rectangle, new int[] { mapData.WallTiles[i].mapPoint[0], mapData.WallTiles[i].mapPoint[1] }));
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
                            //mapObject.Add(new Rectangle(mapData.WallTiles[i].Rectangle.X + (64 * xOffset), mapData.WallTiles[i].Rectangle.Y,
                            //    mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height));

                            mapObject.Add(new Object(new Rectangle(mapData.WallTiles[i].Rectangle.X + (64 * xOffset), 
                                mapData.WallTiles[i].Rectangle.Y, mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height), 
                                new int[] { mapData.WallTiles[i].mapPoint[0], mapData.WallTiles[i].mapPoint[1] + xOffset }));

                            tempMap.map[mapData.WallTiles[i].mapPoint[0], mapData.WallTiles[i].mapPoint[1] + xOffset].fitness = false;
                            xOffset += 1;
                            right = true;
                        }
                        //Left
                        xOffset = 1;
                        while (mapData.WallTiles[i].mapPoint[1] - xOffset > 0 &&
                            mapData.WallIndexes.Contains(tempMap.map[mapData.WallTiles[i].mapPoint[0], mapData.WallTiles[i].mapPoint[1] - xOffset].num) &&
                            tempMap.map[mapData.WallTiles[i].mapPoint[0], mapData.WallTiles[i].mapPoint[1] - xOffset].fitness)
                        {
                            //mapObject.Add(new Rectangle(mapData.WallTiles[i].Rectangle.X - (64 * xOffset), mapData.WallTiles[i].Rectangle.Y,
                            //  mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height));

                            mapObject.Add(new Object(new Rectangle(mapData.WallTiles[i].Rectangle.X - (64 * xOffset),
                               mapData.WallTiles[i].Rectangle.Y, mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height),
                               new int[] { mapData.WallTiles[i].mapPoint[0], mapData.WallTiles[i].mapPoint[1] - xOffset }));

                            tempMap.map[mapData.WallTiles[i].mapPoint[0], mapData.WallTiles[i].mapPoint[1] - xOffset].fitness = false;
                            xOffset += 1;
                            left = true;
                        }
                        //Up and Down
                        //Up
                        int yOffset = 1;
                        while (mapData.WallTiles[i].mapPoint[0] - yOffset > 0
                            && mapData.WallIndexes.Contains(tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1]].num) &&
                            tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1]].fitness)
                        {
                            //mapObject.Add(new Rectangle(mapData.WallTiles[i].Rectangle.X, mapData.WallTiles[i].Rectangle.Y - (64 * yOffset),
                            //    mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height));

                            mapObject.Add(new Object(new Rectangle(mapData.WallTiles[i].Rectangle.X /*+ (64 * xOffset)*/,
                               mapData.WallTiles[i].Rectangle.Y - (64 * yOffset), mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height),
                               new int[] { mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1] /*+ xOffset*/ }));

                            tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1]].fitness = false;
                            yOffset += 1;
                            up = true;
                        }
                        //Down
                        yOffset = 1;
                        while (mapData.WallTiles[i].mapPoint[0] + yOffset < tempMap.map.GetLength(0) - 1 &&
                            mapData.WallIndexes.Contains(tempMap.map[mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1]].num) &&
                            tempMap.map[mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1]].fitness)
                        {
                            //mapObject.Add(new Rectangle(mapData.WallTiles[i].Rectangle.X, mapData.WallTiles[i].Rectangle.Y + (64 * yOffset),
                            //   mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height));

                            mapObject.Add(new Object(new Rectangle(mapData.WallTiles[i].Rectangle.X /*+ (64 * xOffset)*/,
                              mapData.WallTiles[i].Rectangle.Y + (64 * yOffset), mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height),
                              new int[] { mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1] /*+ xOffset*/ }));

                            tempMap.map[mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1]].fitness = false;
                            yOffset += 1;
                            down = true;
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
                            //mapObject.Add(new Rectangle(mapData.WallTiles[i].Rectangle.X + (64 * xOffset), mapData.WallTiles[i].Rectangle.Y + (64 * yOffset),
                            //  mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height));

                            mapObject.Add(new Object(new Rectangle(mapData.WallTiles[i].Rectangle.X + (64 * xOffset),
                              mapData.WallTiles[i].Rectangle.Y + (64 * yOffset), mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height),
                              new int[] { mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1] + xOffset }));

                            tempMap.map[mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1] + xOffset].fitness = false;
                            yOffset += 1;
                            xOffset += 1;
                            diag = true;
                            down = true;
                            right = true;
                        }

                        xOffset = 1;
                        yOffset = 1;

                        //Up and Right
                        while (mapData.WallTiles[i].mapPoint[1] + xOffset < tempMap.map.GetLength(1) - 1 &&
                           mapData.WallTiles[i].mapPoint[0] - yOffset > 0 &&
                           mapData.WallIndexes.Contains(tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1] + xOffset].num) &&
                           tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1] + xOffset].fitness)
                        {
                            //mapObject.Add(new Rectangle(mapData.WallTiles[i].Rectangle.X + (64 * xOffset), mapData.WallTiles[i].Rectangle.Y - (64 * yOffset),
                            // mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height));

                            mapObject.Add(new Object(new Rectangle(mapData.WallTiles[i].Rectangle.X + (64 * xOffset),
                              mapData.WallTiles[i].Rectangle.Y - (64 * yOffset), mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height),
                              new int[] { mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1] + xOffset }));

                            tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1] + xOffset].fitness = false;
                            yOffset += 1;
                            xOffset += 1;
                            diag = true;
                            up = true;
                            right = true;   
                        }

                        xOffset = 1;
                        yOffset = 1;

                        //Down and Left
                        while (mapData.WallTiles[i].mapPoint[0] + yOffset < tempMap.map.GetLength(0) - 1 &&
                            mapData.WallTiles[i].mapPoint[1] - xOffset > 0 &&
                            mapData.WallIndexes.Contains(tempMap.map[mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1] - xOffset].num) &&
                            tempMap.map[mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1] - xOffset].fitness)
                        {
                            //mapObject.Add(new Rectangle(mapData.WallTiles[i].Rectangle.X - (64 * xOffset), mapData.WallTiles[i].Rectangle.Y + (64 * yOffset),
                            //  mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height));

                            mapObject.Add(new Object(new Rectangle(mapData.WallTiles[i].Rectangle.X - (64 * xOffset),
                              mapData.WallTiles[i].Rectangle.Y + (64 * yOffset), mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height),
                              new int[] { mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1] - xOffset }));

                            tempMap.map[mapData.WallTiles[i].mapPoint[0] + yOffset, mapData.WallTiles[i].mapPoint[1] - xOffset].fitness = false;
                            yOffset += 1;
                            xOffset += 1;
                            diag = true;
                            down = true;
                            left = true;
                        }

                        xOffset = 1;
                        yOffset = 1;

                        //Up and Left
                        while (
                            mapData.WallTiles[i].mapPoint[0] - yOffset > 0 && mapData.WallTiles[i].mapPoint[1] - xOffset > 0 &&
                            mapData.WallIndexes.Contains(tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1] - xOffset].num) &&
                            tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1] - xOffset].fitness)
                        {
                            //mapObject.Add(new Rectangle(mapData.WallTiles[i].Rectangle.X - (64 * xOffset), mapData.WallTiles[i].Rectangle.Y - (64 * yOffset),
                            //  mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height));

                            mapObject.Add(new Object(new Rectangle(mapData.WallTiles[i].Rectangle.X - (64 * xOffset),
                              mapData.WallTiles[i].Rectangle.Y - (64 * yOffset), mapData.WallTiles[i].Rectangle.Width, mapData.WallTiles[i].Rectangle.Height),
                              new int[] { mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1] - xOffset }));

                            tempMap.map[mapData.WallTiles[i].mapPoint[0] - yOffset, mapData.WallTiles[i].mapPoint[1] - xOffset].fitness = false;
                            yOffset += 1;
                            xOffset += 1;
                            diag = true;
                            up = true;
                            left = true;
                        }
                        #endregion

                        if (mapObjects.Contains(new MapObject(mapObject, "Wall")) == false && mapObject.Count >= 2)
                        {
                            if(diag && up && right || diag && up && left || diag && right && down || diag && right && up)
                            {
                                if(mapObjects.Contains(new MapObject(mapObject, "Wall", "Diag")) == false)
                                {
                                    mapObjects.Add(new MapObject(mapObject, "Wall", "Diag"));
                                }
                              
                            }
                            else if(up && right || up && left || down && right || down && left)
                            {
                                if(mapObjects.Contains(new MapObject(mapObject, "Wall", "LShape")) == false)
                                {
                                    mapObjects.Add(new MapObject(mapObject, "Wall", "LShape"));
                                }
                               
                            }
                            else if(right || left)
                            {
                                if(mapObjects.Contains(new MapObject(mapObject, "Wall", "Horizontal")) == false)
                                {
                                    mapObjects.Add(new MapObject(mapObject, "Wall", "Horizontal"));
                                }
                                
                            }
                            else if(up || down)
                            {
                                if(mapObjects.Contains(new MapObject(mapObject, "Wall", "Vertical")) == false)
                                {
                                    mapObjects.Add(new MapObject(mapObject, "Wall", "Vertical"));
                                }
                               
                            }
                            
                        }
                            
                        //mapObjects.Add(mapObject);
                        up = false;
                        down = false;
                        left = false;
                        right = false;
                        diag = false;

                    }
                }

               

                List<Object> enemyLoc = new List<Object>();
                //int i = 0;
                foreach(EnemySpawn enemySpawnPoint in mapData.enemySpawnPoints)
                {
                    enemyLoc = new List<Object>();
                    if (currBounds.Contains(enemySpawnPoint.Rectangle))
                    {
                        enemyLoc.Add(new Object(enemySpawnPoint.Rectangle,
                            enemySpawnPoint.mapPoint));
                        mapObjects.Add(new MapObject(enemyLoc, "Enemy"));
                    }
                    //i++;
                }
            }
        }

        class MapObject
        {
            // int index;
            public List<Object> objectRects;
            public string id;
            public string wallId;


            public MapObject(List<Object> rects, string id, string wallId = "horizonal")
            {
                //this.index = index;
                objectRects = rects;
                this.id = id;
                this.wallId = wallId;
            }
        }

        class Object
        {
            public Rectangle rect;
            public int[] mapPoint;

            public Object(Rectangle rect, int[] mapPoint)
            {
                this.rect = rect;
                this.mapPoint = mapPoint;
            }
        }

        class CellMap
        {
            public Cell[,] map;
            public int fitness;
            List<int> possNums;
            int minNum;
            int maxNum;

            public CellMap(int[,] map)
            {
                Cell[,] tempMap = new Cell[map.GetLength(0), map.GetLength(1)];
                possNums = new List<int>();
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    for (int x = 0; x < map.GetLength(1); x++)
                    {
                        if (!possNums.Contains(map[y, x]))
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

                for (int i = 0; i < possNums.Count; i++)
                {
                    if (minNum > possNums[i])
                    {
                        minNum = possNums[i];
                    }
                    if (maxNum < possNums[i])
                    {
                        maxNum = possNums[i];
                    }
                }
                this.map = tempMap;
                fitness = 0;

            }

            //Map build will need the indexes of the used tiles. 
            public CellMap CreateChromosome(int[,] map)
            {
                return new CellMap(map);
            }

        }

        struct Cell
        {
            public int num;
            public bool fitness;
            public int[] mapPoint;

            public Cell(int num, int[] point)
            {
                this.num = num;
                fitness = false;
                mapPoint = point;
            }
        }
    }
}

