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
	private static double netStartTime = -1.0;
	private static double dspStartTime = -1.0;
	private static int playerWhoIsIt = -1;
	private static double netStartTimeSent = -1.0f;

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

	private GameSession session;

	void Start()
	{
		ScenePhotonView = this.GetComponent<PhotonView>();
		session = this.GetComponent<GameSession>();
	}

	void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (PhotonNetwork.isMasterClient)
		{
			TagPlayer(playerWhoIsIt, netStartTime);
			session.NewPlayerConnected(player.ID);
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
			
			session.PlayerDisconnected(player.ID);
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
					dspStartTime = dspTime - delta;

					bool looped;
					float musicTime = (float)(delta + 1.0);
					CalculateLoop(ref musicTime, out looped, out currentBeat);

					music.time = musicTime;
					music.PlayScheduled(dspTime + 1.0);
				}
			}
		}

		netStartTime = remoteScheduledStartTime;
		Debug.Log("TaggedPlayer: " + playerID);
	}

	private double CalculateBeat(double currentTime, double startTime)
	{
		return ((currentTime - startTime) * beatsPerMinute) / 60.0;
	}

	private void CalculateLoop(ref float musicTime, out bool looped, out double dspBeat)
	{
		looped = false;
		double dspTime = AudioSettings.dspTime;
		do
		{
			dspBeat = CalculateBeat(dspTime, dspStartTime);
			if (dspBeat > lastBeat)
			{
				double beatsToGoBack = lastBeat - loopBeat;
				double timeToGoBack = (60.0 * beatsToGoBack) / 90.0;
				musicTime -= (float)timeToGoBack;
				dspStartTime += timeToGoBack;
				looped = true;
			}
			else
				break;
		}
		while (true);
	}
	
	void Update ()
	{
		double netCurrentTime = PhotonNetwork.time;
		if (netStartTime < 0.0f)
		{
			if (PhotonNetwork.player.ID == playerWhoIsIt &&
				PhotonNetwork.playerList.Length > 1)
			{
				ScenePhotonView.RPC("TaggedPlayer", PhotonTargets.All, playerWhoIsIt, netCurrentTime + 3.0);
			}
		}
		else
		{
			bool looped;
			double dspCurrentBeat;
			float musicTime = music.time;
			CalculateLoop(ref musicTime, out looped, out dspCurrentBeat);
			
			if (looped)
				music.time = musicTime;

			currentBeat = dspCurrentBeat;
			beatObjects.BroadcastMessage("BeatTime", currentBeat, SendMessageOptions.DontRequireReceiver);
		}
	}
}
