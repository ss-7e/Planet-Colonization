using UnityEngine;
    
namespace Factory
{
    /// <summary>
    /// 能够连接机器需要实现这个接口
    /// 体现在能够输出物品
    /// </summary>
    public interface IConnection 
    {
        BuildingConnection Connection { get; }
    }

    public interface IItemOutput : IConnection
    {
        /// <summary>
        /// 这个方法主要实现检查目前grid是否有其他机器放置的接受输入，然后进行绑定
        /// 如果这个是IItemInput，还需要绑定其他机器在grid上放的IConnectTo（接受其他机器输入）
        /// 信任grid上的信息是正确的
        /// </summary>
        void ConnectInputOnGrid(Grid grid)
        {
            if (grid.ItemInputToBuilding != null)
            {
                Connection.SetTarget(grid.ItemInputToBuilding);
                grid.ItemInputToBuilding.Connection.SetSource(this);
            }
        }
        /// <summary>
        /// TODO: 添加一个在所有输出grid上放置IConnectTo的方法
        /// </summary>
        void SetOutputOnGrid(Grid centerGrid);
    }



    /// <summary>
    /// 进一步的连接器
    /// 接受物品输入的机器需要实现
    /// 能输入就必然能够输出
    /// </summary>
    public interface IItemInput : IConnection
    {
        bool InputItem(GameObject item, ItemIDPrev itemType);

        void ConnetTo(Grid grid)
        {
            if (grid.ItemOutputFromBuilding != null)
            {
                Connection.SetSource(grid.ItemOutputFromBuilding);
                grid.ItemOutputFromBuilding.Connection.SetTarget(this);
            }

            //危险！对于机器输入方向可能不对
            if (grid.ItemInputToBuilding != null)
            {
                Connection.SetTarget(grid.ItemInputToBuilding);
                grid.ItemInputToBuilding.Connection.SetSource(this);
            }
        }
        void SetInputOnGrid(Grid centerGrid);

    }


    /// <summary>
    /// 维护机器连接关系的组件
    /// TODO: 多个连接？
    /// </summary>
    public class BuildingConnection
    {
        private IConnection _from = null;
        private IItemInput _to = null;
        public IConnection From => _from;
        public IItemInput To => _to;
        public void SetTarget(IItemInput target)
        {
            _to = target;
        }

        public void SetSource(IConnection source)
        {
            _from = source;
        }

        ~BuildingConnection()
        {
            _from?.Connection.SetTarget(null);
            _to?.Connection.SetSource(null);
            _from = null;
            _to = null;
        }

    }

}
