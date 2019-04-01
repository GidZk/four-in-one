using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByBoundary : MonoBehaviour
{


    void onTriggerExit(GameObject other)
    {
        Destroy(other.gameObject);

        Debug.Log("bajs");
    }


}

