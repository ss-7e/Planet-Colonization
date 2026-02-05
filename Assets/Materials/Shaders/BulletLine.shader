Shader "Custom/AdvancedTwoColorTrail_Simple"
{
    Properties
    {
        _ColorA ("Color A", Color) = (1, 0.5, 0, 1)
        _ColorB ("Color B", Color) = (1, 1, 0, 1)
        [HDR] _EmissionColor ("Emission Color", Color) = (1, 0.5, 0, 1)
        _EmissionIntensity ("Emission Intensity", Float) = 3
        _GradientOffset ("Gradient Offset", Range(-1, 1)) = 0
        _GradientScale ("Gradient Scale", Range(0.1, 2)) = 1
        _AlphaFalloff ("Alpha Falloff", Range(0.1, 5)) = 1
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "ShaderModel" = "4.5"
        }
        
        Blend SrcAlpha One
        ZWrite Off
        Cull Off
        
        // 启用GPU Instancing
        HLSLINCLUDE
        #pragma multi_compile_instancing
        #pragma instancing_options renderinglayer
        #pragma multi_compile _ DOTS_INSTANCING_ON
        ENDHLSL
        
        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            // GPU Instancing宏
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            // 使用CBUFFER但确保支持instancing
            CBUFFER_START(UnityPerMaterial)
                float4 _ColorA;
                float4 _ColorB;
                float4 _EmissionColor;
                float _EmissionIntensity;
                float _GradientOffset;
                float _GradientScale;
                float _AlphaFalloff;
            CBUFFER_END
            
            Varyings vert (Attributes v)
            {
                Varyings o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.color = v.color;
                
                return o;
            }
            
            half4 frag (Varyings i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                
                float gradient = saturate(i.uv.x * _GradientScale + _GradientOffset);
                
                half4 color = lerp(_ColorA, _ColorB, gradient);
                color.rgb += _EmissionColor.rgb * _EmissionIntensity;
                color *= i.color;
                
                color.a *= pow(saturate(1.0 - i.uv.y), _AlphaFalloff);
                
                return color;
            }
            ENDHLSL
        }
    }
}