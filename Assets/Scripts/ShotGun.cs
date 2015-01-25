using System;
using UnityEngine;

class ShotGun : Photon.MonoBehaviour
{
	void Start()
	{
		Camera.main.gameObject.SendMessage("ShootShotGun", transform.position);
		PhotonNetwork.Destroy(gameObject);
	}
}
