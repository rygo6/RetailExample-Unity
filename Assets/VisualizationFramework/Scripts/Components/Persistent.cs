using UnityEngine;
using System.Collections;


namespace EC
{
	public class Persistent : MonoBehaviour 
	{
		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		static public T Get<T>()
		{
			return Find().GetComponentInChildren<T>();
		}

		static public Persistent Find()
		{
			return FindObjectOfType<Persistent>();
		}
	}
}
