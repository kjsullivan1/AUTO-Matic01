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

            enum MapEffectObjects { AddWall, AddEnemy, RemoveEnemy, RemoveWall }
            MapEffectObjects mapObjectEffect;
            public void ChooseEffectObject()
            {
                int num = rand.Next(0, 4);
                switch (num)
                {
                    case 0:
                        mapObjectEffect = MapEffectObjects.AddWall;
                        break;
                    case 1:
                        mapObjectEffect = MapEffectObjects.AddEnemy;
                        break;
                    case 2:
                        mapObjectEffect = MapEffectObjects.RemoveWall;
                        break;
                    case 3:
                        mapObjectEffect = MapEffectObjects.RemoveEnemy;
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
                int affectedObjects = rand.Next(5, 9);
                //int removeObjectChance = 10;
                //mapObjectEffect = MapEffectObjects.AddWall;



                mapLayoutEffect = MapEffectLayout.Horizontal;
                //Effect layout
                switch (mapLayoutEffect)
                {
                    case MapEffectLayout.Horizontal: //Flip the map horizontally (Flip the x)
                        int middleX = tempMap.map.GetLength(1) / 2;

                        for(int y = 1; y < tempMap.map.GetLength(0) - 1; y++)
                        {
                            for(int x = 1; x < tempMap.map.GetLength(1) - 1; x++)
                            {
                                if(mapData.WallIndexes.Contains(tempMap.map[y,x].num))
                                {
                                    tempMap.map[y, x].num = 9;
                                    //tempMap.map[Math.Abs(x - tempMap.map.GetLength(0)), Math.Abs(y - tempMap.map.GetLength(1))].num = 10;
                                }
                                if(mapData.EnemyIndexes.Contains(tempMap.map[y,x].num))
                                {
                                    tempMap.map[y, x].num = 9;
                                    //tempMap.map[y, x].num = 9;
                                    //tempMap.map[Math.Abs(x - tempMap.map.GetLength(0)), Math.Abs(y - tempMap.map.GetLength(1))].num = 11;
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
                        //case MapEffectLayout.Vertical:
                        //    foreach (MapObject obj in mapObjects)
                        //    {
                        //        for (int i = 0; i < obj.objectRects.Count; i++)
                        //        {
                        //            tempMap.map[obj.objectRects[i].mapPoint[0], obj.objectRects[i].mapPoint[1]].num = 9;

                        //            tempMap.map[tempMap.map.GetLength(0) - obj.objectRects[i].mapPoint[0],
                        //                (obj.objectRects[i].mapPoint[1])].num = 10;
                        //            //tempMap.map[obj.objectRects[]
                        //        }
                        //    }
                        //    break;
                }
                //Effect objects
                for (int i = 0; i < affectedObjects; i++)
                {
                    EffectObjects(mapData, tempMap, numWalls);
                    ChooseEffectObject();
                }

                //Remove everything to test
                foreach (MapObject obj in mapObjects)
                {
                    for (int j = 0; j < obj.objectRects.Count; j++)
                    {

                        foreach (WallTiles tile1 in mapData.WallTiles)
                        {
                            if (tile1.Rectangle == obj.objectRects[j].rect)
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

            private void EffectObjects(TopDownMap mapData, Chromosome tempMap, int numWalls)
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
                                    //Object farLeft = mapObjects[num].objectRects[0];
                                    //for (int k = 0; k < mapObjects[num].objectRects.Count; k++)
                                    //{
                                    //    if (farLeft.rect.X > mapObjects[num].objectRects[k].rect.X)
                                    //    {
                                    //        farLeft = mapObjects[num].objectRects[k];
                                    //    }
                                    //}

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
                                    //Object farLeft = mapObjects[num].objectRects[0];
                                    //for (int k = 0; k < mapObjects[num].objectRects.Count; k++)
                                    //{
                                    //    if (farLeft.rect.X > mapObjects[num].objectRects[k].rect.X)
                                    //    {
                                    //        farLeft = mapObjects[num].objectRects[k];
                                    //    }
                                    //}

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
                        //switch(mapObjects[pick].wallId)
                        //{
                        //    case "Horizontal":
                        //        if (rand.Next(0, 101) < 50) //Choose right or left... less than 50 is left and > is right
                        //        {
                        //            Object FarLeft = mapObjects[pick].objectRects[0];
                        //            for (int k = 0; k < mapObjects[pick].objectRects.Count; k++)
                        //            {
                        //                if (FarLeft.rect.X > mapObjects[pick].objectRects[k].rect.X)
                        //                {
                        //                    FarLeft = mapObjects[pick].objectRects[k];
                        //                }
                        //            }

                        //            if (mapData.WallIndexes.Contains(tempMap.map[FarLeft.mapPoint[0],
                        //               FarLeft.mapPoint[1]].num))
                        //            {
                        //                tempMap.map[FarLeft.mapPoint[0],
                        //                FarLeft.mapPoint[1]].num = 9;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            Object farRight = mapObjects[pick].objectRects[0];
                        //            for (int k = 0; k < mapObjects[pick].objectRects.Count; k++)
                        //            {
                        //                if (farRight.rect.X < mapObjects[pick].objectRects[k].rect.X)
                        //                {
                        //                    farRight = mapObjects[pick].objectRects[k];
                        //                }
                        //            }

                        //            if (mapData.FloorIndexes.Contains(tempMap.map[farRight.mapPoint[0],
                        //               farRight.mapPoint[1]].num))
                        //            {
                        //                tempMap.map[farRight.mapPoint[0],
                        //                farRight.mapPoint[1]].num = 9;
                        //            }
                        //        }
                        //        break;
                        //    case "Vertical":
                        //        if (rand.Next(0, 101) < 50) //Choose up or down... less than 50 is up and > is down
                        //        {
                        //            Object farUp = mapObjects[pick].objectRects[0];
                        //            for (int k = 0; k < mapObjects[pick].objectRects.Count; k++)
                        //            {
                        //                if (farUp.rect.Y > mapObjects[pick].objectRects[k].rect.Y)
                        //                {
                        //                    farUp = mapObjects[pick].objectRects[k];
                        //                }
                        //            }

                        //            if (mapData.FloorIndexes.Contains(tempMap.map[farUp.mapPoint[0] - 1,
                        //               farUp.mapPoint[1]].num))
                        //            {
                        //                tempMap.map[farUp.mapPoint[0] - 1,
                        //                farUp.mapPoint[1]].num = 10;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            Object farDown = mapObjects[pick].objectRects[0];
                        //            for (int k = 0; k < mapObjects[pick].objectRects.Count; k++)
                        //            {
                        //                if (farDown.rect.Y < mapObjects[pick].objectRects[k].rect.Y)
                        //                {
                        //                    farDown = mapObjects[pick].objectRects[k];
                        //                }
                        //            }

                        //            if (mapData.FloorIndexes.Contains(tempMap.map[farDown.mapPoint[0] + 1,
                        //               farDown.mapPoint[1]].num))
                        //            {
                        //                tempMap.map[farDown.mapPoint[0] + 1,
                        //                farDown.mapPoint[1]].num = 10;
                        //            }
                        //        }
                        //        break;
                        //    case "Diag":
                        //        int num1 = rand.Next(0, 101);
                        //        if (num1 < 50) //Chooses to add a diag block 1/4 chance of being in same dir, 1/4 not happening
                        //        {
                        //            Object chosenObj;
                        //            if (rand.Next(0, 2) < 1)
                        //            {
                        //                chosenObj = mapObjects[pick].objectRects[0];//Far left
                        //                for (int k = 0; k < mapObjects[pick].objectRects.Count; k++)
                        //                {
                        //                    if (chosenObj.rect.X > mapObjects[pick].objectRects[k].rect.X)
                        //                    {
                        //                        chosenObj = mapObjects[pick].objectRects[k];
                        //                    }
                        //                }
                        //            }
                        //            else
                        //            {
                        //                chosenObj = mapObjects[pick].objectRects[0];// Far Right
                        //                for (int k = 0; k < mapObjects[pick].objectRects.Count; k++)
                        //                {
                        //                    if (chosenObj.rect.X < mapObjects[pick].objectRects[k].rect.X)
                        //                    {
                        //                        chosenObj = mapObjects[pick].objectRects[k];
                        //                    }
                        //                }
                        //            }
                        //            //Object farLeft = mapObjects[num].objectRects[0];
                        //            //for (int k = 0; k < mapObjects[num].objectRects.Count; k++)
                        //            //{
                        //            //    if (farLeft.rect.X > mapObjects[num].objectRects[k].rect.X)
                        //            //    {
                        //            //        farLeft = mapObjects[num].objectRects[k];
                        //            //    }
                        //            //}

                        //            if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj.mapPoint[0] - 1,
                        //               chosenObj.mapPoint[1] - 1].num))
                        //            {
                        //                tempMap.map[chosenObj.mapPoint[0] - 1,
                        //                chosenObj.mapPoint[1] - 1].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj.mapPoint[0] + 1,
                        //               chosenObj.mapPoint[1] - 1].num))
                        //            {
                        //                tempMap.map[chosenObj.mapPoint[0] + 1,
                        //                chosenObj.mapPoint[1] - 1].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj.mapPoint[0] - 1,
                        //              chosenObj.mapPoint[1] + 1].num))
                        //            {
                        //                tempMap.map[chosenObj.mapPoint[0] - 1,
                        //                chosenObj.mapPoint[1] + 1].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj.mapPoint[0] + 1,
                        //              chosenObj.mapPoint[1] + 1].num))
                        //            {
                        //                tempMap.map[chosenObj.mapPoint[0] + 1,
                        //                chosenObj.mapPoint[1] + 1].num = 10;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            Object chosenObj1;
                        //            if (rand.Next(0, 2) < 1)
                        //            {
                        //                chosenObj1 = mapObjects[pick].objectRects[0];//Far up
                        //                for (int k = 0; k < mapObjects[pick].objectRects.Count; k++)
                        //                {
                        //                    if (chosenObj1.rect.Y > mapObjects[pick].objectRects[k].rect.Y)
                        //                    {
                        //                        chosenObj1 = mapObjects[pick].objectRects[k];
                        //                    }
                        //                }
                        //            }
                        //            else
                        //            {
                        //                chosenObj1 = mapObjects[pick].objectRects[0];// Far down
                        //                for (int k = 0; k < mapObjects[pick].objectRects.Count; k++)
                        //                {
                        //                    if (chosenObj1.rect.Y < mapObjects[pick].objectRects[k].rect.Y)
                        //                    {
                        //                        chosenObj1 = mapObjects[pick].objectRects[k];
                        //                    }
                        //                }
                        //            }
                        //            //Object farLeft = mapObjects[num].objectRects[0];
                        //            //for (int k = 0; k < mapObjects[num].objectRects.Count; k++)
                        //            //{
                        //            //    if (farLeft.rect.X > mapObjects[num].objectRects[k].rect.X)
                        //            //    {
                        //            //        farLeft = mapObjects[num].objectRects[k];
                        //            //    }
                        //            //}

                        //            if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj1.mapPoint[0] - 1,
                        //               chosenObj1.mapPoint[1] - 1].num))
                        //            {
                        //                tempMap.map[chosenObj1.mapPoint[0] - 1,
                        //                chosenObj1.mapPoint[1] - 1].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj1.mapPoint[0] + 1,
                        //               chosenObj1.mapPoint[1] - 1].num))
                        //            {
                        //                tempMap.map[chosenObj1.mapPoint[0] + 1,
                        //                chosenObj1.mapPoint[1] - 1].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj1.mapPoint[0] - 1,
                        //              chosenObj1.mapPoint[1] + 1].num))
                        //            {
                        //                tempMap.map[chosenObj1.mapPoint[0] - 1,
                        //                chosenObj1.mapPoint[1] + 1].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj1.mapPoint[0] + 1,
                        //              chosenObj1.mapPoint[1] + 1].num))
                        //            {
                        //                tempMap.map[chosenObj1.mapPoint[0] + 1,
                        //                chosenObj1.mapPoint[1] + 1].num = 10;
                        //            }
                        //        }

                        //        break;
                        //    case "LShape":

                        //        Object chosenObj2;
                        //        int num2 = rand.Next(0, 4);
                        //        if (num2 == 0)
                        //        {
                        //            chosenObj2 = mapObjects[pick].objectRects[0];//Far up and right
                        //            for (int k = 0; k < mapObjects[pick].objectRects.Count; k++)
                        //            {
                        //                if (chosenObj2.rect.Y > mapObjects[pick].objectRects[k].rect.Y ||
                        //                    chosenObj2.rect.X < mapObjects[pick].objectRects[k].rect.X)
                        //                {
                        //                    chosenObj2 = mapObjects[pick].objectRects[k];
                        //                }
                        //            }

                        //            if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0] - 1,
                        //               chosenObj2.mapPoint[1]].num))
                        //            {
                        //                tempMap.map[chosenObj2.mapPoint[0] - 1,
                        //                chosenObj2.mapPoint[1]].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0] + 1,
                        //               chosenObj2.mapPoint[1]].num))
                        //            {
                        //                tempMap.map[chosenObj2.mapPoint[0] + 1,
                        //                chosenObj2.mapPoint[1]].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0],
                        //              chosenObj2.mapPoint[1] + 1].num))
                        //            {
                        //                tempMap.map[chosenObj2.mapPoint[0],
                        //                chosenObj2.mapPoint[1] + 1].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0],
                        //              chosenObj2.mapPoint[1] - 1].num))
                        //            {
                        //                tempMap.map[chosenObj2.mapPoint[0],
                        //                chosenObj2.mapPoint[1] - 1].num = 10;
                        //            }
                        //        }
                        //        else if (num2 == 1)
                        //        {
                        //            chosenObj2 = mapObjects[pick].objectRects[0];// Far down and right
                        //            for (int k = 0; k < mapObjects[pick].objectRects.Count; k++)
                        //            {
                        //                if (chosenObj2.rect.Y < mapObjects[pick].objectRects[k].rect.Y ||
                        //                     chosenObj2.rect.X < mapObjects[pick].objectRects[k].rect.X)
                        //                {
                        //                    chosenObj2 = mapObjects[pick].objectRects[k];
                        //                }
                        //            }

                        //            if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0] - 1,
                        //              chosenObj2.mapPoint[1]].num))
                        //            {
                        //                tempMap.map[chosenObj2.mapPoint[0] - 1,
                        //                chosenObj2.mapPoint[1]].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0] + 1,
                        //               chosenObj2.mapPoint[1]].num))
                        //            {
                        //                tempMap.map[chosenObj2.mapPoint[0] + 1,
                        //                chosenObj2.mapPoint[1]].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0],
                        //              chosenObj2.mapPoint[1] + 1].num))
                        //            {
                        //                tempMap.map[chosenObj2.mapPoint[0],
                        //                chosenObj2.mapPoint[1] + 1].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0],
                        //              chosenObj2.mapPoint[1] - 1].num))
                        //            {
                        //                tempMap.map[chosenObj2.mapPoint[0],
                        //                chosenObj2.mapPoint[1] - 1].num = 10;
                        //            }
                        //        }
                        //        else if (num2 == 2)
                        //        {
                        //            chosenObj2 = mapObjects[pick].objectRects[0];// Far up and left
                        //            for (int k = 0; k < mapObjects[pick].objectRects.Count; k++)
                        //            {
                        //                if (chosenObj2.rect.Y > mapObjects[pick].objectRects[k].rect.Y ||
                        //                     chosenObj2.rect.X > mapObjects[pick].objectRects[k].rect.X)
                        //                {
                        //                    chosenObj2 = mapObjects[pick].objectRects[k];
                        //                }
                        //            }

                        //            if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0] - 1,
                        //              chosenObj2.mapPoint[1]].num))
                        //            {
                        //                tempMap.map[chosenObj2.mapPoint[0] - 1,
                        //                chosenObj2.mapPoint[1]].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0] + 1,
                        //               chosenObj2.mapPoint[1]].num))
                        //            {
                        //                tempMap.map[chosenObj2.mapPoint[0] + 1,
                        //                chosenObj2.mapPoint[1]].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0],
                        //              chosenObj2.mapPoint[1] + 1].num))
                        //            {
                        //                tempMap.map[chosenObj2.mapPoint[0],
                        //                chosenObj2.mapPoint[1] + 1].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0],
                        //              chosenObj2.mapPoint[1] - 1].num))
                        //            {
                        //                tempMap.map[chosenObj2.mapPoint[0],
                        //                chosenObj2.mapPoint[1] - 1].num = 10;
                        //            }
                        //        }
                        //        else if (num2 == 3)
                        //        {
                        //            chosenObj2 = mapObjects[pick].objectRects[0];// Far down and left
                        //            for (int k = 0; k < mapObjects[pick].objectRects.Count; k++)
                        //            {
                        //                if (chosenObj2.rect.Y < mapObjects[pick].objectRects[k].rect.Y ||
                        //                     chosenObj2.rect.X > mapObjects[pick].objectRects[k].rect.X)
                        //                {
                        //                    chosenObj2 = mapObjects[pick].objectRects[k];
                        //                }
                        //            }

                        //            if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0] - 1,
                        //              chosenObj2.mapPoint[1]].num))
                        //            {
                        //                tempMap.map[chosenObj2.mapPoint[0] - 1,
                        //                chosenObj2.mapPoint[1]].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0] + 1,
                        //               chosenObj2.mapPoint[1]].num))
                        //            {
                        //                tempMap.map[chosenObj2.mapPoint[0] + 1,
                        //                chosenObj2.mapPoint[1]].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0],
                        //              chosenObj2.mapPoint[1] + 1].num))
                        //            {
                        //                tempMap.map[chosenObj2.mapPoint[0],
                        //                chosenObj2.mapPoint[1] + 1].num = 10;
                        //            }
                        //            else if (mapData.FloorIndexes.Contains(tempMap.map[chosenObj2.mapPoint[0],
                        //              chosenObj2.mapPoint[1] - 1].num))
                        //            {
                        //                tempMap.map[chosenObj2.mapPoint[0],
                        //                chosenObj2.mapPoint[1] - 1].num = 10;
                        //            }
                        //        }
                        //        break;
                        //}
                        //break;
                        #endregion
                }
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
                //for (int i = 0; i < mapData.EnemySpawns.Count; i++)
                //{
                //    enemyLoc = new List<WallObject>();
                //    if (currBounds.Contains(mapData.EnemySpawns[i]))
                //    {
                //        enemyLoc.Add(new WallObject(new Rectangle(mapData.EnemySpawns[i].ToPoint(), new Point(64, 64)), 
                //            new int[] { }));
                //        mapObjects.Add(new MapObject(enemyLoc, "Enemy"));
                //    }

                //}
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
}

