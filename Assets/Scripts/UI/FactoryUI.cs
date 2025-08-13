using Game.Towers;
using UnityEngine;
using UnityEngine.UI;
using Game.Towers.Factory;

namespace Game.UI
{
    public class FactoryUI : MonoBehaviour
    {
        FactroyTowerBase currentFactory;
        Tower currentTower;
        public void Update()
        {
            if (currentFactory != null)
            {
                PopulateFactoryList(currentFactory);
            }
        }
        public void SetUI(FactroyTowerBase factory, bool isLeft)
        {
            currentFactory = factory;
            SetUIPos(factory, isLeft);
            PopulateFactoryList(factory);
        }
        public void SetUIPos(FactroyTowerBase factory, bool isLeft)
        {
            if (factory == null)
            {
                Debug.LogError("Factory is null, cannot set UI.");
                return;
            }
            // Set the position of the UI based on the factory's position
            float UIPosX = isLeft ? 400f : -400f; // Adjust the X position based on left or right
            RectTransform rectTransform = GetComponent<RectTransform>();
            Vector3 UIPos = rectTransform.localPosition;
            UIPos.x = UIPosX;
            rectTransform.localPosition = UIPos;
            currentFactory = factory;
            this.gameObject.SetActive(true);
        }
        public void HideUI(FactroyTowerBase factory)
        {
            this.gameObject.SetActive(false);
            if (factory != null)
            {
                // Additional cleanup if necessary
            }
        }
        private void PopulateFactoryList(FactroyTowerBase factory)
        {
            // Implement logic to populate the factory list in the UI
        }
    }
}
