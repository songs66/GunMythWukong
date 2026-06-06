using UnityEngine;

public class DualViewPlayerController : MonoBehaviour
{
    [Header("移动参数")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpHeight = 1.4f;
    public float gravity = -9.81f;

    [Header("视角参数")]
    public float mouseSensitivity = 2.0f;
    public float minLookAngle = -75f;
    public float maxLookAngle = 75f;

    [Header("摄像机引用")]
    public Transform cameraPivot;
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;

    [Header("角色模型")]
    public GameObject modelRoot;
    public Animator characterAnimator;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private float xRotation = 0f;
    private bool isFirstPerson = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraPivot == null)
        {
            Transform foundPivot = transform.Find("CameraPivot");
            if (foundPivot != null)
            {
                cameraPivot = foundPivot;
            }
        }

        if (firstPersonCamera == null && cameraPivot != null)
        {
            Transform fp = cameraPivot.Find("FirstPersonCamera");
            if (fp != null)
            {
                firstPersonCamera = fp.GetComponent<Camera>();
            }
        }

        if (thirdPersonCamera == null && cameraPivot != null)
        {
            Transform tp = cameraPivot.Find("ThirdPersonCamera");
            if (tp != null)
            {
                thirdPersonCamera = tp.GetComponent<Camera>();
            }
        }

        if (modelRoot == null)
        {
            Transform foundModelRoot = transform.Find("ModelRoot");
            if (foundModelRoot != null)
            {
                modelRoot = foundModelRoot.gameObject;
            }
        }

        if (characterAnimator == null && modelRoot != null)
        {
            characterAnimator = modelRoot.GetComponentInChildren<Animator>();
        }

        SetViewMode(true);
        LockCursor();
    }

    void Update()
    {
        if (GameFlowManager.Instance != null && !GameFlowManager.Instance.IsPlaying)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        HandleViewSwitch();
        HandleMouseLook();
        HandleMovement();
    }

    void HandleCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            LockCursor();
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void HandleViewSwitch()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;
            SetViewMode(isFirstPerson);
        }
    }

    void SetViewMode(bool firstPerson)
    {
        if (firstPersonCamera != null)
        {
            firstPersonCamera.gameObject.SetActive(firstPerson);
        }

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.gameObject.SetActive(!firstPerson);
        }

        if (modelRoot != null)
        {
            modelRoot.SetActive(!firstPerson);
        }
    }

    void HandleMouseLook()
    {
        if (cameraPivot == null)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minLookAngle, maxLookAngle);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        if (controller == null)
        {
            return;
        }

        bool isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
        }

        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * inputX + transform.forward * inputZ;

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        controller.Move(verticalVelocity * Time.deltaTime);

        UpdateAnimation(moveDirection.magnitude, isRunning);
    }

    void UpdateAnimation(float moveAmount, bool isRunning)
    {
        if (characterAnimator == null)
        {
            return;
        }

        float animationSpeed = 0f;

        if (moveAmount > 0.1f)
        {
            animationSpeed = isRunning ? 1f : 0.5f;
        }

        characterAnimator.SetFloat("MoveSpeed", animationSpeed);
    }
}