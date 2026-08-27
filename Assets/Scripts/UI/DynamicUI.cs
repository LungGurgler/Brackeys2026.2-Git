using UnityEngine;
using UnityEngine.InputSystem;

public class DynamicUI : MonoBehaviour
{
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float maxDistance = 25f;

    private RectTransform rectTransform;
    private Vector2 startPos;
    private Vector2 targetPos;
    private Camera uiCamera;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.localPosition;

        Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
        uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
    }

    void Update()
    {
        if (rectTransform.parent == null) return;

        if (Mouse.current == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            mousePosition,
            uiCamera,
            out targetPos
        );

        targetPos += offset;

        Vector2 direction = targetPos - startPos;
        if (direction.magnitude > maxDistance)
            targetPos = startPos + direction.normalized * maxDistance;

        rectTransform.localPosition = Vector2.Lerp(
            rectTransform.localPosition,
            targetPos,
            Time.deltaTime * followSpeed
        );
    }
}