using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EyE.Unity.RGBPicker;
public class TesterMono : MonoBehaviour
{
    public RGBColorPicker pickerPreFab;
    public RGBColorPicker pickerInstance;
    public Color color;

    public bool openPicker;

    void callback(Color c)
    {
        color = c;
        Destroy(pickerInstance.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (openPicker)
        {

            openPicker = false;
            pickerInstance = Instantiate<RGBColorPicker>(pickerPreFab, transform);
            pickerInstance.Open(color,callback);
        }
    }
}
