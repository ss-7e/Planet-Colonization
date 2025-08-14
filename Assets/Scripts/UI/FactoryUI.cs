using Game.Towers;
using UnityEngine;
using UnityEngine.UI;
using Game.Towers.Factory;
using System.Xml.Serialization;

namespace Game.UI
{
    public class FactoryUI : MonoBehaviour
    {
        FactroyTowerBase currentFactory;
        Tower currentTower;
        [SerializeField] private GameObject selectImage;
        [SerializeField] private AssembleLinePress[] assembleLines;
        [SerializeField] private GameObject progressBar;
        public void Update()
        {
            if (currentFactory != null)
            {
                PopulateFactoryList(currentFactory);
            }
            UpdateProgressBar();
        }
        public void SetUI(FactroyTowerBase factory, bool isLeft)
        {
            currentFactory = factory;
            SetUIPos(factory, isLeft);
            PopulateFactoryList(factory);
            if(currentFactory.assblelinePressUI != null)
            {
                OnAssemblelineSelect(currentFactory.assblelinePressUI);
            }
            else
            {
                selectImage.SetActive(false);
                progressBar.SetActive(false);
            }
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

        public void OnAssemblelineSelect(AssembleLinePress assembleLine)
        {
            if (assembleLine == null)
            {
                Debug.LogError("AssembleLine is null, cannot select.");
                return;
            }
            selectImage.SetActive(true);
            progressBar.SetActive(true);
            Vector3 pos = selectImage.transform.localPosition;
            pos.y = 0;
            selectImage.transform.SetParent(assembleLine.transform, false);
            selectImage.transform.localPosition = pos;
            progressBar.transform.SetParent(assembleLine.transform, false);


            assembleLine.isSelected = true; 
            foreach (AssembleLinePress line in assembleLines)
            {
                if (line != assembleLine)
                {
                    line.isSelected = false;
                }
            }
            currentFactory.assblelinePressUI = assembleLine;

        }
        public void SetAssembleLine(AssembleLine line)
        {
            if( currentFactory != null)
            {
                currentFactory.ActiveTurretLine(line);
            }
        }

        public void accerateProduce(float time = 0.5f)
        {
            if (currentFactory != null)
            {
                currentFactory.AccelerateProduce(time);
            }
        }

        void UpdateProgressBar()
        {
            RectMask2D mask = progressBar.GetComponent<RectMask2D>();
            Rect rect = mask.rectTransform.rect;
            float progress = currentFactory.GetProducePercent();
            mask.padding = new Vector4(-1.0f, 0 , rect.width * (1.0f - progress), 0);
        }
    }
}
