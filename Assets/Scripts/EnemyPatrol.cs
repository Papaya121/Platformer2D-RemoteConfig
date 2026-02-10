using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float arriveThreshold = 0.1f;
    [SerializeField] private bool flipByScale = true;

    private Rigidbody2D _rigidbody;
    private Transform _currentTarget;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _currentTarget = pointA != null ? pointA : pointB;
    }

    private void FixedUpdate()
    {
        if (pointA == null || pointB == null || _currentTarget == null)
            return;

        Vector2 toTarget = _currentTarget.position - transform.position;

        if (toTarget.magnitude <= arriveThreshold)
        {
            _currentTarget = _currentTarget == pointA ? pointB : pointA;
            toTarget = _currentTarget.position - transform.position;
        }

        Vector2 direction = toTarget.normalized;
        Vector2 velocity = _rigidbody.linearVelocity;
        velocity.x = direction.x * moveSpeed;
        _rigidbody.linearVelocity = velocity;

        if (flipByScale && Mathf.Abs(direction.x) > 0.01f)
        {
            var scale = transform.localScale;
            scale.x = Mathf.Sign(direction.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pointA == null || pointB == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pointA.position, 0.15f);
        Gizmos.DrawWireSphere(pointB.position, 0.15f);
        Gizmos.DrawLine(pointA.position, pointB.position);
    }
}
