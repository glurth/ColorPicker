using UnityEngine;
using UnityEngine.UI;

namespace EyE.Unity.RGBPicker
{
    [RequireComponent(typeof(Slider))]
    public class RGBSliderView : MonoBehaviour, IRGBColorChanged
    {
        [TextArea(4, 20)]
        [SerializeField]
        private string hierarchyNotes =
@"Hierarchy:

RGBColorPicker
 |-- Sliders
      |-- RedSlider
      |-- GreenSlider
      |-- BlueSlider

Updates the slider background shader and
sends user edits back to the RGBColorPicker.";

        [SerializeField]
        private RGBChannel channel;

        [SerializeField]
        private RGBColorPicker picker;

        [SerializeField]
        private Material backgroundMaterial;

        Slider slider;

        static readonly int FixedValue0ID = Shader.PropertyToID("_FixedValue0");
        static readonly int FixedValue1ID = Shader.PropertyToID("_FixedValue1");
        static readonly int UnfixedChannelID = Shader.PropertyToID("_UnfixedChannel");

        bool suppressEvents;

        void Awake()
        {
            slider = GetComponent<Slider>();
            backgroundMaterial.SetFloat(UnfixedChannelID, (int)channel);
            slider.onValueChanged.AddListener(OnSliderChanged);
        }

        void OnDestroy()
        {
            slider.onValueChanged.RemoveListener(OnSliderChanged);
        }

        void OnSliderChanged(float value)
        {
            if (suppressEvents)
                return;

            picker.SetChannel(channel, value);
        }

        public void OnColorChanged(RGBColor color)
        {
            suppressEvents = true;

            slider.SetValueWithoutNotify(color.GetChannel(channel));

            switch (channel)
            {
                case RGBChannel.Red:

                    backgroundMaterial.SetFloat(FixedValue0ID, color.Green);
                    backgroundMaterial.SetFloat(FixedValue1ID, color.Blue);

                    break;

                case RGBChannel.Green:

                    backgroundMaterial.SetFloat(FixedValue0ID, color.Red);
                    backgroundMaterial.SetFloat(FixedValue1ID, color.Blue);

                    break;

                case RGBChannel.Blue:

                    backgroundMaterial.SetFloat(FixedValue0ID, color.Red);
                    backgroundMaterial.SetFloat(FixedValue1ID, color.Green);

                    break;
            }

            suppressEvents = false;
        }
    }
}