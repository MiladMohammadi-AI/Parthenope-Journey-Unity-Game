using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Distance")]
    public float distance = 4f;
    public float height = 2f;

    [Header("Mouse Rotation")]
    public float mouseSensitivity = 120f;
    public float minPitch = -20f;
    public float maxPitch = 60f;

    [Header("Follow")]
    public float followSpeed = 10f;
    public float lookHeight = 1.1f;

    private float yaw;
    private float pitch = 15f;

    private void Start()
    {
        // کنترل موس فقط توسط MainMenuController انجام می‌شود.
        // اینجا دیگر موس قفل یا مخفی نمی‌شود.
    }

    private void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * mouseSensitivity * Time.deltaTime;
        pitch -= mouseY * mouseSensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(
            pitch,
            minPitch,
            maxPitch
        );

        Quaternion cameraRotation =
            Quaternion.Euler(pitch, yaw, 0f);

        Vector3 targetPosition =
            player.position +
            Vector3.up * lookHeight;

        Vector3 desiredPosition =
            targetPosition
            - cameraRotation * Vector3.forward * distance
            + Vector3.up * height;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        transform.LookAt(targetPosition);
    }
}