Shader "Custom/Floor" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf CustomLight

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		
		half4 LightingCustomLight_PrePass (SurfaceOutput s, half4 light) {
			half4 col = half4(s.Albedo,1);
			return col;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
