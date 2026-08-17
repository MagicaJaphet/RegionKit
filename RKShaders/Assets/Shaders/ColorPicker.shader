Shader "Futile/ColorPicker"
{
    Properties 
    {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }
    
    Category 
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off  // we can turn backface culling off because we know nothing will be facing backwards

        SubShader
        {
            Pass 
            {
                CGPROGRAM
                #pragma target 3.0
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
                #include "_ShaderFix.cginc"
                #include "_Functions.cginc"

                float4 _MainTex_ST;
                sampler2D _MainTex;
                sampler2D _LevelTex;
                sampler2D _PreLevelColorGrab;

                //sampler2D _GrabTexture;
                
                uniform float _RAIN;
                uniform float4 _spriteRect;
                uniform float2 _screenSize;

                // Taken from UnityColorPicker
                static const float recip2Pi = 0.159154943;
                static const float twoPi = 6.2831853;
                float _HueCircleInner = 0.4;
                float _SVSquareSize = 0.25;

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float2 scrPos : TEXCOORD1;
                    float2 textCoord : TEXCOORD2;
                    float4 clr : COLOR;
                };

                v2f vert (appdata_full v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.scrPos = ComputeScreenPos(o.pos);
                    o.textCoord = iLerp(_spriteRect.xy,_spriteRect.zw,o.scrPos);
                    o.clr = v.color;
                    return o;
                }

                // Taken from UnityColorPicker
                half4 hueRing(float2 uv)
                {
                    float2 coords = uv - .5;
                    float r = length(coords);
                    float fw = fwidth(r);
                    float a = smoothstep(.5, .5 - fw, r) * smoothstep(_HueCircleInner - fw, _HueCircleInner, r);
                    float angle = atan2(coords.y, coords.x) * recip2Pi;
                    return half4(hsv2rgb(float3(angle, 1, 1)), a);
                }
                
                half4 svSquare(float3 clr, float2 uv)
                {
                    float2 sv = (uv - .5) / (_SVSquareSize * 2) + .5;

                    float dx = abs(ddx(sv.x));
                    float dy = abs(ddy(sv.y));

                    float a =
                        smoothstep(0, dx, sv.x) * smoothstep(1, 1 - dx, sv.x) *
                        smoothstep(0, dy, sv.y) * smoothstep(1, 1 - dy, sv.y);

                    return float4(hsv2rgb(float3(rgb2hsv(clr).x, sv)), a);
                }

                half4 frag (v2f i) : SV_Target
                {
                    // Hue ring
                    half4 color = hueRing(i.uv);

                    // Saturation value Square
                    half4 sv = svSquare(i.clr, i.uv);

                    color = sv.a > 0 ? sv : color;

                    return color;
                }
                ENDCG
            }
        } 
    }
}
