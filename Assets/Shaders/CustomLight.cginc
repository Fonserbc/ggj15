#ifndef CUSTOM_LIGHT_MODEL
#define CUSTOM_LIGHT_MODEL

float Epsilon = 1e-10;
 
float3 RGBtoHCV(in float3 RGB) {
    // Based on work by Sam Hocevar and Emil Persson
    float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0/3.0) : float4(RGB.gb, 0.0, -1.0/3.0);
    float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
    float C = Q.x - min(Q.w, Q.y);
    float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
    return float3(H, C, Q.x);
}
  
float3 RGBtoHSL(in float3 RGB) {
	float3 HCV = RGBtoHCV(RGB);
	float L = HCV.z - HCV.y * 0.5;
	float S = HCV.y / (1 - abs(L * 2 - 1) + Epsilon);
	return float3(HCV.x, S, L);
}

float3 HUEtoRGB(in float H) {
    float R = abs(H * 6 - 3) - 1;
    float G = 2 - abs(H * 6 - 2);
    float B = 2 - abs(H * 6 - 4);
    return saturate(float3(R,G,B));
}

float3 HSLtoRGB(in float3 HSL) {
    float3 RGB = HUEtoRGB(HSL.x);
    float C = (1 - abs(2 * HSL.z - 1)) * HSL.y;
    return (RGB - 0.5) * C + HSL.z;
}

float4 LightingCustomLight_PrePass (SurfaceOutput s, float4 light) {
	half4 col;
	if(s.Gloss > .5f) {
		float3 surfHSL = RGBtoHSL(s.Albedo);
		float3 lightHSL = RGBtoHSL(light.rgb);
		float3 result = surfHSL;
		if(lightHSL.z > 0.5f) {
			float weight = (lightHSL.z-0.5f)/0.5f;
			result.x = (result.x)*(1-weight)+((surfHSL.x+lightHSL.x)/2.0f)*weight;
		}
		result.y = 1;
		col = float4(HSLtoRGB(result),1);
	} else {
		col = float4(s.Albedo,1)*light;
	}
	return col;
}

#endif