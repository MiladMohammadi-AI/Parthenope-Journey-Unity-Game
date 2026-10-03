using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public float rotationSpeed = 10f;
    public float gravity = -20f;

    [Header("Camera")]
    public Transform cameraTransform;

    private CharacterController controller;
    private float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        // اگر دوربین را دستی وصل نکردیم،
        // دوربین اصلی را خودکار پیدا می‌کند.
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection =
            new Vector3(horizontal, 0f, vertical).normalized;

        Vector3 moveDirection = Vector3.zero;

        if (inputDirection.magnitude >= 0.1f)
        {
            // جهت جلوی دوربین، بدون بالا و پایین
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();

            // جهت راست دوربین
            Vector3 cameraRight = cameraTransform.right;
            cameraRight.y = 0f;
            cameraRight.Normalize();

            // تبدیل ورودی WASD به جهت دوربین
            moveDirection =
                cameraForward * vertical +
                cameraRight * horizontal;

            moveDirection.Normalize();

            // چرخش شخصیت به سمت حرکت
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 finalMovement =
            moveDirection * moveSpeed;

        finalMovement.y = verticalVelocity;

        controller.Move(
            finalMovement * Time.deltaTime
        );
    }
}