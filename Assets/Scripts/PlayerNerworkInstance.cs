using UnityEngine;
using System.Collections;

public class PlayerNerworkInstance : MonoBehaviour 
{
	public GameObject beatObjects;
	public GameObject Camera;
	public GameObject[] SpawnPoints;
	
	private GameObject player;
	
	public Transform PlayerTransform
	{
		get
		{
			return player.transform;
		}
	}

	void OnJoinedRoom()
	{
		Debug.Log("Local player ID: " + PhotonNetwork.player.ID);
		CreatePlayerObject();
	}

	void CreatePlayerObject()
	{
		Vector3 position = Vector3.zero;
		Quaternion rotation = Quaternion.identity;
		if (SpawnPoints.Length > 0) {
			int spawnPoint = PhotonNetwork.player.ID % SpawnPoints.Length;
			position = SpawnPoints[spawnPoint].transform.position;
			rotation = SpawnPoints[spawnPoint].transform.rotation;
		}

		player = PhotonNetwork.Instantiate("Cube", position, rotation, 0);

		if (beatObjects != null)
			player.transform.SetParent(beatObjects.transform, false);

		if (Camera != null)
			Camera.transform.SetParent(player.transform, false);
	}
	
	public void Die() {
		
		PhotonNetwork.Instantiate("DieParticle", player.transform.position, player.transform.rotation, 0);
			
		Vector3 position = Vector3.zero;
		Quaternion rotation = Quaternion.identity;
		if (SpawnPoints.Length > 0) {
			int spawnPoint = (PhotonNetwork.player.ID + Random.Range(0, 8)) % SpawnPoints.Length;
			position = SpawnPoints[spawnPoint].transform.position;
			rotation = SpawnPoints[spawnPoint].transform.rotation;
		}
		
		player.transform.position = position;
		player.transform.rotation = rotation;
		
		player.GetComponent<RhythmMovement>().Die();
	}
}
