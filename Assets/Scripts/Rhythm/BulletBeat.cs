using UnityEngine;
using System.Collections;

public class BulletBeat : MonoBehaviour {

	float beatTime = 0f;
	public float velocity = 10f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Beat(Vector2 data) {
		//Debug.Log (data.x + " " + beatTime);
		if(data.x == beatTime) {
			rigidbody.velocity = transform.forward*velocity;
		}
	}

	public void setBeat(float beat) {
		beatTime = beat;
	}
}
