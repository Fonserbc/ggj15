using System;
using UnityEngine;

class Bullet : Photon.MonoBehaviour
{
	public float bulletSpeed = 30.0f;
	public float lifeTime = 3.0f;

	void Start()
	{
		BulletColor bcollor = GetComponent<BulletColor>();
		bcollor.SetColor(RhythmMovement.PlayerColor(photonView.owner.ID));
		rigidbody.velocity = transform.forward * 20.0f;
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
}
