using Unity.Entities;



namespace DOTS
{
    public partial struct ItemTransformSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            // 确保场景中有物品数据才运行系统
        }

    }
}