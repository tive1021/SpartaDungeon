using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpPower;
    private bool isDashing = false;
    public float dashSpeed;
    private Vector2 curMovementInput;
    public LayerMask groundLayerMask;
    public Animator animator;

    [Header("Look")]
    public Transform cameraContainer;
    public Vector3 firstPersonCameraPosition;
    public Vector3 thirdPersonCameraPosition;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    public bool canLook = true;
    private bool isFirstPerson = true;


    private Rigidbody _rigidbody;
    public Action inventory;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    public void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= isDashing ? dashSpeed : moveSpeed;
        // ������ ���� ���� ���� �������� �ϱ� ������
        dir.y = _rigidbody.velocity.y;

        _rigidbody.velocity = dir;
    }

    public void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnSwitchCamera(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            isFirstPerson = !isFirstPerson;
            SetCameraPosition();
        }
    }

    private void SetCameraPosition()
    {
        if (isFirstPerson)
        {
            // 1��Ī �������� ī�޶� ĳ���� ������ 1��Ī ��ġ�� ����
            cameraContainer.localPosition = firstPersonCameraPosition; // ���� ��ġ
            cameraContainer.localRotation = Quaternion.identity;
        }
        else
        {
            // 3��Ī �������� ī�޶� ĳ���� �������� ����
            cameraContainer.localPosition = thirdPersonCameraPosition; // ���� ��ġ
            cameraContainer.localRotation = Quaternion.identity;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            animator.SetBool("IsWalking", true);
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            animator.SetBool("IsWalking", false);
            curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            animator.SetTrigger("Jump");
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up *0.01f), Vector2.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up *0.01f), Vector2.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up *0.01f), Vector2.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up *0.01f), Vector2.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    public void OnInventoryButton(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (!isDashing)
            {
                StartCoroutine(DashCoroutine());
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isDashing = false;
        }
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        animator.SetBool("IsRunning", isDashing);

        while (isDashing && CharacterManager.Instance.Player.condition.UseStamina(30 * Time.deltaTime))
        {
            yield return null; // �� �����Ӹ��� ����ϸ鼭 ���¹̳� �Ҹ�
        }

        // ��ð� ������ isDashing�� false�� ������ �̵� �ӵ��� ���� �ӵ��� ����
        isDashing = false;
        animator.SetBool("IsRunning", isDashing);
    }
}
