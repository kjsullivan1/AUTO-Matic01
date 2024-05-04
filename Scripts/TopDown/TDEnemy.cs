﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AUTO_Matic.TopDown;

namespace AUTO_Matic.Scripts.TopDown
{
    class TDEnemy
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private Rectangle rectangle;
        public float moveSpeed = 2f;
        private bool hasJumped = false;
        public bool isColliding = false;
        int visionLength = 7;
        float health = 3.5f;
        public float Health
        {
            get { return health; }
            set { health = value;
                if(health <= 0)
                {
                    health = 0;
                }
            }
        }


        public bool strafeUp;
        public bool strafeDown;
        public bool strafeLeft;
        public bool strafeRight;
        bool isStuck = false;

        bool blockedRight;
        bool blockedLeft;
        bool blockedTop;
        bool blockedBottom;

        bool inSight = false;
        //Vector2 spawnLoc = new Vector2(140, 280);

        bool xDiag = false;
        bool prioXStrafe = false;
        bool hasTarget = false;

        public int points = 0;
        int buffer = 64;
        int tileSize = 55;

        float pauseX = 0;
        float pauseY = 0;

        Tiles collidingTileX;
        Tiles collidingTileY;

        public List<Vector2> targets = new List<Vector2>();
        public Vector2 target = new Vector2();
        Vector2 tempTarget = new Vector2();

        public List<Rectangle> vision = new List<Rectangle>();

        public TopDownMap map;

        int tilesOut = 1;
        public Vector2 distToTravel = new Vector2();
        int[,] mapDims;

        bool fixer = true;
        bool stopper = false;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Rectangle Rectangle
        {
            get { return rectangle; }
        }

        int GetRow(int yMod)
        {
            return Math.Abs((int)((position.Y + yMod) / tileSize));


        }
        int GetCol(int xMod)
        {
            return Math.Abs((int)((position.X + xMod) / tileSize));
        }

        public TDEnemy(ContentManager Content, Vector2 spawnPos, TopDownMap map, int[,] mapDims)
        {
            texture = Content.Load<Texture2D>("TopDown/MapTiles/Tile11");
            this.map = map;
            this.mapDims = mapDims;
            position = spawnPos;

        }

        //List<Vector2> GetTargets(List<SkullTiles> targets)
        //{
        //    List<Vector2> pos = new List<Vector2>();

        //    foreach (SkullTiles tile in targets)
        //    {
        //        pos.Add(tile.GetPosition());
        //    }

        //    return pos;
        //}

        //public void GiveDims(int[,] mapDims)
        //{
        //    map.mapDims = mapDims;
        //}

        //void DetermineClosestTarget()
        //{
        //    Vector2 shortest = new Vector2();
        //    List<int> distances = new List<int>();

        //    for (int i = 0; i < targets.Count; i++)
        //    {

        //        if (i + 1 < targets.Count)
        //        {
        //            distances.Add(distForm(position, targets[i]));
        //        }


        //    }

        //    if (targets.Count > 0)
        //    {
        //        shortest = targets[0];
        //    }


        //    for (int i = 0; i < distances.Count; i++)
        //    {
        //        for (int j = i + 1; j < distances.Count; j++)
        //        {
        //            if (distances[i] > distances[j])
        //            {
        //                shortest = targets[j];
        //            }
        //        }
        //    }

        //    target = shortest;
        //    hasTarget = true;
        //}

        //public void SetTarget(List<SkullTiles> skulls)
        //{
        //    targets = GetTargets(skulls);
        //    DetermineClosestTarget();
        //}

        //public void SetTarget(List<SkullTiles> skulls, int[,] mapDims)
        //{
        //    targets = GetTargets(skulls);
        //    DetermineClosestTarget();
        //    this.mapDims = mapDims;
        //    map.Refresh(mapDims, tileSize);
        //}

        //public void SetTarget(List<SkullTiles> skulls, Vector2 collidedPos)
        //{
        //    targets = GetTargets(skulls);

        //    if (collidedPos == target)
        //    {
        //        SetTarget(skulls);
        //    }
        //    else
        //    {

        //    }
        //}
        public void SetTarget(Vector2 playerPos)
        {
            target = playerPos;
        }
        public void Upate(GameTime gameTime, Rectangle playerRect)
        {

            rectangle = new Rectangle((int)position.X, (int)position.Y, tileSize, tileSize);

            SetVision();

            blockedRight = false;
            blockedLeft = false;
            blockedTop = false;
            blockedBottom = false;

            for (int i = vision.Count - 1; i >= 0; i--)
            {
                bool removed = true;
                foreach (WallTiles tile in map.WallTiles)
                {
                    if (vision[i].Contains(tile.Rectangle))//If touching the right side
                    {
                        vision.Remove(vision[i]);
                        break;
                    }
                }


            }

            bool inSight = false;
            foreach(Rectangle rect in vision)
            {
                if(rect.Intersects(playerRect))
                {
                    inSight = true;
                }
                
            }
            if(inSight)
            {
                target = new Vector2(playerRect.X, playerRect.Y);
            }
            else
            {
                target = new Vector2(position.X, position.Y);
            }
            //for (int i = vision.Count - 1; i >= 0; i--)
            //{
            //    Vector2 temp = target;
            //    if (vision[i].Intersects(new Rectangle((int)target.X, (int)target.Y, 28, 28)))
            //    {
            //        target = temp;
            //        inSight = true;
            //        break;
            //    }
            //    else
            //    {
            //        inSight = false;
            //    }
            //}
            //if(!inSight)
            //{

            //}
            foreach (WallTiles tile in map.WallTiles)
            {

                if (rectangle.TouchLeftOf(tile.Rectangle)) //Right
                {
                    collidingTileX = tile;
                    blockedRight = true;
                    pauseY = 0;
                    if ((!strafeUp && !strafeDown))
                        strafeRight = false;

                    //position.X -= moveSpeed;
                }

                if (rectangle.TouchRightOf(tile.Rectangle)) //Left
                {
                    collidingTileX = tile;
                    blockedLeft = true;
                    if ((!strafeUp && !strafeDown))
                        strafeLeft = false;
                    //position.X += moveSpeed;
                    pauseY = 0;
                }


                if (Rectangle.TouchBottomOf(tile.Rectangle)) //Bottom
                {
                    collidingTileY = tile;
                    blockedTop = true;
                    if (!strafeLeft && !strafeRight)
                        strafeUp = false;

                    //position.Y -= moveSpeed;
                }


                if (rectangle.TouchTopOf(tile.Rectangle)) //Top
                {
                    collidingTileY = tile;
                    blockedBottom = true;
                    if (!strafeLeft && !strafeRight)
                        strafeDown = false;
                    //position.Y += moveSpeed;
                }

                //if (vision[8].Contains(tile.Rectangle)) //Bottom right
                //{
                //    if (!blockedRight && (!blockedBottom || !blockedTop) && ((int)position.X < (int)target.X) && (!strafeDown && !strafeUp))
                //    {
                //        pauseY = tileSize;

                //    }
                //}
                //if (vision[10].Contains(tile.Rectangle)) //Bottom left
                //{
                //    if (!blockedLeft && (!blockedBottom || !blockedTop) && (position.X > target.X) && (!strafeDown && !strafeUp))
                //    {
                //        pauseY = tileSize;
                //    }
                //}
                //if (vision[12].Contains(tile.Rectangle)) //Top right
                //{
                //    if (!blockedRight && (!blockedBottom || !blockedTop) && (position.X < target.X) && (!strafeDown && !strafeUp))
                //    {
                //        pauseY = tileSize;
                //    }
                //}
                //if (vision[14].Contains(tile.Rectangle))//Top left
                //{
                //    if (!blockedLeft && (!blockedBottom || !blockedTop) && (position.X > target.X) && (!strafeDown && !strafeUp))
                //    {
                //        pauseY = tileSize;
                //    }
                //}



            }

            if (((distToTravel.X >= 0 && blockedRight) || (distToTravel.X <= 0 && blockedLeft)) || !prioXStrafe || strafeUp || strafeDown || pauseY <= 0 || pauseX <= 0)
            {
                if (strafeUp && !prioXStrafe)
                {
                    stopper = false;
                    if (distToTravel.Y < 0 && !blockedTop)
                    {
                        distToTravel.Y += moveSpeed;
                        position.Y -= moveSpeed;
                        blockedBottom = false;
                        stopper = true;
                    }
                    else if (distToTravel.Y >= 0)
                    {
                        strafeUp = false;
                        distToTravel.Y = 0;
                        stopper = false;
                        fixer = true;
                        //pauseY = 32;
                    }

                    if (blockedTop)
                    {
                        stopper = false;
                        if (distToTravel.X != 0)
                        {

                        }
                        else
                        {
                            strafeUp = false;

                        }
                    }

                }
                else if (strafeDown && !prioXStrafe)
                {
                    stopper = false;
                    if (distToTravel.Y > 0 && !blockedBottom)
                    {
                        distToTravel.Y -= moveSpeed;
                        position.Y += moveSpeed;
                        blockedTop = false;
                        stopper = true;
                    }
                    else if (distToTravel.Y <= 0)
                    {
                        strafeDown = false;
                        distToTravel.Y = 0;
                        stopper = false;
                        fixer = true;
                        //pauseY = 32;
                    }

                    if (blockedBottom)
                    {
                        stopper = false;
                        if (distToTravel.X != 0)
                        {

                        }
                        else
                        {
                            strafeDown = false;
                        }
                    }
                }
                else
                {
                    stopper = false;
                    if (!prioXStrafe)
                    {
                        stopper = false;
                        if (strafeUp)
                        {
                            strafeUp = false;
                        }
                        if (strafeDown)
                        {
                            strafeDown = false;
                        }
                    }
                    if (!strafeLeft && !strafeRight)
                    {
                        fixer = true;
                    }
                }
            }
            if (((distToTravel.Y >= 0 && blockedBottom) || (distToTravel.Y <= 0 && blockedTop)) || prioXStrafe || strafeLeft || strafeRight)
            {
                fixer = false;
                if (strafeLeft)
                {
                    if (distToTravel.X < 0 && !blockedLeft)
                    {
                        distToTravel.X += moveSpeed;
                        position.X -= moveSpeed;
                        blockedRight = false;
                    }
                    else if (distToTravel.X >= 0)
                    {
                        strafeLeft = false;
                        if (!stopper)
                        {
                            fixer = true;
                        }
                        else if (!prioXStrafe)//Check the logic for this. Could be reason why pauses would happen with fixer = false
                        {
                            fixer = true;
                        }
                        prioXStrafe = false;
                        distToTravel.X = 0;
                        //fixer = true;
                        stopper = false;
                        //pauseX = 32;
                    }

                    if (blockedLeft)
                    {
                        prioXStrafe = false;
                        if (distToTravel.Y > 0 && blockedBottom)
                        {
                            fixer = true;

                        }
                        if (distToTravel.Y < 0 && blockedTop)
                        {
                            fixer = true;
                        }
                    }

                }
                else if (strafeRight)
                {
                    fixer = false;
                    if (distToTravel.X > 0 && !blockedRight)
                    {
                        distToTravel.X -= moveSpeed;
                        position.X += moveSpeed;
                        blockedLeft = false;
                    }
                    else if (distToTravel.X <= 0)
                    {
                        strafeRight = false;
                        if (!stopper)
                        {
                            fixer = true;
                        }
                        else if (!prioXStrafe)
                        {
                            fixer = true;
                        }
                        prioXStrafe = false;
                        distToTravel.X = 0;

                        //stopper = false;
                        //pauseX = 32;
                    }

                    if (blockedRight)
                    {
                        prioXStrafe = false;

                        if (distToTravel.Y > 0 && blockedBottom)
                        {
                            fixer = true;
                        }
                        if (distToTravel.Y < 0 && blockedTop)
                        {
                            fixer = true;
                        }

                    }
                }
                else
                {
                    fixer = true;
                    if (strafeRight)
                    {
                        strafeRight = false;
                    }
                    if (strafeLeft)
                    {
                        strafeLeft = false;
                    }
                }
            }


            if (fixer && !stopper)
            {
                if (pauseX > 0)
                    pauseX -= moveSpeed;
                else if (pauseY > 0)
                    pauseY -= moveSpeed;

                //Handle Right Vision
                else if ((int)position.X < (int)target.X)
                {
                    if (blockedLeft && blockedBottom && blockedRight)
                    {
                        distToTravel.Y = -WallUntilOpening(GetRow(0), GetCol(0), "up", "right", GetRow(0), 0) * (tileSize * 2);
                        strafeUp = true;
                    }
                    else if (blockedLeft && blockedTop && blockedRight)
                    {
                        distToTravel.Y = WallUntilOpening(GetRow(32), GetCol(0), "down", "right", 15 - GetRow(0), 0) * (tileSize * 2);
                        strafeDown = true;
                    }
                    else if (blockedTop && blockedRight && blockedBottom)
                    {
                        distToTravel.X = -WallUntilOpening(GetRow(0), GetCol(0), "left", "up", GetCol(0), 0) * (tileSize);
                        strafeLeft = true;
                    }
                    else if (blockedRight && blockedTop && ((int)position.Y == (int)target.Y))
                    {

                        if (map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] - 1, mapDims))
                             || map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] + 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            ChooseDiagStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "up", "right");

                            //ChooseDiagStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "down", "right");
                        }
                        else
                        {
                            //ChooseYStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "", );
                            //if(distToTravel.Y > 0) //Wants to go down
                            //{

                            //}
                            //if(distToTravel.Y < 0) //Wants to go up
                            //{ 
                            //    prioXStrafe = true;
                            //}
                            distToTravel.X = -WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "left", "up", collidingTileY.mapPoint[0] - 1, 0) * (tileSize * 2);
                            strafeLeft = true;

                            ChooseYStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1] + (int)(distToTravel.X / 32), "", "right");

                        }

                        //distToTravel.Y = (-tileSize * 2) - 1;
                        //strafeUp = true;


                    }
                    else if (blockedRight && blockedTop)
                    {
                        if ((int)position.Y > (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] - 1, mapDims))
                            || (int)position.Y > (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] + 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            xDiag = true;
                            //ChooseDiagStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "up", "right");

                        }
                        else if ((int)position.Y > (int)target.Y)
                        {
                            distToTravel.Y = -WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "up", "right", (collidingTileY.mapPoint[0]), 0) * tileSize;
                            strafeUp = true;
                            prioXStrafe = true;
                        }
                        if ((int)position.Y < (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] - 1, mapDims))
                             || (int)position.Y < (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0] + 1, collidingTileY.mapPoint[1], mapDims)))
                        {
                            xDiag = true;
                            //ChooseDiagStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "down", "right");
                        }
                        else if ((int)position.Y < (int)target.Y)
                        {
                            distToTravel.Y = WallUntilOpening(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "right", 15 - (collidingTileX.mapPoint[0]), 0) * (tileSize * 2);
                            strafeDown = true;
                        }
                    }
                    else if (blockedRight && blockedBottom && ((int)position.Y == (int)target.Y))
                    {

                        if (map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] - 1, mapDims))
                             || map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            ChooseDiagStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "down", "right");
                        }
                        else
                        {
                            distToTravel.X = -WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "left", "down", collidingTileY.mapPoint[1] + 1, 0) * (tileSize * 2);
                            strafeLeft = true;

                            ChooseYStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1] + (int)(distToTravel.X / 32), "", "right");
                            //distToTravel.Y = (tileSize * 2) + 1;
                            //strafeDown = true;
                            prioXStrafe = true;
                        }


                    }
                    else if (blockedRight && blockedBottom)
                    {
                        if ((int)position.Y > (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], mapDims) - 1)
                             || (int)position.Y > (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims))) // 
                        {
                            xDiag = true;
                            //ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "right");
                        }
                        else if ((int)position.Y > (int)target.Y)
                        {
                            //distToTravel.Y = WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "down", "right", 15 - collidingTileY.mapPoint[0], 0) * (tileSize);
                            //strafeDown = true;
                            //prioXStrafe = true;
                        }
                        if ((int)position.Y < (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], mapDims) - 1)
                             || (int)position.Y < (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims)))// 
                        {
                            xDiag = true;
                            //ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "right");
                        }
                        else if ((int)position.Y < (int)target.Y)
                        {
                            distToTravel.Y = -WallUntilOpening(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "right", collidingTileX.mapPoint[0], 0) * (tileSize * 2);
                            strafeUp = true;
                        }

                    }
                    else if (blockedRight && ((int)position.Y == (int)target.Y))
                    {
                        ChooseYStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "", "left");
                    }
                    else if (blockedRight && (!blockedTop && !blockedBottom))
                    {
                        //ChooseYStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "", "left");
                    }
                    else if (!blockedRight)
                    {
                        position.X += moveSpeed;
                        if (distToTravel.X < 0)
                        {
                            distToTravel.X = 0;
                        }
                        //blockedLeft = false;
                    }


                }

                //Handle Left Vision
                else if ((int)position.X > (int)target.X)
                {

                    if (blockedLeft && blockedBottom && blockedRight)
                    {
                        distToTravel.Y = -WallUntilOpening(GetRow(0), GetCol(0), "up", "left", GetRow(0), 0) * (tileSize * 2);
                        strafeUp = true;
                    }
                    else if (blockedLeft && blockedTop && blockedRight)
                    {
                        distToTravel.Y = WallUntilOpening(GetRow(32), GetCol(0), "down", "right", 15 - GetRow(0), 0) * (tileSize * 2);
                        strafeDown = true;
                    }
                    else if (blockedTop && blockedLeft && blockedBottom)
                    {
                        distToTravel.X = WallUntilOpening(GetRow(0), GetCol(32), "right", "up", 25 - GetCol(0), 0) * (tileSize * 2);
                        strafeRight = true;
                    }
                    else if (blockedLeft && blockedTop && ((int)position.Y == (int)target.Y))
                    {
                        if (map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], mapDims) + 1)
                             || map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] + 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            ChooseDiagStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "up", "left");
                        }
                        else
                        {
                            distToTravel.X = WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "right", "up", 25 - collidingTileY.mapPoint[1], 0) * (tileSize * 2);
                            strafeRight = true;

                            ChooseYStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1] + (int)(distToTravel.X / 32), "", "left");
                            prioXStrafe = true;
                        }

                        //distToTravel.Y = (-tileSize * 2) - 1;
                        //strafeUp = true;

                    }
                    else if (blockedLeft && blockedTop)
                    {
                        if ((int)position.Y > (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] + 1, mapDims))
                             || (int)position.Y > (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] + 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            xDiag = true;
                            //ChooseDiagStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "up", "left");
                        }
                        else if ((int)position.Y > (int)target.Y)
                        {
                            distToTravel.Y = -WallUntilOpening(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "left", collidingTileX.mapPoint[0], 0) * (tileSize * 2);
                            strafeUp = true;
                            prioXStrafe = true;
                        }
                        if ((int)position.Y < (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] + 1, mapDims))
                             || (int)position.Y < (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] + 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            xDiag = true;
                            //ChooseDiagStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "down", "left");
                        }
                        else if ((int)position.Y < (int)target.Y)
                        {
                            distToTravel.Y = WallUntilOpening(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "left", 15 - collidingTileX.mapPoint[0], 0) * (tileSize * 2);
                            strafeDown = true;
                        }
                    }
                    else if (blockedLeft && blockedBottom && ((int)position.Y == (int)target.Y))
                    {
                        if (map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], mapDims) + 1)
                             || map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            ChooseDiagStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "up", "left");
                        }
                        else
                        {
                            distToTravel.X = WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "right", "down", 25 - collidingTileY.mapPoint[1], 0) * (tileSize * 2);
                            strafeRight = true;

                            ChooseYStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1] + (int)(distToTravel.X / 32), "", "left");
                            prioXStrafe = true;
                        }
                        //distToTravel.Y = (tileSize * 2) + 1;
                        //strafeDown = true;


                    }
                    else if (blockedLeft && blockedBottom)
                    {
                        if ((int)position.Y > (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] + 1, mapDims))
                             && (int)position.Y > (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            xDiag = true;
                            //ChooseDiagStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "up", "left");
                        }
                        else if ((int)position.Y > (int)target.Y)
                        {
                            distToTravel.Y = -WallUntilOpening(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "left", collidingTileX.mapPoint[0], 0) * (tileSize);
                            strafeUp = true;
                        }
                        if ((int)position.Y < (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] + 1, mapDims))
                             && (int)position.Y < (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            xDiag = true;
                            //ChooseDiagStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "down", "left");
                        }
                        else if ((int)position.Y < (int)target.Y)
                        {

                            distToTravel.Y = WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "down", "left", 15 - collidingTileY.mapPoint[0], 0) * (tileSize * 2);
                            strafeDown = true;
                            prioXStrafe = true;
                        }


                    }
                    else if (blockedLeft && ((int)position.Y == (int)target.Y))
                    {
                        ChooseYStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "", "right");
                    }
                    else if (blockedLeft && (!blockedTop && !blockedBottom))
                    {
                        //ChooseYStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "", "right");
                    }
                    else if (!blockedLeft)
                    {
                        position.X -= moveSpeed;
                        if (distToTravel.X > 0)
                        {
                            distToTravel.X = 0;
                        }
                        //blockedRight = false;
                    }


                }

                //Handle Up vision
                if ((int)position.Y > (int)target.Y)
                {
                    if (blockedTop && blockedRight && blockedLeft)
                    {
                        distToTravel.Y = WallUntilOpening(GetRow(32), GetCol(0), "down", "left", 15 - GetRow(0), 0) * (tileSize * 2);
                        strafeDown = true;
                    }
                    else if (blockedTop && blockedRight && blockedBottom)
                    {
                        distToTravel.X = -WallUntilOpening(GetRow(0), GetCol(0), "left", "down", GetCol(0), 0) * (tileSize * 2);
                        strafeLeft = true;
                    }
                    else if (blockedTop && blockedLeft && blockedBottom)
                    {
                        distToTravel.X = WallUntilOpening(GetRow(0), GetCol(32), "right", "down", 25 - GetCol(0), 0) * (tileSize * 2);
                        strafeRight = true;
                    }
                    else if (blockedTop && blockedRight && ((int)position.X == (int)target.X))
                    {
                        if (map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] + 1, collidingTileX.mapPoint[1], mapDims))
                             || map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] - 1, mapDims)))
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "right");
                        }
                        else
                        {
                            distToTravel.Y = WallUntilOpening(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "right", 15 - collidingTileX.mapPoint[0], 0) * (tileSize * 2);
                            strafeDown = true;
                            ChooseXStrafe(collidingTileY.mapPoint[0] + (int)(distToTravel.Y / 32), collidingTileY.mapPoint[1], "");
                        }


                        //distToTravel.X = tileSize + 1;
                        //strafeRight = true;
                    }
                    else if (blockedTop && blockedRight)
                    {
                        if ((int)position.X < (int)target.X && xDiag)
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "right");
                            xDiag = false;
                        }
                        else if ((int)position.X < (int)target.X)
                        {
                            distToTravel.X = -WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "left", "up", collidingTileY.mapPoint[1], 0) * (tileSize * 2);
                            strafeLeft = true;
                        }
                        if ((int)position.X > (int)target.X && xDiag)
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "left");
                            xDiag = false;
                        }
                        else if ((int)position.X > (int)target.X)
                        {
                            //distToTravel.X = WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "right", "up", 25 - collidingTileY.mapPoint[1], 0) * (tileSize);
                            //strafeRight = true;
                        }
                    }
                    else if (blockedTop && blockedLeft && ((int)position.X == (int)target.X))
                    {
                        if (map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] + 1, collidingTileX.mapPoint[1], mapDims))
                             || map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] + 1, mapDims)))
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "left");
                        }
                        else
                        {
                            distToTravel.Y = WallUntilOpening(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "right", 15 - collidingTileX.mapPoint[0], 0) * (tileSize * 2);
                            strafeDown = true;
                            ChooseXStrafe(collidingTileY.mapPoint[0] + (int)(distToTravel.Y / 32), collidingTileY.mapPoint[1], "");
                        }

                        //distToTravel.X = -tileSize - 1;
                        //strafeLeft = true;

                    }
                    else if (blockedTop && blockedLeft)
                    {
                        if ((int)position.X > (int)target.X && xDiag)
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "left");
                            xDiag = false;
                        }
                        else if ((int)position.X > (int)target.X)
                        {
                            distToTravel.X = WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "right", "up", 25 - collidingTileY.mapPoint[1], 0) * (tileSize * 2);
                            strafeRight = true;
                        }
                        if ((int)position.X < (int)target.X && xDiag)
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "right");
                            xDiag = false;
                        }
                        else if ((int)position.X < (int)target.X)
                        {

                            //distToTravel.X = -WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "left", "up", collidingTileY.mapPoint[1], 0) * tileSize;
                            //strafeLeft = true;
                        }
                        xDiag = false;

                    }
                    else if (blockedTop && ((int)position.X == (int)target.X))
                    {
                        ChooseXStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "up");
                    }
                    else if (blockedTop && (!blockedLeft && !blockedRight))
                    {
                        //ChooseXStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "up");
                    }
                    else if (!blockedTop)
                    {
                        position.Y -= moveSpeed;
                        if (distToTravel.Y > 0)
                        {
                            distToTravel.Y = 0;
                        }
                        //blockedTop = false;
                    }
                }

                //Handle down vision
                else if ((int)position.Y < (int)target.Y)
                {
                    if (blockedBottom && blockedLeft && blockedRight)
                    {
                        distToTravel.Y = -WallUntilOpening(GetRow(0), GetCol(0), "up", "left", GetRow(0), 0) * tileSize;
                        strafeUp = true;
                    }
                    else if (blockedBottom && blockedLeft && blockedTop)
                    {
                        distToTravel.X = WallUntilOpening(GetRow(0), GetCol(32), "right", "down", 25 - GetCol(0), 0) * tileSize;
                        strafeRight = true;
                    }
                    else if (blockedBottom && blockedRight && blockedTop)
                    {
                        distToTravel.X = -WallUntilOpening(GetRow(0), GetCol(0), "left", "down", GetCol(0), 0) * tileSize;
                        strafeLeft = true;
                    }
                    else if (blockedBottom && blockedRight && ((int)position.X == (int)target.X))
                    {
                        if (map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims))
                             || map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] - 1, mapDims)))
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "right");
                        }
                        else
                        {
                            distToTravel.Y = -WallUntilOpening(collidingTileX.mapPoint[1], collidingTileX.mapPoint[1], "up", "right", collidingTileX.mapPoint[0], 0) * (tileSize * 2);
                            strafeUp = true;
                            ChooseXStrafe(collidingTileY.mapPoint[0] + (int)(distToTravel.Y / 32), collidingTileY.mapPoint[1], "");
                        }

                        //distToTravel.X = tileSize;
                        //strafeRight=true;

                    }
                    else if (blockedBottom && blockedRight)
                    {
                        if ((int)position.X < (int)target.X && xDiag)
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "right");
                            xDiag = false;
                        }
                        else if ((int)position.X < (int)target.X)
                        {
                            distToTravel.X = -WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "left", "down", collidingTileY.mapPoint[1], 0) * (tileSize * 2);
                            strafeLeft = true;
                        }
                        if ((int)position.X > (int)target.X && xDiag)
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "left");
                            xDiag = false;
                        }
                        else if ((int)position.X > (int)target.X)
                        {
                            //distToTravel.X = WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "right","down", 25 - collidingTileY.mapPoint[1], 0) * tileSize;
                            //strafeRight = true;
                        }


                    }
                    else if (blockedBottom && blockedLeft && ((int)position.X == (int)target.X))
                    {

                        if (map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims))
                             || map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] + 1, mapDims)))
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "left");
                            //if it is one or the other need to set a condition to check if the enemy needs to strafe X if still blocked bottom to the left or the right 
                            if (map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims))
                                && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] + 1, mapDims)) == false)
                            {
                                //strafe left
                                if (map.WallIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] + (int)(distToTravel.Y / 32), collidingTileX.mapPoint[1] + (int)(distToTravel.X / 32), mapDims)))
                                {

                                }
                            }
                            if (map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims)) == false
                                && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] + 1, mapDims)))
                            {
                                //strafe right 
                            }

                        }
                        else
                        {
                            distToTravel.Y = -WallUntilOpening(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "left", collidingTileX.mapPoint[0], 0) * (tileSize * 2);
                            strafeUp = true;
                            ChooseXStrafe(collidingTileY.mapPoint[0] + (int)(distToTravel.Y / 32), collidingTileY.mapPoint[1], "");
                        }
                        //distToTravel.X = -tileSize;
                        //strafeLeft = true;

                    }
                    else if (blockedBottom && blockedLeft)
                    {
                        if ((int)position.X < (int)target.X && xDiag)
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "right"); //Needs to check Down + Right and Up + Left ONLY
                            xDiag = false;
                        }
                        else if ((int)position.X < (int)target.X)
                        {
                            //distToTravel.X = -WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "left", "down", collidingTileY.mapPoint[1], 0) * tileSize;
                            //strafeLeft = true;
                        }
                        if ((int)position.X > (int)target.X && xDiag)
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "left");
                            xDiag = false;
                        }
                        else if ((int)position.X > (int)target.X)
                        {
                            distToTravel.X = WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "right", "down", 25 - collidingTileY.mapPoint[1], 0) * (tileSize * 2);
                            strafeRight = true;
                        }

                    }
                    else if (blockedBottom && ((int)position.X == (int)target.X))
                    {
                        ChooseXStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "down");
                    }
                    else if (blockedBottom && (!blockedLeft && !blockedRight))
                    {

                        //ChooseXStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "down");
                        //check if going to the right or left. if ChooseXStrafe didnt result in desired outcome. Restart
                    }
                    else if (!blockedBottom)
                    {
                        position.Y += moveSpeed;
                        if (distToTravel.Y < 0)
                        {
                            distToTravel.Y = 0;
                        }
                        //blockedBottom = false;
                    }

                }
            }




            #region OLD MOVEMENT (POSITION BASED)
            //if (strafeRight)
            //{
            //    if (distToTravel.X > 0)
            //    {
            //        position.X += moveSpeed;
            //        distToTravel.X -= moveSpeed;
            //    }
            //    else if (distToTravel.X <= 0)
            //    {
            //        strafeRight = false;
            //    }
            //    else if (isStuck)
            //    {
            //        strafeRight = false;
            //        strafeLeft = false;
            //        distToTravel.X = -tileSize;
            //        isStuck = false;
            //    }

            //}
            //else if ((position.X < target.X && !strafeLeft)
            //    && WallsFromPoint(tilesOut, Math.Abs((int)((position.Y) / tileSize)), Math.Abs((int)((position.X + 28) / tileSize)), 0) < 1) //right
            //{
            //    position.X += moveSpeed;
            //    blockedRight = false;
            //}
            //else if ((position.X < target.X && (int)position.Y == (int)target.Y)
            //    && WallsFromPoint(tilesOut, Math.Abs((int)((position.Y) / tileSize)), Math.Abs((int)((position.X + 28) / tileSize)), 0) >= 1)
            //{
            //    ChooseYStrafe(Math.Abs((int)((position.Y) / tileSize)), Math.Abs((int)((position.X + 28) / tileSize)), tilesOut);
            //}
            //else if (position.X < target.X && WallsFromPoint(tilesOut, Math.Abs((int)((position.Y) / tileSize)), Math.Abs((int)((position.X + 28) / tileSize)), 0) >= 1 && !strafeDown && !strafeUp)
            //{
            //    ChooseYStrafe(Math.Abs((int)((position.Y) / tileSize)), Math.Abs((int)((position.X + 28) / tileSize)), tilesOut);
            //}
            //else if (WallsFromPoint(tilesOut, Math.Abs((int)((position.Y) / tileSize)), Math.Abs((int)((position.X + 28) / tileSize)), 0) >= 1 && !strafeLeft && !strafeRight)
            //{
            //    position.X -= moveSpeed;
            //    blockedRight = true;
            //}

            ////else if(WallsFromCenter(2, Math.Abs((int)((position.Y) / tileSize)), Math.Abs((int)((position.X + 28) / tileSize)) ) > 4)
            ////{
            ////    tempTarget = target;

            ////    target = 

            ////}

            //if (strafeLeft)
            //{
            //    if (distToTravel.X < 0)
            //    {
            //        position.X -= moveSpeed;
            //        distToTravel.X += moveSpeed;
            //    }
            //    else if (distToTravel.X >= 0)
            //    {
            //        strafeLeft = false;
            //    }
            //    else if (isStuck)
            //    {
            //        strafeLeft = false;
            //        strafeRight = true;
            //        distToTravel.X = tileSize;
            //        isStuck = false;
            //    }
            //}
            //else if ((position.X > target.X && !strafeRight)
            //    && WallsFromPoint(tilesOut, Math.Abs((int)((position.Y) / tileSize)), Math.Abs((int)((position.X) / tileSize)), 1) < 1) //left
            //{
            //    position.X -= moveSpeed;
            //    blockedLeft = false;
            //}
            //else if ((position.X > target.X && (int)position.Y == (int)target.Y)
            //    && WallsFromPoint(tilesOut, Math.Abs((int)((position.Y) / tileSize)), Math.Abs((int)(position.X) / tileSize), 1) >= 1)
            //{
            //    ChooseYStrafe(Math.Abs((int)((position.Y) / tileSize)), Math.Abs((int)((position.X) / tileSize)), -tilesOut);
            //}
            //else if (position.X > target.X && WallsFromPoint(tilesOut, Math.Abs((int)((position.Y) / tileSize)), Math.Abs((int)((position.X) / tileSize)), 1) >= 1 && !strafeDown && !strafeUp)
            //{
            //    ChooseYStrafe(Math.Abs((int)((position.Y) / tileSize)), Math.Abs((int)((position.X) / tileSize)), -tilesOut);
            //}
            //else if (WallsFromPoint(tilesOut, Math.Abs((int)((position.Y) / tileSize)), Math.Abs((int)((position.X) / tileSize)), 1) >= 1 && !strafeRight && !strafeLeft)
            //{
            //    position.X += moveSpeed;
            //    blockedLeft = true;
            //}




            //if (strafeDown)
            //{
            //    if (distToTravel.Y > 0)
            //    {
            //        position.Y += moveSpeed;
            //        distToTravel.Y -= moveSpeed;
            //    }
            //    else if (distToTravel.Y <= 0)
            //    {
            //        strafeDown = false;
            //    }
            //    else if (isStuck)
            //    {
            //        strafeUp = true;
            //        strafeDown = false;
            //        distToTravel.Y = -tileSize;
            //        isStuck = false;
            //    }

            //}
            //else if ((position.Y < target.Y && !strafeUp
            //    && WallsFromPoint(tilesOut, Math.Abs((int)((position.Y)) / tileSize), Math.Abs((int)((position.X) / tileSize)), 2) < 1)) //down
            //{
            //    position.Y += moveSpeed;
            //    blockedBottom = false;
            //}
            //else if ((position.Y < target.Y && position.X == target.X)
            //    && WallsFromPoint(tilesOut, Math.Abs((int)((position.Y + 28)) / tileSize), Math.Abs((int)((position.X + 5) / tileSize)), 2) >= 1)
            //{
            //    ChooseXStrafe(Math.Abs((int)((position.Y + 28)) / tileSize), Math.Abs((int)((position.X + 5) / tileSize)), tilesOut);
            //}
            //else if (position.Y < target.Y && WallsFromPoint(tilesOut, Math.Abs((int)((position.Y + 28)) / tileSize), Math.Abs((int)((position.X + 5) / tileSize)), 2) >= 1 && !strafeLeft && !strafeRight)
            //{
            //    ChooseXStrafe(Math.Abs((int)((position.Y + 28)) / tileSize), Math.Abs((int)((position.X + 5) / tileSize)), tilesOut);
            //}
            //else if (WallsFromPoint(tilesOut, Math.Abs((int)((position.Y + 28)) / tileSize), Math.Abs((int)((position.X) / tileSize)), 2) >= 1 && (!strafeUp && !strafeDown))
            //{
            //    position.Y -= moveSpeed;
            //    blockedBottom = true;
            //}





            //if (strafeUp)
            //{
            //    if (distToTravel.Y < 0)
            //    {
            //        position.Y -= moveSpeed;
            //        distToTravel.Y += moveSpeed;
            //    }
            //    else if (distToTravel.Y >= 0)
            //    {
            //        strafeUp = false;
            //    }
            //    else if (isStuck)
            //    {
            //        strafeDown = true;
            //        strafeUp = false;
            //        distToTravel.Y = tileSize;
            //        isStuck = false;
            //    }

            //}
            //else if ((position.Y > target.Y && !strafeDown
            //    && WallsFromPoint(tilesOut, Math.Abs((int)(position.Y) / tileSize), Math.Abs((int)((position.X) / tileSize)), 3) < 1)) //up
            //{
            //    position.Y -= moveSpeed;
            //    blockedTop = false;
            //}
            //else if ((position.Y > target.Y && position.X == target.X)
            //    && WallsFromPoint(tilesOut, Math.Abs((int)(position.Y - 28) / tileSize), Math.Abs((int)((position.X + 5) / tileSize)), 3) >= 1)
            //{
            //    ChooseXStrafe(Math.Abs((int)(position.Y - 28) / tileSize), Math.Abs((int)((position.X + 5) / tileSize)), -tilesOut);
            //}
            //else if (position.Y > target.Y && WallsFromPoint(tilesOut, Math.Abs((int)(position.Y - 28) / tileSize), Math.Abs((int)((position.X + 5) / tileSize)), 3) >= 1 && !strafeLeft && !strafeRight)
            //{
            //    ChooseXStrafe(Math.Abs((int)(position.Y - 28) / tileSize), Math.Abs((int)((position.X + 5) / tileSize)), -tilesOut);
            //}
            //else if (WallsFromPoint(tilesOut, Math.Abs((int)(position.Y) / tileSize), Math.Abs((int)((position.X) / tileSize)), 3) >= 1 && !strafeDown && !strafeUp)
            //{
            //    position.Y += moveSpeed;
            //    blockedTop = true;
            //}
            #endregion




        }

        private void SetVision()
        {
            vision.Clear();
            //Right
            for (int i = 1; i < visionLength; i++)
            {
                vision.Add(new Rectangle(rectangle.X + (tileSize * i), rectangle.Y, tileSize, tileSize));// 0 1
            }
            //left
            for (int i = 1; i < visionLength; i++)
            {
                vision.Add(new Rectangle(rectangle.X + (tileSize * -i), rectangle.Y, tileSize, tileSize)); // 2 3
            }

            //Down
            for (int i = 1; i < visionLength; i++)
            {
                vision.Add(new Rectangle(rectangle.X, rectangle.Y + (tileSize * i), tileSize, tileSize)); // 4 5
            }
            //Up
            for (int i = -1; i > -visionLength; i--)
            {
                vision.Add(new Rectangle(rectangle.X, rectangle.Y + (tileSize * i), tileSize, tileSize)); // 6 7 
            }

            //Diag
            for (int i = 1; i < visionLength; i++) //Down to right
            {
                vision.Add(new Rectangle(rectangle.X + (tileSize * i), rectangle.Y + (tileSize * i), tileSize, tileSize)); // 8 9

            }
            for (int i = 2; i < visionLength; i++) //Fill in DownRight
            {
                vision.Add(new Rectangle(rectangle.X + (tileSize * i), (rectangle.Y + (tileSize * i)) - tileSize, tileSize, tileSize));
                vision.Add(new Rectangle((rectangle.X + (tileSize * i)) - tileSize, (rectangle.Y + (tileSize * i)), tileSize, tileSize));
            }
            for (int i = 3; i < visionLength; i++)
            {
                vision.Add(new Rectangle(rectangle.X + (tileSize * i), (rectangle.Y + (tileSize * i)) - (tileSize * (i - 1)), tileSize, tileSize));
                vision.Add(new Rectangle((rectangle.X + (tileSize * i)) - (tileSize * (i - 1)), (rectangle.Y + (tileSize * i)), tileSize, tileSize));
            }


            for (int i = 1; i < visionLength; i++) // Down to Left
            {
                vision.Add(new Rectangle(rectangle.X + (tileSize * -i), rectangle.Y + (tileSize * i), tileSize, tileSize)); // 10 11 
            }
            for (int i = 2; i < visionLength; i++) //Fill in DownLeft
            {
                vision.Add(new Rectangle(rectangle.X + (tileSize * -i), (rectangle.Y + (tileSize * i)) - tileSize, tileSize, tileSize));
                vision.Add(new Rectangle((rectangle.X + (tileSize * -i)) + tileSize, (rectangle.Y + (tileSize * i)), tileSize, tileSize));
            }
            for (int i = 3; i < visionLength; i++)
            {
                vision.Add(new Rectangle(rectangle.X + (tileSize * -i), (rectangle.Y + (tileSize * i)) - (tileSize * (i - 1)), tileSize, tileSize));
                vision.Add(new Rectangle((rectangle.X + (tileSize * -i)) + (tileSize * (i - 1)), (rectangle.Y + (tileSize * i)), tileSize, tileSize));
            }

            for (int i = 1; i < visionLength; i++) // Up to Right
            {
                vision.Add(new Rectangle(rectangle.X + (tileSize * i), rectangle.Y + (tileSize * -i), tileSize, tileSize)); // 12 13 
            }
            for (int i = 2; i < visionLength; i++) //Fill in UpRight
            {
                vision.Add(new Rectangle(rectangle.X + (tileSize * i), (rectangle.Y + (tileSize * -i)) + tileSize, tileSize, tileSize));
                vision.Add(new Rectangle((rectangle.X + (tileSize * i)) - tileSize, (rectangle.Y + (tileSize * -i)), tileSize, tileSize));
            }
            for (int i = 3; i < visionLength; i++)
            {
                vision.Add(new Rectangle(rectangle.X + (tileSize * i), (rectangle.Y + (tileSize * -i)) + (tileSize * (i - 1)), tileSize, tileSize));
                vision.Add(new Rectangle((rectangle.X + (tileSize * i)) - (tileSize * (i - 1)), (rectangle.Y + (tileSize * -i)), tileSize, tileSize));
            }

            for (int i = 1; i < visionLength; i++) //Up to Left
            {
                vision.Add(new Rectangle(rectangle.X + (tileSize * -i), rectangle.Y + (tileSize * -i), tileSize, tileSize)); // 14 15
            }
            for (int i = 2; i < visionLength; i++) //Fill in UpLeft
            {
                vision.Add(new Rectangle(rectangle.X + (tileSize * -i), (rectangle.Y + (tileSize * -i)) + tileSize, tileSize, tileSize));
                vision.Add(new Rectangle((rectangle.X + (tileSize * -i)) + tileSize, (rectangle.Y + (tileSize * -i)), tileSize, tileSize));
            }
            for (int i = 3; i < visionLength; i++)
            {
                vision.Add(new Rectangle(rectangle.X + (tileSize * -i), (rectangle.Y + (tileSize * -i)) + (tileSize * (i - 1)), tileSize, tileSize));
                vision.Add(new Rectangle((rectangle.X + (tileSize * -i)) + (tileSize * (i - 1)), (rectangle.Y + (tileSize * -i)), tileSize, tileSize));
            }

        }

        void ChooseDiagStrafe(int row, int col, string dirY, string dirX) //Need to send row and col of the tile to right or left or up and down depending on direction going and block direction
        {
            int numUp = 1;
            int numDown = 1;
            int numLeft = 1;
            int numRight = 1;

            Vector2 UpRight = new Vector2();
            Vector2 UpLeft = new Vector2();
            Vector2 DownRight = new Vector2();
            Vector2 DownLeft = new Vector2();
            UpRight = Vector2.One;
            UpLeft = Vector2.One;
            DownRight = Vector2.One;
            DownLeft = Vector2.One;

            bool endUp = false;
            bool endDown = false;
            bool endLeft = false;
            bool endRight = false;
            int tempCol = 0;
            int tempRow = 0;
            switch (dirX)
            {

                case "right":
                    numRight = 1;
                    switch (dirY)
                    {

                        case "up":
                            {
                                numUp = 1;
                                tempCol = col;
                                col--;
                                for (int i = row + 1; i < 15 && col >= 0; i++) //Down and Left
                                {
                                    //col--;

                                    if (map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {

                                        numDown++;
                                        numLeft++;
                                        DownLeft = new Vector2(numLeft, numDown);
                                    }
                                    else if (map.FloorIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {
                                        break;
                                    }


                                    if (i == 14 || col == 0 && map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {
                                        endDown = true;
                                        endLeft = true;
                                        break;
                                    }
                                    else if (map.WallIndexes.Contains(map.GetPoint(i, col + 1, mapDims)))
                                    {
                                        endDown = true;
                                        endLeft = true;
                                        break;
                                    }
                                    col--;
                                }
                                numDown = 1;
                                numLeft = 1;
                                col = tempCol;
                                col++;
                                for (int i = row - 1; i >= 0 && col < 25; i--)//Up and Right
                                {
                                    //col++;
                                    if (map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {

                                        numUp++;
                                        numRight++;
                                        UpRight = new Vector2(numRight, numUp);
                                    }
                                    else if (map.FloorIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {
                                        break;
                                    }


                                    if (i == 0 || col == 24 && map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {
                                        endUp = true;
                                        endRight = true;
                                        break;
                                    }
                                    else if (map.WallIndexes.Contains(map.GetPoint(i, col - 1, mapDims)))
                                    {
                                        endUp = true;
                                        endRight = true;
                                        break;
                                    }
                                    col++;
                                }
                                numRight = 1;
                                numUp = 1;
                                tempRow = row;
                                row--;
                                col = tempCol;
                                for (int i = col - 1; i >= 0 && row >= 0; i--)//Up and left
                                {
                                    //row--;
                                    if (map.WallIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        numUp++;
                                        numLeft++;
                                        UpLeft = new Vector2(numLeft, numUp);
                                    }
                                    else if (map.FloorIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        break;
                                    }

                                    if (i == 0 || row == 0 && map.WallIndexes.Contains(map.GetPoint(row, i, mapDims))) //Logic can be changed to move over obstacles and walls
                                    {

                                        endUp = true;
                                        endLeft = true;
                                        break;
                                    }
                                    else if (map.WallIndexes.Contains(map.GetPoint(row, i + 1, mapDims)))
                                    {

                                        endUp = true;
                                        endLeft = true;
                                        break;
                                    }
                                    row--;
                                }
                                numUp = 1;
                                numLeft = 1;
                                row = tempRow;
                                row++;
                                for (int i = col + 1; i < 25 && row < 15; i++) //Down and Right
                                {
                                    //row++;
                                    if (map.WallIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {
                                        //row++;
                                        numDown++;
                                        numRight++;
                                        DownRight = new Vector2(numRight, numDown);
                                    }
                                    else if (map.FloorIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        break;
                                    }

                                    if (i == 24 || row == 14 && map.WallIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        endDown = true;
                                        endRight = true;
                                        break;
                                    }
                                    else if (map.WallIndexes.Contains(map.GetPoint(row + 1, i, mapDims)))
                                    {

                                        endDown = true;
                                        endRight = true;
                                        break;
                                    }
                                    row++;
                                }

                            }

                            if (UpLeft.Length() > DownRight.Length()) // if left and up is more than right and down
                            {
                                if (endDown && endRight)
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize * 1.5f);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }


                            }
                            else if (DownRight.Length() > UpLeft.Length()) //if right and down is more than left and up
                            {
                                if (endUp && endLeft)
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }
                                else
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }

                            }
                            else if (UpLeft.Length() == DownRight.Length())
                            {
                                if (endDown && endRight)
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }
                                else if (endUp && endLeft)
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }
                                else
                                {
                                    Random rand = new Random();

                                    if (rand.Next(11) > 5)
                                    {
                                        distToTravel.Y = DownRight.Y * (tileSize);
                                        distToTravel.X = DownRight.X * (tileSize * 2);
                                        strafeDown = true;
                                        strafeRight = true;
                                    }
                                    else
                                    {
                                        distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                        distToTravel.X = -UpLeft.X * (tileSize);
                                        strafeUp = true;
                                        strafeLeft = true;
                                        prioXStrafe = true;
                                    }
                                }

                            }
                            else if (DownLeft.Length() > UpRight.Length())//Left and Down is more than Right and Up
                            {
                                if (endUp && endRight)
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }
                                else
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                            }
                            else if (UpRight.Length() > DownLeft.Length())//Right and Up is more than Left and Down
                            {
                                if (endDown && endLeft)
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }

                            }
                            else if (DownLeft.Length() == UpRight.Length())
                            {
                                if (endUp && endRight)
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }
                                else if (endDown && endLeft)
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                                else
                                {

                                    Random rand = new Random();

                                    if (rand.Next(11) > 5)
                                    {
                                        distToTravel.Y = -UpRight.Y * (tileSize);
                                        distToTravel.X = UpRight.X * (tileSize * 2);
                                        strafeUp = true;
                                        strafeRight = true;
                                        prioXStrafe = true;
                                    }
                                    else
                                    {
                                        distToTravel.X = -DownLeft.X * (tileSize * 2);
                                        distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                        strafeLeft = true;
                                        strafeDown = true;
                                    }
                                }

                            }

                            //SetStrafe(numUp, numDown, numLeft, numRight, dirX, dirY);
                            break;
                        case "down":

                            numDown = 1;
                            tempCol = col;
                            col--;
                            for (int i = row + 1; i < 15 && col >= 0; i++) //Down and Left
                            {
                                //col--;

                                if (map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {

                                    numDown++;
                                    numLeft++;
                                    DownLeft = new Vector2(numLeft, numDown);
                                }
                                else if (map.FloorIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {

                                    break;
                                }


                                if (i == 14 || col == 0 && map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {

                                    endDown = true;
                                    endLeft = true;
                                    break;
                                }
                                else if (map.WallIndexes.Contains(map.GetPoint(i, col - 1, mapDims)))
                                {

                                    endDown = true;
                                    endLeft = true;
                                    break;
                                }
                                col--;
                            }
                            numDown = 1;
                            numLeft = 1;
                            col = tempCol;
                            col++;
                            for (int i = row - 1; i >= 0 && col < 25; i--)//Up and Right
                            {
                                //col++;
                                if (map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {

                                    numUp++;
                                    numRight++;
                                    UpRight = new Vector2(numRight, numUp);
                                }
                                else if (map.FloorIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {

                                    break;
                                }


                                if (i == 0 || col == 24 && map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {

                                    endUp = true;
                                    endRight = true;
                                    break;
                                }
                                else if (map.WallIndexes.Contains(map.GetPoint(i, col - 1, mapDims)))
                                {
                                    endUp = true;
                                    endRight = true;
                                    break;
                                }
                                col++;
                            }
                            numUp = 1;
                            numRight = 1;
                            tempRow = row;
                            row--;
                            col = tempCol;
                            for (int i = col - 1; i >= 0 && row >= 0; i--)//Up and left
                            {
                                //row--;
                                if (map.WallIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {

                                    numUp++;
                                    numLeft++;
                                    UpLeft = new Vector2(numLeft, numUp);
                                }
                                else if (map.FloorIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {

                                    break;
                                }

                                if (i == 0 || row == 0 && map.WallIndexes.Contains(map.GetPoint(row, i, mapDims))) //Logic can be changed to move over obstacles and walls
                                {

                                    endUp = true;
                                    endLeft = true;
                                    break;
                                }
                                else if (map.WallIndexes.Contains(map.GetPoint(row, i + 1, mapDims)))
                                {

                                    endUp = true;
                                    endLeft = true;
                                    break;
                                }
                                row--;
                            }
                            numUp = 1;
                            numLeft = 1;
                            row = tempRow;
                            row++;
                            for (int i = col + 1; i < 25 && row < 15; i++) //Down and Right
                            {
                                //row++;
                                if (map.WallIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {

                                    numDown++;
                                    numRight++;
                                    DownRight = new Vector2(numRight, numDown);
                                }
                                else if (map.FloorIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {

                                    break;
                                }

                                if (i == 24 || row == 14 && map.WallIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {


                                    endDown = true;
                                    endRight = true;
                                    break;
                                }
                                else if (map.WallIndexes.Contains(map.GetPoint(row, i - 1, mapDims)))
                                {

                                    endDown = true;
                                    endRight = true;
                                    break;
                                }
                                row++;
                            }


                            if (DownLeft.Length() > UpRight.Length())//Left and Down is more than Right and Up
                            {
                                if (endRight && endUp)
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 3);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }
                                else
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }

                            }
                            else if (UpRight.Length() > DownLeft.Length())//Right and Up is more than Left and Down
                            {
                                if (endDown && endLeft)
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize * 2);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 1.5f);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }

                            }
                            else if (UpRight.Length() == DownLeft.Length())
                            {
                                if (endDown && endLeft)
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                                else if (endUp && endRight)
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }
                                else
                                {
                                    Random rand = new Random();

                                    if (rand.Next(11) > 5)
                                    {
                                        distToTravel.Y = -UpRight.Y * (tileSize);
                                        distToTravel.X = UpRight.X * (tileSize * 2);
                                        strafeUp = true;
                                        strafeRight = true;
                                        prioXStrafe = true;
                                    }
                                    else
                                    {
                                        distToTravel.X = -DownLeft.X * (tileSize * 2);
                                        distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                        strafeLeft = true;
                                        strafeDown = true;
                                    }
                                }

                            }
                            else if (UpLeft.Length() > DownRight.Length()) // if left and up is more than right and down
                            {
                                if (endDown && endRight)
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }


                            }
                            else if (DownRight.Length() > UpLeft.Length()) //if right and down is more than left and up
                            {
                                if (endUp && endLeft)
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }
                                else
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }

                            }
                            else if (DownRight.Length() == UpLeft.Length())
                            {

                                if (endLeft && endUp)
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }
                                else if (endRight && endDown)
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    Random rand = new Random();
                                    if (rand.Next(11) > 5)
                                    {
                                        distToTravel.Y = DownRight.Y * (tileSize);
                                        distToTravel.X = DownRight.X * (tileSize * 2);
                                        strafeDown = true;
                                        strafeRight = true;
                                    }
                                    else
                                    {
                                        distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                        distToTravel.X = -UpLeft.X * (tileSize);
                                        strafeUp = true;
                                        strafeLeft = true;
                                        prioXStrafe = true;
                                    }
                                }

                            }

                            //SetStrafe(numUp, numDown, numLeft, numRight, dirX, dirY);
                            break;
                    }
                    break;
                case "left":
                    numLeft = 1;
                    switch (dirY)
                    {
                        case "up":
                            {
                                numUp = 1;
                                tempCol = col;
                                col--;
                                for (int i = row + 1; i < 15 && col >= 0; i++) //Down and Left
                                {
                                    //col--;

                                    if (map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {

                                        numDown++;
                                        numLeft++;
                                        DownLeft = new Vector2(numLeft, numDown);
                                    }
                                    else if (map.FloorIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {

                                        break;
                                    }


                                    if (i == 14 || col == 0 && map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {

                                        endDown = true;
                                        endLeft = true;
                                        break;
                                    }
                                    else if (map.WallIndexes.Contains(map.GetPoint(i + 1, col, mapDims)))
                                    {

                                        endDown = true;
                                        endLeft = true;
                                        break;
                                    }
                                    col--;

                                }
                                numLeft = 1;
                                numDown = 1;
                                col = tempCol;
                                col++;
                                for (int i = row - 1; i >= 0 && col < 25; i--)//Up and Right
                                {
                                    //col++;
                                    if (map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {

                                        numUp++;
                                        numRight++;
                                        UpRight = new Vector2(numRight, numUp);
                                    }
                                    else if (map.FloorIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {

                                        break;
                                    }

                                    if (i == 0 || col == 24 && map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {

                                        endUp = true;
                                        endRight = true;
                                        break;
                                    }
                                    else if (map.WallIndexes.Contains(map.GetPoint(i, col + 1, mapDims)))
                                    {
                                        endUp = true;
                                        endRight = true;
                                        break;
                                    }

                                    col++;
                                }
                                numUp = 1;
                                numRight = 1;
                                tempRow = row;
                                row--;
                                col = tempCol;
                                for (int i = col - 1; i >= 0 && row >= 0; i--)//Up and left
                                {
                                    //row--;
                                    if (map.WallIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        numUp++;
                                        numLeft++;
                                        UpLeft = new Vector2(numLeft, numUp);
                                    }
                                    else if (map.FloorIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        break;
                                    }

                                    if (i == 0 || row == 0 && map.WallIndexes.Contains(map.GetPoint(row, i, mapDims))) //Logic can be changed to move over obstacles and walls
                                    {

                                        endUp = true;
                                        endLeft = true;
                                        break;
                                    }
                                    else if (map.WallIndexes.Contains(map.GetPoint(row, i + 1, mapDims)))
                                    {

                                        endUp = true;
                                        endLeft = true;
                                        break;
                                    }
                                    row--;
                                }
                                numUp = 1;
                                numLeft = 1;
                                row = tempRow;
                                row++;
                                for (int i = col + 1; i < 25 && row < 15; i++) //Down and Right
                                {
                                    //row++;
                                    if (map.WallIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        numDown++;
                                        numRight++;
                                        DownRight = new Vector2(numRight, numDown);
                                    }
                                    else if (map.FloorIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        break;
                                    }

                                    if (i == 24 || row == 14 && map.WallIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        endDown = true;
                                        endRight = true;
                                        break;
                                    }
                                    else if (map.WallIndexes.Contains(map.GetPoint(row, i + 1, mapDims)))
                                    {

                                        endDown = true;
                                        endRight = true;
                                        break;
                                    }
                                    row++;
                                }
                            }
                            numDown = 1;
                            if (DownLeft.Length() > UpRight.Length())//Left and Down is more than Right and Up
                            {
                                if (endUp && endRight)
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2);
                                    distToTravel.Y = DownLeft.Y * (tileSize);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }
                                else
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize * 2);
                                    distToTravel.X = UpRight.X * (tileSize);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }

                            }
                            else if (UpRight.Length() > DownLeft.Length())//Right and Up is more than Left and Down
                            {
                                if (endDown && endLeft)
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize * 2);
                                    distToTravel.X = UpRight.X * (tileSize);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2.5f);
                                    distToTravel.Y = DownLeft.Y * (tileSize);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }

                            }
                            else if (DownLeft.Length() == UpRight.Length())
                            {
                                if (endUp && endRight)
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2);
                                    distToTravel.Y = DownLeft.Y * (tileSize);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }
                                else if (endDown && endLeft)
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    Random rand = new Random();

                                    if (rand.Next(11) > 5)
                                    {
                                        distToTravel.Y = -UpRight.Y * (tileSize);
                                        distToTravel.X = UpRight.X * (tileSize * 2);
                                        strafeUp = true;
                                        strafeRight = true;
                                        prioXStrafe = true;
                                    }
                                    else
                                    {
                                        distToTravel.X = -DownLeft.X * (tileSize * 2);
                                        distToTravel.Y = DownLeft.Y * (tileSize);
                                        strafeLeft = true;
                                        strafeDown = true;
                                    }
                                }

                            }
                            else if (UpLeft.Length() > DownRight.Length()) // if left and up is more than right and down
                            {
                                if (endDown && endRight)
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }


                            }
                            else if (DownRight.Length() > UpLeft.Length()) //if right and down is more than left and up
                            {
                                if (endUp && endLeft)
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }
                                else
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }

                            }
                            else if (UpLeft.Length() == DownRight.Length())
                            {
                                if (endDown && endRight)
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }
                                else if (endUp && endLeft)
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }
                                else
                                {
                                    Random rand = new Random();

                                    if (rand.Next(11) > 5)
                                    {
                                        distToTravel.Y = DownRight.Y * (tileSize);
                                        distToTravel.X = DownRight.X * (tileSize * 2);
                                        strafeDown = true;
                                        strafeRight = true;
                                    }
                                    else
                                    {
                                        distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                        distToTravel.X = -UpLeft.X * (tileSize);
                                        strafeUp = true;
                                        strafeLeft = true;
                                        prioXStrafe = true;
                                    }
                                }


                            }
                            break;
                        case "down":
                            numDown = 1;
                            tempCol = col;
                            col--;
                            for (int i = row + 1; i < 15 && col >= 0; i++) //Down and Left
                            {
                                //col--;

                                if (map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {

                                    numDown++;
                                    numLeft++;
                                    DownLeft = new Vector2(numLeft, numDown);
                                }
                                else if (map.FloorIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {
                                    break;
                                }


                                if (i == 14 || col == 0 && map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {
                                    endDown = true;
                                    endLeft = true;
                                    break;
                                }
                                else if (map.WallIndexes.Contains(map.GetPoint(i, col + 1, mapDims)))
                                {

                                    endDown = true;
                                    endLeft = true;
                                    break;
                                }
                                col--;
                            }
                            numLeft = 1;
                            numDown = 1;
                            col = tempCol;
                            col++;
                            for (int i = row - 1; i >= 0 && col < 25; i--)//Up and Right
                            {
                                //col++;
                                if (map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {

                                    numUp++;
                                    numRight++;
                                    UpRight = new Vector2(numUp, numRight);
                                }
                                else if (map.FloorIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {
                                    break;
                                }


                                if (i == 0 || col == 24 && map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {
                                    endUp = true;
                                    endRight = true;
                                    break;
                                }
                                else if (map.WallIndexes.Contains(map.GetPoint(i, col - 1, mapDims)))
                                {
                                    endUp = true;
                                    endRight = true;
                                    break;
                                }
                                col++;
                            }
                            numUp = 1;
                            numRight = 1;
                            tempRow = row;
                            row--;
                            col = tempCol;
                            for (int i = col - 1; i >= 0 && row >= 0; i--)//Up and left
                            {
                                //row--;
                                if (map.WallIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {

                                    numUp++;
                                    numLeft++;
                                    UpLeft = new Vector2(numLeft, numUp);
                                }
                                else if (map.FloorIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {
                                    break;
                                }


                                if (i == 0 || row == 0 && map.WallIndexes.Contains(map.GetPoint(row, i, mapDims))) //Logic can be changed to move over obstacles and walls
                                {
                                    endUp = true;
                                    endLeft = true;
                                    break;
                                }
                                else if (map.WallIndexes.Contains(map.GetPoint(row, i + 1, mapDims)))
                                {
                                    endUp = true;
                                    endLeft = true;
                                    break;
                                }
                                row--;
                            }
                            numUp = 1;
                            numLeft = 1;
                            row = tempRow;
                            row++;
                            for (int i = col + 1; i < 25 && row < 15; i++) //Down and Right
                            {
                                //row++;
                                if (map.WallIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {

                                    numDown++;
                                    numRight++;
                                    DownRight = new Vector2(numRight, numDown);
                                }
                                else if (map.FloorIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {
                                    break;
                                }

                                if (i == 24 || row == 14 && map.WallIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {
                                    endDown = true;
                                    endRight = true;
                                    break;
                                }
                                else if (map.WallIndexes.Contains(map.GetPoint(row, i + 1, mapDims)))
                                {
                                    endDown = true;
                                    endRight = true;
                                    break;
                                }
                                row++;
                            }
                            //Check if numX == numX      and      if numY==numY
                            if (UpLeft.Length() > DownRight.Length()) // if left and up is more than right and down
                            {
                                if (endDown && endRight)
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize * 2);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }


                            }
                            else if (DownRight.Length() > UpLeft.Length()) //if right and down is more than left and up
                            {
                                if (endUp && endLeft)
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize * 2);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }
                                else
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize);
                                    distToTravel.X = -UpLeft.X * (tileSize * 3);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }

                            }
                            else if (UpLeft.Length() == DownRight.Length())
                            {
                                if (endDown && endRight)
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }
                                else if (endUp && endLeft)
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }
                                else
                                {
                                    Random rand = new Random();

                                    if (rand.Next(11) > 5)
                                    {
                                        distToTravel.Y = DownRight.Y * (tileSize);
                                        distToTravel.X = DownRight.X * (tileSize * 2);
                                        strafeDown = true;
                                        strafeRight = true;
                                    }
                                    else
                                    {
                                        distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                        distToTravel.X = -UpLeft.X * (tileSize);
                                        strafeUp = true;
                                        strafeLeft = true;
                                        prioXStrafe = true;
                                    }
                                }


                            }
                            else if (DownLeft.Length() > UpRight.Length())//Left and Down is more than Right and Up
                            {
                                if (endUp && endRight)
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }
                                else
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }

                            }
                            else if (UpRight.Length() > DownLeft.Length())//Right and Up is more than Left and Down
                            {
                                if (endDown && endLeft)
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }

                            }
                            else if (DownLeft.Length() == UpRight.Length())
                            {
                                if (endUp && endRight)
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }
                                else if (endDown && endLeft)
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    Random rand = new Random();

                                    if (rand.Next(11) > 5)
                                    {
                                        distToTravel.Y = -UpRight.Y * (tileSize);
                                        distToTravel.X = UpRight.X * (tileSize * 2);
                                        strafeUp = true;
                                        strafeRight = true;
                                        prioXStrafe = true;
                                    }
                                    else
                                    {
                                        distToTravel.X = -DownLeft.X * (tileSize * 2);
                                        distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                        strafeLeft = true;
                                        strafeDown = true;
                                    }
                                }

                            }

                            //SetStrafe(numUp, numDown, numLeft, numRight, dirX, dirY);
                            break;
                    }
                    break;
            }

            //SetStrafe(numUp, numDown, numLeft, numRight);
        }

        private void SetStrafe(int numUp, int numDown, int numLeft, int numRight, string xDir, string yDir)
        {

            if ((numLeft > numRight && numLeft > numDown) && (numUp > numRight && numUp > numDown)) // if left and up is more than right and down
            {
                distToTravel.Y = numDown * (tileSize * 2);
                distToTravel.X = -numLeft * tileSize;
            }
            if ((numDown > numLeft && numDown > numUp) && (numRight > numLeft && numRight > numUp)) //if right and down is more than left and up
            {
                distToTravel.Y = -numUp * tileSize;
                distToTravel.X = numRight * (tileSize * 2);
            }
            if ((numLeft > numRight && numLeft > numUp) && (numDown > numUp && numDown > numRight))//Left and Down is more than Right and Up
            {
                distToTravel.Y = -numUp * tileSize;
                distToTravel.X = -numLeft * tileSize;
            }
            if ((numRight > numLeft && numRight > numDown) && (numUp > numLeft && numUp > numDown))//Right and Up is more than Left and Down
            {
                distToTravel.X = numRight * (tileSize * 2);
                distToTravel.Y = numDown * (tileSize * 2);
            }




            //if (numDown < numUp)
            //{
            //    strafeDown = true;
            //    strafeUp = false;
            //    distToTravel.Y = numDown * tileSize;
            //}
            //if (numUp < numDown)
            //{
            //    strafeUp = true;
            //    strafeDown = false;
            //    distToTravel.Y = -numUp * tileSize;
            //}

            //if (numUp == numDown)
            //{
            //    Random rand = new Random();
            //    if (rand.Next(0, 2) > 0)
            //    {
            //        strafeUp = true;
            //        strafeDown = false;
            //        distToTravel.Y = -numUp * tileSize;
            //    }
            //    else
            //    {
            //        strafeDown = true;
            //        strafeUp = false;
            //        distToTravel.Y = numDown * (tileSize * 2);
            //    }
            //}

            //if (numRight < numLeft)
            //{
            //    strafeRight = true;
            //    strafeLeft = false;
            //    distToTravel.X = numRight * tileSize;
            //}
            //if (numLeft < numRight)
            //{
            //    strafeLeft = true;
            //    strafeRight = false;
            //    distToTravel.X = -numLeft * tileSize;
            //}

            //if (numLeft == numRight)
            //{
            //    Random rand = new Random();
            //    if (rand.Next(0, 2) > 0)
            //    {
            //        strafeLeft = true;
            //        strafeRight = false;
            //        distToTravel.X = -numLeft * tileSize;
            //    }
            //    else
            //    {
            //        strafeRight = true;
            //        strafeLeft = false;
            //        distToTravel.X = numRight * (tileSize * 2);
            //    }
            //}
        }

        void ChooseYStrafe(int row, int col, string dir, string blockDir)
        {
            int numDown = 1; //Number of walls going down
            int numUp = 1; //Number of walls going up
            bool endDown = false;
            bool endUp = false;
            for (int i = row + 1; i < 15; i++)
            {
                if (blockDir == "left" && map.WallIndexes.Contains(map.GetPoint(i, col - 1, mapDims)))
                {
                    endDown = true;
                    break;

                }
                if (blockDir == "right" && map.WallIndexes.Contains(map.GetPoint(i, col + 1, mapDims)))
                {
                    endDown = true; ;
                    break;

                }

                if (i < 15 && map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                {
                    numDown++;
                }
                else
                {
                    break;
                }
            }
            for (int i = row - 1; i >= 0; i--)
            {
                if (blockDir == "left" && map.WallIndexes.Contains(map.GetPoint(i, col - 1, mapDims)))
                {
                    endUp = true;
                    break;
                }
                if (blockDir == "right" && map.WallIndexes.Contains(map.GetPoint(i, col + 1, mapDims)))
                {
                    endUp = true;
                    break;
                }
                if (i > 0 && map.WallIndexes.Contains(map.GetPoint(i, col, mapDims)))
                {
                    numUp++;
                }
                else
                {
                    break;
                }
            }



            if (numDown < numUp)
            {
                if (endDown)
                {
                    strafeUp = true;
                    strafeDown = false;

                    if (dir == "up")
                    {
                        distToTravel.Y = -numUp * (tileSize * 2);
                    }
                    if (dir == "down")
                    {
                        distToTravel.Y = -numUp * (tileSize);
                    }
                    if (dir == "")
                    {
                        distToTravel.Y = -numUp * (tileSize * 2);
                    }
                }
                else
                {
                    strafeDown = true;
                    strafeUp = false;

                    if (dir == "up")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                    if (dir == "down")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                    if (dir == "")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                }


            }
            if (numUp < numDown)
            {
                if (endUp)
                {
                    strafeDown = true;
                    strafeUp = false;

                    if (dir == "up")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                    if (dir == "down")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                    if (dir == "")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                }
                else
                {
                    strafeUp = true;
                    strafeDown = false;

                    if (dir == "up")
                    {
                        distToTravel.Y = -numUp * (tileSize * 2);
                    }
                    if (dir == "down")
                    {
                        distToTravel.Y = -numUp * (tileSize);
                    }
                    if (dir == "")
                    {
                        distToTravel.Y = -numUp * (tileSize * 2);
                    }
                }

            }

            if (numUp == numDown)
            {
                if (endUp)
                {
                    strafeDown = true;
                    strafeUp = false;

                    if (dir == "up")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                    if (dir == "down")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                    if (dir == "")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                }
                else if (endDown)
                {
                    strafeUp = true;
                    strafeDown = false;

                    if (dir == "up")
                    {
                        distToTravel.Y = -numUp * (tileSize * 2);
                    }
                    if (dir == "down")
                    {
                        distToTravel.Y = -numUp * (tileSize);
                    }
                    if (dir == "")
                    {
                        distToTravel.Y = -numUp * (tileSize * 2);
                    }
                }
                else
                {
                    Random rand = new Random();
                    if (rand.Next(0, 2) > 0)
                    {
                        strafeUp = true;
                        strafeDown = false;
                        if (dir == "up")
                        {
                            distToTravel.Y = -numUp * (tileSize * 2);
                        }
                        if (dir == "down")
                        {
                            distToTravel.Y = -numUp * (tileSize);
                        }
                        if (dir == "")
                        {
                            distToTravel.Y = -numUp * (tileSize * 2);
                        }

                    }
                    else
                    {
                        strafeDown = true;
                        strafeUp = false;
                        if (dir == "up")
                        {
                            distToTravel.Y = numDown * (tileSize * 2);
                        }
                        if (dir == "down")
                        {
                            distToTravel.Y = numDown * (tileSize);
                        }
                        if (dir == "")
                        {
                            distToTravel.Y = numDown * (tileSize * 2);
                        }
                    }
                }

            }
        }

        int WallUntilOpening(int row, int col, string checkDirConst, string checkDir, int blocksout, int tilesFromCenter)
        {
            int walls = 1;

            switch (checkDirConst)
            {
                case "right":
                    switch (checkDir)
                    {
                        case "up":
                            for (int i = 1; i <= blocksout; i++)//Checking to the right up tilesFrom
                            {
                                if (map.WallIndexes.Contains(map.GetPoint(row - tilesFromCenter, col + i, mapDims)))
                                {
                                    walls++;
                                }
                                if (map.FloorIndexes.Contains(map.GetPoint(row - tilesFromCenter, col + i, mapDims)))
                                {
                                    break;
                                }
                            }
                            break;
                        case "down":
                            for (int i = 1; i <= blocksout; i++)//Checking to right down tilesFrom
                            {
                                if (map.WallIndexes.Contains(map.GetPoint(row + tilesFromCenter, col + i, mapDims)))
                                {
                                    walls++;
                                }
                                if (map.FloorIndexes.Contains(map.GetPoint(row + tilesFromCenter, col + i, mapDims)))
                                {
                                    break;
                                }
                            }
                            break;

                    }

                    break;
                case "left":

                    switch (checkDir)
                    {
                        case "up":
                            for (int i = 1; i <= blocksout; i++)//Checking to the left up one
                            {
                                if (map.WallIndexes.Contains(map.GetPoint(row - tilesFromCenter, col - i, mapDims)))
                                {
                                    walls++;
                                }
                                if (map.FloorIndexes.Contains(map.GetPoint(row - tilesFromCenter, col - i, mapDims)))
                                {
                                    break;
                                }
                            }
                            break;
                        case "down":
                            for (int i = 1; i <= blocksout; i++)//Checking to the left down one
                            {
                                if (map.WallIndexes.Contains(map.GetPoint(row + tilesFromCenter, col - i, mapDims)))
                                {
                                    walls++;
                                }
                                if (map.FloorIndexes.Contains(map.GetPoint(row + tilesFromCenter, col - i, mapDims)))
                                {
                                    break;
                                }
                            }
                            break;

                    }
                    break;
                case "up":
                    switch (checkDir)
                    {
                        case "right":
                            for (int i = 1; i <= blocksout; i++)//Checking upwards to the right one
                            {
                                if (map.WallIndexes.Contains(map.GetPoint(row - i, col + tilesFromCenter, mapDims)))
                                {
                                    walls++;
                                }
                                if (map.FloorIndexes.Contains(map.GetPoint(row - i, col + tilesFromCenter, mapDims)))
                                {
                                    break;
                                }
                            }
                            break;
                        case "left":
                            for (int i = 1; i <= blocksout; i++)//Checking upwards to the left once
                            {
                                if (map.WallIndexes.Contains(map.GetPoint(row - i, col - tilesFromCenter, mapDims)))
                                {
                                    walls++;
                                }
                                if (map.FloorIndexes.Contains(map.GetPoint(row - i, col - tilesFromCenter, mapDims)))
                                {
                                    break;
                                }
                            }
                            break;
                    }
                    break;
                case "down":
                    switch (checkDir)
                    {
                        case "right":
                            for (int i = 1; i <= blocksout; i++) //Checking downwards to the right once
                            {
                                if (map.WallIndexes.Contains(map.GetPoint(row + i, col + tilesFromCenter, mapDims)))
                                {
                                    walls++;
                                }
                                if (map.FloorIndexes.Contains(map.GetPoint(row + i, col + tilesFromCenter, mapDims)))
                                {
                                    break;
                                }
                            }
                            break;
                        case "left":
                            for (int i = 1; i <= blocksout; i++) //Checking downwards to the left once
                            {
                                if (map.WallIndexes.Contains(map.GetPoint(row + i, col - tilesFromCenter, mapDims)))
                                {
                                    walls++;
                                }
                                if (map.FloorIndexes.Contains(map.GetPoint(row + i, col - tilesFromCenter, mapDims)))
                                {
                                    break;
                                }
                            }
                            break;

                    }
                    break;
            }
            return walls;
        }

        int WallsFromCenter(int blocksOut, int row, int col)
        {
            int numBlocks = 0;
            int[,] startPos = new int[row, col];

            for (int i = 0; i < blocksOut; i++) // Check right
            {
                if (row + i < 15 && map.WallIndexes.Contains(map.GetPoint(row + i, col, mapDims)))
                    numBlocks++;
            }
            for (int i = -1; i < -blocksOut; i--) //checks left
            {
                if (row - i > 0 && map.WallIndexes.Contains(map.GetPoint(row + i, col, mapDims)))
                    numBlocks++;
            }
            for (int i = 1; i < blocksOut; i++) //checks down
            {
                if (col + i < 25 && map.WallIndexes.Contains(map.GetPoint(row, col + i, mapDims)))
                    numBlocks++;
            }
            for (int i = -1; i < -blocksOut; i--) //checks up
            {
                if (col - i > 0 && map.WallIndexes.Contains(map.GetPoint(row, col + i, mapDims)))
                    numBlocks++;
            }
            for (int i = 1; i < blocksOut; i++) //check diagonally 
            {
                if (col + i < 25 && row + i < 15 && map.WallIndexes.Contains(map.GetPoint(row + i, col + i, mapDims)))
                    numBlocks++;
            }
            for (int i = -1; i < -blocksOut; i--)
            {
                if (col + i > 0 && row + i > 0 && map.WallIndexes.Contains(map.GetPoint(row + i, col + i, mapDims)))
                    numBlocks++;
            }
            for (int i = 1; i < blocksOut; i++)
            {
                if (col - i > 0 && row + i < 15 && map.WallIndexes.Contains(map.GetPoint(row + i, col - i, mapDims)))
                    numBlocks++;
            }
            for (int i = 1; i < blocksOut; i++)
            {
                if (col + i < 25 && row - i > 0 && map.WallIndexes.Contains(map.GetPoint(row - i, col + i, mapDims)))
                    numBlocks++;
            }
            return numBlocks;
        }
        void ChooseXStrafe(int row, int col, string blocDir)
        {
            int numRight = 1;
            int numLeft = 1;

            bool endRight = false;
            bool endLeft = false;

            for (int i = col + 1; i < 25; i++)
            {
                if (blocDir == "up" && map.WallIndexes.Contains(map.GetPoint(row + 1, i, mapDims)))
                {
                    endRight = true;
                    break;

                }
                if (blocDir == "down" && map.WallIndexes.Contains(map.GetPoint(row - 1, i, mapDims)))
                {
                    endRight = true;
                    break;

                }
                if (map.WallIndexes.Contains(map.GetPoint(row, i, mapDims)))
                {
                    numRight++;
                }
                else
                {
                    break;
                }
            }
            for (int i = col - 1; i > 0; i--)
            {
                if (blocDir == "up" && map.WallIndexes.Contains(map.GetPoint(row + 1, i, mapDims)))
                {
                    endLeft = true;
                    break;

                }
                if (blocDir == "down" && map.WallIndexes.Contains(map.GetPoint(row - 1, i, mapDims)))
                {
                    endLeft = true;
                    break;
                }
                if (map.WallIndexes.Contains(map.GetPoint(row, i, mapDims)))
                {

                    numLeft++;
                }
                else
                {
                    break;
                }
            }

            if (numRight < numLeft)
            {
                if (endRight)
                {
                    strafeLeft = true;
                    strafeRight = false;

                    distToTravel.X = -numLeft * (tileSize);
                }
                else
                {
                    strafeRight = true;
                    strafeLeft = false;
                    distToTravel.X = numRight * (tileSize * 2);
                }


            }
            if (numLeft < numRight)
            {

                if (endLeft)
                {
                    strafeRight = true;
                    strafeLeft = false;
                    distToTravel.X = numRight * (tileSize * 2);
                }
                else
                {
                    strafeLeft = true;
                    strafeRight = false;

                    distToTravel.X = -numLeft * (tileSize * 2);
                }


            }

            if (numLeft == numRight)
            {

                if (endRight)
                {
                    strafeLeft = true;
                    strafeRight = false;

                    distToTravel.X = -numLeft * (tileSize * 2);
                }
                else if (endLeft)
                {
                    strafeRight = true;
                    strafeLeft = false;
                    distToTravel.X = numRight * (tileSize * 2);
                }
                else
                {
                    Random rand = new Random();
                    if (rand.Next(0, 2) > 0)
                    {

                        strafeLeft = true;
                        strafeRight = false;

                        distToTravel.X = -numLeft * (tileSize * 2);
                    }
                    else
                    {
                        strafeRight = true;
                        strafeLeft = false;
                        distToTravel.X = numRight * (tileSize * 2);
                    }
                }
            }
        }

        int WallsFromPoint(int blocksOut, int row, int col, int checkDir)
        {
            int numBlocks = 0;
            int[,] startPos = new int[row, col];

            switch (checkDir)
            {
                case 0:
                    for (int i = 0; i < blocksOut; i++) // Check right
                    {
                        if (col + i <= 25 && map.WallIndexes.Contains(map.GetPoint(row, col + i, mapDims)))
                        {
                            strafeRight = false;
                            numBlocks++;
                        }

                    }
                    break;
                case 1:
                    for (int i = 0; i < blocksOut; i++) //checks left
                    {
                        if (col - i >= 0 && map.WallIndexes.Contains(map.GetPoint(row, col - i, mapDims)))
                        {
                            strafeLeft = false;
                            numBlocks++;
                        }

                    }
                    break;
                case 2:
                    for (int i = 0; i < blocksOut; i++) //checks down
                    {
                        if (row + i <= 15 && map.WallIndexes.Contains(map.GetPoint(row + i, col, mapDims)))
                        {
                            strafeDown = false;
                            numBlocks++;
                        }

                    }
                    break;
                case 3:
                    for (int i = 0; i < blocksOut; i++) //checks up
                    {
                        if (row - i >= 0 && map.WallIndexes.Contains(map.GetPoint(row - i, col, mapDims)))
                        {
                            strafeUp = false;
                            numBlocks++;
                        }

                    }
                    break;


            }


            return numBlocks;


            //for (int i = 1; i < blocksOut; i++) //check diagonally 
            //{
            //    if (col + i < 25 && row + i < 15 && map.GetPoint(row + i, col + i) == 2)
            //        numBlocks++;
            //}
            //for (int i = -1; i < -blocksOut; i--)
            //{
            //    if (col + i > 0 && row + i > 0 && map.GetPoint(row + i, col + i) == 2)
            //        numBlocks++;
            //}
            //for (int i = 1; i < blocksOut; i++)
            //{
            //    if (col - i > 0 && row + i < 15 && map.GetPoint(row + i, col - i) == 2)
            //        numBlocks++;
            //}
            //for (int i = 1; i < blocksOut; i++)
            //{
            //    if (col + i < 25 && row - i > 0 && map.GetPoint(row - i, col + i) == 2)
            //        numBlocks++;
            //}

        }



        int distForm(Vector2 pos1, Vector2 pos2)
        {
            return (int)Math.Sqrt(Math.Pow(pos1.X - pos2.X, 2) + Math.Pow(pos1.Y - pos2.Y, 2));
        }

        //SkullTiles DetermineClosestTarget()
        //{

        //}

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);

            //if (vision.Count > 5)
            //{
            //    spriteBatch.Draw(texture, vision[4], Color.White * .5f);
            //}

            //foreach (Rectangle rect in vision)
            //{
            //    spriteBatch.Draw(texture, rect, Color.White * .5f);
            //}
        }
    }
}