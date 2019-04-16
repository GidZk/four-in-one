using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 618

public class AimingController : NetworkBehaviour, InputListener
{
    private const float LaunchSpeedConstant = 30;
    private const float YankConstant = 200;
    private const float MinRange = 0.8f;
    private const int CollectHookThreshold = 2;

    // Game objects related to the crosshair
    public GameObject crosshair;

    // Game objects related to the hook and line
    public LineRenderer ropeRenderer;
    public GameObject hook;
    private Rigidbody2D _hookRb;

    // State
    [SerializeField] private HookState state = HookState.Idle;
    private bool _shouldReel;

    [SerializeField] private Vector2 aimVector;
    private Transform[] transformsLineRender;
    private bool _hookVisible;

    private bool _onServer;

    private bool HookVisible
    {
        get => _hookVisible;
        set
        {
            _hookVisible = value;
            ropeRenderer.enabled = value;
            hook.SetActive(value);
            // needs to re-apply this every time the hook is enabled
            if (value)
                Physics2D.IgnoreCollision(
                    GetComponent<Collider2D>(),
                    hook.GetComponent<Collider2D>());
            if (_onServer)
            {
                Debug.Log($"Call RpcHookVisible: {value}");
                RpcHookVisible(value);
            }
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
        _onServer = NetworkController.Instance.IsServer();

        aimVector = Vector2.right;

        if (_onServer)
        {
            hook = Instantiate(Resources.Load("spawnable/hook")) as GameObject;
            NetworkServer.Spawn(hook);
        }
        else
        {
            hook = GameObject.FindWithTag("Hook");
            if (hook == null)
                Debug.Log("Did not find the hook on the client");
        }

        _hookRb = hook.GetComponent<Rigidbody2D>();
        hook.GetComponent<SpriteRenderer>();
        NetworkController.Instance.Register(this);

        transformsLineRender = new Transform[2];
        transformsLineRender[0] = gameObject.transform;
        transformsLineRender[1] = hook.transform;
        HookVisible = false;
    }

    void Update()
    {
        // Render the synchronized state
        SetCrosshairPosition(aimVector);
        UpdateLineRenderer();
        if (!_onServer)
            return;

        // Logic that should only be executed on the server below here

        if (state == HookState.Reeling)
        {
            ReturnHookWithPhysics();
            CollectNearbyHook();
        }
    }

    /// Update the positions of the line between player and hook
    private void UpdateLineRenderer()
    {
        ropeRenderer.SetPosition(0, transform.position);
        ropeRenderer.SetPosition(1, hook.transform.position);
    }

    /// Collects the hook if nearby, hiding if so
    private void CollectNearbyHook()
    {
        if (!(Math.Abs((transform.position - hook.transform.position).magnitude) < CollectHookThreshold)) return;
        state = HookState.Idle;
        HookVisible = false;
    }

    /// Sets the position and rotation of the crosshair
    private void SetCrosshairPosition(Vector3 v)
    {
        var p = transform.position;
        const float distFromPlayer = 5f;
        var pos = new Vector2(
            p.x + v.x * distFromPlayer,
            p.y + v.y * distFromPlayer);

        var angle = Vector2.Angle(v, Vector2.right);
        if (v.y < 0) angle = -angle;
        var rot = Quaternion.Euler(0, 0, angle);

        crosshair.transform.SetPositionAndRotation(pos, rot);
    }


    /// Fires the hook
    private void Fire(float force)
    {
        state = HookState.Firing;
        HookVisible = true;
        _hookRb.position = transform.position;

        var vec = new Vector2
        {
            x = aimVector.x * force,
            y = aimVector.y * force
        };
        var angle = Vector2.Angle(aimVector, Vector2.right);
        if (aimVector.y < 0) angle = -angle;
        hook.transform.rotation = Quaternion.Euler(0, 0, angle);
        _hookRb.velocity = vec * LaunchSpeedConstant;
    }


    /// "Yanks" the hook towards the controller
    private void ReturnHookWithPhysics()
    {
        if (!_shouldReel)
            return;

        _shouldReel = false;
        var thisPos = transform.position;
        var hookPos = hook.transform.position;
        var vec = (thisPos - hookPos).normalized * YankConstant;
        //_hookRb.velocity = vec / 50;
        _hookRb.AddForce(vec);
        Task.Delay(500).ContinueWith(t => _shouldReel = true);
    }

    [ClientRpc]
    public void RpcHookVisible(bool visible)
    {
        if (_onServer) return;
        HookVisible = visible;
    }

    [ClientRpc]
    private void RpcSetAimAngle(float x, float y)
    {
        aimVector.Set(x, y);
    }

    public void OnVerticalMovementInput(float value)
    {
    }

    public void OnHorizontalMovementInput(float value)
    {
    }

    public void OnCannonAngleInput(float value)
    {
        var x = Mathf.Cos(value);
        var y = Mathf.Sin(value);
        RpcSetAimAngle(x, y);
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