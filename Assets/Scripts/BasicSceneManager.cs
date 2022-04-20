using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class BasicSceneManager : MonoBehaviour {
    public void loadScene(int index) {
        SceneManager.LoadScene(index);
    }
    private void Update() {
    }
}
