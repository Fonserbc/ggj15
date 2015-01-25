﻿using System;
using UnityEngine;

class Bullet : Photon.MonoBehaviour
{
	float bulletSpeed = 25.0f;
	float lifeTime = 3.0f;

	void Start()
	{
		BulletColor bcollor = GetComponent<BulletColor>();
		bcollor.SetColor(RhythmMovement.PlayerColor(photonView.owner.ID));
		rigidbody.velocity = transform.forward * bulletSpeed;
	}

	void Update()
	{
		lifeTime -= Time.deltaTime;
		if (lifeTime < 0.0f)
		{
			if (photonView.isMine)
				PhotonNetwork.Destroy(gameObject);
			else
				enabled = false;
		}
	}
	
	void OnTriggerEnter (Collider col) {
		if (col.gameObject.tag == "Player") {
			if (photonView.isMine) {
				if(col.gameObject.GetComponentInParent<PhotonView>().ownerId != photonView.owner.ID) {
					//hit
					Debug.Log("You hit player " + col.gameObject.GetComponent<PhotonView>().ownerId);
					GameObject.FindGameObjectWithTag("Logic").GetComponent<GameSession>().Frag(col.gameObject.GetComponent<PhotonView>().ownerId);
					
					PhotonNetwork.Destroy(gameObject);
				}
			}
		}
	}
}
