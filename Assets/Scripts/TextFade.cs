using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFade : MonoBehaviour {
    Text myText;
    float opacity;
    public float fadeSpeed;
    public void activateText(string text) {
        myText.text = text;
        opacity = fadeSpeed + 1;
    }
    void Start() {
        myText = GetComponent<Text>();
    }

    void Update() {
        if (opacity > 0) {
            myText.enabled = true;
            opacity -= Time.deltaTime;
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, opacity / fadeSpeed);
            Outline outline = GetComponent<Outline>();
            outline.effectColor = new Color(outline.effectColor.r, outline.effectColor.g, outline.effectColor.b, opacity / fadeSpeed);
        } else
            myText.enabled = false;
    }
}
