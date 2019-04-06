Shader "Azee/ResearchScannableTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Top ("Top", vector) = (0,0,0)
        _Bottom ("Bottom", vector) = (0,0,0)
        _ScanlineColor ("Scanline Color", Color) = (1,1,1,1)
        _ScanlineWidth ("Scanline Width", float) = 2.0
        _ScanlinePos ("Scanline Position", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
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
                float4 localVertex: TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _Top;
            float4 _Bottom;
            float4 _ScanlineColor;
            float _ScanlineWidth;
            float _ScanlinePos;
            float4 _MainTex_ST;
            

            v2f vert (appdata v)
            {
                // UNITY_INITIALIZE_OUTPUT(Input,o);
            
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.localVertex = v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                float3 dDir = normalize(_Bottom - _Top);
                float projMagnitude = dot(dDir, i.localVertex - _Top);
                
                float3 totalMagnitude = length(_Bottom - _Top);
                
                float posPercent = projMagnitude/totalMagnitude;
                                
                if (posPercent > _ScanlinePos - _ScanlineWidth && posPercent < _ScanlinePos) {
                    col = _ScanlineColor;
                }
                
                return col;
            }
            ENDCG
        }
    }
}
