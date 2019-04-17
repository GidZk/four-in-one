using UnityEngine;

public class HookCollisionHandler : MonoBehaviour
{
    private bool canHook;
    private bool hasHooked;
    private GameObject hooke;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (canHook && !hasHooked && other.gameObject.CompareTag("crabplast"))
        {
            hasHooked = true;
            canHook = false;

            var oGameObj = hooke = other.gameObject;
            var oRigidBody = oGameObj.GetComponent<Rigidbody2D>();
            Physics2D.IgnoreCollision(
                GetComponent<Collider2D>(),
                oGameObj.GetComponent<Collider2D>());
            oGameObj.GetComponent<EnemyMovement>().enabled = false;

            oRigidBody.MovePosition(transform.position);
            var joint = gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = oRigidBody;
            joint.connectedAnchor = new Vector2(0, 0);
            //joint.distance = 0.5f;
            //joint.dampingRatio = 0.5f;
            //joint.frequency = 10E4f;

            oRigidBody.bodyType = RigidbodyType2D.Dynamic;
            oRigidBody.gravityScale = 0;
            oRigidBody.mass = 0.3f;
        }
    }

    private void Update()
    {
        if (hasHooked)
        {
            if (hooke == null)
            {
                hasHooked = false;
                Destroy(GetComponent<Joint2D>());
            }
            else
            {
                // TODO might want to move this to Reel() in AimingController
                var diff = hooke.transform.position - playerController.Instance.transform.position;
                var angle = Vector2.Angle(diff, Vector2.right);
                if (diff.y < 0) angle = -angle;
                _rb.MoveRotation(angle);
            }
        }
    }

    private void OnEnable()
    {
        canHook = true;
    }

    private void OnDisable()
    {
        Destroy(GetComponent<Joint2D>());
    }
}