using UnityEngine;

public class StageSlidingDoor : MonoBehaviour
{
    [Header("Sliding Door")]
    public Transform door;

    [Header("Movement")]
    public Vector3 openDirection = Vector3.right;
    public float slideDistance = 1.5f;
    public float openSpeed = 3f;

    [Header("Stage State")]
    public bool isUnlocked;

    [Header("Interaction")]
    public KeyCode interactionKey = KeyCode.O;

    private bool playerIsNear;
    private bool isOpen;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    void Start()
    {
        if (door == null)
        {
            Debug.LogError(name + ": Door reference is missing.");
            enabled = false;
            return;
        }

        closedPosition = door.localPosition;

        openDirection = openDirection.normalized;

        openPosition =
            closedPosition + openDirection * slideDistance;
    }

    void Update()
    {
        if (playerIsNear && Input.GetKeyDown(interactionKey))
        {
            if (isUnlocked)
            {
                isOpen = !isOpen;
            }
            else
            {
                Debug.Log(name + " is locked.");
            }
        }

        MoveDoor();
    }

    void MoveDoor()
    {
        if (door == null)
        {
            return;
        }

        Vector3 targetPosition =
            isOpen ? openPosition : closedPosition;

        door.localPosition = Vector3.MoveTowards(
            door.localPosition,
            targetPosition,
            openSpeed * Time.deltaTime
        );
    }

    public void UnlockDoor()
    {
        isUnlocked = true;
        Debug.Log(name + " unlocked.");
    }

    public void LockDoor()
    {
        isUnlocked = false;
        isOpen = false;
    }

    public void CompleteStage()
    {
        isUnlocked = true;
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
        if (!playerIsNear)
        {
            return;
        }

        string message;

        if (!isUnlocked)
        {
            message = "This stage is locked";
        }
        else
        {
            message = isOpen
                ? "Press O to close"
                : "Press O to open";
        }

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 26;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;

        GUI.Label(
            new Rect(
                Screen.width / 2f - 250f,
                Screen.height - 130f,
                500f,
                50f
            ),
            message,
            style
        );
    }
}