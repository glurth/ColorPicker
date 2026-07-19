Shader "EyE/RGBPicker/GridPlane"
{
    Properties
    {
        _CurrentValue("Current Value", Float) = 1
        _FixedChannel("Fixed Channel", Float) = 0

        _NumGridLines("Grid Lines", Float) = 10
        _GridThickness("Grid Thickness", Float) = 0.01
    }

        SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _CurrentValue;
            float _FixedChannel;
            float _NumGridLines;
            float _GridThickness;
            float3 _AxisInversion;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = mul(UNITY_MATRIX_MVP, input.positionOS);
                output.uv = input.uv;
                return output;
            }

            float InvertUVdim(float value, float invertSign)
            {
                if (invertSign > 0)
                    return value;
                return 1.0 - value;
            }

            float GridLine(float value)
            {
                float scaled = value * _NumGridLines;

                float edge = min(frac(scaled), 1.0 - frac(scaled));

                return 1.0 - smoothstep(
                    0.0,
                    _GridThickness,
                    edge);
            }

            float4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                float3 rgb;

                if (_FixedChannel == 0)
                {
                    uv.x = InvertUVdim(uv.x, _AxisInversion.y);
                    uv.y = InvertUVdim(uv.y, _AxisInversion.z);
                    rgb = float3(_CurrentValue, uv.x, uv.y);
                }
                else if (_FixedChannel == 1)
                {
                    uv.x = InvertUVdim(uv.x, _AxisInversion.x);
                    uv.y = InvertUVdim(uv.y, _AxisInversion.z);
                    rgb = float3(uv.x, _CurrentValue, uv.y);
                }
                else
                {
                    uv.x = InvertUVdim(uv.x, _AxisInversion.x);
                    uv.y = InvertUVdim(uv.y, _AxisInversion.y);
                    rgb = float3(uv.x, uv.y, _CurrentValue);
                }

                float alpha = max(
                    GridLine(uv.x),
                    GridLine(uv.y));

                return float4(rgb, alpha);
            }

            ENDHLSL
        }
    }
}