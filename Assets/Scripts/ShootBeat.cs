using UnityEngine;
using System.Collections;

public class ShootBeat : MonoBehaviour {

	public enum Gun {
		Pistol,
		Semi
	};
	
	public Gun gun = Gun.Pistol;
	
	public GameObject bullet;
	float bulletSpeed = 30.0f;
	float lastShoot = 0.0f;


	bool shoot = false;
	float beatTime = 0f;
	public float myCompassBeat = 1f;


	// Update is called once per frame
	void Update () {
		if (lastShoot > 0.0f) {
			lastShoot -= Time.deltaTime;
			
			if (lastShoot <= 0.0f) {
				lastShoot = 0.0f;
				light.enabled = false;
			}
		}
		if(!shoot) return;
		shoot = false;
		switch (gun) {
		case Gun.Pistol:
			if (Input.GetButton("Fire1")) {
				ShootBulletBeat();
			}
			break;
		default:
			break;
		}
	}
	
	public void ShootBullet() {
		GameObject b = (GameObject) GameObject.Instantiate(bullet, transform.position, transform.rotation);
		b.rigidbody.velocity = transform.forward * bulletSpeed;
		b.GetComponent<BulletColor>().SetColor(Color.red);
		Destroy(b, 3);
		lastShoot = 0.2f;
		light.enabled = true;
	}

	public void ShootBulletBeat() {
		GameObject b = (GameObject) GameObject.Instantiate(bullet, transform.position+transform.forward, transform.rotation);
		b.transform.SetParent(GameObject.Find("/BeatObjects").transform, true);
		b.SendMessage ("setBeat", beatTime);
		b.GetComponent<BulletColor>().SetColor(Color.red);
		Destroy(b, 3);
		lastShoot = 0.2f;
		light.enabled = true;
	}

	public void Beat(Vector2 data) {
		//Debug.Log (data.x + " " + beatTime);
		if(data.x == beatTime) {
			beatTime += 1f/myCompassBeat;
			shoot = true;
		}
	}
}
