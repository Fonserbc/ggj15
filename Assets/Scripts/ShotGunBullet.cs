﻿using System;
using UnityEngine;

class ShotGunBullet : Photon.MonoBehaviour
{
	float bulletSpeed = 50.0f;
	double initialBeatTime;
	bool started = false;
	float lifeTime = 3.0f;
	private double lastBeat;

	void Start()
	{
		BulletColor bcollor = GetComponent<BulletColor>();
		bcollor.SetColor(RhythmMovement.PlayerColor(photonView.owner.ID));
		if (photonView.isMine) transform.SetParent (Camera.main.transform, true);
		else enabled = false;

		initialBeatTime = MusicBeat.BeatTime;
		lastBeat = MusicBeat.BeatTime;

	}

	void Update()
	{
		double beatTime = MusicBeat.BeatTime;
		if (photonView.isMine) {
			if(!started) {
				if (lastBeat > beatTime)
					initialBeatTime -= lastBeat - beatTime + 1.0;
				if((beatTime-initialBeatTime) >= 2) {
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