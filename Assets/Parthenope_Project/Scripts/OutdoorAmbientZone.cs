using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OutdoorAmbientZone : MonoBehaviour
{
    [Header("Player Detection")]
    [SerializeField] private string playerTag = "Player";

    [Header("Fade Settings")]
    [SerializeField] private float fadeSpeed = 1.5f;
    [SerializeField, Range(0f, 1f)] private float maximumVolume = 0.25f;

    private AudioSource audioSource;
    private bool playerIsOutside;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = 0f;

        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    private void Update()
    {
        float targetVolume = playerIsOutside ? maximumVolume : 0f;

        audioSource.volume = Mathf.MoveTowards(
            audioSource.volume,
            targetVolume,
            fadeSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerIsOutside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerIsOutside = false;
        }
    }
}