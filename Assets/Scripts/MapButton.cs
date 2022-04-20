using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapButton : MonoBehaviour {
    public int index; 
	public void loadLevel () {
        GameObject.Find("EventSystem").GetComponent<OnlineMapSearch>().goToLevel(index); 
    }
}
