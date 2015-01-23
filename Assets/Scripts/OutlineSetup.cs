using UnityEngine;
using System.Collections;

public class OutlineSetup : MonoBehaviour {

	public bool isPlane = false;
	
	// Use this for initialization
	void Start () {
		renderer.material.SetVector("_TransformScale", (isPlane? 10f : 1f) * transform.lossyScale);
	}
}
