using System.Collections.Generic;
using UnityEngine;

namespace Game.Map
{
    public static class GridUtils
    {
        public static List<Grid> GetCrossNeighbors(Grid[] grids, int index, int width, int height)
        {
            List<Grid> neighbors = new List<Grid>();

            int x = index % width;
            int y = index / width;

            if (y + 1 < height)
                neighbors.Add(grids[(y + 1) * width + x]);

            if (y - 1 >= 0)
                neighbors.Add(grids[(y - 1) * width + x]);

            if (x - 1 >= 0)
                neighbors.Add(grids[y * width + (x - 1)]);

            if (x + 1 < width)
                neighbors.Add(grids[y * width + (x + 1)]);

            return neighbors;
        }

        public static List<Grid> GetAllNeighbors(Grid[] grids, int index, int width, int height)
        {
            List<Grid> neighbors = new List<Grid>();

            int x = index % width;
            int y = index / width;

            if (y + 1 < height)
            {
                neighbors.Add(grids[(y + 1) * width + x]);
                if (x - 1 >= 0)
                    neighbors.Add(grids[(y + 1) * width + (x - 1)]);
                if (x + 1 < width)
                    neighbors.Add(grids[(y + 1) * width + (x + 1)]);
            }

            if (y - 1 >= 0){
                neighbors.Add(grids[(y - 1) * width + x]);
                if (x - 1 >= 0)
                    neighbors.Add(grids[(y - 1) * width + (x - 1)]);
                if (x + 1 < width)
                    neighbors.Add(grids[(y - 1) * width + (x + 1)]);
            }

            if (x - 1 >= 0)
                neighbors.Add(grids[y * width + (x - 1)]);

            if (x + 1 < width)
                neighbors.Add(grids[y * width + (x + 1)]);

            return neighbors;
        }
    }
}