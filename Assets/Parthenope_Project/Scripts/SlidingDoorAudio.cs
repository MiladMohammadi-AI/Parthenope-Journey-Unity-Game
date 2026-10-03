using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SlidingDoorAudio : MonoBehaviour
{
    [Header("Door")]
    [Tooltip("آبجکتی که واقعاً هنگام باز و بسته شدن حرکت می‌کند")]
    [SerializeField] private Transform movingDoor;

    [Header("Audio")]
    [SerializeField] private AudioClip doorSound;

    [Header("Detection Settings")]
    [SerializeField] private float movementThreshold = 0.002f;
    [SerializeField] private float soundCooldown = 0.4f;

    private AudioSource audioSource;
    private Vector3 previousPosition;
    private bool wasMoving;
    private float nextAllowedSoundTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f;
        audioSource.volume = 0.7f;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = 15f;

        if (movingDoor == null)
        {
            movingDoor = transform;
        }

        previousPosition = movingDoor.position;
    }

    private void Update()
    {
        float movementAmount =
            Vector3.Distance(movingDoor.position, previousPosition);

        bool isMoving = movementAmount > movementThreshold;

        if (isMoving && !wasMoving && Time.time >= nextAllowedSoundTime)
        {
            PlayDoorSound();
            nextAllowedSoundTime = Time.time + soundCooldown;
        }

        wasMoving = isMoving;
        previousPosition = movingDoor.position;
    }

    private void PlayDoorSound()
    {
        if (doorSound != null)
        {
            audioSource.PlayOneShot(doorSound);
        }
    }
}