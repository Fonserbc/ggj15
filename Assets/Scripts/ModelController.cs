using UnityEngine;
using System.Collections;

public class ModelController : MonoBehaviour {

	public Color playerColor;
	float outlineWidth = 0.01f;
	
	Animator anim;
	
	// Use this for initialization
	void Start () {
		// Color
		Color baseColor = playerColor;
		float maxColor = Mathf.Max(baseColor.r, Mathf.Max(baseColor.g, baseColor.b));
		float minFactor = 15.0f/255f;
		baseColor.r = baseColor.r*minFactor/maxColor;
		baseColor.g = baseColor.g*minFactor/maxColor;
		baseColor.b = baseColor.b*minFactor/maxColor;
		
		Color outlineColor = playerColor;
		outlineColor.r = outlineColor.r/maxColor;
		outlineColor.g = outlineColor.g/maxColor;
		outlineColor.b = outlineColor.b/maxColor;
		
		foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
			r.material.SetColor("_OutlineColor", outlineColor);
			r.material.SetColor("_BaseColor", baseColor);
			r.material.SetFloat("_OutlineWidth", outlineWidth);
		}
		
		// Animations
		anim = GetComponent<Animator>();
	}
	
	void Update() {
		float hAxis = Input.GetAxis("Horizontal");
		float vAxis = Input.GetAxis("Vertical");
		
		float absHAxis = Mathf.Abs(hAxis);
		float absVAxis = Mathf.Abs(vAxis);
		float axisRelation = absHAxis - absVAxis;
		float speedFactor = Mathf.Max(absHAxis, absVAxis);
		
		anim.SetFloat("hAxis", hAxis);
		anim.SetFloat("vAxis", vAxis);
		anim.SetFloat("axisRelation", axisRelation);
		anim.SetFloat("speedFactor", speedFactor);
	}
}
