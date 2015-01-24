using UnityEngine;
using System.Collections;

public class PlayerNerworkInstance : MonoBehaviour 
{
	public GameObject beatObjects;
	public GameObject Camera;

	void OnJoinedRoom()
	{
		CreatePlayerObject();
	}

	void CreatePlayerObject()
	{
		Vector3 position = new Vector3(0.0f, 0.5f, 0.0f);
		GameObject newPlayerObject = PhotonNetwork.Instantiate("Cube", position, Quaternion.identity, 0);

		if (beatObjects != null)
			newPlayerObject.transform.SetParent(beatObjects.transform, false);

		if (Camera != null)
			Camera.transform.SetParent(newPlayerObject.transform, false);
	}
}
