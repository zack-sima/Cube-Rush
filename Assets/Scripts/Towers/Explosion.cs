using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
    public string soundName;
    public float explodeDelay;
    [HideInInspector]
    public float impactRange;
    [HideInInspector]
    public Builder builder;
    public int damage;

    void Start() {
        if (explodeDelay <= 0)
            triggerExplosion();
    }
    void triggerExplosion() {
        builder.audioManager.Play(soundName, 1.8f);
        foreach (Collider i in Physics.OverlapSphere(transform.position, transform.lossyScale.x * impactRange)) {
            if (i.GetComponent<Enemy>() != null) {
                i.GetComponent<Enemy>().takeDamage(damage);
                int extraDamage = 0;
                if (i.GetComponent<Enemy>().health < 0)
                    extraDamage = 0 - i.GetComponent<Enemy>().health;
                i.GetComponent<Enemy>().updateHealth();

                //prevent bug from wiping out player cash 
                if (damage - extraDamage > 0)
                    builder.playerCash += (damage - extraDamage);
            }
        }
    }
    private void Update() {
        if (explodeDelay > 0) {
            explodeDelay -= Time.deltaTime;
            if (explodeDelay <= 0)
                triggerExplosion();
        }
    }
}