﻿using UnityEngine;
using System.Collections;

public class PlayerNerworkInstance : MonoBehaviour 
{
	public GameObject beatObjects;
	public GameObject Camera;

	private int lastSpawn = 0;
	public GameObject[] SpawnPoints;

	void OnJoinedRoom()
	{
		CreatePlayerObject();
	}

	void CreatePlayerObject()
	{
		Vector3 position = Vector3.zero;
		if (SpawnPoints.Length > 0) {
			int spawnPoint = Random.Range(0, SpawnPoints.Length - 1);
			position = SpawnPoints[spawnPoint].transform.position;
		}

		GameObject newPlayerObject = PhotonNetwork.Instantiate("Cube", position, Quaternion.identity, 0);

		if (beatObjects != null)
			newPlayerObject.transform.SetParent(beatObjects.transform, false);

		if (Camera != null)
			Camera.transform.SetParent(newPlayerObject.transform, false);
	}
}
