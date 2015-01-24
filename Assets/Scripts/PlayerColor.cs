using UnityEngine;
using System.Collections;

public class PlayerColor : MonoBehaviour {

	public Light playerLight;
	float outlineWidth = 0.01f;

	// Use this for initialization
	void Start () {
		Color baseColor = playerLight.color;
		float maxColor = Mathf.Max(baseColor.r, Mathf.Max(baseColor.g, baseColor.b));
		float minFactor = 15.0f/255f;
		baseColor.r = baseColor.r*minFactor/maxColor;
		baseColor.g = baseColor.g*minFactor/maxColor;
		baseColor.b = baseColor.b*minFactor/maxColor;
		
		Color outlineColor = playerLight.color;
		outlineColor.r = outlineColor.r/maxColor;
		outlineColor.g = outlineColor.g/maxColor;
		outlineColor.b = outlineColor.b/maxColor;
		
		foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
			r.material.SetColor("_OutlineColor", outlineColor);
			r.material.SetColor("_BaseColor", baseColor);
			r.material.SetFloat("_OutlineWidth", outlineWidth);
		}
	}
}
