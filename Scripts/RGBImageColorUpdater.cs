using UnityEngine;
using UnityEngine.UI;

namespace EyE.Unity.RGBPicker
{
    [RequireComponent(typeof(Image))]
    public class RGBImageColorUpdater : MonoBehaviour,IRGBColorChanged
    {
        private Image image;

        public void OnColorChanged(RGBColor color)
        {
            if(enabled && gameObject.activeInHierarchy)
                image.color = color.UnityColor;
        }

        private void Awake()
        {
            image = GetComponent<Image>();
        }
    }
}