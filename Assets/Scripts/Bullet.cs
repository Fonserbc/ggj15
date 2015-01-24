using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Bullet : Photon.MonoBehaviour
{

	float bulletSpeed = 30.0f;

	void Start()
	{
		BulletColor bcollor = GetComponent<BulletColor>();
		bcollor.SetColor(RhythmMovement.PlayerColor(photonView.owner.ID));
		rigidbody.velocity = transform.forward * 20.0f;

		Destroy(gameObject, 3.0f);
	}
}
