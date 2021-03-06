//////////////////////////////////////////////////////////////////////////
// GrayScale-AlphaMasked.shader
//
// NGUI shader for unlit grayscale sprite with alpha mask.
//
// (c) 2015 hwkim
//////////////////////////////////////////////////////////////////////////

Shader "Hidden/Unlit/GrayScale-AlphaMasked"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
		_MaskTex ("MaskTexture", 2D) = "white" {}
		//_Color ("Color", Color) = (0.5, 0.5, 0.5, 0.5)
		//_Alpha ("Alpha", Range (0,1)) = 1.0
		_EffectAmount ("Effect Amount", Range (0,1)) = 1.0
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
				//fixed4 color : COLOR;
			};
 
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MaskTex_ST;
			//float4 _Color;
			sampler2D _MaskTex;
			uniform float _EffectAmount;
			//fixed _Alpha;
 
			vertexToFragment vertexProgram (appdata_t vertexData)
			{
				vertexToFragment output;
				output.vertex = mul(UNITY_MATRIX_MVP, vertexData.vertex);
				output.uv1 = TRANSFORM_TEX(vertexData.uv1, _MainTex);
				output.uv2 = TRANSFORM_TEX(vertexData.uv2, _MaskTex);
				//output.color = vertexData.color;
				return output;
			}
 
			half4 fragmentProgram (vertexToFragment input) : COLOR
			{
				half4 base = tex2D (_MainTex, input.uv1);
				half4 mask = tex2D (_MaskTex, input.uv2);
				base.w = mask.x * mask.x * mask.x;
    			//return dot(base.rgb, float3(0.3, 0.59, 0.11));
    			base.rbg = lerp(base.rgb, dot(base.rgb, float3(0.3, 0.59, 0.11)), _EffectAmount);
    			return base;
			}
			ENDCG
		}
	}
}
