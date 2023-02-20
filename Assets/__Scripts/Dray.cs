using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dray : MonoBehaviour
{
    public enum eMode { idle, move, attack, transition }

    [Header("Set in Inspector")]
    public float speed = 5;
    public float attackDuration = 0.25f;
    public float attackDelay = 0.5f;

    [Header("Set Dynamically")]
    public int dirHeld = -1;
    public int facing = 1;
    public eMode mode = eMode.idle;
    private float timeAtkDone = 0;
    private float timeAtkNext = 0;
    private Rigidbody2D rigid;
    private Animator anim;
    private Vector3[] directions =  new Vector3[]{
        Vector3.right,
        Vector3.up,
        Vector3.left,
        Vector3.down
    };

    private Dictionary<int, KeyCode> keys = new Dictionary<int, KeyCode>{
        {0,KeyCode.RightArrow},
        {1,KeyCode.UpArrow},
        {2,KeyCode.LeftArrow},
        {3,KeyCode.DownArrow}
    };

    private void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    
    private void Update() {
        dirHeld = -1;
        //按住移动键
        for (int i = 0; i < 4; i++){
            if(Input.GetKey(keys[i])) dirHeld = i;
        }
        //按住攻击键
        if(Input.GetKeyDown(KeyCode.Z) && Time.time >= timeAtkNext){
            mode = eMode.attack;
            timeAtkDone = Time.time + attackDuration;
            timeAtkNext = Time.time + attackDelay;
        }
        //结束攻击
        if(Time.time >= timeAtkDone){
            mode = eMode.idle;
        }

        //切换模式
        if(mode != eMode.attack){
            if(dirHeld == -1){
                mode = eMode.idle;
            }else{
                facing = dirHeld;
                mode = eMode.move;
            }
        }
        //动画
        
        Vector3 vel = Vector3.zero;
        // if(dirHeld > -1) vel = directions[dirHeld];
        // if(dirHeld == -1){
        //     anim.speed = 0;
        // }else{
        //     anim.CrossFade("Dray_Walk_" + dirHeld, 0);
        //     anim.speed = 1;
        // }
        switch (mode)
        {
            case eMode.attack:
                anim.CrossFade("Dray_Attack_" + facing, 0);
                anim.speed = 0;
                break;
            case eMode.idle:
                anim.CrossFade("Dray_Walk_" + facing, 0);
                anim.speed = 0;
                break;
            case eMode.move:
                vel = directions[dirHeld];
                anim.CrossFade("Dray_Walk_" + facing, 0);
                anim.speed = 1;
                break;
        }
        rigid.velocity = vel * speed;
    }
}
