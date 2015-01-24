using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {

	public enum Gun {
		Pistol,
		Semi
	};
	
	public Gun gun = Gun.Pistol;
	
	public GameObject bullet;
	float lastShoot = 0.0f;
	private double lastBeat;

	
	// Update is called once per frame
	void Update () {
		double beatTime = MusicBeat.BeatTime;
		float currentBeatFloor = Mathf.Floor((float) beatTime);

		if (lastShoot > 0.0f) {
			lastShoot -= Time.deltaTime;
			
			if (lastShoot <= 0.0f) {
				lastShoot = 0.0f;
				light.enabled = false;
			}
		}

		if (Mathf.Floor((float)lastBeat) != currentBeatFloor)
		{
			switch (gun) {
			case Gun.Pistol:
				if (Input.GetButton("Fire1")) {
					ShootBullet();
				}
				break;
			default:
				break;
			}
		}

		lastBeat = beatTime;

	}
	
	public void ShootBullet()
	{
		GameObject b = PhotonNetwork.Instantiate("Bullet", transform.position, transform.rotation, 0);
		
		lastShoot = 0.2f;
		light.enabled = true;
	}
}
