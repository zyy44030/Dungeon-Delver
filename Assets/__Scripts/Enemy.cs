using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected static Vector3[] directions = new Vector3[] {
        Vector3.right,
        Vector3.up,
        Vector3.left,
        Vector3.down
    };

    [Header("Set in Inspector: Enemy")]
    public float MaxHealth = 1;
    public float knockbackSpeed = 10;
    public float knockbackDuration = 0.25f;
    public float invincibleDuration = 0.5f;
    public int damage;
    public bool pushAway;
    [Header("Set Dynamically: Enemy")]
    public float health;
    public bool invincible = false;
    public bool knockback = false;
    private float knockbackDone = 0;
    private float invincibleDone = 0;
    private Vector3 knockbackVel;
    protected Animator anim;
    protected Rigidbody2D rigid;
    protected SpriteRenderer sRend;

    protected virtual void Awake() {
        health = MaxHealth;
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sRend = GetComponent<SpriteRenderer>();
    }
    protected virtual  void Update() {
        if(invincible && Time.timeSinceLevelLoad > invincibleDone) invincible = false;
        sRend.color = invincible ? Color.red : Color.white;
        //受击
        if(knockback){
            rigid.velocity = knockbackVel;
            if(Time.time < knockbackDone) return;
        }
        anim.speed = 1;
        knockback = false;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(invincible) return;
        Sword dmg = other.gameObject.GetComponent<Sword>();
        if(dmg == null) return;

        health -= dmg.damage;
        if(health <= 0) Destroy(gameObject);
        invincible = true;
        invincibleDone = Time.time + invincibleDuration;
        if(dmg.pushAway){
            Vector3 delta = transform.position - other.transform.position;
            if(Mathf.Abs(delta.x) >= Mathf.Abs(delta.y)){
                delta.x = (delta.x > 0) ? 1 : -1;
                delta.y = 0;
            }else{
                delta.x = 0;
                delta.y = (delta.y > 0) ? 1 : -1;
            }
            knockbackVel = delta * knockbackSpeed;
            rigid.velocity = knockbackVel;
            knockback = true;
            knockbackDone = Time.time + knockbackDuration;
        }
    }
}
