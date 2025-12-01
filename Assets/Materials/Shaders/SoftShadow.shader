Shader "Custom/HexSoftShadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ShadowColor ("Shadow Color", Color) = (0,0,0,0.5)
        _ShadowIntensity ("Shadow Intensity", Range(0, 1)) = 0.5
        _ShadowSoftness ("Shadow Softness", Range(0, 1)) = 0.5
        _ShadowDistance ("Shadow Distance", Float) = 10
        _HexRadius ("Hex Radius", Float) = 0.5
        _SampleSpread ("Sample Spread", Float) = 0.001
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        
        struct attribute
        {
            float4 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float2 uv : TEXCOORD0;
        };

        struct varying
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 positionWS : TEXCOORD1;
            float3 normalWS : TEXCOORD2;
            float3 viewDirWS : TEXCOORD3;
        };
        
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _ShadowColor;
            float _ShadowIntensity;
            float _ShadowSoftness;
            float _ShadowDistance;
            float _HexRadius;
            float _SampleSpread;
        CBUFFER_END
        
        // 六边形采样点集
        static const float2 HexKernel[7] = 
        {
            float2(0, 0),
            float2(1, 0),
            float2(-1, 0),
            float2(0.5, 0.866),
            float2(-0.5, 0.866),
            float2(0.5, -0.866),
            float2(-0.5, -0.866)
        };
        
        // 计算采样权重
        float GetSampleWeight(int index)
        {
            float baseWeight = 1.0;
            if (index == 0) 
                return baseWeight; // 中心点权重最高
            
            float distance = length(HexKernel[index]);
            return baseWeight * (1.0 - distance * 0.3);
        }
        
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fog
            
            varying vert(attribute input)
            {
                varying output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                
                return output;
            }
            
            // 简化的六边形软阴影计算
            float CalculateHexSoftShadow(float3 positionWS, float3 normalWS)
            {
                float shadow = 1.0;
                
                #if _MAIN_LIGHT_SHADOWS
                    // 获取主光源阴影数据
                    float4 shadowCoord = TransformWorldToShadowCoord(positionWS);
                    float mainShadow = MainLightRealtimeShadow(shadowCoord);
                    
                    // 如果是软阴影，进行六边形采样
                    #if _SHADOWS_SOFT
                        float totalShadow = 0.0;
                        float totalWeight = 0.0;
                        
                        for (int i = 0; i < 7; i++)
                        {
                            // 计算采样偏移（在世界空间中）
                            float3 offset = float3(HexKernel[i].x, 0.0, HexKernel[i].y) * _HexRadius * _SampleSpread;
                            float3 samplePos = positionWS + offset;
                            
                            // 转换到阴影空间
                            float4 sampleShadowCoord = TransformWorldToShadowCoord(samplePos);
                            
                            // 采样阴影
                            float sampleShadow = MainLightRealtimeShadow(sampleShadowCoord);
                            
                            // 应用权重
                            float weight = GetSampleWeight(i);
                            totalShadow += sampleShadow * weight;
                            totalWeight += weight;
                        }
                        
                        shadow = totalShadow / totalWeight;
                    #else
                        // 硬阴影直接使用主阴影
                        shadow = mainShadow;
                    #endif
                #endif
                
                return shadow;
            }
            
            // 计算自阴影效果
            float CalculateSelfShadow(float3 normalWS, float3 lightDir)
            {
                float NdotL = dot(normalize(normalWS), normalize(lightDir));
                
                // 自阴影计算 - 模拟环境光遮蔽和几何衰减
                float selfShadow = saturate(NdotL * 0.5 + 0.5); // 基本兰伯特
                
                // 添加一些微妙的阴影增强
                selfShadow = pow(selfShadow, 1.5);
                
                return selfShadow;
            }
            
            float4 frag(varying input) : SV_Target
            {
                // 基础纹理
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                // 获取主光源
                Light mainLight = GetMainLight();
                
                // 计算漫反射
                float NdotL = saturate(dot(input.normalWS, mainLight.direction));
                float3 diffuse = mainLight.color * NdotL * mainLight.distanceAttenuation;
                
                // 计算六边形软阴影
                float shadow = CalculateHexSoftShadow(input.positionWS, input.normalWS);
                
                // 计算自阴影
                float selfShadow = CalculateSelfShadow(input.normalWS, mainLight.direction);
                shadow *= selfShadow;
                
                // 距离衰减
                float distance = length(input.positionWS - _WorldSpaceCameraPos);
                float distanceFade = saturate(1.0 - distance / _ShadowDistance);
                shadow = lerp(1.0, shadow, distanceFade);
                
                // 应用软阴影柔和度
                shadow = lerp(shadow, 1.0, (1.0 - _ShadowSoftness) * 0.3);
                
                // 基础光照计算
                float3 ambient = SampleSH(input.normalWS);
                float3 directLighting = diffuse * shadow;
                
                // 组合光照
                float3 finalColor = texColor.rgb * (directLighting + ambient);
                
                // 应用阴影颜色叠加
                float shadowFactor = saturate(1.0 - shadow);
                float3 shadowTint = lerp(float3(1.0, 1.0, 1.0), _ShadowColor.rgb, _ShadowIntensity);
                finalColor *= lerp(float3(1.0, 1.0, 1.0), shadowTint, shadowFactor * _ShadowIntensity);
                
                // 添加雾效
                finalColor = MixFog(finalColor, input.viewDirWS);
                
                return float4(finalColor, texColor.a);
            }
            ENDHLSL
        }
        
        // 阴影投射Pass - 使用URP内置的
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            
            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
        
        // 深度Only Pass
        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}
            
            ZWrite On
            ColorMask 0
            
            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
    }
    
    // 后备着色器
    FallBack "Universal Render Pipeline/Lit"
}