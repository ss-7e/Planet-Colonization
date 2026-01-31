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
        public IReadOnlyDictionary<ItemIDPrev, int> Inputs { get; }
        public IReadOnlyDictionary<ItemIDPrev, int> Outputs { get; }
        public float ProduceTime { get; }

        public Recipe(
            Dictionary<ItemIDPrev, int> inputs,
            Dictionary<ItemIDPrev, int> outputs,
            float craftingTime)
        {
            Inputs = new ReadOnlyDictionary<ItemIDPrev, int>(
                new Dictionary<ItemIDPrev, int>(inputs));
            Outputs = new ReadOnlyDictionary<ItemIDPrev, int>(
                new Dictionary<ItemIDPrev, int>(outputs));
            ProduceTime = craftingTime > 0 ? craftingTime : 0.1f;
        }
    }
    public class RecipeManager
    {
        public static RecipeManager Instance { get; } = new RecipeManager();
        public Recipe IronSmelt { get; } = new Recipe(
            new Dictionary<ItemIDPrev, int> //输入物品
            {
                { ItemIDPrev.Raw_IronOre, 2 },
            },
            new Dictionary<ItemIDPrev, int> //输出物品
            {
                { ItemIDPrev.Refined_IronIngot, 1 }
            },
            1.0f
        );
        public Recipe IronBullet { get; } = new Recipe(
            new Dictionary<ItemIDPrev, int>
            {
                { ItemIDPrev.Refined_IronIngot, 1 },
            },
            new Dictionary<ItemIDPrev, int>
            {
                { ItemIDPrev.Product_IronBullet, 5 }
            },
            1.0f
        );
    }
}