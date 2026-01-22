using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// 合成配料表
/// 
/// TODO:根据科技树解锁不同配方
/// TODO:从文件加载配方数据
/// 
/// </summary>
namespace Factory{

    //TODO: 生产条件？
    public class Recipe
    {
        public IReadOnlyDictionary<ItemType, int> Inputs { get; }
        public IReadOnlyDictionary<ItemType, int> Outputs { get; }
        public float ProduceTime { get; }

        public Recipe(
            Dictionary<ItemType, int> inputs,
            Dictionary<ItemType, int> outputs,
            float craftingTime)
        {
            Inputs = new ReadOnlyDictionary<ItemType, int>(
                new Dictionary<ItemType, int>(inputs));
            Outputs = new ReadOnlyDictionary<ItemType, int>(
                new Dictionary<ItemType, int>(outputs));
            ProduceTime = craftingTime > 0 ? craftingTime : 0.1f;
        }
    }
    public class RecipeManager
    {
        public static RecipeManager Instance { get; } = new RecipeManager();
        public Recipe IronSmelt { get; } = new Recipe(
            new Dictionary<ItemType, int>
            {
                { ItemType.Raw_IronOre, 2 },
            },
            new Dictionary<ItemType, int>
            {
                { ItemType.Refined_IronIngot, 1 }
            },
            5.0f
        );
        public Recipe IronBullet { get; } = new Recipe(
            new Dictionary<ItemType, int>
            {
                { ItemType.Refined_IronIngot, 1 },
            },
            new Dictionary<ItemType, int>
            {
                { ItemType.Product_IronBullet, 5 }
            },
            3.0f
        );
    }
}