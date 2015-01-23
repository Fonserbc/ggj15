using UnityEngine;
using System.Collections;

public class OutlineSetup : MonoBehaviour {

	// Use this for initialization
	void Start () {
		renderer.material.SetVector("_TransformScale", transform.lossyScale);
	}
}
