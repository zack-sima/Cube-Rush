using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : Tower {
    public Transform rotatingObject;
    public override void updateTowerLevels() {
        base.updateTowerLevels();
        if (level == 1) {
            //nothing here
        } else if (level == 2) {

        } else if (level == 3) {
            bottom.material = builder.blackMaterial;
        }
    }

    void Update() {
        towerUpdate();
        rotatingObject.Rotate(new Vector3(0, Time.deltaTime * 38, 0));
        foreach (Collider i in Physics.OverlapSphere(transform.position, range - 0.7f)) { 
            if (i.GetComponent<Tower>() != null) {
                i.GetComponent<Tower>().changeCamo(); 
            }
        }
    }
} 