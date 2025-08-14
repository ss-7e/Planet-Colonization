using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AssembleLinePress : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Settings")]
    public float longPressThreshold = 0.35f;   
    public float cancelByMoveDistance = 10f;   

    [Header("State")]
    public bool isSelected = false;            

    [Header("Events")]
    public UnityEvent onLongPress;             
    public UnityEvent onClick;                 

    private bool pressed;
    private bool longPressed;
    private Vector2 pressPos;
    private float pressTime;
    private bool moved;

    public ScrollRect parentScroll;

    private void Awake()
    {
        parentScroll = GetComponentInParent<ScrollRect>();
    }

    private void Update()
    {
        if (!pressed || longPressed || moved) return;

        if (isSelected) return;

        if (Time.unscaledTime - pressTime >= longPressThreshold)
        {
            if (IsPointerOverSelf())
            {
                longPressed = true;
                onLongPress?.Invoke();
            }
            else
            {
                CancelPress();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        longPressed = false;    
        moved = false;
        pressPos = eventData.position;
        pressTime = Time.unscaledTime;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!moved && !longPressed && isSelected)
        {
            onClick?.Invoke();
        }

        CancelPress();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!longPressed) CancelPress();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Vector2.Distance(pressPos, eventData.position) > cancelByMoveDistance)
        {
            moved = true;
            CancelPress();
        }
        if (parentScroll != null)
            parentScroll.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (parentScroll != null)
            parentScroll.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (parentScroll != null)
            parentScroll.OnEndDrag(eventData);
    }


    private bool IsPointerOverSelf()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            (RectTransform)transform, Input.mousePosition, null); 
    }

    private void CancelPress()
    {
        pressed = false;
        longPressed = false;
    }
}
