using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AUTO_Matic.TopDown;
using Microsoft.Xna.Framework.Content;

namespace AUTO_Matic.Scripts.TopDown.Bosses
{
    class SlamBoss
    {
        List<GroundLoc> slamLocs = new List<GroundLoc>();
        Rectangle bossRect;

        int slameTimeMin = 1;
        int slamTimeMax = 2;

        float slamDelay = 0;

        Random rand = new Random();

        enum BossState { SetStats, Shoot, Slam}
        BossState state = BossState.SetStats;

        ContentManager content;

        #region Constructor
        public SlamBoss(Rectangle currBounds, ContentManager content, TopDownMap tdMap, int[,] map)
        {
            this.content = content;

            int sizeMod = 3;
            int width = 64 * sizeMod;
            int height = 64 * sizeMod;
            bossRect = new Rectangle(((currBounds.X + currBounds.Width / 2) - width / 2), (((currBounds.Y + currBounds.Height / 2) - height / 2)), width, height);
            //worldRect = new Rectangle(((rect.X + rect.Width / 2) - size / 2), (((rect.Y + rect.Height / 2) - size / 2)), size, size);
            //Gain the slam locs
            foreach (SlamTiles tile in tdMap.SlamTiles)
            {
                List<SlamTiles> list = new List<SlamTiles>();

                list.Add(tile);

                SlamTiles tTile = tile;
                int i = 1;

                list.Add(new SlamTiles(tdMap.SlamIndexes[0], new Rectangle(tTile.Rectangle.Right, tTile.Rectangle.Bottom,
                       tTile.Rectangle.Width, tTile.Rectangle.Height)));
                list[list.Count - 1].mapPoint = new int[] { tile.mapPoint[0] + 1, tile.mapPoint[1] +1 };

                //while (list.Count < 3)
                //{
                //    list.Add(new SlamTiles(tdMap.SlamIndexes[0], new Rectangle(tTile.Rectangle.Right, tTile.Rectangle.Y,
                //        tTile.Rectangle.Width, tTile.Rectangle.Height)));

                //    list[list.Count - 1].mapPoint = new int[] { tile.mapPoint[0], tile.mapPoint[1] + i };
                //    i++;

                //    //tdMap.dMapDims[tdMap.dMapDims.Count - 1][list[list.Count - 1].mapPoint[0],
                //    //    list[list.Count - 1].mapPoint[1]] = 0;
                //    tTile = list[list.Count - 1];
                //}
                //tTile = tile;
                //tTile.Rectangle = new Rectangle(tTile.Rectangle.X, tTile.Rectangle.Y + tTile.Rectangle.Height,
                //    tTile.Rectangle.Width, tTile.Rectangle.Height);
                //i = 0;
                //while (list.Count < 6)
                //{
                //    list.Add(new SlamTiles(tdMap.SlamIndexes[0], new Rectangle(tTile.Rectangle.Right, tTile.Rectangle.Y,
                //          tTile.Rectangle.Width, tTile.Rectangle.Height)));
                //    list[list.Count - 1].mapPoint = new int[] { tile.mapPoint[0] + 1, tile.mapPoint[1] + i };
                //    i++;

                //    //tdMap.dMapDims[tdMap.dMapDims.Count - 1][list[list.Count - 1].mapPoint[0],
                //    //list[list.Count - 1].mapPoint[1]] = 0;
                //    tTile = list[list.Count - 1];
                //}

                //tTile = tile;
                ////tTile.Rectangle = new Rectangle(tTile.Rectangle.X, tTile.Rectangle.Bottom + (tTile.Rectangle.Height),
                ////    tTile.Rectangle.Width, tTile.Rectangle.Height);
                //i = 0;
                //while (list.Count < 9)
                //{
                //    if (i < 3)
                //    {
                //        list.Add(new SlamTiles(tdMap.SlamIndexes[0], new Rectangle(tTile.Rectangle.Right, tTile.Rectangle.Bottom + (tTile.Rectangle.Height),
                //         tTile.Rectangle.Width, tTile.Rectangle.Height)));

                //        list[list.Count - 1].mapPoint = new int[] { tile.mapPoint[0] + 2, tile.mapPoint[1] + i };
                //        i++;

                //        //tdMap.dMapDims[tdMap.dMapDims.Count - 1][list[list.Count - 1].mapPoint[0],
                //        //list[list.Count - 1].mapPoint[1]] = 0;
                //        tTile = list[list.Count - 1];
                //    }
                //    else
                //    {
                //        list.Add(new SlamTiles(tdMap.SlamIndexes[0], new Rectangle(tTile.Rectangle.Right, tTile.Rectangle.Bottom,
                //         tTile.Rectangle.Width, tTile.Rectangle.Height)));
                //    }

                //}

                //tdMap.dMapDims[tdMap.dMapDims.Count - 1][tile.mapPoint[0] + 3, tile.mapPoint[1]] = 10;
                //tdMap.dMapDims[tdMap.dMapDims.Count - 1][list[4].mapPoint[0], list[4].mapPoint[1]] = 9;
                //list.RemoveAt(4);
                slamLocs.Add(new GroundLoc(list));

            }

            //int locNum = 4;
            //for(int i = 0; i < slamLocs[locNum].slamTiles.Count; i++)
            //{
            //    tdMap.dMapDims[tdMap.dMapDims.Count - 1][slamLocs[locNum].slamTiles[i].mapPoint[0],
            //        slamLocs[locNum].slamTiles[i].mapPoint[1]] = 9;
            //}
        }
        #endregion

        public void Update(GameTime gameTime, TDPlayer tdPlayer, TopDownMap tdMap)
        {
            switch(state)
            {
                case BossState.SetStats:
                    slamDelay = RandFloat(slameTimeMin, slamTimeMax);
                    state = BossState.Shoot;
                    break;
                case BossState.Shoot:
                    slamDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if(slamDelay <= 0)
                    {
                        state = BossState.Slam;
                    }

                    break;
                case BossState.Slam:

                    Point centerPlayer = tdPlayer.rectangle.Center;
                    Rectangle centerRec = slamLocs[0].slamTiles[slamLocs[0].slamTiles.Count / 2].Rectangle;
                    Rectangle centerRec2 = slamLocs[1].slamTiles[slamLocs[1].slamTiles.Count / 2].Rectangle;
                    Rectangle centerRec3 = slamLocs[2].slamTiles[slamLocs[2].slamTiles.Count / 2].Rectangle;
                    Rectangle centerRec4 = slamLocs[3].slamTiles[slamLocs[3].slamTiles.Count / 2].Rectangle;
                    Rectangle centerRec5 = slamLocs[4].slamTiles[slamLocs[4].slamTiles.Count / 2].Rectangle;

                    GroundLoc closestLoc = slamLocs[0];
                    closestLoc.index = 0;

                   for(int i = 0; i < 1; i++) //This is weird cause the intial testing was weird IE stupid
                    {
                        centerRec = slamLocs[i].slamTiles[1].Rectangle;
                        for(int j = i + 1; j < slamLocs.Count; j++)
                        {
                            Rectangle centerRec1 = slamLocs[j].slamTiles[1].Rectangle;

                            if(DistForm(centerPlayer.ToVector2(), centerRec.Center.ToVector2()) <  
                                DistForm(centerPlayer.ToVector2(), centerRec1.Center.ToVector2()))
                            {
                                //closestLoc = slamLocs[i];
                                //closestLoc.index = i;
                            }
                            else
                            {
                                centerRec = centerRec1;
                                closestLoc = slamLocs[j];
                                closestLoc.index = j;
                            }
                          
                        }
                    }

                    

                    int number = closestLoc.index;
                    bossRect = new Rectangle(closestLoc.slamTiles[0].Rectangle.X, closestLoc.slamTiles[0].Rectangle.Y,
                        bossRect.Width, bossRect.Height);

                    state = BossState.SetStats;
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            foreach (GroundLoc loc in slamLocs)
            {
                for (int i = 0; i < loc.slamTiles.Count; i++)
                {
                    spriteBatch.Draw(content.Load<Texture2D>("TopDown/MapTiles/Tile0"), loc.slamTiles[i].Rectangle, Color.White);
                }

            }
            spriteBatch.Draw(content.Load<Texture2D>("TopDown/MapTiles/Tile11"), bossRect, Color.White);

    
        }

        public float RandFloat(int min, int max)
        {
            Random r = new Random();
            float decimalNumber;
            string beforePoint = r.Next(min, max).ToString();//number before decimal point
            string afterPoint = r.Next(0, 10).ToString();
            string afterPoint2 = r.Next(0, 10).ToString();
            string afterPoint3 = r.Next(0, 10).ToString();//1st decimal point
                                                          //string secondDP = r.Next(0, 9).ToString();//2nd decimal point
            string combined = beforePoint + "." + afterPoint + afterPoint2 + afterPoint3;
            return decimalNumber = float.Parse(combined);
        }

        int DistForm(Vector2 pos1, Vector2 pos2)
        {
            int num = (int)Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
            return num;

        }
    }

    class GroundLoc
    {
        public List<SlamTiles> slamTiles;
        public int index;
        public GroundLoc(List<SlamTiles> tiles)
        {
            slamTiles = tiles;
        }

    }
}
