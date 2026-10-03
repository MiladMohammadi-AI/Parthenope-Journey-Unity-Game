using UnityEngine;

public class ReceptionInteraction : MonoBehaviour
{
    [Header("Player References")]
    public CharacterSwitcher characterSwitcher;
    public PlayerMovement playerMovement;

    [Header("Game Progress")]
    public GameProgressManager progressManager;

    [Header("Profession UI")]
    public GameObject professionPanel;

    private bool playerIsNear;
    private bool menuIsOpen;
    private bool professionSelected;

    private void Start()
    {
        // پنل انتخاب شغل در شروع بازی بسته باشد
        if (professionPanel != null)
        {
            professionPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // باز و بسته کردن منوی انتخاب شغل با E
        if (playerIsNear &&
            !professionSelected &&
            Input.GetKeyDown(KeyCode.E))
        {
            if (menuIsOpen)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }

        if (!menuIsOpen)
        {
            return;
        }

        // انتخاب Engineer با عدد 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectEngineer();
        }

        // انتخاب Doctor با عدد 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectDoctor();
        }

        // بستن منو با Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenu();
        }
    }

    private void OpenMenu()
    {
        menuIsOpen = true;

        if (professionPanel != null)
        {
            professionPanel.SetActive(true);
        }

        SetPlayerMovement(false);

        // نمایش و آزادکردن نشانگر ماوس
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void SelectEngineer()
    {
        if (professionSelected)
        {
            return;
        }

        if (characterSwitcher != null)
        {
            characterSwitcher.ShowSuit();
        }

        if (progressManager != null)
        {
            progressManager.SelectProfession(
                ProfessionType.Engineer
            );
        }

        FinishProfessionSelection();
    }

    public void SelectDoctor()
    {
        if (professionSelected)
        {
            return;
        }

        if (characterSwitcher != null)
        {
            characterSwitcher.ShowDoctor();
        }

        if (progressManager != null)
        {
            progressManager.SelectProfession(
                ProfessionType.Doctor
            );
        }

        FinishProfessionSelection();
    }

    private void FinishProfessionSelection()
    {
        professionSelected = true;

        CloseMenu();

        Debug.Log(
            "Profession selected. Bachelor stage unlocked."
        );
    }

    public void CloseMenu()
    {
        menuIsOpen = false;

        if (professionPanel != null)
        {
            professionPanel.SetActive(false);
        }

        SetPlayerMovement(true);

        // مخفی و قفل‌کردن نشانگر ماوس
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void SetPlayerMovement(bool canMove)
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = canMove;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;

            if (!professionSelected)
            {
                CloseMenu();
            }
        }
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);

        style.fontSize = 27;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;

        // پیام نزدیک Reception
        if (playerIsNear &&
            !menuIsOpen &&
            !professionSelected)
        {
            GUI.Label(
                new Rect(
                    Screen.width / 2f - 300f,
                    Screen.height - 130f,
                    600f,
                    50f
                ),
                "Press E to choose your profession",
                style
            );
        }

        // پیام بعد از انتخاب شغل
        if (playerIsNear &&
            professionSelected &&
            !menuIsOpen)
        {
            GUI.Label(
                new Rect(
                    Screen.width / 2f - 300f,
                    Screen.height - 130f,
                    600f,
                    50f
                ),
                "Profession already selected",
                style
            );
        }
    }
}