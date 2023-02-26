using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeletos : Enemy, IFacingMover
{
    private InRoom InRm;
    [Header("Set in Inspector: Skeletos")]
    public int speed = 2;
    public float timeThinkMin = 1f;
    public float timeThinkMax = 4f;

    [Header("Set Dynamically: Skeletos")]
    public int facing = 0;
    public float timeNextDecision = 0;

    protected override void Awake()
    {
        base.Awake();
        InRm = GetComponent<InRoom>();
    }

    private void Update() {
        if(Time.time >= timeNextDecision){
            DecideDirection();
        }
        rigid.velocity = directions[facing] * speed;
    }

    private void DecideDirection(){
        facing = Random.Range(0, 4);
        timeNextDecision = Time.time + Random.Range(timeThinkMin, timeThinkMax);
    }

    //实现接口
    public int GetFacing(){
        return facing;
    }
    public bool moving{
        get { return true;}
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
