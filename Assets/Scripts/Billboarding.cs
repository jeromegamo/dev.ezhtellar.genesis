using System;
using UnityEngine;

[ExecuteInEditMode]
public class Billboarding : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main; 
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null) return;

        // Face the canvas toward the camera's forward direction
        transform.LookAt(transform.position + targetCamera.transform.forward);
        //transform.rotation = Quaternion.Euler(90f, transform.rotation.eulerAngles.y, 0f); 
        //transform.forward = targetCamera.transform.forward;
    }
}