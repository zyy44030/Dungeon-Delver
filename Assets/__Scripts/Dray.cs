using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dray : MonoBehaviour, IFacingMover
{
    public enum eMode { idle, move, attack, transition, knockback }

    [Header("Set in Inspector")]
    public float speed = 5;
    public float attackDuration = 0.25f;
    public float attackDelay = 0.5f;
    public float transitionDelay = 0.5f;
    public int maxHealth = 10;
    public float knockbackSpeed = 10;
    public float knockbackDuration = 0.25f;
    public float invincibleDuration = 0.5f;

    [Header("Set Dynamically")]
    public bool invincible = false;
    public int dirHeld = -1;
    public int facing = 0;
    public eMode mode = eMode.idle;
    public int numKeys = 0;
    private float timeAtkDone = 0;
    private float timeAtkNext = 0;
    private Rigidbody2D rigid;
    private Animator anim;
    private InRoom InRm;
    private float transitionDone = 0;
    private Vector2 transitionPos;
    private float knockbackDone = 0;
    private float invincibleDone = 0;
    private Vector3 knockbackVel;
    private SpriteRenderer sRender;

    [SerializeField]
    private int _health;

    public int health{
        get { return _health;}
        set { _health = value;}
    }

    public int keyCount{
        get { return numKeys; }
        set { numKeys = value; }
    }

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
        sRender = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        InRm = GetComponent<InRoom>();
        health = maxHealth;
    }
    
    private void Update() {
        //无敌状态
        if(invincible && Time.timeSinceLevelLoad > invincibleDone) invincible = false;
        sRender.color = invincible ? Color.red : Color.white;
        //受击
        if(mode == eMode.knockback){
            rigid.velocity = knockbackVel;
            if(Time.time < knockbackDone) return;
        }
        //场景转换
        if(mode == eMode.transition){
            rigid.velocity = Vector2.zero;
            anim.speed = 0;
            roomPos = transitionPos;
            if(Time.time < transitionDone) return;
            mode = eMode.idle;
        }
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
        //动画，移动
        Vector3 vel = Vector3.zero;
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

    private void LateUpdate() {
        int doorNum;
        if (roomPos.x >= InRoom.DOORS[0].x && roomPos.y == InRoom.DOORS[0].y) { doorNum = 0;}
        else if (roomPos.x == InRoom.DOORS[1].x && roomPos.y >= InRoom.DOORS[1].y) { doorNum = 1;}
        else if (roomPos.x <= InRoom.DOORS[2].x && roomPos.y == InRoom.DOORS[2].y) { doorNum = 2;}
        else if (roomPos.x == InRoom.DOORS[3].x && roomPos.y <= InRoom.DOORS[3].y) { doorNum = 3;}
        else { doorNum = 4;}
        
        if(doorNum > 3 || doorNum != facing) return;

        Vector2 rm = roomNum;
        switch (doorNum)
        {
            case 0:
                rm.x += 1;
                break;
            case 1:
                rm.y += 1;
                break;
            case 2:
                rm.x -= 1;
                break;
            case 3:
                rm.y -= 1;
                break;
        }
        if(rm.x >= 0 && rm.x <= InRoom.MAX_RM_X){
            if(rm.y >= 0 && rm.y <= InRoom.MAX_RM_Y){
                roomNum = rm;
                transitionPos = InRoom.DOORS[(doorNum + 2) % 4];
                roomPos = transitionPos;
                mode = eMode.transition;
                transitionDone = Time.time + transitionDelay;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(invincible) return;
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if(enemy == null) return;
        health -= enemy.damage;
        invincible = true;
        invincibleDone = Time.time + invincibleDuration;
        if(enemy.pushAway){
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
            mode = eMode.knockback;
            knockbackDone = Time.time + knockbackDuration;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        PickUp item = other.gameObject.GetComponent<PickUp>();
        if(item == null) return;
        switch(item.itemType){
            case PickUp.eType.key:
                keyCount += 1;
                break;
            case PickUp.eType.health:
                health = Mathf.Min( health + 2, maxHealth ); 
                break;
        }
        Destroy(other.gameObject);
    }

    //实现接口
    public int GetFacing(){
        return facing;
    }
    public bool moving{
        get {
            return mode == eMode.move;
        }
    }
    public float GetSpeed(){
        return speed;
    }
    public float gridMult{
        get {
            return InRm.gridMult;
        }
    }
    public Vector2 roomPos{
        get { return InRm.roomPos; }
        set { InRm.roomPos = value; }
    }
    public Vector2 roomNum{
        get { return InRm.roomNum; }
        set { InRm.roomNum = value; }
    }
    public Vector2 GetRoomPosOnGrid(float mult = -1){
        return InRm.GetRoomPosOnGrid(mult);
    }
}
