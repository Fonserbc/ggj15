using System;
using UnityEngine;

class Grenade : Photon.MonoBehaviour
{
	float bulletSpeed = 25.0f;
	double initialBeatTime;
	void Start()
	{
		BulletColor bcollor = GetComponent<BulletColor>();
		bcollor.SetColor(RhythmMovement.PlayerColor(photonView.owner.ID));
		rigidbody.velocity = transform.forward * bulletSpeed;
		initialBeatTime = MusicBeat.BeatTime;

	}

	void Update()
	{
		double beatTime = MusicBeat.BeatTime;
		if (photonView.isMine) {
			if((beatTime-initialBeatTime) >= 2) {
				Camera.main.gameObject.SendMessage("ShootExplosion", transform.position);
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
