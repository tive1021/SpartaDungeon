using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Vector3 firstPostion;
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private float speed = 2f;
    private bool toEndPosition = true;

    // Start is called before the first frame update
    void Start()
    {
        firstPostion = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 target = toEndPosition ? endPosition : firstPostion;
        float step = speed * Time.deltaTime; // 이동 거리 계산
        transform.position = Vector3.MoveTowards(transform.position, target, step);

        // 목표 위치에 도달했는지 확인
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            toEndPosition = !toEndPosition; // 방향 전환
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.transform.parent = transform;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.transform.parent = null;
        }
    }
}
