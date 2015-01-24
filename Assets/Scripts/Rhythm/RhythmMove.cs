using UnityEngine;
using System.Collections;

public class RhythmMove : MonoBehaviour {

	bool move = false;
	float beatTime = 0f;
	Vector3 initPos;
	public Vector3 endPos;
	public float myCompassBeat;


	// Use this for initialization
	void Start () {
		initPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(move) {
			move = false;
			if(transform.position == initPos) transform.position = initPos+endPos;
			else transform.position = initPos;
		}
	}

	public void Beat(Vector2 data) {
		if(data.x == beatTime) {
			move = true;
			beatTime += myCompassBeat;
		}
	}
}
