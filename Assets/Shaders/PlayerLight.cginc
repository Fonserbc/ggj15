#ifndef PLAYER_LIGHT_MODEL
#define PLAYER_LIGHT_MODEL

float4 LightingPlayerLight_PrePass (SurfaceOutput s, float4 light) {
	if (s.Gloss > .5) {
		return float4(s.Albedo,1);
	}
	else {
		return float4(s.Albedo * length(light.rgb),1);
	}
}

#endif