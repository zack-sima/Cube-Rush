using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow : Tower {
    public GameObject dartPrefab;
    public Transform dartSpawnpoint;
    public Renderer leftWing, rightWing; 

    public Transform leftString;
    public Transform rightString;
    public Transform bowTop; 

    //[HideInInspector]
    public float shootSpeed;
    public float[] shootSpeedByLevels; 

    //only x angle
    public float stringRotation;

    //this determines whether or not the string is retracting to "fire" a round
    private bool isPulling;

    private Vector3 leftStringOriginalRotation;
    private Vector3 rightStringOriginalRotation;

    //localscale
    private Vector3 leftStringOriginalScale;
    private Vector3 rightStringOriginalScale;

    public override void Start() {
        base.Start();
        stringRotation = -45f; 
        leftStringOriginalRotation = leftString.localEulerAngles;
        rightStringOriginalRotation = rightString.localEulerAngles;
        leftStringOriginalScale = leftString.localScale;
        rightStringOriginalScale = rightString.localScale;
    }
    public override void updateTowerLevels() {
        base.updateTowerLevels(); 
        shootSpeed = shootSpeedByLevels[level - 1];
        if (level == 1) {
            //nothing here
        } else if (level == 2) {
            leftWing.material = builder.greyMaterial;
            rightWing.material = builder.greyMaterial;
        } else if (level == 3) {
            bottom.material = builder.blackMaterial;
        }
    }
    void Update() {
        //parent function
        towerUpdate();

        if (isPulling) {
            stringRotation += Time.deltaTime * shootSpeed;
            if (stringRotation > 0) {
                stringRotation = 0;
                if (isShooting) {
                    isPulling = false;
                    builder.audioManager.Play("Crossbow", 1);
                    GameObject insItem = Instantiate(dartPrefab, dartSpawnpoint.position, dartSpawnpoint.rotation);
                    insItem.transform.Rotate(0, 180, 0);
                    insItem.transform.Translate(Vector3.forward * 0.3f);
                    insItem.GetComponent<CollidingProjectile>().isActive = true;
                    insItem.GetComponent<CollidingProjectile>().parent = this;
                    insItem.GetComponent<CollidingProjectile>().currentTarget = currentTarget;
                    insItem.GetComponent<CollidingProjectile>().health = level + 1;
                    insItem.GetComponent<CollidingProjectile>().maxHealth = level + 1;
                    StartCoroutine(bowRecoilCycle());
                }
            }
        } else {
            stringRotation -= Time.deltaTime * 480;
            if (stringRotation < -45) {
                isPulling = true;
                stringRotation = -45;
            }
        }
        float scale = stringRotation * 17;
        if (scale < 0) {
            scale = -Mathf.Sqrt(-scale);
        }

        leftString.localRotation = Quaternion.Euler(leftStringOriginalRotation.x, leftStringOriginalRotation.y + stringRotation, leftStringOriginalRotation.z);
        rightString.localRotation = Quaternion.Euler(rightStringOriginalRotation.x, rightStringOriginalRotation.y - stringRotation, rightStringOriginalRotation.z);
        leftString.localScale = new Vector3(leftStringOriginalScale.x, leftStringOriginalScale.y, leftStringOriginalScale.z + scale / 1120f);
        rightString.localScale = new Vector3(rightStringOriginalScale.x, rightStringOriginalScale.y, rightStringOriginalScale.z + scale / 1120f);
    }
    IEnumerator bowRecoilCycle () {
        for (float i = 0; i < 0.35f; i += Time.deltaTime) {
            if (i < 0.1f) {
                bowTop.localPosition = Vector3.zero;
                bowTop.transform.Translate(Vector3.forward * i * 0.4f);
            } else {
                bowTop.localPosition = Vector3.zero;
                bowTop.transform.Translate(Vector3.forward * (0.3f - i) * 0.4f);
            }
            yield return null;
        }
        bowTop.localPosition = Vector3.zero; 
        //yield return new WaitForSeconds(0);
    }
}
