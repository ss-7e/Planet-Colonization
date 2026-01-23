using UnityEngine;
using System.Collections.Generic;

namespace Factory
{

    /// <summary>
    /// 支持输入输出物品的储存工厂
    /// TODO: 加入storage component支持真正物品存储
    /// </summary>
    public class FactoryStorager : FactorySquare, IItemInput
    {
        [SerializeField]
        private GameObject _itemprefab;

        public BuildingConnection Connection { get; } = new(); //TODO: 将后面两个接口替换成这个

        private Grid _targetGrid;
        private List<ItemID> _storedItemTypes = new();
        //private StorageComponent _storageComp;

        public Grid TargetGrid => _targetGrid;
        public BeltDir OutputDir => BeltDir.up;

        public new void Start()
        {
            base.Start();
        }

        bool IItemInput.InputItem(GameObject item, ItemID itemType)
        {
            return true;
        }


        private void Update()
        {
            IItemInput itemTo = Connection.To;
            //TODO 测试代码，后续删除
            if (Input.GetKeyDown(KeyCode.O))
            {
                if (itemTo != null)
                {
                    Debug.Log("FactoryProducer: Start Output Item");
                    itemTo.InputItem(CreateItem(), ItemID.Raw_IronOre);
                }
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (itemTo != null)
                {
                    Debug.Log("FactoryProducer: Start Output Item");
                    itemTo.InputItem(CreateItem(), ItemID.Refined_IronIngot);
                }
            }
        }
        GameObject CreateItem()
        {
            GameObject item = Instantiate(_itemprefab);
            item.transform.position = transform.position;
            return item;
        }

        public override void ConfirmBuild()
        {
            base.ConfirmBuild();
            Debug.Log("storage Factory ConfirmBuild");  
            Grid grid = GridManager.Instance.GetGridXY(transform.position, out Vector2Int XYpos);
            if (grid != null) 
            {
                _targetGrid = GridManager.Instance.GetGridXY(XYpos.x, XYpos.y + 1);
                if (_targetGrid != null)
                {
                    _targetGrid.ProducerFrom = this;
                }
            }
        }

        private void OnDestroy()
        {
            if(_targetGrid != null)
            {
                _targetGrid.ProducerFrom = null;
            }
        }

    }
}
