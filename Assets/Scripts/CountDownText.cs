using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountDownText : MonoBehaviour
{
	public Text child;

	RectTransform rect;
	Text text;
	float quake = 5.0f;

	void SetText(string str)
	{
		if (text != null)
			text.text = str;

		if (child != null)
			child.text = str;
	}

	void Start ()
	{
		rect = gameObject.GetComponent<RectTransform>();
		text = gameObject.GetComponent<Text>();
	}
	
	void Update ()
	{
		if (PhotonNetwork.connectionStateDetailed == PeerState.Joined)
		{
			if (PhotonNetwork.playerList.Length < 2)
			{
				SetText("Waiting for players\n" + GetConnectingDots());
			}
			else
			{
				if (MusicBeat.BeatTime < 1.0f)
				{
					quake = 5.0f * Mathf.Max(0.0f, 5.0f - (1.0f - (float)MusicBeat.BeatTime));

					if (MusicBeat.BeatTime < -3.0f)
					{
						SetText("");
					}
					else if (MusicBeat.BeatTime < 0.0f)
					{
						SetText("" + Mathf.Abs(Mathf.Floor((float)MusicBeat.BeatTime)));
					}
					else
					{
						SetText("GO!!!");
					}

					Vector2 anchoredPosition = new Vector2(
						UnityEngine.Random.Range(-quake, quake),
						UnityEngine.Random.Range(-quake, quake));

					rect.anchoredPosition = anchoredPosition;
				}
				else
				{
					SetText("");
				}
			}
		}
		else
		{
			SetText("Connecting" + GetConnectingDots() + "\nStatus: " + PhotonNetwork.connectionStateDetailed);
		}
	}

	string GetConnectingDots()
	{
		string str = "";
		int numberOfDots = Mathf.FloorToInt(Time.timeSinceLevelLoad * 3f % 4);

		for (int i = 0; i < numberOfDots; ++i)
		{
			str += " .";
		}

		return str;
	}
}
