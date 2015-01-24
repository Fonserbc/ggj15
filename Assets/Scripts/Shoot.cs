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
	
	// Update is called once per frame
	void Update () {
		if (lastShoot > 0.0f) {
			lastShoot -= Time.deltaTime;
			
			if (lastShoot <= 0.0f) {
				lastShoot = 0.0f;
				light.enabled = false;
			}
		}
		switch (gun) {
		case Gun.Pistol:
			if (Input.GetButtonDown("Fire1")) {
				ShootBullet();
			}
			break;
		default:
			break;
		}
	}
	
	public void ShootBullet()
	{
		GameObject b = PhotonNetwork.Instantiate("Bullet", transform.position, transform.rotation, 0);
		
		lastShoot = 0.2f;
		light.enabled = true;
	}
}
