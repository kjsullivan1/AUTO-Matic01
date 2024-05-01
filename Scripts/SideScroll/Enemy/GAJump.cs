using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AUTO_Matic.SideScroll.Enemy
{
    class GAJump
    {
        List<Chromosome> population = new List<Chromosome>();
        int popSize = 12;
        double crossoverRate = 0.7;
        double mutationRate = .9;
        float bestFitness;
        float totalFitness;
        int generations;
        List<int> fitnesses = new List<int>();
        Chromosome bestChromosome;
        SSEnemy enemy;
        public bool isStarted = false;
        List<Rectangle> possRects;
        Vector2 currPos;
        Rectangle bounds;

        public void CreatePopulation(List<Rectangle> possibleRects, Vector2 currPos, SSEnemy enemy, Rectangle bounds)
        {
            population.Clear();
            isStarted = true;
            this.bounds = bounds;
            for(int i = 0; i < popSize; i++)
            {
                population.Add(new Chromosome(possibleRects, currPos, bounds));
            }
            this.enemy = enemy;
            generations = 0;
            totalFitness = 0;
            possRects = possibleRects;
            this.currPos = currPos;
        }

        public void CalcFitness()
        {
            fitnesses.Clear();

            foreach(Chromosome chromosome in population)
            {
                //fitnesses.Add(Math.Abs(enemy.TestJump(chromosome.goalRect, chromosome.jumpForce, chromosome.startPos)));
            }

            bestFitness = fitnesses[0];
            bestChromosome = population[0];

            for(int i = 0; i < fitnesses.Count - 1; i++)
            {
                totalFitness += fitnesses[i];
                if(bestFitness > fitnesses[i])
                {
                    bestFitness = fitnesses[i];
                    bestChromosome = population[i];
                }
            } 

           
        }

        public Chromosome RouletteWheelSelection()
        {
            CalcFitness();

            float slice = RandFloat() * totalFitness;

            float total = 0;

            int selectedIndex = 0;

            for (int i = 0; i < popSize; i++)
            {
                total += fitnesses[i];

                if(total > slice)
                {
                    selectedIndex = i;
                    break;
                }
            }

            return population[selectedIndex];

        }

        public void Crossover(Chromosome mom, Chromosome dad, Chromosome baby1, Chromosome baby2)
        {
            if(RandPercent() > crossoverRate || mom == dad)
            {
                baby1 = mom;
                baby2 = dad;
            }
            else
            {
                Random rand = new Random();
                if(rand.Next(0,2) > 0)
                {
                    baby1.goalRect = mom.goalRect;
                    baby1.startPos = mom.startPos;
                    baby1.jumpForce = dad.jumpForce;

                    baby2.goalRect = dad.goalRect;
                    baby2.startPos = dad.startPos;
                    baby2.jumpForce = mom.jumpForce;
                }
                else
                {
                    baby1.goalRect = dad.goalRect;
                    baby1.startPos = dad.startPos;
                    baby1.jumpForce = mom.jumpForce;

                    baby2.goalRect = mom.goalRect;
                    baby2.startPos = mom.startPos;
                    baby2.jumpForce = dad.jumpForce;
                }
            }


        }

        public Chromosome Mutate(Chromosome chromosome)
        {
            if(RandPercent() < mutationRate)
            {
                chromosome = new Chromosome(possRects, currPos, bounds);
            }
            return chromosome;
        }
        
        public Chromosome GetBestChromosome()
        {
            return bestChromosome;
        }
        public float GetBestFitness()
        {
            return bestFitness;
        }

        public void RunGA()
        {
            int newBabies = 0;

            CalcFitness();

            List<Chromosome> babyChromosomes = new List<Chromosome>();

            while(newBabies < popSize)
            {
                Chromosome mom = RouletteWheelSelection();
                Chromosome dad = RouletteWheelSelection();
                babyChromosomes.Add(bestChromosome);
                Chromosome baby1 = new Chromosome(possRects, currPos, bounds);
                Chromosome baby2 = new Chromosome(possRects, currPos, bounds);
                Crossover(mom, dad, baby1, baby2);

                baby1 = Mutate(baby1);
                baby2 = Mutate(baby2);


                babyChromosomes.Add(baby1);
                babyChromosomes.Add(baby2);

                newBabies += 3;
            }
        }


       
        public float RandFloat()
        {
            Random r = new Random();
            float decimalNumber;
            string beforePoint = r.Next(0, 1).ToString();//number before decimal point
            string afterPoint = r.Next(0, 10).ToString();
            string afterPoint2 = r.Next(0, 10).ToString();
            string afterPoint3 = r.Next(0, 10).ToString();//1st decimal point
                                                          //string secondDP = r.Next(0, 9).ToString();//2nd decimal point
            string combined = beforePoint + "." + afterPoint + afterPoint2 + afterPoint3;
            return decimalNumber = float.Parse(combined);
        }

        public float RandPercent()
        {
            Random r = new Random();
            float decimalNumber;
            string beforePoint = r.Next(0, 2).ToString();//number before decimal point
            string afterPoint = r.Next(0, 10).ToString();
            string afterPoint2 = r.Next(0, 10).ToString();
            //string afterPoint3 = r.Next(0, 10).ToString();//1st decimal point
                                                          //string secondDP = r.Next(0, 9).ToString();//2nd decimal point
            string combined = beforePoint + "." + afterPoint + afterPoint2;
            return decimalNumber = float.Parse(combined);
        }
    }

    class Chromosome
    {
        public Rectangle goalRect;
        public Vector2 startPos;
        public float jumpForce;

        public Chromosome(List<Rectangle> possibleRects, Vector2 currPos, Rectangle bounds)
        {
            Random random = new Random();
            goalRect = possibleRects[0];
            foreach(Rectangle rect in possibleRects)
            {
                if (Distance(new Vector2(goalRect.X, goalRect.Y), currPos) > Distance(new Vector2(rect.X, rect.Y), currPos))
                {
                    goalRect = rect;
                }
            }
            //goalRect = possibleRects[random.Next(0, possibleRects.Count)];
            if(currPos.X < goalRect.X)
            {
                startPos = new Vector2(random.Next((int)currPos.X,bounds.Width), currPos.Y);
            }
            if(currPos.X > goalRect.X)
            {
                startPos = new Vector2(random.Next(0, goalRect.X), currPos.Y);
            }
            jumpForce = RandomFloat();
        }

        public Chromosome CreateChromosome(List<Rectangle> possibleRects, Vector2 currPos, Rectangle bounds)
        {
            Chromosome chromosome = new Chromosome(possibleRects, currPos, bounds);

            return chromosome;
        }

        public float RandomFloat()
        {
            Random r = new Random();
            float decimalNumber;
            string beforePoint = r.Next(4, 25).ToString();//number before decimal point
            string afterPoint = r.Next(0, 10).ToString();
            string afterPoint2 = r.Next(0, 10).ToString();
            string afterPoint3 = r.Next(0, 10).ToString();//1st decimal point
                                                          //string secondDP = r.Next(0, 9).ToString();//2nd decimal point
            string combined = beforePoint + "." + afterPoint + afterPoint2 + afterPoint3;
            return decimalNumber = -float.Parse(combined);
        }

        public float Distance(Vector2 pos1, Vector2 pos2)
        {
            return (float)Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
        }
    }
}
