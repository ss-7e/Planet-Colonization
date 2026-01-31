using System.Collections.Generic;
using UnityEngine;

namespace Factory
{

    /// <summary>
    /// 而且是自动生产的
    /// 输入：原材料，合成表
    ///     只能接受一种合成表原料缓存，切换导致原料完全丢失（这好吗？
    ///     列表化输入，如果接受多种原料单口输入，会堵传送带 (可能要改
    /// 输出：产出物，输出接口
    ///     输出缓存？
    /// </summary>
    public class FactoryProducer : FactorySquare, IItemInput, IItemOutput
    {
        public GameObject ProducerPrefab;//TODO: 后续删除


        public BuildingConnection Connection { get; } = new();
        private Recipe _activeRecipe;
        private Dictionary<ItemIDPrev, int> _inputItems; //原材料缓存
        private Dictionary<ItemIDPrev, int> _storageItems; //产出物缓存？
        private int _storageCapacity = 10;
        private float _productionTimer = 0f;

        public new void Start()
        {
            base.Start();
            _productionTimer = 0;
            _inputItems = new Dictionary<ItemIDPrev, int>();
            _storageItems = new Dictionary<ItemIDPrev, int>();
            ChangeRecipe(RecipeManager.Instance.IronSmelt);     //TODO： 临时测试，后续修改
        }

        protected virtual void ChangeRecipe(Recipe newRecipe)
        {
            _activeRecipe = newRecipe;
            // 重置输入和存储字典
            _inputItems.Clear();
            foreach (var item in _activeRecipe.Inputs)
            {
                _inputItems[item.Key] = 0;
            }
            _storageItems.Clear();
            foreach (var item in _activeRecipe.Outputs)
            {
                _storageItems[item.Key] = 0;
            }
        }

        public bool InputItem(GameObject item, ItemIDPrev type)
        {
            if (!_inputItems.ContainsKey(type))
            {
                return false; // 该物品不是当前配方所需的原材料
            }
            _inputItems[type]++;
            item.transform.position = transform.position + Vector3.down * 2;
            return true;
        }

        public bool InputItem(ItemIDPrev item)
        {
            if (_inputItems.ContainsKey(item))
            {
                _inputItems[item]++;
                return true;
            }
            return false;
        }

        private void Update()
        {
            ProduceItem();
        }

        private void ProduceItem()
        {
            bool canProduce = true;
            foreach (var input in _inputItems)
            {
                if (input.Value < _activeRecipe.Inputs[input.Key])
                {
                    canProduce = false;
                    break; // 原材料不足，无法生产
                }
            }
            foreach (var output in _activeRecipe.Outputs)
            {
                if (_storageItems.ContainsKey(output.Key))
                {
                    if (_storageItems[output.Key] + output.Value > _storageCapacity)
                    {
                        canProduce = false;
                        break; // 存储空间不足，无法生产 这个逻辑是不对的（
                    }
                }
            }

            if (canProduce)
            {
                if (_productionTimer >= _activeRecipe.ProduceTime)
                {
                    _productionTimer = 0;
                    foreach (var input in _activeRecipe.Inputs)
                    {
                        _inputItems[input.Key] -= input.Value; // 消耗原材料
                    }
                    // 生产物品
                    foreach (var output in _activeRecipe.Outputs)
                    {
                        _storageItems[output.Key] += output.Value;
                    }
                }
                else
                {
                    _productionTimer += Time.deltaTime;
                }
            }

            if (_storageItems.Count > 0) //TODO 这个不对
            {
                
                IItemInput outputPort = Connection.To;
                foreach (var output in _storageItems)
                {
                    // 每次都需要遍历所有产出类型，效率有点低
                    if (outputPort != null && output.Value > 0 && outputPort.InputItem(Instantiate(ProducerPrefab), output.Key))
                    {
                        _storageItems[output.Key] -= 1; // 假设每次传输一个单位
                        break; // 每次只传输一个物品
                    }
                }
            }
        }
        public void SetOutputOnGrid(Grid centerGrid)
        {
            Vector2Int centerXY = new Vector2Int((int)centerGrid.Pos.x, (int)centerGrid.Pos.z);
            Grid targetGrid = GridManager.Instance.GetGridByXZ(centerXY.x, centerXY.y + 1, out _);
            targetGrid?.AddItemOutputFromBuildingDir(this, PortDir.down);  //TODO: 临时写为输入到上面格子
        }
        public void SetInputOnGrid(Grid centerGrid)
        {
            Vector2Int centerXY = new Vector2Int((int)centerGrid.Pos.x, (int)centerGrid.Pos.z);
            Grid targetGrid = GridManager.Instance.GetGridByXZ(centerXY.x, centerXY.y - 1, out _);
            targetGrid?.AddItemInputToBuildingDir(this, PortDir.up);  //TODO: 临时写为从下面格子输入
        }
    }
}
