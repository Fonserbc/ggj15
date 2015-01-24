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

	public double lastBeat = 188;
	public double loopBeat = 16;

	private static double currentBeat = 0.0;
	private static double loopBeatOffset = 0.0;
	private static double netStartTime = -1.0;
	private static double dspStartTime = -1.0;
	private static int playerWhoIsIt = -1;

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
			TagPlayer(playerWhoIsIt, netStartTime);
		}
	}

	void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerDisconnected: " + player);
		if (PhotonNetwork.isMasterClient)
		{
			if (player.ID == playerWhoIsIt)
			{
				TagPlayer(PhotonNetwork.player.ID, netStartTime);
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
		if (netStartTime != remoteScheduledStartTime)
		{
			if (remoteScheduledStartTime < 0.0f)
			{
				music.Stop();
			}
			else
			{
				double netTime = PhotonNetwork.time;
				double dspTime = AudioSettings.dspTime;
				if (netTime < remoteScheduledStartTime)
				{
					double delta = remoteScheduledStartTime - netTime;
					dspStartTime = dspTime + delta;
					music.PlayScheduled(dspTime + delta);
				}
				else
				{
					double delta = netTime - remoteScheduledStartTime;
					dspStartTime = dspTime + delta;
					music.time = (float) (delta + 1.0);
					music.PlayScheduled(dspTime + 1.0);
				}
			}
		}

		netStartTime = remoteScheduledStartTime;
		Debug.Log("TaggedPlayer: " + playerID);
	}
	
	void Update ()
	{
		double netCurrentTime = PhotonNetwork.time;
		if (netStartTime < 0.0f)
		{
			if (PhotonNetwork.player.ID == playerWhoIsIt &&
				Input.GetKeyDown(KeyCode.Space))
				ScenePhotonView.RPC("TaggedPlayer", PhotonTargets.All, playerWhoIsIt, netCurrentTime + 3.0);
		}
		else
		{
			double dstCurrentTime = AudioSettings.dspTime;
			double dspCurrentBeat = (((dstCurrentTime - dspStartTime) * beatsPerMinute) / 60.0) - loopBeatOffset;
			if (dspCurrentBeat > lastBeat)
			{
				double beatsToGoBack = lastBeat - loopBeat;
				double timeToGoBack = (60.0 * beatsToGoBack) / 90.0;
				music.time -= (float) timeToGoBack;
				loopBeatOffset += beatsToGoBack;
			}

			currentBeat = dspCurrentBeat;
			beatObjects.BroadcastMessage("BeatTime", currentBeat, SendMessageOptions.DontRequireReceiver);
		}
	}
}
