using UnityEngine;
using System.Collections;

public class MusicBeat : Photon.MonoBehaviour
{
	public static double beatsPerMinute = 90.0;
	public static double compassNumerator = 4.0;
	public static double compassDenominator = 4.0;

	public AudioSource music;
	public GameObject beatObjects;
	public GameObject beatObjectsSched;

	private static double currentBeat = 0f;
	private static double startTime = -1.0f;
	private static int playerWhoIsIt = -1;

	public static double Time
	{
		get
		{
			return PhotonNetwork.time;
		}
	}

	public static double StartTime
	{
		get
		{
			return startTime;
		}
	}

	public static double BeatTime
	{
		get
		{
			return currentBeat;
		}
	}

	public static double PlayerWhoIsIt
	{
		get
		{
			return playerWhoIsIt;
		}
	}

	private static PhotonView ScenePhotonView;

	void Start()
	{
		ScenePhotonView = this.GetComponent<PhotonView>();
	}

	void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (PhotonNetwork.isMasterClient)
		{
			TagPlayer(playerWhoIsIt, startTime);
		}
	}

	void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerDisconnected: " + player);
		if (PhotonNetwork.isMasterClient)
		{
			if (player.ID == playerWhoIsIt)
			{
				TagPlayer(PhotonNetwork.player.ID, startTime);
			}
		}
	}

	public static void TagPlayer(int playerID, double scheduledStartTime)
	{
		Debug.Log("TagPlayer: " + playerID);
		ScenePhotonView.RPC("TaggedPlayer", PhotonTargets.All, playerID, scheduledStartTime);
	}

	void OnJoinedRoom()
	{
		if (PhotonNetwork.playerList.Length == 1) {
			playerWhoIsIt = PhotonNetwork.player.ID;
		}

		Debug.Log("playerWhoIsIt: " + playerWhoIsIt);
	}

	[RPC]
	void TaggedPlayer(int playerID, double remoteScheduledStartTime)
	{
		playerWhoIsIt = playerID;
		if (startTime != remoteScheduledStartTime)
		{
			if (remoteScheduledStartTime < 0.0f)
			{
				music.Stop();
			}
			else
			{
				double time = Time;
				double dspTime = AudioSettings.dspTime;
				if (time < remoteScheduledStartTime)
				{
					double delta = remoteScheduledStartTime - time;
					music.PlayScheduled(dspTime + delta);
				}
				else
				{
					double delta = time - remoteScheduledStartTime;
					music.time = (float) (delta + 1.0);
					music.PlayScheduled(dspTime + 1.0);
				}
			}
		}

		startTime = remoteScheduledStartTime;
		Debug.Log("TaggedPlayer: " + playerID);
	}
	
	void Update ()
	{
		if (startTime < 0.0f)
		{
			if (PhotonNetwork.player.ID == playerWhoIsIt &&
				Input.GetKeyDown(KeyCode.Space))
				ScenePhotonView.RPC("TaggedPlayer", PhotonTargets.All, playerWhoIsIt, Time + 3.0);
		}
		else
		{
			currentBeat = ((Time - startTime) * beatsPerMinute) / 60.0;
			beatObjects.BroadcastMessage("BeatTime", currentBeat, SendMessageOptions.DontRequireReceiver);
		}
	}
}
