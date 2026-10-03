using UnityEngine;

public class FloatingStand : MonoBehaviour
{
    [Header("Floating Movement")]
    public float floatingHeight = 0.15f;
    public float floatingSpeed = 1.2f;

    [Header("Rotation")]
    public bool rotateObject = true;
    public float rotationSpeed = 15f;

    [Header("Optional Tilt")]
    public bool useTilt = false;
    public float tiltAmount = 2f;
    public float tiltSpeed = 0.8f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        FloatObject();
        RotateObject();
    }

    void FloatObject()
    {
        float newY =
            startPosition.y +
            Mathf.Sin(Time.time * floatingSpeed) *
            floatingHeight;

        transform.position =
            new Vector3(
                startPosition.x,
                newY,
                startPosition.z
            );
    }

    void RotateObject()
    {
        if (rotateObject)
        {
            transform.Rotate(
                Vector3.up,
                rotationSpeed * Time.deltaTime,
                Space.World
            );
        }

        if (useTilt)
        {
            float tiltX =
                Mathf.Sin(Time.time * tiltSpeed) *
                tiltAmount;

            float currentY =
                transform.eulerAngles.y;

            transform.rotation =
                Quaternion.Euler(
                    tiltX,
                    currentY,
                    0f
                );
        }
    }
}