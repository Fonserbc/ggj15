using System;
using UnityEngine;

class Grenade : Photon.MonoBehaviour
{
	float bulletSpeed = 25.0f;
	double initialBeatTime;
	private double lastBeat;

	void Start()
	{
		BulletColor bcollor = GetComponent<BulletColor>();
		bcollor.SetColor(RhythmMovement.PlayerColor(photonView.owner.ID));
		rigidbody.velocity = transform.forward * bulletSpeed;
		initialBeatTime = MusicBeat.BeatTime;
		lastBeat = MusicBeat.BeatTime;
	}

	void Update()
	{
		double beatTime = MusicBeat.BeatTime;
		if (photonView.isMine) {
			if (lastBeat > beatTime)
				initialBeatTime -= lastBeat - beatTime + 1.0;
			if((beatTime-initialBeatTime) >= 2) {
				Camera.main.gameObject.SendMessage("ShootExplosion", transform.position);
				PhotonNetwork.Destroy(gameObject);
			}
		} else {
			enabled = false;
		}
		lastBeat = beatTime;
	}
	
	void OnCollisionEnter (Collision col) {
		if (col.gameObject.tag == "Player") {
			if (photonView.isMine) {
				//col.gameObject.GetComponent<PhotonView>().ownerId
			}
		}
	}
}
