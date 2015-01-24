using UnityEngine;
using System.Collections;

public class BulletDecalScript : MonoBehaviour {

	public GameObject bulletDecal;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision col) {
		Quaternion hitRotation = Quaternion.FromToRotation(Vector3.up, col.contacts[0].normal);
		GameObject b = (GameObject)Instantiate(bulletDecal, col.contacts[0].point, hitRotation);
		b.transform.Rotate (new Vector3(90,0,0));
		Destroy (gameObject);
	}
}
