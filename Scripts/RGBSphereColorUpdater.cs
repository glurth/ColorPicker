using UnityEngine;

namespace EyE.Unity.RGBPicker
{
    [RequireComponent(typeof(Renderer))]
    public class RGBSphereColorUpdater : MonoBehaviour, IRGBColorChanged
    {
        [TextArea(4, 20)]
        [SerializeField]
        private string hierarchyNotes =
@"Hierarchy:

RGBColorPicker
 |-- Cube
      |-- Sphere
           O-- RGBSphereColorUpdater

Updates the sphere material to match the selected color.";
        
        [SerializeField]
        private Material sphereMaterial;

        public void OnColorChanged(RGBColor color)
        {
            sphereMaterial.color = color.UnityColor;
        }
    }
}