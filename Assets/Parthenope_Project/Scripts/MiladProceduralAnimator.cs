using UnityEngine;

public class MiladProceduralAnimator : MonoBehaviour
{
    [Header("Body Joints")]
    public Transform leftArm;
    public Transform rightArm;
    public Transform leftLeg;
    public Transform rightLeg;

    [Header("Animation Settings")]
    public float animationSpeed = 7f;
    public float armSwing = 25f;
    public float legSwing = 30f;

    private Vector3 lastPosition;
    private bool isMoving;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        CheckMovement();
        AnimateCharacter();
    }

    void CheckMovement()
    {
        float distance = Vector3.Distance(
            transform.position,
            lastPosition
        );

        isMoving = distance > 0.001f;
        lastPosition = transform.position;
    }

    void AnimateCharacter()
    {
        float cycle = Mathf.Sin(
            Time.time * animationSpeed
        );

        if (isMoving)
        {
            leftArm.localRotation =
                Quaternion.Euler(cycle * armSwing, 0f, 0f);

            rightArm.localRotation =
                Quaternion.Euler(-cycle * armSwing, 0f, 0f);

            leftLeg.localRotation =
                Quaternion.Euler(-cycle * legSwing, 0f, 0f);

            rightLeg.localRotation =
                Quaternion.Euler(cycle * legSwing, 0f, 0f);
        }
        else
        {
            leftArm.localRotation = Quaternion.identity;
            rightArm.localRotation = Quaternion.identity;
            leftLeg.localRotation = Quaternion.identity;
            rightLeg.localRotation = Quaternion.identity;
        }
    }
}