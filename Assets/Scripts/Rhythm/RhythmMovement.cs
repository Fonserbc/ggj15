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
		previousEventTime = (float)AudioSettings.dspTime;
		nextEventTime = (float)AudioSettings.dspTime;

	}
	
	// Update is called once per frame
	void Update () {
		if(move) {
			move = false;
			Vector3 position = -Vector3.up;

			position += transform.forward*Input.GetAxis("Vertical") + transform.right*Input.GetAxis("Horizontal");

			/*
			if(Input.GetKey(KeyCode.A)) position -= transform.right;
			if(Input.GetKey(KeyCode.D)) position += transform.right;
			if(Input.GetKey(KeyCode.S)) position -= transform.forward;
			if(Input.GetKey(KeyCode.W)) position += transform.forward;
			*/


			startPosition = transform.position;
			endPosition = startPosition + position.normalized*cubesPerBeat;
		}
		float time = Mathf.Min (1.0f,((float) AudioSettings.dspTime - previousEventTime) / (nextEventTime - previousEventTime));
		time = Easing.Circular.Out (time);
		CharacterController controller = GetComponent<CharacterController>();
		if(startPosition != endPosition) controller.Move(Vector3.Lerp(startPosition, endPosition,time)-transform.position);
	}


	public void Beat(Vector2 data) {
		//Debug.Log (data.x + " " + beatTime);
		if(data.x == beatTime) {
			move = true;
			beatTime += 1f/myCompassBeat;
			previousEventTime = data.y;
			nextEventTime = previousEventTime + (60f/(MusicBeat.beatsPerMinute*(MusicBeat.compassDenominator/MusicBeat.compassNumerator)*myCompassBeat*velocity));

		}
	}
}
