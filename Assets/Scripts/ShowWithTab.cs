using UnityEngine;
using System.Collections;

public class ShowWithTab : MonoBehaviour 
{
	public GameObject target;
	
	void OnGUI ()
	{
		if (target != null)
		{
			if (Input.GetKey(KeyCode.Tab))
			{
				if (!target.activeSelf)
					target.SetActive(true);
			}
			else
			{
				if (target.activeSelf)
					target.SetActive(false);
			}
		}
	}
}
