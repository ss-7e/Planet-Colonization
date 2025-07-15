using Game.Turret;
using System;
using UnityEngine;

namespace Game.UI
{
    public class TurretUI : MonoBehaviour
    {
        public void SetUI(TurretBase turret, bool isLeft)
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
            turret.transform.Find("Quad").gameObject.SetActive(true);

        }
        public void HideUI(TurretBase turret)
        {
            this.gameObject.SetActive(false);
            if (turret != null)
            {
                turret.transform.Find("Quad").gameObject.SetActive(false);
            }
        }
    }
}
