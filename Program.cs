using System;

namespace Voronoi;

internal class Program
{
    const char FILLED = '#';
    const char UNFILLED = ' ';

    private class VoronoiNoise(int xSize, int ySize, int zSize, int numberOfPoints, float threshold)
    {
        public int XSize { get; } = xSize;
        public int YSize { get; } = ySize;
        public int ZSize { get; } = zSize;
        private int NumberOfPoints { get; } = numberOfPoints;
        private float Threshold { get; } = threshold;

        public char[,,] FilledGrid { get; set; } = new char[xSize, ySize, zSize];
        private int[,] PointsOnGrid { get; } = new int[numberOfPoints, 3];

        private float GetDistanceToNearestPoint(int x, int y, int z)
        {
            // get distance to nearest point by looping and overriding-
            //- the previously recorded nearestDistance if necessary
            float nearestDistance = 1000;
            for (int i = 0; i < NumberOfPoints; i++)
            {
                float distance = MathF.Sqrt(MathF.Pow(x - PointsOnGrid[i, 0], 2) + MathF.Pow(y - PointsOnGrid[i, 1], 2) + MathF.Pow(z - PointsOnGrid[i, 2], 2));
                if (distance < nearestDistance)
                    nearestDistance = distance;
            }

            return nearestDistance;
        }

        public void GenerateGrid(int seed)
        {
            Random r = new(seed);

            // choose NumberOfPoints amount of random coordinates to-
            //- sit on the grid for later point-distance calculations
            for (int i = 0; i < NumberOfPoints; i++)
            {
                PointsOnGrid[i, 0] = r.Next(XSize);
                PointsOnGrid[i, 1] = r.Next(YSize);
                PointsOnGrid[i, 2] = r.Next(ZSize);
            }

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

                        if (distance > Threshold)
                            FilledGrid[x, y, z] = FILLED;
                        else
                            FilledGrid[x, y, z] = UNFILLED;
                    }               
                }
            }
        }
    }

    private static string Arr2Str(VoronoiNoise n, int level)
    {
        string result = "";

        // append to string a particular slice of the 3D noise
        // z coordinate is determined by "level"
        for (int i = 0; i < n.XSize; i++)
        {
            result += '\n';
            for (int j = 0; j < n.YSize; j++)
                result += n.FilledGrid[i, j, level];
        }

        return result;
    }

    public static void Main()
    {
        VoronoiNoise noise = new(27, 125, 54, 750, 3.5f);

        int seed = new Random().Next();
        noise.GenerateGrid(seed);
        int level = 0;
    
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"{Arr2Str(noise, level)}\nSeed: {seed}, Z Level: {level}");
           
            ConsoleKeyInfo keyPress = Console.ReadKey();
            
            if (keyPress.Key == ConsoleKey.RightArrow)
            {
                if (level != noise.ZSize - 1)
                    level++;
            }
            else if (keyPress.Key == ConsoleKey.LeftArrow)
            {
                if (level != 0)
                    level--;
            }
        }      
    }
}