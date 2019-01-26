#include "UnityCG.cginc"
 
struct v2f {
    float4 pos : SV_POSITION;
};

float _OutlineThickness;

v2f vert(appdata_base v) {
    v2f o;

    // Convert vertex to clip space
    o.pos = UnityObjectToClipPos(v.vertex);

    // Convert normal to view space (camera space)
    float3 normal = mul((float3x3) UNITY_MATRIX_IT_MV, v.normal);

    // Compute normal value in clip space
    normal.x *= UNITY_MATRIX_P[0][0];
    normal.y *= UNITY_MATRIX_P[1][1];

    // Scale the model depending the previous computed normal and outline value
    o.pos.xy += _OutlineThickness * normal.xy;
    return o;
}

fixed4 _OutlineColor;

fixed4 frag(v2f i) : SV_Target {
    return _OutlineColor;
}