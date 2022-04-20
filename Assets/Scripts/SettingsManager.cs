using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class SettingsManager : MonoBehaviour {
    public Toggle soundToggle; 
    public void loadScene(int index) {
        SceneManager.LoadScene(index);
    }
    private void Start() {
        if (PlayerPrefs.GetInt("useSound") == 0) {
            soundToggle.isOn = false;
        } else
            soundToggle.isOn = true; 
    }

    public void reviewGame() {
        Application.OpenURL("https://itunes.apple.com/us/app/cube-rush-td/id1472763921?mt=8");
    }
    private void Update() {
        if (soundToggle.isOn) {
            PlayerPrefs.SetInt("useSound", 1);
        } else
            PlayerPrefs.SetInt("useSound", 0);
    }
}
