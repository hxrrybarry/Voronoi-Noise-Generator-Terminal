using System;
using System.Reflection.Emit;

namespace Voronoi;

internal class Program
{
    const char FILLED = '#';
    const char UNFILLED = ' ';

    private class VoronoiNoise(int xSize, int ySize, int zSize, int numberOfPoints, float threshold, int seed)
    {
        private int XSize { get; } = xSize;
        private int YSize { get; } = ySize;
        public int ZSize { get; } = zSize;
        private int NumberOfPoints { get; } = numberOfPoints;
        private float Threshold { get; } = threshold;

        public char[,,] FilledGrid { get; set; } = new char[xSize, ySize, zSize];
        private int[,] PointsOnGrid { get; } = new int[numberOfPoints, 3];

        public int Seed { get; } = seed;

        private float GetDistanceToNearestPoint(int x, int y, int z)
        {
            // get distance to nearest point by looping and overriding-
            //- the previously recorded nearestDistance if necessary
            float nearestDistance = 1000;
            for (int i = 0; i < NumberOfPoints; i++)
            {
                // distance formula: s = √((x1 - x2)^2 + (y1 - y2)^2 + (z1 - z2)^2)
                float distance = MathF.Sqrt(MathF.Pow(x - PointsOnGrid[i, 0], 2) + MathF.Pow(y - PointsOnGrid[i, 1], 2) + MathF.Pow(z - PointsOnGrid[i, 2], 2));
                if (distance < nearestDistance)
                    nearestDistance = distance;
            }

            return nearestDistance;
        }

        public void GenerateGrid()
        {
            Random r = new(Seed);

            // choose NumberOfPoints amount of random coordinates to-
            //- sit on the grid for later point-distance calculations
            for (int i = 0; i < NumberOfPoints; i++)
            {
                PointsOnGrid[i, 0] = r.Next(XSize);
                PointsOnGrid[i, 1] = r.Next(YSize);
                PointsOnGrid[i, 2] = r.Next(ZSize);
            }

            // *this is yikes but no other real solution
            // loop through each point (very slow in 3D!) and perform-
            // - a distance calculation which is then evaluated at a threshold-
            // - to determine whether to fill that point with a value
            for (int x = 0; x < XSize; x++)
            {
                for (int y = 0; y < YSize; y++)
                {
                    for (int z = 0; z < ZSize; z++)
                    {
                        float distance = GetDistanceToNearestPoint(x, y, z);

                        if (distance < Threshold)
                            FilledGrid[x, y, z] = FILLED;
                        else
                            FilledGrid[x, y, z] = UNFILLED;
                    }               
                }
            }
        }

        public string ToString(int level)
        {
            string result = "";

            // append to string a particular slice of the 3D noise
            // z coordinate is determined by "level"
            for (int i = 0; i < XSize; i++)
            {
                result += '\n';
                for (int j = 0; j < YSize; j++)
                    result += FilledGrid[i, j, level];
            }

            return result;
        }
    }


    public static void Main()
    {
        // worth noting X and Y coordinate are flipped - cba fixing doesn't really matter
        VoronoiNoise noise = new(27, 54, 1, 50, 3.5f, 189453129);
        noise.GenerateGrid();
        
        int level = 0;
        while (true)
        {
            Console.WriteLine($"{noise.ToString(level)}\nSeed: {noise.Seed}, Z Level: {level}");
           
            ConsoleKeyInfo keyPress = Console.ReadKey();
            
            if (keyPress.Key == ConsoleKey.RightArrow || keyPress.Key == ConsoleKey.D)
            {
                if (level != noise.ZSize - 1)
                    level++;
                else
                    level = 0;
            }
            else if (keyPress.Key == ConsoleKey.LeftArrow || keyPress.Key == ConsoleKey.A)
            {
                if (level != 0)
                    level--;
                else
                    level = noise.ZSize - 1;
            }
            else if (keyPress.Key == ConsoleKey.Enter)
            {
                noise = new(28, 56, 56, 750, 3.5f, new Random().Next());
                noise.GenerateGrid();
            }
            Console.Clear();
        }      
    }
}