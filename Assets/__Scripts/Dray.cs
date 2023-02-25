using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dray : MonoBehaviour, IFacingMover
{
    public enum eMode { idle, move, attack, transition }

    [Header("Set in Inspector")]
    public float speed = 5;
    public float attackDuration = 0.25f;
    public float attackDelay = 0.5f;
    public float transitionDelay = 0.5f;

    [Header("Set Dynamically")]
    public int dirHeld = -1;
    public int facing = 0;
    public eMode mode = eMode.idle;
    private float timeAtkDone = 0;
    private float timeAtkNext = 0;
    private Rigidbody2D rigid;
    private Animator anim;
    private InRoom InRm;
    private float transitionDone = 0;
    private Vector2 transitionPos;
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
        InRm = GetComponent<InRoom>();
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
        //场景转换
        if(mode== eMode.transition){
            rigid.velocity = Vector2.zero;
            anim.speed = 0;
            roomPos = transitionPos;
            if(Time.time < transitionDone) return;
            mode = eMode.idle;
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

    private void LateUpdate() {
        int doorNum;
        if (roomPos.x >= InRoom.DOORS[0].x && roomPos.y == InRoom.DOORS[0].y) { doorNum = 0;}
        else if (roomPos.x == InRoom.DOORS[1].x && roomPos.y >= InRoom.DOORS[1].y) { doorNum = 1;}
        else if (roomPos.x <= InRoom.DOORS[2].x && roomPos.y == InRoom.DOORS[2].y) { doorNum = 2;}
        else if (roomPos.x == InRoom.DOORS[3].x && roomPos.y <= InRoom.DOORS[3].y) { doorNum = 3;}
        else { doorNum = 4;}
        
        // print("InRoom.DOORS[doorNum].x : " + InRoom.DOORS[doorNum].x);
        

        if(doorNum > 3 || doorNum != facing) return;
        print("here??");
        // print("roomPos.x: " + roomPos.x);
        // print("doorNum: "+ doorNum);
        // print("facing: " + facing);

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
        roomNum = rm;
        transitionPos = InRoom.DOORS[(doorNum + 2) % 4];
        roomPos = transitionPos;
        mode = eMode.transition;
        transitionDone = Time.time + transitionDelay;
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
