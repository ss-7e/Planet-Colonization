using System;
using System.Collections.Generic;
using UnityEngine;
namespace Factory
{

    /// <summary>
    /// 
    /// </summary>
    public enum BeltDir //TODO 这个玩意后续不应该放这里
    {
        up, down, left, right
    }
    public static class BeltDirExtensions
    {
        public static BeltDir Opposite(this BeltDir dir)
        {
            return dir switch
            {
                BeltDir.up => BeltDir.down,
                BeltDir.down => BeltDir.up,
                BeltDir.left => BeltDir.right,
                BeltDir.right => BeltDir.left,
                _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
            };
        }

        public static BeltDir Not(this BeltDir dir) => dir.Opposite();
    }
    //----------------------------------------------------------------------------------------------------

    // TODO 将Mono和非Mono部分拆开
    // 不知道要不要做三通
    public class CanveyerBeltUnit : MonoBehaviour,  IItemInput
    {
        [SerializeField]
        private float _itemDeltaY;

        private IItemInput _itemTo;   //从本单元输出到这个接口
        private IConnectTo _itemFrom;
        internal IItemInput ItemTo => _itemTo;
        internal IConnectTo ItemFrom => _itemFrom;


        private BeltDir _inputDir;
        private BeltDir _outputDir;
        public BeltDir InputDir => _inputDir;
        public BeltDir OutputDir => _outputDir;


        private float _beltSpeed;
        private LinkedList<GameObject> _itemsOnBelt;
        private LinkedList<ItemType> _itemTypeOnBelt;
        private List<Vector3> _itemPositions;       //满排情况下物品的位置
        private readonly int _maxItems = 4;
        private int _currentItemCount;
        private float _deltaAxisValue;


        private void Update()
        {
            UpdateItemOnBelt();
        }
        

        // 初始化传送带：所有物品满排位置
        public void CanveyerBeltUnitInit(BeltDir inputDir, BeltDir outputDir)
        {
            _currentItemCount = 0;
            _itemTo = null;
            _beltSpeed = 2f; 
            _itemsOnBelt = new LinkedList<GameObject>();
            _itemPositions = new List<Vector3>(_maxItems + 1);
            _itemDeltaY = 0.1f;
            _inputDir = inputDir;
            _outputDir = outputDir;
            // 暂时把初始化放这里

            _deltaAxisValue = 0.25f;
            Vector3 deltaPos1 = Vector3.zero;
            Vector3 deltaPos2 = Vector3.zero;
            Vector3 outputPos = Vector3.zero;
            switch (outputDir)
            {
                case BeltDir.up:
                    deltaPos2 = new Vector3(0, 0, -_deltaAxisValue);
                    outputPos = new Vector3(0, _itemDeltaY, 0.5f) + transform.position;
                    break;
                case BeltDir.down:
                    deltaPos2 = new Vector3(0, 0, _deltaAxisValue);
                    outputPos = new Vector3(0, _itemDeltaY, -0.5f) + transform.position;
                    break;
                case BeltDir.left:
                    deltaPos2 = new Vector3(_deltaAxisValue, 0, 0);
                    outputPos = new Vector3(-0.5f, _itemDeltaY, 0) + transform.position;
                    break;
                case BeltDir.right:
                    deltaPos2 = new Vector3(-_deltaAxisValue, 0, 0);
                    outputPos = new Vector3(0.5f, _itemDeltaY, 0) + transform.position;
                    break;
            }
            switch (inputDir)
            {
                case BeltDir.up:
                    deltaPos1 = new Vector3(0, 0, _deltaAxisValue);
                    break;
                case BeltDir.down:
                    deltaPos1 = new Vector3(0, 0, -_deltaAxisValue);
                    break;
                case BeltDir.left:
                    deltaPos1 = new Vector3(-_deltaAxisValue, 0, 0);
                    break;
                case BeltDir.right:
                    deltaPos1 = new Vector3(_deltaAxisValue, 0, 0);
                    break;
            }
            
            for (int i = 0; i <= _maxItems; i++)
            {
                if (i <= _maxItems / 2)
                {
                    _itemPositions.Add(outputPos + i * deltaPos2 );
                }
                else
                {
                    _itemPositions.Add(_itemPositions[i - 1] + deltaPos1);
                }
            }
        }


        internal void SetItemDeliverTarget(IItemInput unit)
        {
            _itemTo = unit;
        }

        internal void SetItemDeliverFrom(IConnectTo itemInput)
        {
            _itemFrom = itemInput;
        }

        bool IItemInput.InputItem(GameObject item, ItemType itemType)
        {
            if (_currentItemCount >= _maxItems)
            {
                return false;
            }
            if(_itemsOnBelt.Count != 0)
            {
                GameObject lastItem = _itemsOnBelt.Last.Value;
                if((lastItem.transform.position - _itemPositions[_maxItems]).magnitude <= _deltaAxisValue)
                {
                    return false;
                }
            }

            _itemsOnBelt.AddLast(item);
            _itemTypeOnBelt.AddLast(itemType);
            item.transform.position = _itemPositions[_maxItems];
            _currentItemCount++;
            return true;
        }

        private void UpdateItemOnBelt()
        {
            int idx = 0;
            var type = _itemTypeOnBelt.First;
            for (var node = _itemsOnBelt.First; node != null;)
            {
                var item = node.Value;
                Vector3 targetPos = _itemPositions[idx];
                // 将当前编号物品移动到目标位置
                item.transform.position = Vector3.MoveTowards(item.transform.position, targetPos, _beltSpeed * Time.deltaTime);
                if(item.transform.position == targetPos && idx == 0)
                {
                    if(_itemTo != null && _itemTo.InputItem(item, type.Value))
                    {
                        _currentItemCount--;
                        _itemsOnBelt.Remove(node);
                        _itemTypeOnBelt.Remove(type);
                    }
                }
                idx++;
                node = node.Next;
                
            }
        }
    }
}