using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace EC {
public class StartSequence : MonoBehaviour {
	void Start() {
		SceneManager.LoadScene(1);
	}
}
}