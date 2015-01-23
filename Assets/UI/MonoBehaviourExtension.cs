using UnityEngine;
using System;
using System.Collections.Generic;

public static class MonoBehaviourExtension
{
	public static T AddOrGetComponent<T> (this GameObject gameObject) where T : Component
	{
		T comp = gameObject.GetComponent<T>();
		if(comp == null)
			comp = gameObject.AddComponent<T>();
		
		return comp;
	}
	
	public static T FindManagerOrCreateIt<T> (this GameObject gameObject) where T : Component
	{
		return gameObject.FindObjectOrCreateIt<T>(typeof(T).FullName);
	}
	
	public static T FindObjectOrCreateIt<T> (this GameObject gameObject, string nameIfNotFound) where T : Component
	{
		T res = GameObject.FindObjectOfType<T>();
		if(res==null)
		{
			GameObject go = new GameObject(nameIfNotFound);
			res = go.AddComponent<T>();
		}
		return res;
	}
		
	public static T[] FindObjectsOfType<T>(this GameObject gameObject)
	{
		T[] res = GameObject.FindObjectsOfType(typeof(T)) as T[];
		return res;
	}
	
}

