Shader "EyE/RGBPicker/SliderWithOutline"
{
    Properties
    {
        _MainTex("Image Texture", 2D) = "white" {}

        _FixedValue0("Fixed Value 0", Float) = 0
        _FixedValue1("Fixed Value 1", Float) = 0
        _UnfixedChannel("Unfixed Channel", Float) = 0
        _OutlineColor("Outline Color", Color) = (0,0,1,1)
        _OutlineWidth("Outline Width", Float) = 2
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "RenderType" = "Transparent"
                "IgnoreProjector" = "True"
                "CanUseSpriteAtlas" = "True"
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

                sampler2D _MainTex;

                float _FixedValue0;
                float _FixedValue1;
                float _UnfixedChannel;

                float4 _OutlineColor;
                float _OutlineWidth;

                struct Attributes
                {
                    float4 positionOS : POSITION;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
                };

                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
                };

                Varyings vert(Attributes input)
                {
                    Varyings output;

                    output.positionCS = mul(UNITY_MATRIX_MVP, input.positionOS);
                    output.uv = input.uv;
                    output.color = input.color;

                    return output;
                }

                float4 frag(Varyings input) : SV_Target
                {
                    float t = input.uv.x;
                    float3 rgb;

                    if (_UnfixedChannel == 0)
                    {
                        rgb = float3(t, _FixedValue0, _FixedValue1);
                    }
                    else if (_UnfixedChannel == 1)
                    {
                        rgb = float3(_FixedValue0, t, _FixedValue1);
                    }
                    else
                    {
                        rgb = float3(_FixedValue0, _FixedValue1, t);
                    }

                    float alpha = tex2D(_MainTex, input.uv).a * input.color.a;

                    if (_OutlineWidth > 0)
                    {
                        if (input.uv.y < _OutlineWidth)
                        {
                            float frac = input.uv.y / _OutlineWidth;
                            frac = 1 - frac;
                            rgb = lerp(rgb, _OutlineColor.rgb, frac);

                        }
                        if (input.uv.y > 1 - _OutlineWidth)
                        {
                            float frac = (1 - input.uv.y) / _OutlineWidth;
                            frac = 1 - frac;
                            rgb = lerp(rgb, _OutlineColor.rgb, frac);
                        }
                    }

                    return float4(rgb, alpha);
                }

                ENDHLSL
            }
        }
}