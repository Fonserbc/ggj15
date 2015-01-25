using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSession : Photon.MonoBehaviour {

	
	private static PhotonView ScenePhotonView;
	
	public class PlayerInfo {
		public int kills;
		public int deaths;
		
		public PlayerInfo (int k, int d) {
			kills = k;
			deaths = d;
		}
	};
	
	private Dictionary<int, PlayerInfo> localFrags;

	// Use this for initialization
	void Awake () {
		ScenePhotonView = this.GetComponent<PhotonView>();
		
		localFrags = new Dictionary<int, PlayerInfo>();
	}
	
	//Only should be called called when being master client
	public void NewPlayerConnected (int playerID)
	{
		Debug.Log("New player connected to host "+playerID);
		if (PhotonNetwork.isMasterClient) {
			foreach (KeyValuePair<int, PlayerInfo> entry in localFrags) {
				ScenePhotonView.RPC("NewPlayerInfo", PhotonTargets.Others, entry.Key, entry.Value.kills, entry.Value.deaths);
			}
		
			ScenePhotonView.RPC("NewPlayerInfo", PhotonTargets.All, playerID, 0, 0);
		}
	}
	
	//Only should be called called when being master client
	public void PlayerDisconnected (int playerID)
	{
		Debug.Log("New player disconnected to host "+playerID);
		if (PhotonNetwork.isMasterClient) {			
			ScenePhotonView.RPC("PlayerInfoUpdate", PhotonTargets.All, playerID, false, 0, 0);
		}
	}
	
	[RPC]
	void NewPlayerInfo (int playerID, int kills, int deaths)
	{
		if (!localFrags.ContainsKey(playerID))
		{
			Debug.Log("New player info registered");
			localFrags.Add (playerID, new PlayerInfo(kills, deaths));
		}
	}
	
	[RPC]
	void PlayerInfoUpdate (int playerID, bool connected, int newKills, int newDeaths)
	{
		if (!connected) // Disconnect
		{
			Debug.Log("Player Disconnected "+playerID);
			localFrags.Remove(playerID);
		}
		else
		{
			Debug.Log("Player Update "+playerID);
			if (localFrags.ContainsKey(playerID))
			{
				localFrags[playerID].kills += newKills;
				localFrags[playerID].deaths += newDeaths;
			}
			else
			{
				Debug.Log("Hwat");
			}
		}
	}
	
	[RPC]
	void NewFrag (int fromPlayer, int toPlayer)
	{
		if (PhotonNetwork.isMasterClient)
		{
			if (localFrags.ContainsKey(fromPlayer) && localFrags.ContainsKey(toPlayer))
			{
				ScenePhotonView.RPC("PlayerInfoUpdate", PhotonTargets.All, fromPlayer, true, 1, 0);
				ScenePhotonView.RPC("PlayerInfoUpdate", PhotonTargets.All, toPlayer, true, 0, 1);
			}
			else
			{
				Debug.Log("Twaht");
			}
		}
	}
	
	public void Frag (int toPlayer)
	{
		ScenePhotonView.RPC("NewFrag", PhotonTargets.MasterClient, PhotonNetwork.player.ID, toPlayer);
	}
	
	void OnGUI() {
		int i = 0;
		foreach (KeyValuePair<int, PlayerInfo> entry in localFrags) {
			GUI.Label(new Rect(10 + (entry.Key == PhotonNetwork.player.ID? 10 : 0),0 + 100*i,200,100), entry.Key + ": " + entry.Value.kills + " / " + entry.Value.deaths);
			++i;
		}
	}
}
