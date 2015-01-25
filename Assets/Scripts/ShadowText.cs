using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShadowText : MonoBehaviour
{
	public Text parent;
	Text text;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		if (parent != null && text != null)
			text.text = parent.text;
	}
}
