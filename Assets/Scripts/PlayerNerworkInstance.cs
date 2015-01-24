using UnityEngine;
using System.Collections;

public class PlayerNerworkInstance : MonoBehaviour 
{
	public GameObject beatObjects;
	public GameObject Camera;
	public GameObject[] SpawnPoints;

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

		GameObject newPlayerObject = PhotonNetwork.Instantiate("Cube", position, rotation, 0);

		if (beatObjects != null)
			newPlayerObject.transform.SetParent(beatObjects.transform, false);

		if (Camera != null)
			Camera.transform.SetParent(newPlayerObject.transform, false);
	}
}
