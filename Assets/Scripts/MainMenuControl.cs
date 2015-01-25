using UnityEngine;
using System.Collections;

public class MainMenuControl : MonoBehaviour 
{
	public GameObject MainMenu;
	public GameObject InGame;

	public MouseLook look;
	public WaapConnectAndJoinRandom connect;

	void Start () {
	
	}
	
	void Update () {
	
	}

	public void OnPlayPressed()
	{
		MainMenu.SetActive(false);
		InGame.SetActive(true);
		look.enabled = true;
		connect.AutoConnect = true;
	}
}
