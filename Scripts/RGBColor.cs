using UnityEngine;
using System;

namespace EyE.Unity.RGBPicker
{
    public enum RGBChannel
    {
        Red = 0,
        Green = 1,
        Blue = 2
    }

    public enum RGBAxisMapping
    {
        XYZ,    // R=X, G=Y, B=Z
        XZY,    // R=X, G=Z, B=Y
        YXZ,    // R=Y, G=X, B=Z
        YZX,    // R=Y, G=Z, B=X
        ZXY,    // R=Z, G=X, B=Y
        ZYX     // R=Z, G=Y, B=X
    }

    public enum RGBDragState
    {
        None,
        RedFixed,
        GreenFixed,
        BlueFixed
    }

    [Serializable]
    public struct RGBColor
    {
        [Range(0, 1)]
        public float Red;

        [Range(0, 1)]
        public float Green;

        [Range(0, 1)]
        public float Blue;

        public Color UnityColor => new Color(Red, Green, Blue, 1);

        public RGBColor(float red, float green, float blue)
        {
            Red = Mathf.Clamp01(red);
            Green = Mathf.Clamp01(green);
            Blue = Mathf.Clamp01(blue);
        }
        public RGBColor(Color color)
        {
            Red = color.r;
            Green = color.g;
            Blue = color.b;
        }
        public void SetChannel(RGBChannel channel, float value)
        {
            value = Mathf.Clamp01(value);

            switch (channel)
            {
                case RGBChannel.Red:
                    Red = value;
                    break;

                case RGBChannel.Green:
                    Green = value;
                    break;

                case RGBChannel.Blue:
                    Blue = value;
                    break;
            }
        }

        public float GetChannel(RGBChannel channel)
        {
            switch (channel)
            {
                case RGBChannel.Red:
                    return Red;

                case RGBChannel.Green:
                    return Green;

                case RGBChannel.Blue:
                    return Blue;

                default:
                    return 0;
            }
        }
    }

    public interface IRGBColorSource
    {
        void SetColor(RGBColor color);
    }

    public interface IRGBColorChanged
    {
        void OnColorChanged(RGBColor color);
    }

    public interface IRGBDragStateChanged
    {
        void OnDragStateChanged(RGBDragState state);
    }

    public static class RGBVolume
    {
       // public static Quaternion globalRotation = Quaternion.identity;
        public static Vector3 axisInversion = Vector3.one;
        static RGBVolume()
        {
            // globalRotation = Quaternion.AngleAxis(180,Vector3.up);
            axisInversion = new Vector3(-1,1,-1);
        }
        public static Vector3 ColorToLocalPosition(RGBColor color, RGBAxisMapping axisMapping)
        {
            Vector3 position = Vector3.zero;
            SetAxis(ref position, axisMapping, RGBChannel.Red, ColorChannelValueToAxisValue(color.Red));
            SetAxis(ref position, axisMapping, RGBChannel.Green, ColorChannelValueToAxisValue(color.Green));
            SetAxis(ref position, axisMapping, RGBChannel.Blue, ColorChannelValueToAxisValue(color.Blue));

            return position;
        }

        public static RGBColor LocalPositionToColor(Vector3 position, RGBAxisMapping axisMapping)
        {
            return new RGBColor(
                AxisValueToColorChannelValue(GetAxis(position, axisMapping, RGBChannel.Red)),
                AxisValueToColorChannelValue(GetAxis(position, axisMapping, RGBChannel.Green)),
                AxisValueToColorChannelValue(GetAxis(position, axisMapping, RGBChannel.Blue)));
        }

        private static void SetAxis(ref Vector3 position, RGBAxisMapping mapping, RGBChannel channel, float value)
        {
            switch (GetAxisIndex(mapping, channel))
            {
                case 0:
                    position.x = value;
                    break;

                case 1:
                    position.y = value;
                    break;

                case 2:
                    position.z = value;
                    break;
            }
            position.Scale(axisInversion);
        }

        private static float GetAxis(Vector3 position, RGBAxisMapping mapping, RGBChannel channel)
        {
            position.Scale(axisInversion);
            switch (GetAxisIndex(mapping, channel))
            {
                case 0:
                    return position.x;

                case 1:
                    return position.y;

                default:
                    return position.z;
            }
        }

        private static int GetAxisIndex(RGBAxisMapping mapping, RGBChannel channel)
        {
            switch (mapping)
            {
                case RGBAxisMapping.XYZ:
                    return (int)channel;

                case RGBAxisMapping.XZY:
                    return channel == RGBChannel.Green ? 2 :
                           channel == RGBChannel.Blue ? 1 : 0;

                case RGBAxisMapping.YXZ:
                    return channel == RGBChannel.Red ? 1 :
                           channel == RGBChannel.Green ? 0 : 2;

                case RGBAxisMapping.YZX:
                    return channel == RGBChannel.Red ? 2 :
                           channel == RGBChannel.Green ? 0 : 1;

                case RGBAxisMapping.ZXY:
                    return channel == RGBChannel.Red ? 1 :
                           channel == RGBChannel.Green ? 2 : 0;

                case RGBAxisMapping.ZYX:
                    return channel == RGBChannel.Red ? 2 :
                           channel == RGBChannel.Green ? 1 : 0;

                default:
                    return (int)channel;
            }
        }

        public static RGBChannel GetChannelForAxis(RGBAxisMapping mapping, int axisIndex)
        {
            switch (mapping)
            {
                case RGBAxisMapping.XYZ:
                    return (RGBChannel)axisIndex;

                case RGBAxisMapping.XZY:
                    switch (axisIndex)
                    {
                        case 0: return RGBChannel.Red;
                        case 1: return RGBChannel.Blue;
                        default: return RGBChannel.Green;
                    }

                case RGBAxisMapping.YXZ:
                    switch (axisIndex)
                    {
                        case 0: return RGBChannel.Green;
                        case 1: return RGBChannel.Red;
                        default: return RGBChannel.Blue;
                    }

                case RGBAxisMapping.YZX:
                    switch (axisIndex)
                    {
                        case 0: return RGBChannel.Green;
                        case 1: return RGBChannel.Blue;
                        default: return RGBChannel.Red;
                    }

                case RGBAxisMapping.ZXY:
                    switch (axisIndex)
                    {
                        case 0: return RGBChannel.Blue;
                        case 1: return RGBChannel.Red;
                        default: return RGBChannel.Green;
                    }

                case RGBAxisMapping.ZYX:
                    switch (axisIndex)
                    {
                        case 0: return RGBChannel.Blue;
                        case 1: return RGBChannel.Green;
                        default: return RGBChannel.Red;
                    }

                default:
                    return RGBChannel.Red;
            }
        }
        public static RGBChannel GetChannelForDirection(RGBAxisMapping mapping, Vector3 localDirection)
        {
            float absX = Mathf.Abs(localDirection.x);
            float absY = Mathf.Abs(localDirection.y);
            float absZ = Mathf.Abs(localDirection.z);

            if (absX > absY && absX > absZ)
                return RGBVolume.GetChannelForAxis(mapping, 0);

            else if (absY > absZ)
                return RGBVolume.GetChannelForAxis(mapping, 1);
            else
                return RGBVolume.GetChannelForAxis(mapping, 2);
        }
        public static int GetAxisForChannel(RGBAxisMapping mapping, RGBChannel channel)
        {
            switch (mapping)
            {
                case RGBAxisMapping.XYZ:
                    return (int)channel;

                case RGBAxisMapping.XZY:
                    switch (channel)
                    {
                        case RGBChannel.Red:
                            return 0;
                        case RGBChannel.Green:
                            return 2;
                        default:
                            return 1;
                    }

                case RGBAxisMapping.YXZ:
                    switch (channel)
                    {
                        case RGBChannel.Red:
                            return 1;
                        case RGBChannel.Green:
                            return 0;
                        default:
                            return 2;
                    }

                case RGBAxisMapping.YZX:
                    switch (channel)
                    {
                        case RGBChannel.Red:
                            return 2;
                        case RGBChannel.Green:
                            return 0;
                        default:
                            return 1;
                    }

                case RGBAxisMapping.ZXY:
                    switch (channel)
                    {
                        case RGBChannel.Red:
                            return 1;
                        case RGBChannel.Green:
                            return 2;
                        default:
                            return 0;
                    }

                case RGBAxisMapping.ZYX:
                    switch (channel)
                    {
                        case RGBChannel.Red:
                            return 2;
                        case RGBChannel.Green:
                            return 1;
                        default:
                            return 0;
                    }

                default:
                    return 0;
            }
        }
        public static int GetAxisForDirection(Vector3 localDirection)
        {
            float absX = Mathf.Abs(localDirection.x);
            float absY = Mathf.Abs(localDirection.y);
            float absZ = Mathf.Abs(localDirection.z);

            if (absX > absY && absX > absZ)
                return 0;

            if (absY > absZ)
                return 1;

            return 2;
        }

        private static float ColorChannelValueToAxisValue(float value)
        {
            float result = Mathf.Lerp(-0.5f, 0.5f, value);
            return result;
        }

        private static float AxisValueToColorChannelValue(float value)
        {
            return Mathf.InverseLerp(-0.5f, 0.5f, value);
        }
    }

    public static class PointerCapture
    {
        public static MonoBehaviour Owner { get; private set; }

        public static void Capture(MonoBehaviour handler)
        {
            Owner = handler;
        }

        public static void Release(MonoBehaviour handler)
        {
            if (Owner == handler)
                Owner = null;
        }
    }

}