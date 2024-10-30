using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserWall : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float laserDistance = 10f;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        ShootLaser();
    }

    private void ShootLaser()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        lineRenderer.SetPosition(0, transform.position);

        if (Physics.Raycast(ray, out RaycastHit hit, laserDistance, layerMask))
        {
            lineRenderer.SetPosition(1, hit.point);
            Debug.Log("주거 침입! - " + hit.collider.name);
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + transform.forward * laserDistance);
        }
    }
}
