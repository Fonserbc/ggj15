using System;
using UnityEngine;

class ShotGunBullet : Photon.MonoBehaviour
{
	float bulletSpeed = 25.0f;
	double initialBeatTime;
	bool started = false;
	float lifeTime = 3.0f;
	private double lastBeat;
	private Vector3 offset;

	void Start()
	{
		BulletColor bcollor = GetComponent<BulletColor>();
		bcollor.SetColor(RhythmMovement.PlayerColor(photonView.owner.ID));


		initialBeatTime = MusicBeat.BeatTime;
		lastBeat = MusicBeat.BeatTime;
		offset = transform.position - Camera.main.transform.position;
		offset = Camera.main.transform.InverseTransformDirection (offset);

	}

	void Update()
	{
		double beatTime = MusicBeat.BeatTime;
		if (photonView.isMine) {
			if(!started) {
				transform.rotation = Camera.main.transform.rotation;
				transform.position = Camera.main.transform.position + Camera.main.transform.TransformDirection(offset);
				if (lastBeat > beatTime)
					initialBeatTime -= lastBeat - beatTime + 1.0;
				if((beatTime-initialBeatTime) >= 2) {
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
	
	void OnTriggerEnter (Collider col) {
		if (col.gameObject.tag == "Player") {
			if (photonView.isMine) {
				PhotonView otherView = col.gameObject.GetComponentInParent<PhotonView>();
				if(otherView.ownerId != photonView.owner.ID) {
					//hit
					Debug.Log("You hit player " + otherView.ownerId);
					GameObject.FindGameObjectWithTag("Logic").GetComponent<GameSession>().Hit(otherView.ownerId,1f);
				}
				PhotonNetwork.Destroy(gameObject);
			}
		}
	}
}
