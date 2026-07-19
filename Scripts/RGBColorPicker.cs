using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;

namespace EyE.Unity.RGBPicker
{
    public class RGBColorPicker : MonoBehaviour, IRGBColorSource
    {
        [TextArea(4, 20)]
        [SerializeField]
        private string hierarchyNotes =
@"Hierarchy:

RGBColorPicker
 |-- Cube
 |    |-- RedPlane
 |    |-- GreenPlane
 |    |-- BluePlane
 |    |-- Sphere
 |    |-- ConnectorLines
 |    |-- Lattice
 |
 |-- Sliders
      |-- RedSlider
      |-- GreenSlider
      |-- BlueSlider

This object owns the RGB color state.
All visual components receive updates through OnColorChanged.";

        [SerializeField]
        private RGBColor currentColor = new RGBColor(1, 0, 0);

        public RGBColor CurrentColor => currentColor;

        [SerializeField]
        private RGBAxisMapping axisMapping = RGBAxisMapping.XYZ;
        public RGBAxisMapping AxisMapping => axisMapping;

        [SerializeField]
        private Transform volumeTransform;
        public Transform VolumeTransform => volumeTransform;

        [SerializeField]
        private Button acceptButton;
        [SerializeField]
        private Button cancelButton;
        public Color startingColor;
        Action<Color> callback=null;

        IRGBColorChanged[] notifyOnColorChangedCache;
        IRGBDragStateChanged[] dragStateChangedCache;
        RGBDragState dragState= RGBDragState.None;

        public void Open(Color startingColor, Action<Color> callback)
        {
            this.startingColor = startingColor;
            this.currentColor = new RGBColor(startingColor);
            this.callback = callback;
            Image cancelImage = cancelButton.GetComponent<Image>();
            cancelImage.color = startingColor;

            cancelButton.onClick.AddListener(HandleCancelClick);
            acceptButton.onClick.AddListener(HandleAcceptClick);
            gameObject.SetActive(true);
            
            NotifyOfColorChange();
        }

        private void HandleAcceptClick()
        {
            gameObject.SetActive(false);
            callback.Invoke(currentColor.UnityColor);
        }
        private void HandleCancelClick()
        {
            gameObject.SetActive(false);
            callback.Invoke(startingColor);
        }

        private void Awake()
        {
            notifyOnColorChangedCache = GetComponentsInChildren<IRGBColorChanged>(true);
            dragStateChangedCache = GetComponentsInChildren<IRGBDragStateChanged>(true);
            //NotifyOfColorChange();
        }
        private void OnEnable()
        {
            StartCoroutine(NotifyInOneCycle());
            //NotifyOfColorChange();
        }

        IEnumerator NotifyInOneCycle()
        {
            yield return new WaitForEndOfFrame();
            NotifyOfColorChange();

        }
        public void SetColor(RGBColor color)
        {
            if (currentColor.Red == color.Red &&
                currentColor.Green == color.Green &&
                currentColor.Blue == color.Blue)
            {
                return;
            }

            currentColor = color;
            NotifyOfColorChange();
        }


        void NotifyOfColorChange()
        {
            foreach (IRGBColorChanged toNotify in notifyOnColorChangedCache)
                toNotify.OnColorChanged(currentColor);
        }

        public void SetChannel(RGBChannel channel, float value)
        {
            RGBColor color = currentColor;
            color.SetChannel(channel, value);
            SetColor(color);
        }
        public void SetDragState(RGBDragState state)
        {
            if (dragState == state)
                return;

            dragState = state;

            for (int i = 0; i < dragStateChangedCache.Length; i++)
                dragStateChangedCache[i].OnDragStateChanged(state);
        }
    }
}