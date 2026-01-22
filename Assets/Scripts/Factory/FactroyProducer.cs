using System.Collections.Generic;
using UnityEngine;

namespace Factory
{

    /// <summary>
    /// 而且是自动生产的
    /// 输入：原材料，合成表
    ///     只能接受一种合成表原料缓存，切换导致原料完全丢失（这好吗？
    /// 输出：产出物，输出接口
    ///     输出缓存？
    /// </summary>
    public class FactoryProducer : FactorySquare, IItemInput
    {
        private IItemInput _outputPort;
        private Recipe _activeRecipe;
        private Dictionary<ItemType, int> _inputItems; //原材料缓存
        private Dictionary<ItemType, int> _storageItems; //产出物缓存？
        private int _storageCapacity = 10;
        private float _productionTimer = 0f;

        public new void Start()
        {
            base.Start();
            _productionTimer = 0;
            _activeRecipe = RecipeManager.Instance.IronSmelt;
            _inputItems = new Dictionary<ItemType, int>(_activeRecipe.Inputs);
            foreach (var item in _activeRecipe.Inputs)
            {
                _inputItems[item.Key] = 0; // 初始化计数为0
            }
            _storageItems = new Dictionary<ItemType, int>();
            foreach (var item in _activeRecipe.Outputs)
            {
                _storageItems[item.Key] = 0; 
            }

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

        public bool InputItem(GameObject item, ItemType type)
        {
            if (!_inputItems.ContainsKey(type))
            {
                return false; // 该物品不是当前配方所需的原材料
            }
            Destroy(item);
            return true;
        }

        public bool InputItem(ItemType item)
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
            foreach(var output in _activeRecipe.Outputs)
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
                if(_productionTimer >= _activeRecipe.ProduceTime)
                {
                    _productionTimer = 0;
                    foreach (var input in _activeRecipe.Inputs)
                    {
                        _inputItems[input.Key] -= input.Value; // 消耗原材料
                    }
                    // 生产物品
                    foreach(var output in _activeRecipe.Outputs)
                    {
                        _storageItems[output.Key] += output.Value;
                    }
                }
                else
                {
                    _productionTimer += Time.deltaTime;
                }
            }

            if (_storageItems.Count > 0) // 传入的物品满足合成表
            {
                GameObject producedItem = null;
                foreach (var output in _storageItems)
                {
                    if (_outputPort is CanveyerBeltUnit beltUnit)
                    {
                        producedItem = Instantiate(ItemListManager.Instance.ItemPrefabs[output.Key]);
                    }
                    if (_outputPort != null && output.Value > 0 && _outputPort.InputItem(producedItem, output.Key)) 
                    {
                        _storageItems[output.Key] -= 1; // 假设每次传输一个单位
                        break; // 每次只传输一个物品
                    }
                }
            }
        }
    }
}
