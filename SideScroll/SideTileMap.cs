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
        //static int[,] Map;
        public int mapWidth;
        public int mapHeight;
        public static SideTileMap tileMap = new SideTileMap();
        private static List<EmptyTile> emptyTiles = new List<EmptyTile>();
        public static List<EmptyTile> EmptyTiles
        {
            get { return emptyTiles; }
        }
        public void SetDims(int width, int height)
        {
            mapWidth = width;
            mapHeight = height;
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
            int width = 0;
            int height = 0;
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    int num = map[y, x];

                    if (num == 0)
                    {
                        emptyTiles.Add(new EmptyTile(num, new Rectangle(x * size, y * size, size, size)));

                    }
                    width += x * size;
                    height += y * size;
                }
            }
            tileMap.SetDims(width, height);


        }

        //public void BuildAndSaveMap(int mapWidth, int mapHeight)

        public static void Draw(SpriteBatch spriteBatch)
        {



            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.transform);
            foreach (EmptyTile tile in emptyTiles)
            {
                tile.Draw(spriteBatch);
            }
            // spriteBatch.End();
        }
    }
}
