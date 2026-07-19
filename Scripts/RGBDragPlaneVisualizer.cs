using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EyE.Unity.RGBPicker
{
    public class RGBDragPlaneVisualizer : MonoBehaviour, IRGBDragStateChanged
    {
        public RGBDragState displayOnDrageState;
        public void OnDragStateChanged(RGBDragState state)
        {
            gameObject.SetActive(state == displayOnDrageState);

        }
    }
}