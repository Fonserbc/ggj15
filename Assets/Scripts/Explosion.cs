using System;
using UnityEngine;

class Explosion : Photon.MonoBehaviour
{
	float bulletSpeed = 25.0f;
	double initialBeatTime;
	void Start()
	{
		Color c = RhythmMovement.PlayerColor (photonView.owner.ID);
		Transform particles = transform.GetChild (0);
		particles.renderer.material.SetColor("_TintColor",c);
		renderer.material.color = c;
		light.color = c;
		initialBeatTime = MusicBeat.BeatTime;

	}

	void Update()
	{
		double beatTime = MusicBeat.BeatTime;
		if (photonView.isMine) {
			if((beatTime-initialBeatTime) >= 1) {
				PhotonNetwork.Destroy(gameObject);
			}
		} else {
			enabled = false;
		}
	}
	
	void OnCollisionEnter (Collision col) {
		if (col.gameObject.tag == "Player") {
			if (photonView.isMine) {
				//col.gameObject.GetComponent<PhotonView>().ownerId
			}
		}
	}
}
