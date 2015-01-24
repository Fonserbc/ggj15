using UnityEngine;
using System.Collections;

public class MusicShed : MonoBehaviour {

	float beatsPerMinute = 90f;
	float compassDenominator = 4f;

	float compassBeatDivision = 4f; // semicorcheras

	float nextEventTime;

	public AudioSource music;
	public GameObject beatObjects;

	float currentBeat = 0f;

	bool running = false;

	// Use this for initialization
	void Start () {
		currentBeat = 0f;

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space) && !running) {
			music.Play ();

			Vector2 data = new Vector2(currentBeat, (float) AudioSettings.dspTime);
			beatObjects.BroadcastMessage("Beat", data);

			nextEventTime = (float) AudioSettings.dspTime + (60f/beatsPerMinute)/compassBeatDivision;
			running = true;
		}

		if(!running) return;

		float time = (float) AudioSettings.dspTime;

		if(time + 1f > nextEventTime) {
			currentBeat += 1f/compassBeatDivision;

			Vector2 data = new Vector2(currentBeat, nextEventTime);

			beatObjects.BroadcastMessage("Beat", data);

			nextEventTime += (60f/(beatsPerMinute*(compassDenominator/4f)*compassBeatDivision));

		}
	}
}
