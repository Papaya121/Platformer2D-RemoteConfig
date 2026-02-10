using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJumpButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private PhysicsController playerController;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerController == null)
            return;

        playerController.RequestJump();
    }
}
