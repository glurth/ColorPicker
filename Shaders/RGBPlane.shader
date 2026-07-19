Shader "EyE/RGBPicker/Plane"
{
    Properties
    {
        _CurrentValue("Current Value", Float) = 1
        _FixedChannel("Fixed Channel", Float) = 0

        _GuideActive("Guide Active", Float) = 0
        _LineX("Guide Line U", Float) = 0.5
        _LineY("Guide Line V", Float) = 0.5
        _LineMode("Fixed Channel", Float) = 0
        _NotGuideDarkness("Not-Guide darkness", Float) = 0.5
    }

    SubShader
    {
       
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

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
            float _GuideActive;
            float _LineX;
            float _LineY;
            float _LineMode;
            float _NotGuideDarkness;
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

            float4 frag(Varyings input) : SV_Target
            {
                float3 rgb;
                float2 uv = input.uv;
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

                

                if (_GuideActive >= 0.5)
                {
                    float3 dimColor = rgb * _NotGuideDarkness;
                    float lineMask = 0;
                    if (_LineMode == 1) // vertical
                    {
                        lineMask = 1 - smoothstep(0.0, 0.01, abs(uv.x - _LineX));
                    }
                    else if (_LineMode == 2) // horizontal
                    {
                        lineMask = 1 - smoothstep(0.0, 0.01, abs(uv.y - _LineY));
                    }

                    rgb = lerp(dimColor, rgb, lineMask);
                }

                return float4(rgb, 1);

            }

            ENDHLSL
        }
    }
}