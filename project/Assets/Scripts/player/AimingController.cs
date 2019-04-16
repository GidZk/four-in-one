using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class AimingController : NetworkBehaviour, InputListener
{
    private const float LaunchSpeedConstant = 30;
    private const float YankConstant = 5;
    private const float MinRange = 0.8f;
    private const int CollectHookThreshold = 1;

    // Game objects related to the crosshair
    public GameObject crossController;
    public Transform crosshair;
    private SpriteRenderer _crosshairSpriteRenderer;
    private ObjectRotator _aimWheel;

    // Game objects related to the hook and line
    public LineRenderer ropeRenderer;
    public GameObject hook;
    private Rigidbody2D _hookRb;

    // State
    [SerializeField] private HookState state = HookState.Idle;
    private bool _shouldReel;
    [SerializeField] private Vector3 aimAngle;
    public Transform[] points;
    [SerializeField] private bool hookVisible;

    private bool HookVisible
    {
        get => hookVisible;
        set
        {
            hookVisible = value;
            ropeRenderer.enabled = value;
            hook.SetActive(value);
        }
    }

    private enum HookState
    {
        Idle,
        Firing,
        Reeling
    }

    void Awake()
    {
        _aimWheel = crossController.GetComponent<ObjectRotator>();
        _crosshairSpriteRenderer = crosshair.GetComponent<SpriteRenderer>();
        _hookRb = hook.GetComponent<Rigidbody2D>();
        hook.GetComponent<SpriteRenderer>();
        NetworkController.Instance.Register(this);
        HookVisible = false;
    }

    void Update()
    {
        //_aimAngle = Quaternion.Euler(0, 0, _aimWheel.GetEulerAngles()) * Vector2.right;

        UpdateCrosshair();
        if (state == HookState.Reeling)
        {
            ReturnHookWithPhysics();
            CollectNearbyHook();
        }

        UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        for (var i = 0; i < points.Length; ++i)
        {
            ropeRenderer.SetPosition(i, points[i].position);
        }
    }

    private void CollectNearbyHook()
    {
        if (!(Math.Abs((transform.position - hook.transform.position).magnitude) < CollectHookThreshold)) return;
        state = HookState.Idle;
        HookVisible = false;
    }

    private void UpdateCrosshair()
    {
        if (state != HookState.Idle)
        {
            _crosshairSpriteRenderer.enabled = false; // Hide sprite
            return;
        }

        if (_crosshairSpriteRenderer.enabled != true) _crosshairSpriteRenderer.enabled = true;
        //aimAngle = Quaternion.Euler(0, 0, _aimWheel.GetEulerAngles()) * Vector2.right;
    }

    //calculates and sets direction of the crosshair, with @param aimangle given in radians
    private void SetCrosshairPosition(float aimAngle)
    {
        if (!_crosshairSpriteRenderer.enabled)
        {
            _crosshairSpriteRenderer.enabled = true;
        }

        var p = transform.position;
        var x = p.x + 3f * Mathf.Cos(aimAngle);
        var y = p.y + 3f * Mathf.Sin(aimAngle);

        var crossHairPosition = new Vector3(x, y, 0);
        crosshair.transform.position = crossHairPosition;
    }


    // Fires the hook
    private void Fire(float force)
    {
        state = HookState.Firing;
        HookVisible = true;
        _hookRb.position = transform.position;

        var vec = new Vector2
        {
            x = aimAngle.x * force,
            y = aimAngle.y * force
        };
        _hookRb.velocity = vec * LaunchSpeedConstant;
    }


    // "Yanks" the hook towards the controller
    private void ReturnHookWithPhysics()
    {
        if (!_shouldReel)
            return;

        _shouldReel = false;
        var thisPos = transform.position;
        var hookPos = hook.transform.position;
        var vec = (thisPos - hookPos).normalized * YankConstant;
        _hookRb.velocity = vec;
        Task.Delay(500).ContinueWith(t => _shouldReel = true);
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
        aimAngle.Set(
            (float) Math.Cos(value),
            (float) Math.Sin(value),
            0
        );
    }

    public void OnCannonLaunchInput(float value)
    {
        if (state != HookState.Idle) return;

        Fire(value);
        Task.Delay((int) (2000 * (value + MinRange))).ContinueWith(t =>
        {
            state = HookState.Reeling;
            _shouldReel = true;
        });
    }
}