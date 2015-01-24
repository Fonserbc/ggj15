Shader "Custom/Floor" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BaseColor ("Base Color", Color) = (1, 1, 1, 1)
		_OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
		_OutlineWidth ("Outline width", Range(0, 0.5)) = 0.1
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
		#pragma exclude_renderers gles
		#include "UnityCG.cginc"
		#include "Assets/Shaders/CustomLight.cginc"
		#pragma surface surf CustomLight

		uniform sampler2D _MainTex;
		uniform float4 _BaseColor;
		uniform float _OutlineWidth;
		uniform float4 _OutlineColor;
		
		float mod(float x, float y) {
		  return x - y * floor(x/y);
		}
		
		float hex(float2 p) {
			p.x += 0.43;
			p.x *= 0.57735*2.0;
			p.y += fmod(floor(p.x), 2.0)*0.5;
			p = abs((fmod(p, 1.0) - 0.5));
			return abs(max(p.x*1.5 + p.y, p.y*2.0) - 1.0);
		}

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float4 screenPos;
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
			float startFog = 10.0;
			float fogWidth = 5.0;
			float fogFactor = min(1.0, max(0.0, IN.screenPos.z - startFog)/fogWidth);
			float4 tex = tex2D (_MainTex, IN.uv_MainTex);
			float3 baseColor = tex.rgb * _BaseColor.rgb;
			float h = hex(abs(IN.worldPos.xz/2.0));
			if ( h < _OutlineWidth ) {
				o.Albedo = _OutlineColor.rgb;
				o.Albedo = lerp(o.Albedo, baseColor, fogFactor);
				o.Gloss = 1.0 - fogFactor;
			}
			else if ( h < _OutlineWidth*3.0 ) {
				o.Albedo = lerp(_OutlineColor.rgb, baseColor, (h -_OutlineWidth)/(_OutlineWidth*2.0));
				o.Albedo = lerp(o.Albedo, baseColor, fogFactor);
				o.Gloss = 1.0 - fogFactor;
			}
			else {
				o.Albedo = baseColor;
				o.Gloss = 0;
			}
			
			o.Alpha = tex.a;		
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
