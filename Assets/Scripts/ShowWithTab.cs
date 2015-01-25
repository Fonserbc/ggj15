using UnityEngine;
using System.Collections;

public class ShowWithTab : MonoBehaviour 
{
	public GameObject target;
	public bool forceShow = false;
			
	void OnGUI ()
	{
		if (target != null)
		{
			if (forceShow || Input.GetKey(KeyCode.Tab))
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
