using UnityEngine;

namespace Factory
{
    /// <summary>
    /// 能够连接机器需要实现这个接口
    /// 体现在能够输出物品
    /// </summary>
    public interface IConnectTo 
    {
        BuildingConnection Connection { get; }
        /// <summary>
        /// 这个方法主要实现检查目前grid是否有其他机器放置的接受输入，然后进行绑定
        /// 如果这个是IItemInput，还需要绑定其他机器在grid上放的IConnectTo（接受其他机器输入）
        /// 信任grid上的信息是正确的
        /// </summary>
        void ConnectTo(Grid grid)
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

    }

    public class BuildingConnection
    {
        private IConnectTo _from = null;
        private IItemInput _to = null;
        public IConnectTo From => _from;
        public IItemInput To => _to;
        public void SetTarget(IItemInput target)
        {
            _to = target;
        }

        public void SetSource(IConnectTo source)
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
