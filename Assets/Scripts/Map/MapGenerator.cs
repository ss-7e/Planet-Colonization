using System.Collections.Generic;
using UnityEngine;


namespace Game.Map
{
    class MapGenerator : MonoBehaviour
    {
        public int length = 120;
        public int width = 120;
        public int refineTimes = 2;

        public float tileSize = 1f;
        public float heightMapScale = 1f;
        public float ObstacleHeight = 0.5f;
        public float ObstacleLow = 0.1f;
        public float gridLevel = 1f;

        public MapGenerateConfig config;
        public GridMeshGenerator gridMeshGenerator;

        [SerializeField]private float seed;
        private void Start()
        {
            setConfig();
            if (GridManager.instance.grid == null)
            {
                RunTimeGenerate();
            }
            gridMeshGenerator = GetComponent<GridMeshGenerator>();
            gridMeshGenerator.config = config;
            gridMeshGenerator.GenerateGridMesh();
        }

        void setConfig()
        {
            config = new MapGenerateConfig
            {
                length = length,
                width = width,
                tileSize = tileSize,
                heightMapScale = heightMapScale,
                seed = seed
            };
        }

        public void RunTimeGenerate()
        {
            GridManager.instance.length = (int)(length * tileSize);
            GridManager.instance.width = (int)(width * tileSize);
            GridManager.instance.grid = GridGenerate();
        }

        public Grid[] GridGenerate()
        {
            Grid[] grids = new Grid[length * width];
            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    float yPos = 0;
                    Vector3 pos = new Vector3(x * tileSize - length * tileSize / 2, yPos, y * tileSize - width * tileSize / 2);
                    int index = y * length + x;
                    Grid grid = new Grid(pos);
                    SetObstacle(grid, (float)x, (float)y);
                    grids[index] = grid;
                }
            }
            for (int i = 0; i < refineTimes; i++)
            {
                HighGroundOptifine(grids);
            }
            return grids;
        }

        void HighGroundOptifine(Grid[] grids)
        {
            for (int i = 0; i < grids.Length; i++)
            {
                List<Grid> neighbors = GridUtils.GetAllNeighbors(grids, i, length, width);
                int count = 0;
                foreach (var neighbor in neighbors)
                {
                    if (neighbor.isObstacle || Random.Range(0f, 1f) > 0.9f)
                    {
                        count++;
                    }
                }
                if (count > 4) { grids[i].isObstacle = true; }
                if(count < 1) { grids[i].isObstacle = false; }
            }
        }

        void SetObstacle(Grid grid, float x, float y)
        {
            float h = yposGengerate(x, y);
            float randomValue = Random.Range(0f, 1f); 
            if (h < ObstacleHeight && h > ObstacleLow && randomValue > 0.5f)
            {
                grid.isObstacle = true;
            }
        }

        float GetPerlinHeight(float x, float y, int octaves = 4, float persistence = 0.5f, float lacunarity = 2f, float scale = 0.5f)
        {
            float noiseHeight = 0f;
            float amplitude = 1f;
            float frequency = 1f;

            for (int i = 0; i < octaves; i++)
            {
                float sampleX = x * frequency * scale;
                float sampleY = y * frequency * scale;
                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2; 
                noiseHeight += perlinValue * amplitude;

                amplitude *= persistence;
                frequency *= lacunarity;
            }

            return noiseHeight;
        }

        float yposGengerate(float x, float y)
        {
            if (config != null)
            {
                heightMapScale = config.heightMapScale;
            }
            float yPos = Mathf.PerlinNoise((x + seed) * 0.1f * heightMapScale, (y + seed) * 0.1f * heightMapScale) * gridLevel;
            yPos = (float)Mathf.Round(yPos) / 10;
            return yPos;
        }

        public void SetRandomSeed()
        {
            seed = Random.Range(0f, 10000f);
        }
        public void SetSeed(float seed)
        {
            this.seed = seed;
        }
    }
}
