using UnityEngine;
using System.Collections;

public class MusicBeat : MonoBehaviour {

	public static float beatsPerMinute = 90f;
	public static float compassNumerator = 4f;
	public static float compassDenominator = 4f;

	float compassBeatDivision = 4f; // semicorcheras

	float nextEventTime;
	float nextEventTimeSched;


	public AudioSource music;
	public GameObject beatObjects;
	public GameObject beatObjectsSched;



	float currentBeat = 0f;
	float currentBeatSched = 0f;

	bool running = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space) && !running) {
			music.PlayScheduled(AudioSettings.dspTime + 1.0);
			currentBeat = -1f/compassBeatDivision;
			currentBeatSched = -1f/compassBeatDivision;

			nextEventTime = (float) AudioSettings.dspTime + 1.0f;
			nextEventTimeSched = (float) AudioSettings.dspTime + 1.0f;

			running = true;
		}

		if(!running) return;

		float time = (float) AudioSettings.dspTime;

		if(time >= nextEventTime) {
			currentBeat += 1f/compassBeatDivision;
			if(currentBeat == 180) {
				music.time -= 172*(60f/(beatsPerMinute*(compassDenominator/compassNumerator)));
				currentBeat -= 172;
			}
			Vector2 data = new Vector2(currentBeat, nextEventTime);	

			beatObjects.BroadcastMessage("Beat", data, SendMessageOptions.DontRequireReceiver);

			nextEventTime += (60f/(beatsPerMinute*(compassDenominator/compassNumerator)*compassBeatDivision));


		}

		if(time + 1f > nextEventTimeSched) {
			currentBeatSched += 1f/compassBeatDivision;

			Vector2 data = new Vector2(currentBeatSched, nextEventTimeSched);

			beatObjectsSched.BroadcastMessage("Beat", data, SendMessageOptions.DontRequireReceiver);

			nextEventTimeSched += (60f/(beatsPerMinute*(compassDenominator/compassNumerator)*compassBeatDivision));

		}
	}
}
