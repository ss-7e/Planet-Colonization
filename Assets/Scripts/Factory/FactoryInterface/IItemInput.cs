using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Factory
{
    internal interface IItemInput : IConnectTo
    {
        bool InputItem(GameObject item);
    }
}
