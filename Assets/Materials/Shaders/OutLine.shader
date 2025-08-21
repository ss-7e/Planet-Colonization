Shader "Custom/ToonWithOutline"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0.001, 0.05)) = 0.01
        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.5
        _LightColor ("Light Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0.4,0.4,0.4,1)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        // Outline pass
        Pass
        {
            Name "Outline"
            Cull Front
            ZWrite On
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            v2f vert(appdata v)
            {
                v2f o;
                float3 norm = normalize(v.normal);
                float3 viewPos = mul(UNITY_MATRIX_MV, float4(v.vertex.xyz, 1.0)).xyz;
                float3 offset = norm * _OutlineWidth * length(viewPos);
                o.pos = TransformObjectToHClip(v.vertex + float4(offset, 0));
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return float4(_OutlineColor.rgb, _OutlineColor.a);
            }
            ENDHLSL
        }
                // ShadowCaster Pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma vertex vertShadow
            ENDHLSL
        }

        // Toon Shading pass with shadows
        Pass
        {
            Name "ToonShading"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            float _ShadowThreshold;
            float4 _LightColor;
            float4 _ShadowColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normalWS = TransformObjectToWorldNormal(v.normal);
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);

                VertexPositionInputs posInputs = GetVertexPositionInputs(v.vertex.xyz);
                o.shadowCoord = GetShadowCoord(posInputs);

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float3 normal = normalize(i.normalWS);
                Light mainLight = GetMainLight(i.shadowCoord);
                float3 lightDir = normalize(mainLight.direction);
                float NdotL = saturate(dot(normal, lightDir));

                half shadowAttenuation = MainLightRealtimeShadow(i.shadowCoord);
                float lightFactor = step(_ShadowThreshold, NdotL * shadowAttenuation);

                float3 lightColor = lerp(_ShadowColor.rgb, _LightColor.rgb, lightFactor);
                float4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                return float4(albedo.rgb * lightColor, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
