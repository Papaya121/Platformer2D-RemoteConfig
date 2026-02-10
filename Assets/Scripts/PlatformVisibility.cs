using UnityEngine;

public class PlatformVisibility : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private bool showOnMobile = true;
    [SerializeField] private bool treatEditorAsMobile = false;

    private void Awake()
    {
        Apply();
    }

    private void OnEnable()
    {
        Apply();
    }

    private void Apply()
    {
        GameObject resolved = target != null ? target : gameObject;
        bool isMobile = Application.isMobilePlatform || (treatEditorAsMobile && Application.isEditor);
        bool shouldShow = showOnMobile ? isMobile : !isMobile;

        if (resolved.activeSelf != shouldShow)
        {
            resolved.SetActive(shouldShow);
        }
    }
}
