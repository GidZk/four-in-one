using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {
    [SerializeField]Transform target;
    
    void onMouseDown(){
        
        target.position = Vector2.right;



    }
    
}
