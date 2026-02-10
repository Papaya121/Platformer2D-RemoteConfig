using UnityEngine;

public class FootstepSoundsController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PhysicsController physicsController;

    [Header("Footsteps")]
    [SerializeField] private float stepInterval = 0.4f;
    [SerializeField] private float pitchRange = 0.1f;

    private Rigidbody2D _rigidbody;
    private float _stepTimer;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        if (physicsController == null)
        {
            physicsController = GetComponent<PhysicsController>();
        }
    }

    private void Update()
    {
        if (_rigidbody == null || physicsController == null)
            return;

        bool isGrounded = physicsController.IsGrounded();
        float speed = Mathf.Abs(_rigidbody.linearVelocity.x);

        if (isGrounded && speed > 0.1f)
        {
            _stepTimer -= Time.deltaTime;
            if (_stepTimer <= 0f)
            {
                AudioManager.PlaySound("Footstep", pitchRange);
                _stepTimer = stepInterval;
            }
        }
        else
        {
            _stepTimer = 0f;
        }
    }
}
