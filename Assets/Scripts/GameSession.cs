using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameSession : Photon.MonoBehaviour {
	
	public Text healthUI;
	public Text killsUI;
	public Text deathsUI;
	public Text timeUI;
	public Text statsUI;
	public int gameDurationInBeats = 270;
	
	private static PhotonView ScenePhotonView;
	
	private bool gameFinished = false;
	
	public class PlayerInfo {
		public float health;
		public int kills;
		public int deaths;
		public float lastDead = 0.0f;
		
		public PlayerInfo (float h, int k, int d) {
			health = h;
			kills = k;
			deaths = d;
		}
		
		public void Copy(PlayerInfo other) {
			health = other.health;
			kills = other.kills;
			deaths = other.deaths;
		}
		
		public void Set (float h, int k, int d) {
			health = h;
			kills = k;
			deaths = d;
		}
	};
	
	private float maxHealth = 5f;
	
	private Dictionary<int, PlayerInfo> localFrags = new Dictionary<int,PlayerInfo>();
	
	// Use this for initialization
	void Awake () {
		ScenePhotonView = this.GetComponent<PhotonView>();
	}
	
	public void RestartSession () 
	{
		if (PhotonNetwork.isMasterClient)
		{
			Debug.Log ("Restarting");
			foreach (KeyValuePair<int, PlayerInfo> entry in localFrags)
			{
				entry.Value.Set(maxHealth,0,0);
				ScenePhotonView.RPC("NewPlayerInfo", PhotonTargets.All, entry.Key, maxHealth, 0, 0);
			}
			
			ScenePhotonView.RPC("Restart", PhotonTargets.All);
		}
	}
	
	[RPC]
	public void Restart ()
	{
		gameFinished = false;
		GameObject.Find("Control").GetComponent<PlayerNerworkInstance>().CreatePlayerObject();
	}
	
	[RPC]
	public void FinishGame() {
		gameFinished = true;
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
			ScenePhotonView.RPC("PlayerInfoUpdate", PhotonTargets.All, playerID, false, 0.0f, 0, 0);
		}
	}
	
	[RPC]
	void NewPlayerInfo (int playerID, float health, int kills, int deaths)
	{
		if (!localFrags.ContainsKey(playerID))
		{
			Debug.Log("New player info registered");
			localFrags.Add (playerID, new PlayerInfo(health, kills, deaths));
		}
		else
		{
			localFrags[playerID].Set(health, kills, deaths);
		}
	}
	
	[RPC]
	void PlayerInfoUpdate(int playerID, bool connected, float newHealth, int newKills, int newDeaths)
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
	void NewFrag (int fromPlayer, int toPlayer, float localHealth, float howMuch)
	{
		if (PhotonNetwork.isMasterClient)
		{
			if (localFrags.ContainsKey(fromPlayer) && localFrags.ContainsKey(toPlayer) && localFrags[toPlayer].health <= localHealth && (Time.time - localFrags[toPlayer].lastDead > 0.5f))
			{
				ScenePhotonView.RPC("PlayerInfoUpdate", PhotonTargets.All, fromPlayer, 	true, 	0.0f, 			0, 	0);
				ScenePhotonView.RPC("PlayerInfoUpdate", PhotonTargets.All, toPlayer, 	true, 	-howMuch, 	0, 	0);
				
				if (localFrags[toPlayer].health <= 0) {
					ScenePhotonView.RPC("PlayerInfoUpdate", PhotonTargets.All, 	fromPlayer, true, 	0.0f, 			1,	0);
					ScenePhotonView.RPC("PlayerInfoUpdate", PhotonTargets.All, 	toPlayer, 	true, 	maxHealth, 	0,	1);
					ScenePhotonView.RPC("PlayerDead", PhotonTargets.All, toPlayer);
					localFrags[toPlayer].lastDead = Time.time;
				}
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
			PlayerNerworkInstance netInst = GameObject.Find("Control").GetComponent<PlayerNerworkInstance>();
			ScenePhotonView.RPC("CreateExplosion", PhotonTargets.All,PhotonNetwork.player.ID, netInst.PlayerTransform.position); 
			netInst.Die();
		}
		
		localFrags[deadPlayer].lastDead = Time.time;
	}
	
	[RPC]
	public void CreateExplosion(int id, Vector3 position)
	{
		Color color = RhythmMovement.PlayerColor(id);
		((GameObject) GameObject.Instantiate(Resources.Load("DieParticle"), position, Quaternion.identity)).renderer.material.color = color;
	}
	
	public void Hit (int toPlayer, float howMuch)
	{
		if (gameFinished) return;
		
		if (Time.time - localFrags[toPlayer].lastDead > 0.5f) {
			ScenePhotonView.RPC("NewFrag", PhotonTargets.MasterClient, PhotonNetwork.player.ID, toPlayer, localFrags[toPlayer].health, howMuch);
		}
	}
	
	void OnGUI()
	{
		/*
		string s = "";
		foreach (KeyValuePair<int, PlayerInfo> entry in localFrags)
		{
			
		}
		
		GUI.Label(new Rect(10, 10,200,200), s);
		 */
	}
	
	void Update()
	{
		string s = "";
		foreach (KeyValuePair<int, PlayerInfo> entry in localFrags)
		{
			s += "\n" + (entry.Key == PhotonNetwork.player.ID? ">" : " ") + entry.Key + ": " + entry.Value.kills + " / " + entry.Value.deaths;
			if (entry.Key == PhotonNetwork.player.ID)
			{
				if (timeUI != null) {
					float timer = (float)gameDurationInBeats-Mathf.Floor ((float)MusicBeat.BeatTimeFromBegin);
					timeUI.text = "" + (timer <= gameDurationInBeats ? timer.ToString("#") : "");
					
					if (!gameFinished && PhotonNetwork.isMasterClient && timer <= 0.0f) {
						Debug.Log("--------------Finishing");
						ScenePhotonView.RPC("FinishGame", PhotonTargets.All);
					}
				}
				if (healthUI != null)
					healthUI.text = "" + Mathf.Floor(entry.Value.health*20);
				
				if (killsUI != null)
					killsUI.text = "" + entry.Value.kills;
				
				if (deathsUI != null)
					deathsUI.text = "" + entry.Value.deaths;
			}
		}

		if (statsUI != null || gameFinished)
		{
			statsUI.text = s;
		}
		
		if (gameFinished && PhotonNetwork.isMasterClient && Input.GetButtonDown("Fire1")) {
			RestartSession();
		}
	}
}