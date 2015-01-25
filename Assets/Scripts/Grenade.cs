using System;
using UnityEngine;

class Grenade : Photon.MonoBehaviour
{
	float bulletSpeed = 25.0f;
	double initialBeatTime;
	private double lastBeat;

	void Start()
	{
		BulletColor bcollor = GetComponent<BulletColor>();
		bcollor.SetColor(RhythmMovement.PlayerColor(photonView.owner.ID));
		rigidbody.velocity = transform.forward * bulletSpeed;
		initialBeatTime = MusicBeat.BeatTime;
		lastBeat = MusicBeat.BeatTime;
	}

	void Update()
	{
		double beatTime = MusicBeat.BeatTime;
		if (photonView.isMine) {
			if (lastBeat > beatTime)
				initialBeatTime -= lastBeat - beatTime + 1.0;
			if((beatTime-initialBeatTime) >= 2) {
				Camera.main.gameObject.SendMessage("ShootExplosion", transform.position);
				PhotonNetwork.Destroy(gameObject);
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
					GameObject.FindGameObjectWithTag("Logic").GetComponent<GameSession>().Hit(otherView.ownerId,0.3f);
				}
			}
		}
	}
}
