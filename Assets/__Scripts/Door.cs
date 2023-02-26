using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Set in Inspector")]
    public int dir;
    void Awake()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.name == "Dray" && other.gameObject.GetComponent<Dray>().GetFacing() == dir){
            Destroy(gameObject);
        }
    }
}
