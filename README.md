# Color Picker UI control

This repository provides an implementation of a color selection control for unity.  It uses the world-canvas for placement and sizing.

## Features

Displays the 3d color space as a cube, where the user may select a point in the volume.
User may click or drag on any of the 3 display planes, the selected color-sphere, or using traditional RGB sliders.

## Installation

### Using Git in Unity Package Manager

To install this package through Unity's Package Manager with Git, follow these steps:

1. Open your Unity project.
2. Navigate to `Window` -> `Package Manager`.
3. In the Package Manager window, click the `+` (plus) button at the top left.
4. Select `Add package from git URL...`.
5. Enter the following URL and press 'Add':
   ```
   https://github.com/glurth/ColorPicker.git
   ```

Unity will clone the repository and the package will appear in your list of packages. Unity might take a few moments to download and import the package.

## Components

###  RGBColorPicker

This component performs initialization and then coordinates all the other components on the various elements of the color picker.

#### How to Use

Use the included ColorPicker prefab by dragging into the scene or instantiating it via code.  The passed in "callback" function will be invoked when the user completes or cancels color selection.

  ```csharp
            pickerInstance = Instantiate<RGBColorPicker>(pickerPreFab, transform);
            pickerInstance.Open(color,callback);
  ```
###  Shaders

Multiple, use-specific shaders are included, along with materials that use them.  The shader allow the system to display the properly colored slider, guidelines, and planes efficiently, without the need to generate custom textures.

## Usage Example

One way to use this control is to create a scene instance based on the provided prefab, at editor-time, and reference that for usage.  However, Open() will still need to be called to properly configure the initial color and callback function.

Another option is to instantiate a scene object at runtime.  Below is an example showing how the object and it's callback could be implemented using this method.

```csharp
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
```



## Contributions

Contributions, issues, and feature requests are welcome! Please submit them via the GitHub repository. Note: Due to licensing, contributions can only be included with explicit written permission from the copyright holder.

## License

This package is licensed under the EyE Dual-Licensing Agreement.

It provides free, perpetual use for indie developers and non-commercial projects whose teams had Total Gross Receipts under $100,000 USD in the previous fiscal year.

Organizations exceeding this threshold must obtain a Perpetual Commercial License (PCL) for each named commercial project.

Please review the full terms in [LICENSE.md](LICENSE.md) before commercial use.