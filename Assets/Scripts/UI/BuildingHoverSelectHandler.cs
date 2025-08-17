using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Game.Towers;

public class BuildingHoverSelectHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public Image selectionFrame; 
    public float hoverSpeed = 4f;
    [SerializeField] GameObject towerPrefab;
    private bool selecting = false;
    private bool isHovering = false;
    TextMeshProUGUI text;
    void Start()
    {
        selectionFrame = UIManager.instance.downSelectionBarFrame;
        text = GetComponentInChildren<TextMeshProUGUI>();
    }
    void Update()
    {
        GameObject tower = BuildManager.instance.GetTowerToBuild();
        int towerCount = Cargo.instance.GetTowerCount(towerPrefab.GetComponent<Tower>());
        text.text = towerCount.ToString();
        selecting = (tower != null);
        if (isHovering && !selecting)
        {
            float scale = 1f + Mathf.Abs(Mathf.Sin(Time.time * hoverSpeed) * 0.05f);
            selectionFrame.gameObject.SetActive(true);
            selectionFrame.transform.localScale = new Vector3(scale, scale, 1);
        } 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selecting) return;
        if (selectionFrame.gameObject.activeSelf)return;
        isHovering = true;
        selectionFrame.gameObject.SetActive(true);
        selectionFrame.transform.position = transform.position;
        selectionFrame.transform.localScale = Vector3.one;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selecting) return;

        isHovering = false;
        selectionFrame.gameObject.SetActive(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isHovering = false;
            BuildManager.instance.SetTurretToBuild(towerPrefab);
            selectionFrame.gameObject.SetActive(true);
            selectionFrame.transform.position = transform.position;
            selectionFrame.transform.localScale = Vector3.one;
            selecting = true;
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            BuildManager.instance.SetTurretToBuild(null);
            isHovering = false;
            selecting = false;
            selectionFrame.gameObject.SetActive(false);
        }
    }

}
