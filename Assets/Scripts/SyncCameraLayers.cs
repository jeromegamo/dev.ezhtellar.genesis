using UnityEngine;

[ExecuteInEditMode]
public class SyncCameraLayers : MonoBehaviour
{
    public Camera mainCam;
    public Camera uiCam;

    void LateUpdate()
    {
        if (mainCam == null || uiCam == null) return;
        uiCam.transform.position = mainCam.transform.position;
        uiCam.transform.rotation = mainCam.transform.rotation;
        uiCam.orthographicSize = mainCam.orthographicSize;
        uiCam.nearClipPlane = mainCam.nearClipPlane;
        uiCam.farClipPlane = mainCam.farClipPlane;
    }
}