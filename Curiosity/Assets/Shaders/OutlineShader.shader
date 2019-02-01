// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Azee/Outline Shader Test"
{
    Properties
    {
        [Header(Outline)]
        _OutlineVal("Outline value", Range(0., 2.)) = 1.
        _OutlineCol("Outline color", color) = (1., 1., 1., 1.)
    }
    SubShader
    {
        Tags {"Queue" = "Geometry" "RenderType" = "Opaque"}

        Pass
        {
            // Cull Front

            CGPROGRAM
            #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

            struct v2f {
                float4 pos : SV_POSITION;
            };

            float _OutlineVal;

            v2f vert(appdata_base v) {
                v2f o;

                // Convert vertex to clip space
                o.pos = UnityObjectToClipPos(v.vertex);

                // Convert normal to view space (camera space)
                float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);

                // Compute normal value in clip space
                normal.x *= UNITY_MATRIX_P[0][0];
                normal.y *= UNITY_MATRIX_P[1][1];

                // Scale the model depending the previous computed normal and outline value
                o.pos.xy += _OutlineVal * normal.xy;
                return o;
            }

            fixed4 _OutlineCol;

            fixed4 frag(v2f i) : SV_Target {
                return _OutlineCol;
            }

            ENDCG
        }
    }
}