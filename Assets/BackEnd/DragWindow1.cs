using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindow1 : MonoBehaviour, IDragHandler
{
    public Canvas canvas;

    private RectTransform rectTransform;
    private RectTransform canvasRectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, eventData.pressEventCamera, out localCursor))
            return;

        Vector2 clampedPosition = ClampToCanvas(localCursor);
        rectTransform.localPosition = clampedPosition;
    }

    private Vector2 ClampToCanvas(Vector2 position)
    {
        Vector2 size = rectTransform.rect.size;
        Vector2 canvasSize = canvasRectTransform.rect.size;

        // Calculate the offset between the canvas center and the parent's center
        Vector2 canvasOffset = (canvasSize - size) / 3;

        float minX = -canvasOffset.x;
        float maxX = canvasOffset.x-400f;
        float minY = -canvasOffset.y+250f;
        float maxY = canvasOffset.y-250f;

        float x = Mathf.Clamp(position.x, minX, maxX);
        float y = Mathf.Clamp(position.y, minY, maxY);

        return new Vector2(x, y);
    }
}
