Shader "Custom/Wall" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BaseColor ("Base Color", Color) = (1, 1, 1, 1)
		_OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
		_OutlineWidth ("Outline width", Range(0, 0.5)) = 0.1
		_TransformScale ("Transform Scale", Vector) = (1, 1, 1)
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
		#pragma exclude_renderers gles
		#include "UnityCG.cginc"
		#include "Assets/Shaders/CustomLight.cginc"
		#pragma surface surf CustomLight vertex:vert

		uniform sampler2D _MainTex;
		uniform float4 _BaseColor;
		uniform float _OutlineWidth;
		uniform float4 _OutlineColor;
		uniform float3 _TransformScale;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float3 localNormal;
		};

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.localNormal = abs(v.normal);
		}
		
		void surf (Input IN, inout SurfaceOutput o) {
			float4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _BaseColor.rgb;
			float3x2 scaleMatrix = { 
				_TransformScale.z, _TransformScale.y, _TransformScale.x, _TransformScale.z, _TransformScale.x, _TransformScale.y
			};
			float2 scaleChachi = mul(IN.localNormal, scaleMatrix);
			float2 uv = IN.uv_MainTex;
			float2 ow = float2(_OutlineWidth,_OutlineWidth)/scaleChachi;
			if ( uv.x < ow.x || uv.y < ow.y || uv.x > (1.0-ow.x) || uv.y > (1.0-ow.y)) {
				o.Albedo = _OutlineColor.rgb;
				o.Gloss = 1;
			}
			else {
				o.Gloss = 0;
			}
			o.Alpha = c.a;
			
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
