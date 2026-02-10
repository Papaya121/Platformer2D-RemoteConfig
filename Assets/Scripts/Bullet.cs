using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody2D _rigidbody;
    private int _damage;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction, float speed, int damage)
    {
        direction.Normalize();
        _damage = damage;

        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = direction * speed;
        }

        transform.right = direction;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayers) != 0)
        {
            IHealth health = other.GetComponent<IHealth>();
            health?.ApplyDamage(_damage);
        }

        Destroy(gameObject);
    }
}
