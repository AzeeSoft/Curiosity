Shader "Azee/ResearchScannableTest"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _Top ("Top", vector) = (0,0,0)
        _Bottom ("Bottom", vector) = (0,0,0)
        _ScanlineColor ("Scanline Color", Color) = (1,1,1,1)
        _ScanlineWidth ("Scanline Width", float) = 2.0
        _ScanlinePos ("Scanline Position", Range(0, 1)) = 0.0
        
        _WireframeBandSize ("Wireframe band size", float) = 0.05
        [PowerSlider(3.0)]
        _WireframeVal ("Wireframe width", Range(0., 0.34)) = 0.05
        _WireframeColor ("Wireframe Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            // compile shader into multiple variants, with and without shadows
            // (we don't care about any lightmaps yet, so skip these variants) 
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            // shadow helper functions and macros
            #include "AutoLight.cginc"

            #if defined (SHADOWS_SCREEN) || defined (SHADOWS_DEPTH) || defined (SHADOWS_CUBE)
            #    define SHADOWS_ENABLED
            #endif

            struct v2g
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 localVertex: TEXCOORD1;
                SHADOW_COORDS(2) // put shadows data into TEXCOORD2
                fixed3 diff : COLOR0;
                fixed3 ambient : COLOR1;
            };
            
            struct g2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 localVertex: TEXCOORD1;
                float3 bary: TEXCOORD2;
                SHADOW_COORDS(3) // put shadows data into TEXCOORD3
                fixed3 diff : COLOR0;
                fixed3 ambient : COLOR1;
            };

            float4 _Color;
            sampler2D _MainTex;
            float4 _Top;
            float4 _Bottom;
            float4 _ScanlineColor;
            float _ScanlineWidth;
            float _ScanlinePos;
            float4 _MainTex_ST;
            
            float _WireframeBandSize;
            float _WireframeVal;
            float4 _WireframeColor;

            v2g vert (appdata_base v)
            {
                // UNITY_INITIALIZE_OUTPUT(Input,o);
            
                v2g o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.localVertex = v.vertex;
                o.uv = v.texcoord;
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0.rgb;
                o.ambient = ShadeSH9(half4(worldNormal,1));
                // compute shadows data
                TRANSFER_SHADOW(o)
                
                return o;
            }
             
            [maxvertexcount(3)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream) 
            {
                g2f o;
                
                for (int i=0; i<3; i++) 
                {                
                    o.uv = IN[i].uv;
                    o.pos = IN[i].pos;
                    o.localVertex = IN[i].localVertex;
                    o.diff = IN[i].diff;
                    o.ambient = IN[i].ambient;
                    
                    #ifdef SHADOWS_ENABLED
                        o._ShadowCoord = IN[i]._ShadowCoord;
                    #endif 
                    
                    switch(i) 
                    {
                        case 0:
                            o.bary = float3(1., 0., 0.);
                            break;
                        case 1:
                            o.bary = float3(0., 1., 0.);
                            break;
                        case 2:
                            o.bary = float3(0., 0., 1.);
                            break;
                    }
                    triStream.Append(o); 
                }
            }

            fixed4 frag (g2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                fixed shadow = SHADOW_ATTENUATION(i);
                
//                fixed3 lighting = i.diff * shadow + i.ambient;
                fixed3 lighting = shadow + i.ambient;
                col.rgb *= length(lighting);
                
                float3 dDir = normalize(_Bottom - _Top);
                float projMagnitude = dot(dDir, i.localVertex - _Top);
                
                float3 totalMagnitude = length(_Bottom - _Top);
                
                float posPercent = projMagnitude/totalMagnitude;
                                
                if (posPercent < _ScanlinePos) 
                {
                    if (posPercent > _ScanlinePos - _ScanlineWidth) 
                    {
                        return lerp(_ScanlineColor, col, (posPercent - (_ScanlinePos - _ScanlineWidth))/(_ScanlineWidth));;
                    }
                    
                    if (posPercent > _ScanlinePos - _WireframeBandSize) 
                    {
                        if(any(bool3(i.bary.x < _WireframeVal, i.bary.y < _WireframeVal, i.bary.z < _WireframeVal))) 
                        {
                            return lerp(col, _WireframeColor, (posPercent - (_ScanlinePos - _WireframeBandSize))/(_WireframeBandSize));
                        }
                    }
                }
                
                return col;
            }
            ENDCG
        }
        
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
