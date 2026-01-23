using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Factory
{
    /// <summary>
    /// 进一步的连接器
    /// 接受物品输入的机器需要实现
    /// 能输入就必然能够输出
    /// </summary>
    public interface IItemInput : IConnectTo
    {
        bool InputItem(GameObject item, ItemID itemType);

        void ConnetTo(Grid grid)
        {
            if (grid.ItemOutputFromBuilding != null)
            {
                Connection.SetSource(grid.ItemOutputFromBuilding);
                grid.ItemOutputFromBuilding.Connection.SetTarget(this);
            }

            //危险！对于机器输入可能不对
            if(grid.ItemInputToBuilding != null)
            {
                Connection.SetTarget(grid.ItemInputToBuilding);
                grid.ItemInputToBuilding.Connection.SetSource(this);
            }
        }

    }
}
