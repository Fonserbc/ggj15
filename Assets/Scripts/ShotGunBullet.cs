using System;
using UnityEngine;

class ShotGunBullet : Photon.MonoBehaviour
{
	float bulletSpeed = 50.0f;
	double initialBeatTime;
	bool started = false;
	float lifeTime = 3.0f;

	void Start()
	{
		BulletColor bcollor = GetComponent<BulletColor>();
		bcollor.SetColor(RhythmMovement.PlayerColor(photonView.owner.ID));
		transform.SetParent (Camera.main.transform, true);

		initialBeatTime = MusicBeat.BeatTime;
	}

	void Update()
	{
		double beatTime = MusicBeat.BeatTime;
		if (photonView.isMine) {
			if(!started) {
				if((beatTime-initialBeatTime) >= 1) {
					transform.SetParent (null, true);

					rigidbody.velocity = transform.forward * bulletSpeed;
					started = true;
				}
			} else {
				lifeTime -= Time.deltaTime;
				if (lifeTime < 0.0f) PhotonNetwork.Destroy(gameObject);
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
