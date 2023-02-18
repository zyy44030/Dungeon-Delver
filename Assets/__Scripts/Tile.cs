using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Set Dynamically")]
    public int x;
    public int y;
    public int tileNum;
    public void SetTile(int ex, int eY, int eTileNum){
        x = ex;
        y = eY;
        transform.localPosition = new Vector3(x, y, 0);
        gameObject.name = x.ToString("D3") + "x" + y.ToString("D3");
        if(eTileNum == -1){
            eTileNum = TileCamera.GET_MAP(x, y);
        }
        tileNum = eTileNum;
        GetComponent<SpriteRenderer>().sprite = TileCamera.SPRITES[tileNum];
    }
}
