using Game.Ammo;
using Game.Modules;
using Game.Turret;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace Game.UI
{
    public class TurretUI : MonoBehaviour
    {

        public GameObject itemPrefab;
        public Transform contentParent;
        TurretBase currentTurret;

        public void Update()
        {
            if ( currentTurret != null) 
            { 
                PopulateList(currentTurret);
            }
        }
        public void SetUI(TurretBase turret, bool isLeft)
        {
            currentTurret = turret;
            SetUIPos(turret, isLeft);
            PopulateList(turret);
        }

        public void SetUIPos(TurretBase turret, bool isLeft)
        {
            if (turret == null)
            {
                Debug.LogError("Turret is null, cannot set UI.");
            }
            // Set the position of the UI based on the turret's position
            float UIPosX = isLeft ? 400f : -400f; // Adjust the X position based on left or right
            RectTransform rectTransform = GetComponent<RectTransform>();
            Vector3 UIPos = rectTransform.localPosition;
            UIPos.x = UIPosX;
            rectTransform.localPosition = UIPos;
            this.gameObject.SetActive(true);
            SetQuad(turret);
        }
        void SetQuad(TurretBase turret)
        {
            GameObject quad = turret.transform.Find("Quad").gameObject;
            RadarModule radarModule = turret.radarInstances[0].module as RadarModule;
            float radarRange = radarModule != null ? radarModule.GetRadarRange() : 0f;
            quad.transform.localScale = Vector3.one * 4f * radarRange;
            quad.SetActive(true);
        }
        public void HideUI(TurretBase turret)
        {
            this.gameObject.SetActive(false);
            if (turret != null)
            {
                turret.transform.Find("Quad").gameObject.SetActive(false);
            }
        }
        void PopulateList(TurretBase turret)
        {
            if (turret == null){return;}
            List<string> dataList = new List<string>();
            TurretAmmoStorage ammoStorage = turret.ammoStorage;
            IReadOnlyList<ShellData> ammoOrder = ammoStorage.GetAmmoOrder();
            dataList.Add("Ammo Storage:");
            foreach (ShellData shellData in ammoOrder)
            {
                int count = ammoStorage.GetAmmoCount(shellData);
                dataList.Add($"{shellData.name}: {count}");
            }


            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
            foreach (string data in dataList)
            {
                GameObject newItem = Instantiate(itemPrefab, contentParent);
                newItem.GetComponent<TextMeshProUGUI>().text = data;
            }
        }
    }
}
