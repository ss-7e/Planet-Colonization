using System.Collections.Generic;
using UnityEngine;

namespace Factory
{

    /// <summary>
    /// 最小单位工厂方块
    /// TODO:添加占用格子信息？
    /// </summary>
    public class FactorySquare : MonoBehaviour
    {
        
        protected List<GameObject> buildSpots;
        List<FactorySquare> _connectedFactorylist;
        public void Start()
        {
            buildSpots = new List<GameObject>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.GetComponent<BuildSpot>())
                {
                    buildSpots.Add(child.gameObject);
                    child.gameObject.SetActive(false);
                }
            }
        }



        /// <summary>
        /// 即将废弃
        /// </summary>
        public virtual void ConfirmBuild()
        {
            
        }
    }
}
