using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering; 

public static class CustomFunctions {
    public enum RenderingMode {
        Opaque,
        Cutout,
        Fade,
        Transparent,
    }
    //this portion of code is from https://blog.csdn.net/u014805066/article/details/52619508
    public static void SetMaterialRenderingMode(Material material, RenderingMode renderingMode) {
        switch (renderingMode) {
        case RenderingMode.Opaque:
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = -1;
            break;
        case RenderingMode.Cutout:
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.EnableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 2450;
            break;
        case RenderingMode.Fade:
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
            break;
        case RenderingMode.Transparent:
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
            break;
        }
    }
    //online code ends here

    public static float getUIScale () {
        //change this inside script to change size of UI 
        return Screen.width / 1350f; 
    }
    public static bool isMobile = true;
    public static void CopyToClipboard(string s) {
        TextEditor te = new TextEditor();
        te.text = s;
        te.SelectAll();
        te.Copy();
    }
}
public class UIAlignment : MonoBehaviour {
    //1 is default text
    [HideInInspector]
    public float scale; 
    public bool dontChangeRect;
    public bool dontChangePosition;
    public bool onlyYAxis;
    public bool onlyXAxis;

    void Awake () {
        //align scale and anchored position to fit device measurements
        scale = CustomFunctions.getUIScale(); 

        if (GetComponent<Text> () != null) {
            GetComponent<Text> ().fontSize = (int)(GetComponent<Text> ().fontSize * scale);
        }
        if (!dontChangeRect) {
            if (onlyYAxis) {
                GetComponent<RectTransform> ().anchoredPosition = new Vector2 (GetComponent<RectTransform> ().anchoredPosition.x, GetComponent<RectTransform> ().anchoredPosition.y * scale);
                GetComponent<RectTransform> ().sizeDelta = new Vector2 (GetComponent<RectTransform> ().rect.width, GetComponent<RectTransform> ().rect.height * scale);
            } else if (onlyXAxis) {
                GetComponent<RectTransform> ().anchoredPosition = new Vector2 (GetComponent<RectTransform> ().anchoredPosition.x * scale, GetComponent<RectTransform> ().anchoredPosition.y);
                GetComponent<RectTransform> ().sizeDelta = new Vector2 (GetComponent<RectTransform> ().rect.width * scale, GetComponent<RectTransform> ().rect.height);
            } else {
                if (!dontChangePosition)
                    GetComponent<RectTransform> ().anchoredPosition = new Vector2 (GetComponent<RectTransform> ().anchoredPosition.x * scale, GetComponent<RectTransform> ().anchoredPosition.y * scale);
                GetComponent<RectTransform> ().sizeDelta = new Vector2 (GetComponent<RectTransform> ().rect.width * scale, GetComponent<RectTransform> ().rect.height * scale);
            }
        }
    }
}


