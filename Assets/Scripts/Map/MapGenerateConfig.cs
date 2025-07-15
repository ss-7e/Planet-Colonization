using UnityEngine;
namespace Game.Map
{
    public class MapGenerateConfig
    {
        public int length = 100;
        public int width = 100;
        public float tileSize = 1f;
        public float heightMapScale = 1f;
        public float seed = 0f;
        public MapGenerateConfig(int length, int width, float tileSize, float heightMapScale, float seed)
        {
            this.length = length;
            this.width = width;
            this.tileSize = tileSize;
            this.heightMapScale = heightMapScale;
            this.seed = seed;
        }
        public MapGenerateConfig()
        {
            
        }
    }
}
