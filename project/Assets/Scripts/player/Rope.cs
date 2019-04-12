using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Rope : MonoBehaviour {
    
public LineRenderer ropeRenderer;    
public GameObject ropeHingeAnchor;
public Transform crosshair;
public SpriteRenderer crosshairSprite;
public playerController playerController;
public float fireForce;
public GameObject crossController;

public Transform[] points;


private ObjectRotator aimWheel;    
private Vector2 playerPosition;
private Rigidbody2D ropeHingeAnchorRb;
private SpriteRenderer ropeHingeAnchorSprite;
private float ropeMaxCastDistance = 20f;
private Vector3 aimDirection;

private bool isFired = false;



void Awake()
{
    aimWheel = crossController.GetComponent<ObjectRotator>();
    playerPosition = transform.position;
    ropeHingeAnchorRb = ropeHingeAnchor.GetComponent<Rigidbody2D>();
    ropeHingeAnchorSprite = ropeHingeAnchor.GetComponent<SpriteRenderer>();
}

void Update(){
    
    var worldMousePosition =
        Camera.main.WorldToScreenPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
    var facingDirection = worldMousePosition - transform.position;
    var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
    if (aimAngle < 0f){
        aimAngle = Mathf.PI * 2 + aimAngle;
    }
    
   // Debug.Log("AimAngle:: "+ aimAngle);
    //aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;
    playerPosition = transform.position;
    
    //Debug.Log("AimDirection:: "+ aimDirection);
    
    if (!isFired)
    {
	SetCrosshairPosition(aimWheel.GetRadianAngles());    
    }
    else {
	crosshairSprite.enabled = false;

	
	
    for (int i = 0; i < points.Length; ++i){ 
            ropeRenderer.SetPosition(i, points[i].position);
	    }
    }

}
// calculates crosshair position
private void SetCrosshairPosition(float aimAngle){
    
    //Debug.Log("Rope:: " + aimAngle);
    
    if (!crosshairSprite.enabled)
    {
        crosshairSprite.enabled = true;
    }
   
    var x = transform.position.x + 3f * Mathf.Cos(aimAngle);
    var y = transform.position.y + 3f * Mathf.Sin(aimAngle);

    var crossHairPosition = new Vector3(x, y, 0);
    crosshair.transform.position = crossHairPosition;
}
//fires the anchor
public void Fire(){
        ropeRenderer.enabled = true;
        ropeHingeAnchorSprite.enabled = true;
        ropeHingeAnchorRb.position = playerPosition;
        
        Vector2 fireVector = new Vector2();
        fireVector.x = aimDirection.x * fireForce;
        fireVector.y = aimDirection.y * fireForce;
        ropeHingeAnchorRb.velocity = fireVector;
        isFired = true;
}
//Halts the rope
public void ResetRope(){

    isFired = false;
    ropeHingeAnchorRb.velocity = new Vector2(0,0);
    ropeRenderer.positionCount = 2;
    ropeRenderer.SetPosition(0, transform.position);
    ropeRenderer.SetPosition(1, transform.position);
    ropeHingeAnchorSprite.enabled = false;
}

    // Start is called before the first frame update
    void Start(){
    }

}
