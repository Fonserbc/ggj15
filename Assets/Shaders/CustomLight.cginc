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

float torramod(float x, float y) {
	return x - y * floor(x/y);
}

float3 computeBorder(float3 albedo, float3 light) {
	float3 surfHSL = RGBtoHSL(albedo);
	float3 lightHSL = RGBtoHSL(light);
	float3 result = surfHSL;
	result.z = 0.6; //because yes, because we like gay pastel lights
	result.y = 1; //in soviet game jams, colors saturate you
	if(lightHSL.z > 0.5f && lightHSL.y != 0.0f) { //general light case
		float weight = min(1,(lightHSL.z-0.5f)/0.5f);
		float dist = abs(surfHSL.x - lightHSL.x); //hue distance
		if (dist > 0.5) { //the other way around the HSL ring must be taken
			if(lightHSL.x < surfHSL.x) lightHSL.x += 1.0f;
			else surfHSL.x += 1.0f;
		}
		result.x = (surfHSL.x)*(1-weight)+lightHSL.x*weight; //average hue
	}
	else if(lightHSL.y == 0.0f) { //white fucking light I hate you so much please die in a fucking fire somewhere far away
		result.z = max(0.6, lightHSL.z); //light up dat pastel
	}
	return HSLtoRGB(result);
}

float3 computeCenter(float3 albedo, float3 light) {
	return albedo*light;
}

float4 LightingCustomLight_PrePass (SurfaceOutput s, float4 light) {
	return float4(s.Gloss > 0.5? computeBorder(s.Albedo, light.rgb) : computeCenter(s.Albedo, light.rgb),1);
}

#endif