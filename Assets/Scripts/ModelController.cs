﻿using UnityEngine;
using System.Collections;

public class ModelController : MonoBehaviour {

	public Color playerColor;
	float outlineWidth = 0.01f;
	
	Animator anim;
	Vector3 localDir = new Vector3();
	
	// Use this for initialization
	void Start () {
		// Color
		SetColor(playerColor);
		
		// Animations
		anim = GetComponent<Animator>();
	}
	
	public void SetAnimationState(float speedFactor, Vector3 worldDirection) {
	
		localDir = transform.parent.InverseTransformDirection(worldDirection);
		localDir.Normalize();
		
		anim.SetFloat("speedFactor", speedFactor > 0.0f? 1.0f : 0.0f);
		anim.SetFloat("vAxis", localDir.x > 0.0f? 1.0f : (localDir.x < 0.0f? -1.0f : 0.0f));
		anim.SetFloat("hAxis", localDir.z > 0.0f? 1.0f : (localDir.z < 0.0f? -1.0f : 0.0f));
		anim.SetFloat("axisRelation", Mathf.Abs(localDir.z) - Mathf.Abs(localDir.x));
		
		
	}
	
	public void SetColor (Color c) {
		playerColor = c;
		Color baseColor = c;
		float maxColor = Mathf.Max(baseColor.r, Mathf.Max(baseColor.g, baseColor.b));
		float minFactor = 15.0f/255f;
		baseColor.r = baseColor.r*minFactor/maxColor;
		baseColor.g = baseColor.g*minFactor/maxColor;
		baseColor.b = baseColor.b*minFactor/maxColor;
		
			Color outlineColor = c;
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
