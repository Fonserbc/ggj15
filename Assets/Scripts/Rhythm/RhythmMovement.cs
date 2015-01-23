using UnityEngine;
using System.Collections;

public class RhythmMovement : MonoBehaviour {

	bool move = false;
	float beatTime = 0f;
	public float myCompassBeat = 1f;
	public float cubesPerBeat = 2f;
	public float velocity = 2f;

	
	Vector3 startPosition;
	Vector3 endPosition;

	float previousEventTime;
	float nextEventTime;

	// Use this for initialization
	void Start () {
		startPosition = transform.position;
		endPosition = transform.position;
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


			startPosition = endPosition;
			endPosition += position*cubesPerBeat;
		}
		float time = Mathf.Min (1.0f,((float) AudioSettings.dspTime - previousEventTime) / (nextEventTime - previousEventTime));
		time = Easing.Circular.Out (time);
		rigidbody.MovePosition(Vector3.Lerp(startPosition, endPosition,time));

	}

	public void Beat(Vector2 data) {
		Debug.Log (data.x + " " + beatTime);
		if(data.x == beatTime) {
			move = true;
			beatTime += 1f/myCompassBeat;
			previousEventTime = data.y;
			nextEventTime = previousEventTime + (60f/(MusicBeat.beatsPerMinute*(MusicBeat.compassDenominator/MusicBeat.compassNumerator)*myCompassBeat*velocity));

		}
	}
}
