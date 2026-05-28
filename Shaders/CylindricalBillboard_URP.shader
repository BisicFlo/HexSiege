Shader "Custom/CylindricalBillboard_URP"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _BaseColor ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline"
               "Queue"="Transparent"
               "RenderType"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4  _BaseColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                float3 worldPivot = TransformObjectToWorld(float3(0, 0, 0));

                // Camera right from the view matrix
                float3 camRight = float3(UNITY_MATRIX_V[0][0],
                                         UNITY_MATRIX_V[1][0],
                                         UNITY_MATRIX_V[2][0]);

                float3 up      = float3(0, 1, 0);
                float3 forward = normalize(cross(up, camRight));
                float3 right   = cross(forward, up);

                float3 worldOffset = IN.positionOS.x * right
                                   + IN.positionOS.y * up;

                float3 worldPos = worldPivot + worldOffset;
                OUT.positionCS  = TransformWorldToHClip(worldPos);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                return SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
            }
            ENDHLSL
        }
    }
}