using UnityEngine;

namespace EyE.Unity.RGBPicker
{
    [RequireComponent(typeof(Renderer))]
    public class RGBPlaneMaterialUpdater : MonoBehaviour, IRGBColorChanged, IRGBDragStateChanged
    {
        [TextArea(4, 20)]
        [SerializeField]
        private string hierarchyNotes =
@"Hierarchy:

RGBPlane
 O-- Renderer
 O-- RGBPlaneMaterialUpdater

Updates the plane shader with the current fixed channel value
and displays drag guides while the sphere is being dragged.";

        [SerializeField]
        private RGBColorPicker picker;

        [SerializeField]
        private Renderer planeRenderer;

        private RGBChannel fixedChannel;
        private MaterialPropertyBlock propertyBlock;

        
        static readonly int CurrentValueID = Shader.PropertyToID("_CurrentValue");
        static readonly int FixedChannelID = Shader.PropertyToID("_FixedChannel");
        static readonly int AxisMappingID = Shader.PropertyToID("_AxisMapping");
        static readonly int GuideActiveID = Shader.PropertyToID("_GuideActive");
        static readonly int LineXID = Shader.PropertyToID("_LineX");
        static readonly int LineYID = Shader.PropertyToID("_LineY");
        static readonly int LineModeID = Shader.PropertyToID("_LineMode");
        static readonly int AxisInversionID = Shader.PropertyToID("_AxisInversion");


        private void Awake()
        {
            Vector3 localForward =
                picker.VolumeTransform.InverseTransformDirection(transform.forward);

            int planeAxis = RGBVolume.GetAxisForDirection(localForward);
            fixedChannel =
                RGBVolume.GetChannelForDirection(
                    picker.AxisMapping,
                    localForward);

           // Debug.Log("plane setup.   local forward: " + localForward + "  planeAxisID: " + planeAxis + " fixed channel:" + fixedChannel);

            propertyBlock = new MaterialPropertyBlock();

            propertyBlock.SetFloat(FixedChannelID, (int)fixedChannel);
            propertyBlock.SetFloat(AxisMappingID, (int)picker.AxisMapping);
            propertyBlock.SetFloat(GuideActiveID, 0);
            propertyBlock.SetFloat(LineXID, 0.5f);
            propertyBlock.SetFloat(LineYID, 0.5f);
            propertyBlock.SetFloat(LineModeID, 0);
            propertyBlock.SetVector(AxisInversionID, RGBVolume.axisInversion);

            planeRenderer.SetPropertyBlock(propertyBlock);
        }

        public void OnColorChanged(RGBColor color)
        {
            if (propertyBlock == null)
                Awake();

            propertyBlock.SetFloat(CurrentValueID,color.GetChannel(fixedChannel));

            planeRenderer.SetPropertyBlock(propertyBlock);
        }

        public void OnDragStateChanged(RGBDragState state)
        {
            if (propertyBlock == null)
                Awake();

            bool guideActive = state != RGBDragState.None;
            bool verticalGuide = false;

            if (guideActive)
            {
                if (fixedChannel == RGBChannel.Red)
                {
                    guideActive = state != RGBDragState.RedFixed;
                    verticalGuide = state == RGBDragState.GreenFixed;
                }
                else if (fixedChannel == RGBChannel.Green)
                {
                    guideActive = state != RGBDragState.GreenFixed;
                    verticalGuide = state == RGBDragState.RedFixed;
                }
                else
                {
                    guideActive = state != RGBDragState.BlueFixed;
                    verticalGuide = state == RGBDragState.RedFixed;
                }
            }

            propertyBlock.SetFloat(GuideActiveID, guideActive ? 1f : 0f);

            propertyBlock.SetFloat(LineModeID, verticalGuide ? 1f : 2f);

            switch (fixedChannel)
            {
                case RGBChannel.Red:
                    // GB plane
                    propertyBlock.SetFloat(LineXID, picker.CurrentColor.Green);
                    propertyBlock.SetFloat(LineYID, picker.CurrentColor.Blue);
                    break;

                case RGBChannel.Green:
                    // RB plane
                    propertyBlock.SetFloat(LineXID, picker.CurrentColor.Red);
                    propertyBlock.SetFloat(LineYID, picker.CurrentColor.Blue);
                    break;

                case RGBChannel.Blue:
                    // RG plane
                    propertyBlock.SetFloat(LineXID, picker.CurrentColor.Red);
                    propertyBlock.SetFloat(LineYID, picker.CurrentColor.Green);
                    break;
            }

            planeRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}