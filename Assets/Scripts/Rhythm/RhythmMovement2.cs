using UnityEngine;
using System.Collections;

public class RhythmMovement2 : MonoBehaviour {

	bool move = false;
	float beatTime = 0f;
	public float myCompassBeat = 1f;
	public float cubesPerBeat = 2f;



	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(move) {
			move = false;

			Vector3 position = Vector3.zero;
			if(Input.GetKey(KeyCode.LeftArrow)) position -= transform.right;
			if(Input.GetKey(KeyCode.RightArrow)) position += transform.right;
			if(Input.GetKey(KeyCode.DownArrow)) position -= transform.forward;
			if(Input.GetKey(KeyCode.UpArrow)) position += transform.forward;

			transform.position += position*cubesPerBeat;

		}


	}

	public void Beat(Vector2 data) {
		Debug.Log (data.x + " " + beatTime);
		if(data.x == beatTime) {
			move = true;
			beatTime += 1f/myCompassBeat;
		}
	}
}
