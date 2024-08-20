using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class boundaries1 : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector2 minPosition;
    private Vector2 maxPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;

        // Calculate the minimum and maximum positions within screen bounds
        Vector2 halfSize = rectTransform.sizeDelta / 2;
        minPosition = new Vector2(halfSize.x, halfSize.y);
        maxPosition = new Vector2(Screen.width - halfSize.x, Screen.height - halfSize.y);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;

        // Clamp the position within screen bounds
        rectTransform.anchoredPosition = new Vector2(
            Mathf.Clamp(rectTransform.anchoredPosition.x, minPosition.x, maxPosition.x),
            Mathf.Clamp(rectTransform.anchoredPosition.y, minPosition.y, maxPosition.y)
        );
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Reset to original position if dragged outside bounds
        if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition))
        {
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}
