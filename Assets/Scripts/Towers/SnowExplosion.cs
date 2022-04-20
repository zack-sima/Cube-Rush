using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowExplosion : MonoBehaviour {
    public string soundName; 
    [HideInInspector]
    public Builder builder;

    void Start() {
        builder.audioManager.Play(soundName, 3.8f); 
        foreach (Collider i in Physics.OverlapSphere(transform.position, transform.lossyScale.x * 2f)) {
            if (i.GetComponent<Enemy>() != null) {
                //slow enemy effect
                if (i.GetComponent<Enemy>().freezeDelay < 1f)
                    i.GetComponent<Enemy>().freezeDelay = 1f; 
            }
        }
    }
}
