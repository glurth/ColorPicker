using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraViewportFromRectTransform : MonoBehaviour
{
    [SerializeField] private RectTransform targetRect;
    [SerializeField] private Canvas canvas;
    [SerializeField] private float aspectRatio = -1;
    private float startingAscpectRatio;
    private Camera cam;

    private void OnEnable()
    {
        Init();

        if (cam != null)
        {
            startingAscpectRatio = cam.aspect;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Init();

        if (cam != null)
        {
            startingAscpectRatio = cam.aspect;
        }
    }
#endif

    private void Init()
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }

        if (canvas == null && targetRect != null)
        {
            canvas = targetRect.GetComponentInParent<Canvas>();
        }
    }

    private void Update()
    {
        Init();

        if (cam == null || targetRect == null)
        {
            return;
        }

        Camera uiCamera = null;

        if (canvas != null &&
            canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = canvas.worldCamera;
        }

        Vector3[] corners = new Vector3[4];
        targetRect.GetWorldCorners(corners);

        Vector2 min = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[0]);
        Vector2 max = min;

        for (int i = 1; i < 4; i++)
        {
            Vector2 p = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[i]);

            min = Vector2.Min(min, p);
            max = Vector2.Max(max, p);
        }

        Rect pixelRect = new Rect(
            min.x,
            min.y,
            max.x - min.x,
            max.y - min.y);

        float targetAspect = aspectRatio < 0f ? startingAscpectRatio : aspectRatio;
        float rectAspect = pixelRect.width / pixelRect.height;

        if (rectAspect > targetAspect)
        {
            // Too wide: reduce width (pillarbox)
            float width = pixelRect.height * targetAspect;
            float x = pixelRect.x + (pixelRect.width - width) * 0.5f;

            pixelRect = new Rect(
                x,
                pixelRect.y,
                width,
                pixelRect.height);
        }
        else
        {
            // Too tall: reduce height (letterbox)
            float height = pixelRect.width / targetAspect;
            float y = pixelRect.y + (pixelRect.height - height) * 0.5f;

            pixelRect = new Rect(
                pixelRect.x,
                y,
                pixelRect.width,
                height);
        }
        Rect viewport = new Rect(
            pixelRect.x / Screen.width,
            pixelRect.y / Screen.height,
            pixelRect.width / Screen.width,
            pixelRect.height / Screen.height);

        cam.rect = viewport;
    }
}