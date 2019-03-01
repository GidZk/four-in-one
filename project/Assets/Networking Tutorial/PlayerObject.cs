
using UnityEngine;

public class PlayerObject : MonoBehaviour
// here goes the data for each attached object
{

    public GameObject PlayerUnitPrefab;
    // Start is called before the first frame update
    void Start() {
        Instantiate(PlayerUnitPrefab);

    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
