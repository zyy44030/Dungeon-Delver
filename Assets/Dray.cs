using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dray : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float speed = 5;

    [Header("Set Dynamically")]
    public int dirHeld = -1;
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
        for (int i = 0; i < 4; i++){
            if(Input.GetKey(keys[i])) dirHeld = i;
        }
        Vector3 vel = Vector3.zero;
        if(dirHeld > -1) vel = directions[dirHeld];
        rigid.velocity = vel * speed;
        //动画
        if(dirHeld == -1){
            anim.speed = 0;
        }else{
            anim.CrossFade("Dray_Walk_" + dirHeld, 0);
            anim.speed = 1;
        }
    }
}
