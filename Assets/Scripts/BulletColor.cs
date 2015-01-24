using UnityEngine;
using System.Collections;

public class BulletColor : MonoBehaviour {

	// Use this for initialization
	public void SetColor (Color c) {
		renderer.material.color = c;
		light.color = c;
		GetComponent<TrailRenderer>().material.color = c;
	}
}
