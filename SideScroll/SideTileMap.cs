using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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
        private static List<EmptyTile> emptyTiles = new List<EmptyTile>();
        private static List<PlatformTile> platformTiles = new List<PlatformTile>();

        public static List<EmptyTile> EmptyTiles
        {
            get { return emptyTiles; }
        }

        public static List<PlatformTile> PlatformTiles
        {
            get { return platformTiles; }
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

            emptyTiles.Clear();
            platformTiles.Clear();
          
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    int num = map[y, x];

                    if (num == 0 || num == 2)
                    {
                        emptyTiles.Add(new EmptyTile(0, new Rectangle(x * size, y * size, size, size)));

                    }
                    if (num == 3)
                    {
                   
                        platformTiles.Add(new PlatformTile(0, new Rectangle(x * size, y * size, size, size)));
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
                if (tileMap.getPoint(row - 1, i) == 3 || tileMap.getPoint(row - 1, i) == 2 || tileMap.getPoint(row - 1, i) == 0)
                {
                    break;
                }


                if (tileMap.getPoint(row, i) == 3 || tileMap.getPoint(row, i) == 2 || tileMap.getPoint(row, i) == 0)
                {
                    numTilesRight++;
                }

            }
            for (int i = col - 1; i >= 0; i--)
            {
                if (tileMap.getPoint(row - 1, i) == 3 || tileMap.getPoint(row - 1, i) == 2 || tileMap.getPoint(row - 1, i) == 0)
                {
                    break;
                }


                if (tileMap.getPoint(row, i) == 3 || tileMap.getPoint(row, i) == 2 || tileMap.getPoint(row, i) == 0)
                {
                    numTilesLeft++;
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
            foreach (EmptyTile tile in emptyTiles)
            {
                tile.Draw(spriteBatch); 
            }
            foreach(PlatformTile tile in platformTiles)
            {
                tile.Draw(spriteBatch);
            }
            // spriteBatch.End();
        }
    }
}
