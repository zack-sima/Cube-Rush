﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDeletion : MonoBehaviour {
    public float timer; 

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
		if (timer <= 0) {
			Destroy(gameObject);
			//Resources.UnloadUnusedAssets();
		}
	}
}
