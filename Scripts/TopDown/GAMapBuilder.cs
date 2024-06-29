using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        //List<List<int[,]>> population = new List<List<int[,]>>();

        public void Start()
        {
            map = new Chromosome(possMaps[rand.Next(0, possMaps.Count)]);
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
