using UnityEngine;
using System.Collections;

namespace EC {
public class Persistent : MonoBehaviour {

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}
}
}