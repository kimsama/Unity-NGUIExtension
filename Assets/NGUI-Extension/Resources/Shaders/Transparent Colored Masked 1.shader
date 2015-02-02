//////////////////////////////////////////////////////////////////////////
// Transparent Colored Masked 1.shader
//
// NGUI shader for unlit transparent texture with alpha mask which is used in 
// UIDrawCall has clip count 1.
//
// (c) 2015 hwkim
//////////////////////////////////////////////////////////////////////////

Shader "Hidden/Unlit/Transparent Colored AlphaMasked 1"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
		_MaskTex ("MaskTexture", 2D) = "white" {}
		_Color ("Color", Color) = (0.5, 0.5, 0.5, 0.5)
	}
 
	SubShader
	{
		LOD 200
 
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
 
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha
 
		Pass
		{
			CGPROGRAM
			#pragma vertex vertexProgram
			#pragma fragment fragmentProgram
			#include "UnityCG.cginc"
 
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 uv1 : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				fixed4 color : COLOR;
			};
 
			struct vertexToFragment
			{
				float4 vertex : SV_POSITION;
				float2 uv1 : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float2 worldPos : TEXCOORD2;
				fixed4 color : COLOR;
			};
 
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MaskTex_ST;
			float4 _Color;
			sampler2D _MaskTex;
 			
 			float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
			float2 _ClipArgs0 = float2(1000.0, 1000.0);

			vertexToFragment vertexProgram (appdata_t vertexData)
			{
				vertexToFragment output;
				output.vertex = mul(UNITY_MATRIX_MVP, vertexData.vertex);
				output.uv1 = TRANSFORM_TEX(vertexData.uv1, _MainTex);
				output.uv2 = TRANSFORM_TEX(vertexData.uv2, _MaskTex);
				output.color = vertexData.color;
				output.worldPos = vertexData.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
				return output;
			}
 
			half4 fragmentProgram (vertexToFragment input) : COLOR
			{
				half4 base = tex2D (_MainTex, input.uv1);
				
				half4 mask = tex2D (_MaskTex, input.uv2);
				base.w = mask.x * mask.x * mask.x;    				

				// Softness factor
				float2 factor = (float2(1.0, 1.0) - abs(input.worldPos)) * _ClipArgs0;

				// Sample the texture
				half4 col = base * input.color;
				col.a *= clamp( min(factor.x, factor.y), 0.0, 1.0);				
				return col;
			}
			ENDCG
		}
	}
	Fallback Off
}
