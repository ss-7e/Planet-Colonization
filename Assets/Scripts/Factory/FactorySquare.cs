using System.Collections.Generic;
using UnityEngine;

namespace Factory
{
    public class FactorySquare : MonoBehaviour
    {

        List<GameObject> buildSpots;
        List<FactorySquare> connectedFactorylist;
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
        /// 确认在这里建造，启用功能，启用BuildSpots
        /// </summary>
        public void ConfirmBuild()
        {
            connectedFactorylist = new List<FactorySquare>();
            foreach(GameObject spot in buildSpots)
            {
                spot.SetActive(true);
            }

        }
        public void GetItem()
        {

        }
    }
}
