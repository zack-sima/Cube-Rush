using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking; 

public class MapPublisher : MonoBehaviour {
    string controllerURL = "http://47.97.222.112:8082/api/";
    bool uploading = false; 

    Vector3 originalBackgroundPosition; 
    public Transform uploadSuccessBackground;
    public PlayerData playerData; 
    public Text errorText;
    public Text uploadDataText; 
    public InputField nameInput;
    public InputField authorInput; 
    public int mapIndex;
    public Transform mapParent;

    public void changeScene (int sceneIndex) {
        if (!uploading) {
            PlayerPrefs.SetInt("map", mapIndex);
            SceneManager.LoadScene(sceneIndex);
        }
    }
    public void uploadDataCall () {
        //check if player bought publishing
        if (!playerData.publishingUnlocked) {
            changeScene(11); 
        }
        if (!uploading && nameInput.text != "" && authorInput.text != "") {
            errorText.enabled = false;
            uploadDataText.text = "Uploading...";
            uploading = true;
            StartCoroutine(uploadData());
        }
    }
    IEnumerator uploadData() {
        WWWForm dataForm = new WWWForm();
        dataForm.AddField("data", PlayerPrefs.GetString("map" + PlayerPrefs.GetInt("level") + "Json"));
        dataForm.AddField("name", nameInput.text);
        dataForm.AddField("author", authorInput.text);
        dataForm.AddField("map", mapIndex); 

        string urlString = controllerURL + "postData";
        UnityWebRequest www = UnityWebRequest.Post(urlString, dataForm);
        yield return www.SendWebRequest(); 
        if (www.downloadHandler.text == "false") {
            errorText.enabled = true; 
        } else if (www.downloadHandler.text == "true") {
            errorText.enabled = false;
            uploadSuccessBackground.position = originalBackgroundPosition; 
        }
        uploadDataText.text = "Upload Data";
        uploading = false; 
    }
    void updateMap () {
        for (int i = 0; i < mapParent.childCount; i++) {
            if (i == mapIndex) {
                foreach (Renderer j in mapParent.GetChild(i).GetChild(0).GetComponentsInChildren<Renderer>()) {
                    if (j != null)
                        j.enabled = true; 
                }
            } else {
                foreach (Renderer j in mapParent.GetChild(i).GetChild(0).GetComponentsInChildren<Renderer>()) {
                    if (j != null)
                        j.enabled = false;
                }
            }
        }
    }
    public void changeMap(bool addIndex) {
        if (addIndex) {
            if (mapIndex < mapParent.childCount - 1)
                mapIndex++;
        } else {
            if (mapIndex > 0)
                mapIndex--;
        }
        updateMap(); 
    }

    void Start() {
        originalBackgroundPosition = uploadSuccessBackground.position;
        uploadSuccessBackground.position = new Vector3(0f, 8000f, 0f); 
        errorText.enabled = false; 
        updateMap();
        if (!playerData.publishingUnlocked) {
            uploadDataText.text = "Locked (buy in shop)"; 
        }
    }

    void Update() {

    }
}
