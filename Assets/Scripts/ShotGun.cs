using System;
using UnityEngine;

class ShotGun : Photon.MonoBehaviour
{
	void Start()
	{
		if (photonView.isMine) {
			Camera.main.gameObject.SendMessage ("ShootShotGun", transform.position);
			PhotonNetwork.Destroy(gameObject);
		} else enabled = false;
	}
}
