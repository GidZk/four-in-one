using System.Threading.Tasks;
using UnityEngine;

public class AimingController : MonoBehaviour, InputListener
{
    public LineRenderer ropeRenderer;
    public GameObject ropeHingeAnchor;
    public Transform crosshair;
    public SpriteRenderer crosshairSprite;
    public playerController playerController;
    public float fireForce;
    public GameObject crossController;

    public Transform[] points;


    private ObjectRotator aimWheel;
    private Rigidbody2D ropeHingeAnchorRb;
    private SpriteRenderer ropeHingeAnchorSprite;
    private float ropeMaxCastDistance = 20f;
    private Vector3 aimDirection;

    private HookState _state = HookState.Idle;

    private enum HookState
    {
        Idle,
        Firing,
        Reeling
    }

    void Awake()
    {
        aimWheel = crossController.GetComponent<ObjectRotator>();
        ropeHingeAnchorRb = ropeHingeAnchor.GetComponent<Rigidbody2D>();
        ropeHingeAnchorSprite = ropeHingeAnchor.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        aimDirection = Quaternion.Euler(0, 0, aimWheel.GetEulerAngles()) * Vector2.right;

        UpdateCrosshair();

        for (int i = 0; i < points.Length; ++i)
        {
            ropeRenderer.SetPosition(i, points[i].position);
        }
    }

    private void UpdateCrosshair()
    {
        if (_state != HookState.Idle)
        {
            crosshairSprite.enabled = false; // Hide sprite
            return;
        }

        if (crosshairSprite.enabled != true) crosshairSprite.enabled = true;
        aimDirection = Quaternion.Euler(0, 0, aimWheel.GetEulerAngles()) * Vector2.right;
    }

//calculates and sets the aimangle, with @param aimangle given in radians
    private void SetCrosshairPosition(float aimAngle)
    {
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
    public void Fire()
    {
        _state = HookState.Firing;
        ropeRenderer.enabled = true;
        ropeHingeAnchorSprite.enabled = true;
        ropeHingeAnchorRb.position = transform.position;

        ropeHingeAnchorRb.velocity = new Vector2
        {
            x = aimDirection.x * fireForce,
            y = aimDirection.y * fireForce
        };
    }

    private void Reel()
    {
        _state = HookState.Reeling;
        // TODO stuff
        // wait for rotations
    }

//Halts the rope
    public void ResetRope()
    {
        ropeHingeAnchorRb.velocity = new Vector2(0, 0);
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, transform.position);
        ropeRenderer.SetPosition(1, transform.position);
        ropeHingeAnchorSprite.enabled = false;
    }

    public void OnVerticalMovementInput(float value)
    {
    }

    public void OnHorizontalMovementInput(float value)
    {
    }

    public void OnCannonAngleInput(float value)
    {
        SetCrosshairPosition(value);
    }

    public void OnCannonLaunchInput(float value)
    {
        if (_state == HookState.Idle)
        {
            Fire();
            Task.Delay(2000).ContinueWith(t => Reel());
        }
    }
}