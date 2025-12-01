Shader "Custom/AdvancedTwoColorTrail"
{
    Properties
    {
        _ColorA ("Color A", Color) = (1, 0.5, 0, 1)           // 橙色
        _ColorB ("Color B", Color) = (1, 1, 0, 1)             // 黄色
        _EmissionA ("Emission A", Float) = 3
        _EmissionB ("Emission B", Float) = 5
        _GradientOffset ("Gradient Offset", Range(-1, 1)) = 0
        _GradientScale ("Gradient Scale", Range(0.1, 2)) = 1
        _AlphaFalloff ("Alpha Falloff", Range(0.1, 5)) = 1
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha One
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };
            
            float4 _ColorA;
            float4 _ColorB;
            float _EmissionA;
            float _EmissionB;
            float _GradientOffset;
            float _GradientScale;
            float _AlphaFalloff;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 计算渐变因子（0到1）
                float gradient = saturate(i.uv.x * _GradientScale + _GradientOffset);
                
                // 颜色插值
                fixed4 color = lerp(_ColorA, _ColorB, gradient);
                
                // 发射强度插值
                float emission = lerp(_EmissionA, _EmissionB, gradient);
                
                // 应用发射强度
                color.rgb *= emission;
                
                // 应用顶点颜色
                color *= i.color;
                
                // Alpha衰减（基于UV的Y坐标或时间）
                color.a *= pow(1.0 - i.uv.y, _AlphaFalloff);
                
                return color;
            }
            ENDCG
        }
    }
}