using UnityEngine;
using System.Collections;

public class UpdateBeatTime : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		renderer.material.SetFloat ("_BeatTime", (float) MusicBeat.BeatTime);
	}
}
