using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {

	public enum Gun {
		Pistol = 0,
		Semi,
		Grenade,
		ShotGun
	};
	
	public Gun gun = Gun.Pistol;
	
	float lightTimer = 0.0f;
	private double lastBulletShoot;
	private double lastBeat;
	private float[] gunBeatRate = new float[] {
		1f,0.5f, 2f, 4f
	};

	private string[] gunBullet = new string[] {
		"Bullet","Semi", "Grenade", "ShotGun"
	};

	void Start() {
		lastBulletShoot = MusicBeat.BeatTime;
		lastBeat = MusicBeat.BeatTime;

	}
	
	float fmod(float x, float y) {
		return x - y * Mathf.Floor(x/y);
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Alpha1)) gun = Gun.Pistol;
		if(Input.GetKeyDown(KeyCode.Alpha2)) gun = Gun.Grenade;
		if(Input.GetKeyDown(KeyCode.Alpha3)) gun = Gun.ShotGun;
		if(Input.GetKeyDown(KeyCode.Alpha4)) gun = Gun.Semi;


		double beatTime = MusicBeat.BeatTime;

		if (lightTimer > 0.0f) {
			lightTimer -= Time.deltaTime;
			
			if (lightTimer <= 0.0f) {
				lightTimer = 0.0f;
				light.enabled = false;
			}
		}
		if (lastBeat > beatTime)
						lastBulletShoot -= lastBeat - beatTime + 1.0;
		if (Input.GetButton ("Fire1") && beatTime - lastBulletShoot >= gunBeatRate[(int)gun]) {
			float rate = Mathf.Min (gunBeatRate[(int)gun],1.0f);
			float current = Mathf.Floor((float) beatTime/rate);
			float currentLast = Mathf.Floor((float) lastBeat/rate);

			if (currentLast != current)
				ShootBullet();
		}

		lastBeat = beatTime;
	}
	
	public void ShootBullet()
	{
		GameObject b = PhotonNetwork.Instantiate(gunBullet[(int)gun], transform.position, transform.rotation, 0);
		lastBulletShoot = (float)MusicBeat.BeatTime - fmod ((float)MusicBeat.BeatTime, Mathf.Min (gunBeatRate[(int)gun],1.0f));
		lightTimer = 0.2f;
		light.enabled = true;
	}

	public void ShootExplosion(Vector3 position) {
		GameObject b = PhotonNetwork.Instantiate("Explosion", position, transform.rotation, 0);
	}

	public void ShootShotGun(Vector3 pos) {
		/*
		float[] bulletOffset = new float[] {
			0f,0.2f, -0.2f, 0.4f, -0.4f
		};
		*/
		Vector3 p0 = transform.up*.3f + transform.forward*.3f;
		Vector3 p1 = transform.right*.3f + transform.up*.1f + transform.forward*.3f;
		Vector3 p2 = transform.right*-.3f + transform.up*.1f + transform.forward*.3f;
		Vector3 p3 = transform.right * .4f + transform.up*-.2f + transform.forward*.3f;		
		Vector3 p4 = transform.right * -.4f + transform.up*-.2f + transform.forward*.3f;

		Vector3[] bulletOffset = new Vector3[] {
			p0, p1, p2, p3, p4
		};
		for(int i = 0; i < 5; ++i) {
			//GameObject b = PhotonNetwork.Instantiate("ShotGunBullet", 0.5f*transform.forward+position+transform.right*bulletOffset[i], transform.rotation, 0);
			GameObject b = PhotonNetwork.Instantiate("ShotGunBullet", transform.position + bulletOffset[i], transform.rotation, 0);

		}
	}
}
