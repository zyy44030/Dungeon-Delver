using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InRoom : MonoBehaviour
{
    static public float ROOM_W = 20;
    static public float ROOM_H = 12;
    static public float WALL_T = 2;

    static public Vector2[] DOORS = new Vector2[]{
        new Vector2(18,6),
        new Vector2(10.5f,11),
        new Vector2(2,6),
        new Vector2(10.5f,1)
    };

    [Header("Set in Inspector")]
    public float gridMult = 1;

    // private void LateUpdate() {
    //     if(keepInRoom){
    //         Vector2 rPos = roomPos;
    //         rPos.x = Mathf.Clamp(rPos.x, WALL_T, ROOM_W - 1 - WALL_T);
    //         rPos.y = Mathf.Clamp(rPos.y, WALL_T, ROOM_H - 1 - WALL_T);
    //         roomPos = rPos;
    //     }
    // }

    public Vector2 roomPos{
        get{
            Vector2 tPos = transform.position;
            tPos.x -= 13.5f;
            tPos.x %= ROOM_W;
            tPos.y += 1;
            tPos.y %= ROOM_H;
            return tPos;
        }
        set{
            Vector2 rm = roomNum;
            rm.x *= ROOM_W;
            rm.x += 13.5f;
            rm.y *= ROOM_H;
            rm.y -= 1;
            rm += value;
            transform.position = rm;
        }
    }

    public Vector2 roomNum{
        get{
            Vector2 tPos = transform.position;
            tPos.x = Mathf.Floor((tPos.x - 13.5f) / ROOM_W);
            tPos.y = Mathf.Floor((tPos.y + 1)/ ROOM_H);
            return tPos;
        }
        set{
            Vector2 rPos = roomPos;
            Vector2 rm = value;
            rm.x *= ROOM_W;
            rm.x += 13.5f;
            rm.y *= ROOM_H;
            rm.y -= 1;
            transform.position = rm + rPos;
        }
    }

    public Vector2 GetRoomPosOnGrid(float mult = -1){
         if(mult == -1){
             mult = gridMult;
         }
        Vector2 rPos = roomPos;
        rPos /= mult;

        rPos.x = Mathf.Round(roomPos.x);
        rPos.y = Mathf.Round(roomPos.y);
        rPos *= mult;
        return rPos;
    }
}