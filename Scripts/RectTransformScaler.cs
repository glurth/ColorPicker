using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class RectTransformScaler : MonoBehaviour
{
    bool initialized = false;
    Vector2 initialSize;
    Vector3 initialScale;

    void Init()
    {
        if (initialized) return;
        initialized = true;
        rect = (RectTransform)transform;
        initialSize = rect.rect.size;
        initialScale = transform.localScale;
        if (initialSize.x == 0 || initialSize.y == 0)
            Debug.LogError("Unable to init RectTransformScaler :" + gameObject.name + " initialSize may not have zeros for dimension: " + initialSize); 
    }
    void OnEnable()
    {
        initialized = false;

        Init();
        UpdateScale();
    }

    RectTransform rect;

    void OnRectTransformDimensionsChange() => UpdateScale();

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying)
            UpdateScale();
    }
#endif

    void UpdateScale()
    {
        if (rect == null)
            rect = (RectTransform)transform;
        if (!enabled) return;
        Init();
        Vector2 size = rect.rect.size;
        float sx = size.x / initialSize.x;
        float sy = size.y / initialSize.y;
        float sz = Mathf.Sqrt(sx * sy);
        transform.localScale = new Vector3(
            initialScale.x * sx,
            initialScale.y * sy,
            initialScale.z * sz);
    }
}