#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace EC {
[InitializeOnLoad]
public class PlayStateChanged {

	static PlayStateChanged() {
		EditorApplication.playmodeStateChanged += InstantiateAndCleanupPersistentGameObject;
	}

	static void InstantiateAndCleanupPersistentGameObject() {
		const string persistentName = "Persistent";
		const string persistentCloneName = "Persistent(Clone)";
		GameObject persistentGameObject = GameObject.Find(persistentName);
		GameObject persistentCloneGameObject = GameObject.Find(persistentCloneName);
		if (EditorApplication.isPlayingOrWillChangePlaymode && persistentGameObject == null && persistentCloneGameObject == null) {
			GameObject prefab = (GameObject)Resources.Load(persistentName);
			GameObject instance = GameObject.Instantiate(prefab);
		} else if (!EditorApplication.isPlayingOrWillChangePlaymode && persistentCloneGameObject != null) {
			MonoBehaviour.DestroyImmediate(persistentCloneGameObject);
		}
	}
}
}
#endif