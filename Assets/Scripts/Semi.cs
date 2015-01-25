using System;
using UnityEngine;

class Semi : Photon.MonoBehaviour
{
	float bulletSpeed = 25.0f;
	float lifeTime = 2f;

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
				PhotonView otherView = col.gameObject.GetComponentInParent<PhotonView>();
				if(otherView.ownerId != photonView.owner.ID) {
					//hit
					Debug.Log("You hit player " + otherView.ownerId);
					GameObject.FindGameObjectWithTag("Logic").GetComponent<GameSession>().Hit(otherView.ownerId, 0.25f);
				}
				PhotonNetwork.Destroy(gameObject);
			}
		}
	}
}
