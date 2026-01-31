using UnityEngine;
using System.Collections.Generic;

namespace Factory
{

    /// <summary>
    /// 支持输入输出物品的储存工厂
    /// TODO: 加入storage component支持真正物品存储
    /// </summary>
    public class FactoryStorager : FactorySquare, IItemInput, IItemOutput
    {
        [SerializeField]
        private GameObject _itemprefab;

        public BuildingConnection Connection { get; } = new();
        private StorageComponent _storageComp = new(5);

        public PortDir OutputDir => PortDir.up;

        public new void Start()
        {
            base.Start();
        }

        bool IItemInput.InputItem(GameObject item, ItemIDPrev itemType)
        {
            if(_storageComp.AddItem(itemType, 1))
            {
                item.transform.position = transform.position + Vector3.down * 2;
                return true;
            }
            return false;
        }


        private void Update()
        {
            IItemInput itemTo = Connection.To;
            //TODO 测试代码，后续删除
            if (Input.GetKeyDown(KeyCode.O))
            {
                itemTo?.InputItem(CreateItem(), ItemIDPrev.Raw_IronOre);
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                itemTo?.InputItem(CreateItem(), ItemIDPrev.Refined_IronIngot);
            }
        }
        GameObject CreateItem()
        {
            GameObject item = Instantiate(_itemprefab);
            item.transform.position = transform.position;
            return item;
        }

        private void OnDestroy()
        {
            
        }
        public void SetOutputOnGrid(Grid centerGrid)
        {
            Vector2Int centerXY = new((int)centerGrid.Pos.x, (int)centerGrid.Pos.z);
            Grid targetGrid = GridManager.Instance.GetGridByXZ(centerXY.x + 1, centerXY.y, out _);
            targetGrid.AddItemOutputFromBuildingDir(this, PortDir.left);  //TODO: 临时写为输出到右边
        }
        public void SetInputOnGrid(Grid centerGrid)
        {
            Vector2Int centerXY = new((int)centerGrid.Pos.x, (int)centerGrid.Pos.z);
            Grid targetGrid = GridManager.Instance.GetGridByXZ(centerXY.x, centerXY.y - 1, out _);
            targetGrid?.AddItemInputToBuildingDir(this, PortDir.up);  //TODO: 临时写为输入到下面
        }
    }
}
