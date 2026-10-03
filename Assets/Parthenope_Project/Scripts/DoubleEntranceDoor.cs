using UnityEngine;

public class DoubleEntranceDoor : MonoBehaviour
{
    [Header("Sliding Doors")]
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Door Settings")]
    public float slideDistance = 1.5f;
    public float openSpeed = 3f;

    [Header("Interaction")]
    public KeyCode interactionKey = KeyCode.O;

    private bool playerIsNear;
    private bool isOpen;

    private Vector3 leftClosedPosition;
    private Vector3 rightClosedPosition;

    private Vector3 leftOpenPosition;
    private Vector3 rightOpenPosition;

    void Start()
    {
        leftClosedPosition = leftDoor.localPosition;
        rightClosedPosition = rightDoor.localPosition;

        leftOpenPosition =
            leftClosedPosition + Vector3.left * slideDistance;

        rightOpenPosition =
            rightClosedPosition + Vector3.right * slideDistance;
    }

    void Update()
    {
        if (playerIsNear && Input.GetKeyDown(interactionKey))
        {
            isOpen = !isOpen;
        }

        Vector3 leftTarget =
            isOpen ? leftOpenPosition : leftClosedPosition;

        Vector3 rightTarget =
            isOpen ? rightOpenPosition : rightClosedPosition;

        leftDoor.localPosition = Vector3.MoveTowards(
            leftDoor.localPosition,
            leftTarget,
            openSpeed * Time.deltaTime
        );

        rightDoor.localPosition = Vector3.MoveTowards(
            rightDoor.localPosition,
            rightTarget,
            openSpeed * Time.deltaTime
        );
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

        string message = isOpen
            ? "Press O to close"
            : "Press O to open";

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 28;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;

        GUI.Label(
            new Rect(
                Screen.width / 2f - 200f,
                Screen.height - 130f,
                400f,
                50f
            ),
            message,
            style
        );
    }
}