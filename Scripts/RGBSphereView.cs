using UnityEngine;
using UnityEngine.EventSystems;

namespace EyE.Unity.RGBPicker
{
    [RequireComponent(typeof(Collider))]
    public class RGBSphereView : MonoBehaviour, IDragHandler, IEndDragHandler, IRGBColorChanged, IPointerDownHandler, IPointerUpHandler
    {
        [TextArea(4, 20)]
        [SerializeField]
        private string hierarchyNotes =
@"Hierarchy:

RGBColorPicker
 |-- Cube
      |-- Sphere
           O-- RGBSphereView

Represents the selected RGB value.
Dragging the sphere changes all three channels.";



        [SerializeField]
        private RGBColorPicker picker;

        private Plane dragPlane;

        /*public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Pointer down on sphere");
            dragPlane = new Plane(transform.forward,transform.position);
        }*/

        RGBDragState currentState;
        private void Update()
        {
            if (currentState == RGBDragState.None) return;
            //if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift) || Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
            {
                GetDragStateAndPlaneNormal();
                picker.SetDragState(currentState);
            }
        }


        Vector3 GetDragStateAndPlaneNormal()
        {
            Vector3 planeNormal = transform.forward;

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                planeNormal = transform.right;
            }
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                planeNormal = transform.up;
            }

            RGBChannel channel = RGBVolume.GetChannelForDirection(picker.AxisMapping, planeNormal);

            if (channel == RGBChannel.Red)
                currentState = RGBDragState.RedFixed;

            if (channel == RGBChannel.Green)
                currentState = RGBDragState.GreenFixed;

            if (channel == RGBChannel.Blue)
                currentState = RGBDragState.BlueFixed;
            return planeNormal;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            PointerCapture.Capture(this);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            PointerCapture.Release(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("dragging sphere event");
            if (PointerCapture.Owner != this) return;
            Debug.Log("DOING: drag on sphere");
            Ray ray = eventData.pressEventCamera.ScreenPointToRay(eventData.position);

            Vector3 planeNormal = GetDragStateAndPlaneNormal();

            picker.SetDragState(currentState);

            dragPlane = new Plane(planeNormal, transform.position);

            if (!dragPlane.Raycast(ray, out float distance))
            {
                return;
            }

            Vector3 worldPosition = ray.GetPoint(distance);

            Vector3 localPosition =
                picker.VolumeTransform.InverseTransformPoint(worldPosition);

            localPosition.x = Mathf.Clamp(
                localPosition.x,
                -0.5f,
                0.5f);

            localPosition.y = Mathf.Clamp(
                localPosition.y,
                -0.5f,
                0.5f);

            localPosition.z = Mathf.Clamp(
                localPosition.z,
                -0.5f,
                0.5f);

            picker.SetColor(
                RGBVolume.LocalPositionToColor(localPosition, picker.AxisMapping));
        }

        public void OnColorChanged(RGBColor color)
        {
            transform.localPosition = RGBVolume.ColorToLocalPosition(
                color,
                picker.AxisMapping);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            currentState = RGBDragState.None;
            picker.SetDragState(RGBDragState.None);
        }
    }
}