using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EyE.Unity.RGBPicker
{
    public class RGBActivateOnSphereDrag : MonoBehaviour, IRGBDragStateChanged
    {
        public bool Reverse;
        public void OnDragStateChanged(RGBDragState state)
        {
            bool isDragging = state != RGBDragState.None;
            if (Reverse) isDragging = !isDragging;
            gameObject.SetActive(isDragging);
        }
    }
}