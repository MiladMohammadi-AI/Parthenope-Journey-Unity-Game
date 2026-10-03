using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerFootstepAudio : MonoBehaviour
{
    [Header("Footstep Sound")]
    [SerializeField] private AudioClip walkingClip;

    [Header("Settings")]
    [SerializeField] private float minimumMovementSpeed = 0.15f;
    [SerializeField, Range(0f, 1f)] private float volume = 0.45f;
    [SerializeField] private float pitch = 1f;

    private CharacterController characterController;
    private AudioSource audioSource;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
    }

    private void Update()
    {
        Vector3 horizontalVelocity = characterController.velocity;
        horizontalVelocity.y = 0f;

        bool isMoving =
            horizontalVelocity.magnitude > minimumMovementSpeed &&
            characterController.isGrounded;

        if (isMoving)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = walkingClip;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}