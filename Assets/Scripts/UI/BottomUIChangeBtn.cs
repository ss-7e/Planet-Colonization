using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BottomUIChangeBtn : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    private Image image;
    private Transform parentTransform;
    public bool isSelected = false;
    public GameObject selectSets;
    public float enterAlpha = 0.3f;
    public float exitAlpha = 0f;
    public float clickAlpha = 1f;

    private void Awake()
    {
        image = GetComponent<Image>();
        parentTransform = transform.parent;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected)
            return;
        SetAlpha(0.3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
            SetAlpha(0f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetAlpha(1f);
        isSelected = true;
        if (selectSets != null)
        {
            selectSets.SetActive(true);
        }
        foreach (Transform sibling in parentTransform)
        {
            if (sibling != transform)
            {
                BottomUIChangeBtn otherController = sibling.GetComponent<BottomUIChangeBtn>();
                if (otherController != null)
                {
                    otherController.isSelected = false;
                    otherController.SetAlpha(0f);
                    if (otherController.selectSets != null)
                    {
                        otherController.selectSets.SetActive(false);
                    }
                }
            }
        }
    }
    private void SetAlpha(float alpha)
    {
        SetAlpha(image, alpha);
    }

    private void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}
