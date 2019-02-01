// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Azee/XRayShader"
{
	Properties
	{
		_XRayColor("X-Ray Color", Color) = (0, 0, 0, 1) // XRay Color set using the custom standard shader. (Black = not set)
	}

	SubShader
	{
		Tags {"Queue" = "Transparent"}

		ZTest Always
		ZWrite Off
		Blend One One

		Pass
		{

			CGPROGRAM
			#pragma vertex vert
				#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float depth : DEPTH;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.depth = - mul(UNITY_MATRIX_MV, v.vertex).z * _ProjectionParams.w;
				return o;
			}

			// Global X-Ray Default Color
			half4 _XRayDefaultColor;
			half4 _XRayColor;

			fixed4 frag(v2f i) : SV_TARGET
			{
				half4 color = _XRayColor;

				if(_XRayColor.r == 0 && _XRayColor.g == 0 && _XRayColor.b == 0)
				{
					color = _XRayDefaultColor;
				}

				// color = color * (i.depth);

				return color;
			}

			ENDCG
		}
	}
	FallBack "Azee/CustomStandard"
}
