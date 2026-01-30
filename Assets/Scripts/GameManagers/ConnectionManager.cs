using System.Collections.Generic;
using UnityEngine;

namespace Factory
{
    /// <summary>
    /// 管理场景中所有的链接关系：
    ///     传送带之间的链接
    ///     传送带和生产器/消费器之间的链接
    /// TODO:是否需要？
    /// </summary>
    //public class ConnectionManager : MonoBehaviour
    //{
    //    public static ConnectionManager Instance;
    //    Dictionary<IConnectTo, IItemInput> _connections = new Dictionary<IConnectTo, IItemInput>();
        

    //    private void Awake()
    //    {
    //        if (Instance == null)
    //        {
    //            Instance = this;
    //        }
    //        else
    //        {
    //            Destroy(this.gameObject);
    //        }
    //    }
    //    internal void RegisterConnection(IConnectTo from, IItemInput to)
    //    {
    //        if (!_connections.ContainsKey(from))
    //        {
    //            _connections.Add(from, to);
    //        }
    //        else
    //        {
    //            _connections[from] = to;
    //        }
    //    }

    //}
}
