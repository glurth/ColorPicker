using UnityEngine;

[ExecuteAlways]
public class TransformBoundsAnchor : MonoBehaviour
{
    [System.Serializable]
    public enum Axis { X, Y, Z }
    [System.Serializable]
    public enum Side { Min, Max }

    [SerializeField] Axis axis = Axis.Z;
    [SerializeField] Side side = Side.Max;

    bool initialized;

    Vector3 initialPosition;
    Vector3 initialScale;
    Bounds localBounds;
    private void OnEnable()
    {
        initialized = false;
        Init();
    }

    void Init()
    {
        if (initialized)
            return;

        initialized = true;

        initialPosition = transform.localPosition;
        initialScale = transform.localScale;

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
        {
            localBounds = new Bounds(Vector3.zero, Vector3.zero);
            return;
        }

        Matrix4x4 worldToLocal = transform.worldToLocalMatrix;

        bool first = true;

        foreach (Renderer renderer in renderers)
        {
            Bounds b = renderer.bounds;

            Vector3 min = worldToLocal.MultiplyPoint3x4(b.min);
            Vector3 max = worldToLocal.MultiplyPoint3x4(b.max);

            Bounds lb = new Bounds();
            lb.SetMinMax(Vector3.Min(min, max), Vector3.Max(min, max));

            if (first)
            {
                localBounds = lb;
                first = false;
            }
            else
            {
                localBounds.Encapsulate(lb.min);
                localBounds.Encapsulate(lb.max);
            }
        }
    }

    void Update()
    {
        Init();

        Vector3 scale = transform.localScale;
        Vector3 pos = initialPosition;

        float initialAxisScale = Get(initialScale, axis);
        float currentAxisScale = Get(scale, axis);

        if (Mathf.Approximately(initialAxisScale, 0f))
            return;

        float initialExtent = GetExtent(initialAxisScale);
        float currentExtent = GetExtent(currentAxisScale);

        float delta = initialExtent - currentExtent;

        Set(ref pos, axis, Get(pos, axis) + delta);

        transform.localPosition = pos;
    }

    float GetExtent(float scale)
    {
        float value = side == Side.Min
            ? Get(localBounds.min, axis)
            : Get(localBounds.max, axis);

        return value * scale;
    }

    static float Get(Vector3 v, Axis axis)
    {
        return axis switch
        {
            Axis.X => v.x,
            Axis.Y => v.y,
            _ => v.z,
        };
    }

    static void Set(ref Vector3 v, Axis axis, float value)
    {
        switch (axis)
        {
            case Axis.X: v.x = value; break;
            case Axis.Y: v.y = value; break;
            case Axis.Z: v.z = value; break;
        }
    }
}