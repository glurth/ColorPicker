using UnityEngine;

namespace EyE.Unity.RGBPicker
{
    [RequireComponent(typeof(Renderer))]
    public class RGBConnectorLine : MonoBehaviour, IRGBColorChanged
    {
        [TextArea(4, 20)]
        [SerializeField]
        private string hierarchyNotes =
@"Hierarchy:

RGBColorPicker
 |-- Cube
      |-- Sphere
      |-- ConnectorLines
           |-- RedLine
           |-- GreenLine
           |-- BlueLine

Quad extends from the sphere to the corresponding RGB volume face.";

        private RGBChannel channel;

        [SerializeField]
        private RGBColorPicker picker;

        private Renderer meshRenderer;
        private MaterialPropertyBlock propertyBlock;

        static readonly int FixedValue0ID = Shader.PropertyToID("_FixedValue0");
        static readonly int FixedValue1ID = Shader.PropertyToID("_FixedValue1");
        static readonly int UnfixedChannelID = Shader.PropertyToID("_UnfixedChannel");
        private void Reset()
        {
            picker = GetComponentInParent<RGBColorPicker>();
        }

        private void Awake()
        {
            meshRenderer = GetComponent<Renderer>();
            propertyBlock = new MaterialPropertyBlock();
            channel = RGBVolume.GetChannelForDirection(picker.AxisMapping, transform.right);
        }

        public void OnColorChanged(RGBColor color)
        {
          //  if (propertyBlock == null) Awake();

            Vector3 localPosition =
                RGBVolume.ColorToLocalPosition(
                    color,
                    picker.AxisMapping);

            int axis = RGBVolume.GetAxisForChannel(
                picker.AxisMapping,
                channel);

            localPosition[axis] = 0;

            transform.localPosition = localPosition;
            transform.localPosition = localPosition;

            propertyBlock.SetFloat(
                UnfixedChannelID,
                (int)channel);

            switch (channel)
            {
                case RGBChannel.Red:
                    propertyBlock.SetFloat(FixedValue0ID, color.Green);
                    propertyBlock.SetFloat(FixedValue1ID, color.Blue);
                    break;

                case RGBChannel.Green:
                    propertyBlock.SetFloat(FixedValue0ID, color.Red);
                    propertyBlock.SetFloat(FixedValue1ID, color.Blue);
                    break;

                case RGBChannel.Blue:
                    propertyBlock.SetFloat(FixedValue0ID, color.Red);
                    propertyBlock.SetFloat(FixedValue1ID, color.Green);
                    break;
            }

            meshRenderer.SetPropertyBlock(propertyBlock);
        }


    }
}