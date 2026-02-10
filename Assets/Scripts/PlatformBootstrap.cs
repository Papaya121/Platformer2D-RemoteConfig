using UnityEngine;

public class PlatformBootstrap : MonoBehaviour
{
    void Start()
    {
        bool isMobile = Application.isMobilePlatform;

        QualitySettings.vSyncCount = isMobile ? 0 : 1;
        Application.targetFrameRate = isMobile ? 60 : 0;
    }
}