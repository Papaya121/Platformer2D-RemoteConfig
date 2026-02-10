using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    
    private Rigidbody2D _rigidbody;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateAnimation();
    }
    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        var speed = Mathf.Abs(_rigidbody.linearVelocity.x);
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsWalking", speed > 0.1f);
    }
}
