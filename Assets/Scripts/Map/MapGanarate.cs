using UnityEngine;

namespace Game.Map
{
    class MapGanarate : MonoBehaviour
    {
        public int length = 120;
        public int width = 120;
        public float tileSize = 1f;
        public float heightMapScale = 1f;
        public MapGenerateConfig config;
        public GridMeshGenerator gridMeshGenerator;
        float seed;
        private void Start()
        {
            config = new MapGenerateConfig
            {
                length = length,
                width = width,
                tileSize = tileSize,
                heightMapScale = heightMapScale,
                seed = seed
            };
            GridGenerate();
            gridMeshGenerator = GetComponent<GridMeshGenerator>();
            gridMeshGenerator.config = config;
            gridMeshGenerator.GenerateGridMesh();
        }

        void GridGenerate()
        {
            GridManager.instance.grid = new Grid[1000, 1000];
            GridManager.instance.length = (int)(length * tileSize);
            GridManager.instance.width = (int)(width * tileSize);
            for(int x = 0; x < length; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    float yPos = yposGengerate(x, y);
                    Vector3 pos = new Vector3(x * tileSize - length * tileSize / 2, yPos, y * tileSize - width * tileSize / 2);
                    GridManager.instance.grid[x, y] = new Grid(pos);
                }
            }
        }

        float yposGengerate(float x, float y)
        {
            if (config != null)
            {
                heightMapScale = config.heightMapScale;
            }
            float yPos = Mathf.PerlinNoise(x * 0.1f * heightMapScale, y * 0.1f * heightMapScale) * 3f;
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
