using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector2 offset = new Vector2(0f, 1f);
    [SerializeField] private float speed = 5f;
    [SerializeField] private Transform target;

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 newPosition = target.position + new Vector3(offset.x, offset.y, -10f);
            transform.position = Vector3.Lerp(transform.position, newPosition, speed * Time.deltaTime);
        }
    }
}
