Shader "Custom/HeatMapVisualizer"
{
    Properties
    {
        _HeatMapTex ("Heat Map Data", 2D) = "white" {}
        _GridCount ("Grid Count (X,Y)", Vector) = (10,10,0,0)
        _ZeroHeatColor ("Zero Heat Color", Color) = (0,0,1,0.5)
        _MaxHeatColor ("Max Heat Color", Color) = (1,0,0,0.5)
        _LineWidth ("Line Width", Range(0, 0.1)) = 0.02
        _ArrowSize ("Arrow Size", Range(0, 1)) = 0.6
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _HeatMapTex;
            float4 _HeatMapTex_ST;
            float4 _GridCount;
            fixed4 _ZeroHeatColor;
            fixed4 _MaxHeatColor;
            float _LineWidth;
            float _ArrowSize;

            float sdfSegment(float2 p, float2 a, float2 b)
            {
                float2 pa = p - a;
                float2 ba = b - a;
                float h = saturate(dot(pa, ba) / dot(ba, ba));
                return length(pa - ba * h);
            }

            float sdfArrow(float2 p, float2 dir, float size)
            {
                p = p - float2(0.5, 0.5);

                float2 perp = float2(-dir.y, dir.x);

                float halfLen = size * 0.5;
                float headLen = halfLen * 0.5;
                float headWidth = halfLen * 0.4;
                float stemWidth = halfLen * 0.15;

                float2 tip = dir * halfLen;
                float2 stemEnd = dir * (halfLen - headLen);
                float2 stemStart = -dir * halfLen;

                float d = 1000;

                d = min(d, sdfSegment(p, stemStart, stemEnd) - stemWidth);

                float2 headCorner1 = tip - dir * headLen + perp * headWidth;
                float2 headCorner2 = tip - dir * headLen - perp * headWidth;

                d = min(d, sdfSegment(p, stemEnd, headCorner1) - stemWidth * 0.5);
                d = min(d, sdfSegment(p, stemEnd, headCorner2) - stemWidth * 0.5);
                d = min(d, sdfSegment(p, headCorner1, tip) - stemWidth * 0.5);
                d = min(d, sdfSegment(p, headCorner2, tip) - stemWidth * 0.5);

                return d;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _HeatMapTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 gridUV = i.uv * _GridCount.xy;
                float2 cellUV = frac(gridUV);
                float2 cellIndex = floor(gridUV);

                float2 texUV = float2(
                    (cellIndex.x + 0.5) / _GridCount.x,
                    (cellIndex.y + 0.5) / _GridCount.y
                );

                half4 heatData = tex2D(_HeatMapTex, texUV);
                float heatRatio = heatData.r;
                float2 gradient = heatData.gb;
                float gradientMag = length(gradient);

                fixed4 cellColor = lerp(_ZeroHeatColor, _MaxHeatColor, heatRatio);

                float lineWidth = _LineWidth;
                float2 lineDist = min(cellUV, 1.0 - cellUV);
                float minLineDist = min(lineDist.x, lineDist.y);

                float lineMask = smoothstep(lineWidth, lineWidth + 0.005, minLineDist);

                float arrowMask = 0;
                if (gradientMag > 0.001)
                {
                    float2 dir = normalize(gradient);
                    float arrowDist = sdfArrow(cellUV, dir, _ArrowSize);
                    arrowMask = 1.0 - smoothstep(-0.01, 0.01, arrowDist);
                }

                fixed4 arrowColor = fixed4(0, 0, 0, 1);

                fixed4 finalColor = lerp(cellColor, arrowColor, arrowMask);
                finalColor.a *= lineMask;

                UNITY_APPLY_FOG(i.fogCoord, finalColor);

                return finalColor;
            }
            ENDCG
        }
    }
}
