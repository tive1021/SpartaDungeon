using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI3D : MonoBehaviour
{
    public Camera mainCamera; 

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + mainCamera.transform.forward);
    }
}
