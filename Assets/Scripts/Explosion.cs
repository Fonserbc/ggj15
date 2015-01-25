using System;
using UnityEngine;

class Explosion : Photon.MonoBehaviour
{
	float bulletSpeed = 25.0f;
	private double lastBeat;

	double initialBeatTime;
	void Start()
	{
		Color c = RhythmMovement.PlayerColor (photonView.owner.ID);
		Transform particles = transform.GetChild (0);
		particles.renderer.material.SetColor("_TintColor",c);
		renderer.material.color = c;
		light.color = c;
		initialBeatTime = MusicBeat.BeatTime;
		lastBeat = MusicBeat.BeatTime;

	}

	void Update()
	{
		double beatTime = MusicBeat.BeatTime;
		if (photonView.isMine) {
			if (lastBeat > beatTime)
				initialBeatTime -= lastBeat - beatTime + 1.0;
			if((beatTime-initialBeatTime) >= 1) {
				PhotonNetwork.Destroy(gameObject);
			}
		} else {
			enabled = false;
		}
		lastBeat = beatTime;
	}
	
	void OnTriggerEnter (Collider col) {
		if (col.gameObject.tag == "Player") {
			if (photonView.isMine) {
				PhotonView otherView = col.gameObject.GetComponentInParent<PhotonView>();
				if(otherView.ownerId != photonView.owner.ID) {
					//hit
					Debug.Log("You hit player " + otherView.ownerId);
					GameObject.FindGameObjectWithTag("Logic").GetComponent<GameSession>().Hit(otherView.ownerId);
				}
			}
		}
	}
}
