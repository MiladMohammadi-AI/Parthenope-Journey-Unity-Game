using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerPushObjects : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private float pushPower = 2.5f;
    [SerializeField] private float maximumPushSpeed = 2.5f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // جسم باید Rigidbody داشته باشد و Kinematic نباشد.
        if (body == null || body.isKinematic)
        {
            return;
        }

        // از هل‌دادن جسم به سمت بالا یا پایین جلوگیری می‌کند.
        Vector3 pushDirection = new Vector3(
            hit.moveDirection.x,
            0f,
            hit.moveDirection.z
        );

        if (pushDirection.sqrMagnitude < 0.01f)
        {
            return;
        }

        pushDirection.Normalize();

        Vector3 horizontalVelocity = body.linearVelocity;
        horizontalVelocity.y = 0f;

        // نمی‌گذاریم میز و صندلی با سرعت خیلی زیاد پرتاب شوند.
        if (horizontalVelocity.magnitude >= maximumPushSpeed)
        {
            return;
        }

        body.AddForce(
            pushDirection * pushPower,
            ForceMode.VelocityChange
        );
    }
}