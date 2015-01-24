using UnityEngine;
using System.Collections;

public class FreeMove : MonoBehaviour {

	public float speed = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += transform.forward * Time.deltaTime * speed * Input.GetAxis("Vertical");
		transform.position += transform.right * Time.deltaTime * speed * Input.GetAxis("Horizontal");
	}
}
