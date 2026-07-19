using UnityEngine;
using UnityEngine.EventSystems;

namespace EyE.Unity.RGBPicker
{
    [RequireComponent(typeof(Collider))]
    public class RGBPlaneView : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [TextArea(4, 20)]
        [SerializeField]
        private string hierarchyNotes =
@"Hierarchy:

RGBColorPicker
 |-- Cube
      |-- RedPlane
      |    O-- RGBPlaneView (Fixed Red)
      |
      |-- GreenPlane
      |    O-- RGBPlaneView (Fixed Green)
      |
      |-- BluePlane
           O-- RGBPlaneView (Fixed Blue)

The plane represents a 2D slice of RGB space.
The fixed channel comes from the picker.
The other two channels are selected by UV position.";

        private RGBChannel fixedChannel;

        [SerializeField]
        private RGBColorPicker picker;
        //[SerializeField]
        //private Renderer planeRenderer;

        private Camera eventCamera;

        public RGBChannel FixedChannel => fixedChannel;

        private void Awake()
        {
            eventCamera = Camera.main;
            Vector3 localForward = picker.VolumeTransform.InverseTransformDirection(transform.forward);
            fixedChannel = RGBVolume.GetChannelForDirection(picker.AxisMapping, localForward);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            PointerCapture.Capture(this);
            UpdateColor(eventData);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            PointerCapture.Release(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("dragging plane event");
            if (PointerCapture.Owner != this) return;
            Debug.Log("DOING: drag on plane");
            UpdateColor(eventData);
        }



        private void UpdateColor(PointerEventData eventData)
        {
            Ray ray = eventData.pressEventCamera.ScreenPointToRay(eventData.position);

            Plane dragPlane = new Plane(transform.forward, transform.position);

            if (!dragPlane.Raycast(ray, out float distance))
            //if (!Physics.Raycast(ray, out RaycastHit hit))
            {
                return;
            }
            Vector3 worldPosition = ray.GetPoint(distance);

            Vector3 localPosition = transform.InverseTransformPoint(worldPosition);

            Vector2 uv = new Vector2(localPosition.x, localPosition.y);// hit.textureCoord;
            uv += Vector2.one * 0.5f;
            uv.x = Mathf.Clamp(uv.x,0,1);
            uv.y = Mathf.Clamp(uv.y,0,1);

            RGBColor color = picker.CurrentColor;

            switch (fixedChannel)
            {
                case RGBChannel.Red:
                    if (RGBVolume.axisInversion.y < 0)
                        uv.x = 1 - uv.x;
                    if (RGBVolume.axisInversion.z < 0)
                        uv.y = 1 - uv.y;

                    color.Green = uv.x;
                    color.Blue = uv.y;
                    break;

                case RGBChannel.Green:
                    if (RGBVolume.axisInversion.x < 0)
                        uv.x = 1 - uv.x;
                    if (RGBVolume.axisInversion.z < 0)
                        uv.y = 1 - uv.y;

                    color.Red = uv.x;
                    color.Blue = uv.y;
                    break;

                case RGBChannel.Blue:
                    if (RGBVolume.axisInversion.x < 0)
                        uv.x = 1 - uv.x;
                    if (RGBVolume.axisInversion.y < 0)
                        uv.y = 1 - uv.y;

                    color.Red = uv.x;
                    color.Green = uv.y;
                    break;
            }

            picker.SetColor(color);
        }
    }
}