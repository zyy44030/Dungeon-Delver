using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    private GameObject sword;
    private Dray dray;

    private void Start() {
        sword = transform.Find("Sword").gameObject;
        dray = transform.parent.GetComponent<Dray>();
        sword.SetActive(false);
    }
    private void Update() {
        if(dray.facing == 2){
            transform.rotation = Quaternion.Inverse(new Quaternion(0,180,0,1));
        }else{
            transform.rotation = Quaternion.Euler(0, 0, 90 * dray.facing);
        }
        sword.SetActive(dray.mode == Dray.eMode.attack);
        
    }
}
