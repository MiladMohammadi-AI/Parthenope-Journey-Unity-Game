using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class FinalRoomController : MonoBehaviour
{
    [Header("Final Room State")]
    public bool isUnlocked;

    [Header("Interaction")]
    public KeyCode interactionKey = KeyCode.O;

    [Header("Final Status Sphere")]
    public Renderer finalSphereRenderer;

    [Header("Sphere Materials")]
    public Material lockedMaterial;
    public Material availableMaterial;
    public Material completedMaterial;

    [Header("Video")]
    public VideoPlayer videoPlayer;
    public GameObject videoCanvas;

    [Header("Player")]
    public PlayerMovement playerMovement;

    [Header("Restart Settings")]
    public float restartDelayAfterVideo = 2f;

    private bool playerIsNear;
    private bool videoIsPlaying;
    private bool finalCompleted;

    void Start()
    {
        isUnlocked = false;
        videoIsPlaying = false;
        finalCompleted = false;

        if (videoCanvas != null)
        {
            videoCanvas.SetActive(false);
        }

        SetFinalSphereLocked();

        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = false;

            videoPlayer.loopPointReached +=
                OnVideoFinished;
        }
    }

    void Update()
    {
        if (!playerIsNear)
        {
            return;
        }

        if (!isUnlocked)
        {
            return;
        }

        if (videoIsPlaying)
        {
            return;
        }

        if (Input.GetKeyDown(interactionKey))
        {
            PlayFinalVideo();
        }
    }

    public void UnlockFinalRoom()
    {
        if (isUnlocked)
        {
            return;
        }

        isUnlocked = true;

        SetFinalSphereAvailable();

        Debug.Log(
            "Final room unlocked."
        );
    }

    void PlayFinalVideo()
    {
        if (!isUnlocked)
        {
            return;
        }

        if (videoPlayer == null)
        {
            Debug.LogError(
                name + ": Video Player is missing."
            );

            return;
        }

        if (videoCanvas == null)
        {
            Debug.LogError(
                name + ": Video Canvas is missing."
            );

            return;
        }

        videoIsPlaying = true;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        videoCanvas.SetActive(true);

        videoPlayer.Stop();
        videoPlayer.frame = 0;
        videoPlayer.Play();

        Debug.Log("Final video started.");
    }

    void OnVideoFinished(VideoPlayer source)
    {
        videoIsPlaying = false;
        finalCompleted = true;

        SetFinalSphereCompleted();

        StartCoroutine(
            RestartGameAfterVideo()
        );
    }

    IEnumerator RestartGameAfterVideo()
    {
        yield return new WaitForSeconds(
            restartDelayAfterVideo
        );

        RestartGame();
    }

    void RestartGame()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    void SetFinalSphereLocked()
    {
        if (finalSphereRenderer != null &&
            lockedMaterial != null)
        {
            finalSphereRenderer.material =
                lockedMaterial;
        }
    }

    void SetFinalSphereAvailable()
    {
        if (finalSphereRenderer != null &&
            availableMaterial != null)
        {
            finalSphereRenderer.material =
                availableMaterial;
        }
    }

    void SetFinalSphereCompleted()
    {
        if (finalSphereRenderer != null &&
            completedMaterial != null)
        {
            finalSphereRenderer.material =
                completedMaterial;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }

    void OnGUI()
    {
        if (!playerIsNear || videoIsPlaying)
        {
            return;
        }

        GUIStyle style =
            new GUIStyle(GUI.skin.label);

        style.fontSize = 25;
        style.alignment =
            TextAnchor.MiddleCenter;

        style.normal.textColor =
            Color.white;

        string message;

        if (!isUnlocked)
        {
            message =
                "Complete the PhD stage first";
        }
        else if (!finalCompleted)
        {
            message =
                "Press O to watch the final video";
        }
        else
        {
            message =
                "Final journey completed";
        }

        GUI.Label(
            new Rect(
                Screen.width / 2f - 350f,
                Screen.height - 110f,
                700f,
                50f
            ),
            message,
            style
        );
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -=
                OnVideoFinished;
        }
    }
}