using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {
    [HideInInspector]
    public Enemy target;
    [HideInInspector]
    public Tower parent; 
    public GameObject explosionPrefab;
    private Vector3 targetPosition;
    public int damage; 

    void Start() {
        targetPosition = target.transform.position; 
        StartCoroutine(missileMovement());
    }

    void Update() {

    }
    IEnumerator missileMovement() { 
        for (float i = 0; i < 1.2f; i += Time.deltaTime) {
            transform.Translate(Vector3.up * 25 * (i + 0.3f) * Time.deltaTime);
            yield return null; 
        }

        transform.Rotate(180, 0, 0);

        if (target != null)
            transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        else {
            transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        }

        for (float i = 0; i < 3; i += Time.deltaTime) {
            if (transform.position.y > 1.3f) {
                if (target != null) {
                    transform.LookAt(target.transform);
                    transform.Rotate(90, 0, 0);
                } else if (parent.currentTarget != null) {
                    transform.LookAt(parent.currentTarget.transform);
                    transform.Rotate(90, 0, 0);
                }
            }
            transform.Translate(Vector3.up * 48 * Time.deltaTime);
            if (transform.position.y <= 0.2f) {
                transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);
                break;
            }
            yield return null;
        }
        GameObject insItem = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        insItem.transform.Rotate(-90, 0, 0);
        insItem.GetComponent<Explosion>().damage = damage;  
        insItem.GetComponent<Explosion>().impactRange = 2.6f;
        insItem.GetComponent<Explosion>().builder = parent.builder;
        Destroy(gameObject);
    }
}
