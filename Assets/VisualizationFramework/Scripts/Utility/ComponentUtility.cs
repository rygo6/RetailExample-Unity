using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace EC {
static public class ComponentUtility {

	static public T FindOnNamedPersistentGameObject<T>() where T : Component {
		string typeName = typeof(T).ToString();
		GameObject gameObject = GameObject.Find("Persistent/" + typeName);
		if (gameObject == null) {
			Debug.LogError("Cannot find GameObject named: " + typeName);
		}
		return gameObject.GetComponent<T>();
	}

	static public T FindOnNamedGameObject<T>() where T : Component {
		string typeName = typeof(T).ToString();
		GameObject gameObject = GameObject.Find(typeName);
		if (gameObject == null) {
			Debug.LogError("Cannot find GameObject named: " + typeName);
		}
		return gameObject.GetComponent<T>();
	}
}
}