using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSession : Photon.MonoBehaviour {

	
	private static PhotonView ScenePhotonView;
	
	public class PlayerInfo {
		public int health;
		public int kills;
		public int deaths;
		
		public PlayerInfo (int h, int k, int d) {
			health = h;
			kills = k;
			deaths = d;
		}
	};
	
	private int maxHealth = 5;
	
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
		if (PhotonNetwork.isMasterClient)
		{
			if (!localFrags.ContainsKey(PhotonNetwork.player.ID)) // Lel
			{
				ScenePhotonView.RPC("NewPlayerInfo", PhotonTargets.All, PhotonNetwork.player.ID, maxHealth, 0, 0);
			}
		
			foreach (KeyValuePair<int, PlayerInfo> entry in localFrags) {
				ScenePhotonView.RPC("NewPlayerInfo", PhotonTargets.Others, entry.Key, entry.Value.health, entry.Value.kills, entry.Value.deaths);
			}
		
			ScenePhotonView.RPC("NewPlayerInfo", PhotonTargets.All, playerID, maxHealth, 0, 0);
		}
	}
	
	//Only should be called called when being master client
	public void PlayerDisconnected (int playerID)
	{
		Debug.Log("New player disconnected to host "+playerID);
		if (PhotonNetwork.isMasterClient) {			
			ScenePhotonView.RPC("PlayerInfoUpdate", PhotonTargets.All, playerID, false, 0, 0, 0);
		}
	}
	
	[RPC]
	void NewPlayerInfo (int playerID, int health, int kills, int deaths)
	{
		if (!localFrags.ContainsKey(playerID))
		{
			Debug.Log("New player info registered");
			localFrags.Add (playerID, new PlayerInfo(health, kills, deaths));
		}
	}
	
	[RPC]
	void PlayerInfoUpdate (int playerID, bool connected, int newHealth, int newKills, int newDeaths)
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
				localFrags[playerID].health += newHealth;
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
				ScenePhotonView.RPC("PlayerInfoUpdate", PhotonTargets.All, fromPlayer, true, 0, 1, 0);
				ScenePhotonView.RPC("PlayerInfoUpdate", PhotonTargets.All, toPlayer, true, -1, 0, 1);
			}
			else
			{
				Debug.Log("Twaht");
			}
		}
	}
	
	[RPC]
	void PlayerDead (int deadPlayer)
	{
		// Explosion
		
		if (deadPlayer == PhotonNetwork.player.ID)
		{
			GameObject.Find("Control").GetComponent<PlayerNerworkInstance>().Die();
		}
	}
	
	public void Hit (int toPlayer)
	{
		ScenePhotonView.RPC("NewFrag", PhotonTargets.MasterClient, PhotonNetwork.player.ID, toPlayer);
	}
	
	void OnGUI() {
		string s = "";
		foreach (KeyValuePair<int, PlayerInfo> entry in localFrags) {
			s += "\n" + (entry.Key == PhotonNetwork.player.ID? ">" : " ") + entry.Key + ": " + entry.Value.kills + " / " + entry.Value.deaths;
		}
		
		GUI.Label(new Rect(10, 10,200,200), s);
	}
}
