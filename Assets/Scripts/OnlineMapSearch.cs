using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

[System.Serializable]
public class Strings {
    public string[] text;
}
public class OnlineMapSearch : MonoBehaviour {
    string controllerURL = "http://47.97.222.112:8082/api/";
    string[] data;

    public Button upButton, downButton;
    public int buttonIndex = 0;
    int buttonIndexLimit = 0; 

    //playable map buttons will be instantiated from this anchor downwards
    public Transform buttonsInstantiateAnchor;
    Vector3 originalAnchorPosition; 

    public GameObject loadMapButtonPrefab;

    //after search complete remove the buttons inside removalui 
    public Transform removalUI;
    public InputField nameInput;
    public InputField authorInput;

    public void changeScene(int sceneIndex) {
        if (removalUI == null) {
            SceneManager.LoadScene(6);
        } else {
            SceneManager.LoadScene(sceneIndex);
        }
    }
    public void changeButtonIndex (bool up) {
        if (up) {
            if (buttonIndex < buttonIndexLimit)
                buttonIndex++;
        } else {
            if (buttonIndex > 0)
                buttonIndex--;
        }
    }
    public void getDataByName() {
        StartCoroutine(getData(0));
    }
    public void getDataByAuthor() {
        StartCoroutine(getData(1));
    }
    public void getPopularData() {
        StartCoroutine(getData(2));
    }
    IEnumerator getData(int type) {
        string urlString = "";
        if (type == 0)
            urlString = controllerURL + "getData?name=" + nameInput.text + "&getByAuthor=False";
        else if (type == 1)
            urlString = controllerURL + "getData?name=" + authorInput.text + "&getByAuthor=True";
        else
            urlString = controllerURL + "getPopularData?amount=300";

        print(urlString);
        UnityWebRequest www = UnityWebRequest.Get(urlString);
        yield return www.SendWebRequest();
        string downloadedText = www.downloadHandler.text;
        Destroy(removalUI.gameObject);

        Strings s = JsonUtility.FromJson<Strings>(downloadedText);

        buttonIndex = 0;
        buttonIndexLimit = (s.text.Length - 6) / 36; //length of 6 * 6 buttons per page
        if (buttonIndexLimit > 0) {
            upButton.GetComponent<Image>().enabled = true;
            downButton.GetComponent<Image>().enabled = true;
        }

        data = s.text; 

        for (int i = 0; i < s.text.Length; i += 6) {
            GameObject insItem = Instantiate(loadMapButtonPrefab);
            insItem.transform.SetParent(buttonsInstantiateAnchor);
            insItem.transform.position = new Vector3(buttonsInstantiateAnchor.position.x, buttonsInstantiateAnchor.position.y - 30 - 100 * CustomFunctions.getUIScale() - i / 6 % 6 * 80 * CustomFunctions.getUIScale() - i / 36 * 1000 * CustomFunctions.getUIScale(), 0);
            insItem.transform.GetChild(0).GetComponent<Text>().text = s.text[i + 1]; 
            insItem.transform.GetChild(1).GetComponent<Text>().text = "by " + s.text[i + 2];
            insItem.transform.GetChild(2).GetComponent<Text>().text = s.text[i + 5] + " views"; 

            insItem.GetComponent<MapButton>().index = i / 6; 
        }
    }
    IEnumerator playLevel (int index) {
        UnityWebRequest www = UnityWebRequest.Get(controllerURL + "addViewToLevel?name=" + data[index * 6 + 1] + "&author=" + data[index * 6 + 2]); 
        yield return www.SendWebRequest();
        PlayerPrefs.SetInt("map", int.Parse(data[index * 6 + 4]));
        PlayerPrefs.SetString("map" + PlayerPrefs.GetInt("map") + "Data", data[index * 6]);

        //print data for editing only
        print(data[index * 6]);

        PlayerPrefs.SetInt("infiniteLevels", 0);
        SceneManager.LoadScene(2);
    }

    public void goToLevel (int index) {
        StartCoroutine(playLevel(index)); 
    }

    void Start() {
        originalAnchorPosition = buttonsInstantiateAnchor.position; 
        upButton.GetComponent<Image>().enabled = false;
        downButton.GetComponent<Image>().enabled = false; 
    }

    void Update() {
        buttonsInstantiateAnchor.position = new Vector3(originalAnchorPosition.x, originalAnchorPosition.y + buttonIndex * 1000 * CustomFunctions.getUIScale(), 0);
    }
}
