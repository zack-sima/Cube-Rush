using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteToggle : MonoBehaviour {
    MapEditor mapEditor; 
    public bool isOn;
    public Sprite onSprite;
    public Sprite offSprite;

    public void Start() {
        mapEditor = GameObject.Find("EventSystem").GetComponent<MapEditor>(); 
    }
    public void Update() {
        if (isOn) {
            GetComponent<Image>().sprite = onSprite;
            GetComponent<Image>().color = Color.green;
        } else {
            GetComponent<Image>().sprite = offSprite;
            GetComponent<Image>().color = new Color(1f, 0.28f, 0.28f); 
        }
    }
    public void activateButton() {
        isOn = !isOn;
        if (!mapEditor.checkTowerToggles()) {
            isOn = true; 
        } 

    }
}
