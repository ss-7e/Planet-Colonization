using UnityEngine;

namespace Factory
{
    public class FactoryStorager : FactorySquare, IConnectTo
    {
        [SerializeField]
        private GameObject _itemprefab;
        private IItemInput _itemTo;
        private Grid _targetGrid;

        public Grid TargetGrid => _targetGrid;
        public BeltDir OutputDir => BeltDir.up;

        public new void Start()
        {
            base.Start();
        }

        internal void SetItemTarget(IItemInput target)
        {
            _itemTo = target;
        }
        private void Update()
        {
            //TODO 测试代码，后续删除
            if (Input.GetKeyDown(KeyCode.O))
            {
                if (_itemTo != null)
                {
                    Debug.Log("FactoryProducer: Start Output Item");
                    _itemTo.InputItem(CreateItem(), ItemType.Raw_IronOre);
                }
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (_itemTo != null)
                {
                    Debug.Log("FactoryProducer: Start Output Item");
                    _itemTo.InputItem(CreateItem(), ItemType.Refined_IronIngot);
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
            Grid grid = GridManager.instance.GetGridXY(transform.position, out Vector2Int XYpos);
            if (grid != null) 
            {
                _targetGrid = GridManager.instance.GetGridXY(XYpos.x, XYpos.y + 1);
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
