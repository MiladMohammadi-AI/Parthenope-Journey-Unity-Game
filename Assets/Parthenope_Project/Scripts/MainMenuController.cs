using TMPro;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject starterCanvas;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject aboutMePanel;
    [SerializeField] private TMP_Text mainButtonText;

    [Header("Player References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private SimpleCameraFollow cameraFollow;

    private bool gameHasStarted;
    private bool menuIsOpen;

    private void Start()
    {
        ShowStartMenu();
    }

    private void Update()
    {
        if (gameHasStarted &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuIsOpen)
            {
                ContinueGame();
            }
            else
            {
                OpenPauseMenu();
            }
        }
    }

    public void MainAction()
    {
        Debug.Log("MAIN BUTTON CLICKED");

        if (gameHasStarted)
        {
            ContinueGame();
        }
        else
        {
            StartNewGame();
        }
    }

    public void OpenAboutMe()
    {
        Debug.Log("ABOUT ME CLICKED");

        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }

        if (aboutMePanel != null)
        {
            aboutMePanel.SetActive(true);
        }
    }

    public void BackToMenu()
    {
        Debug.Log("BACK CLICKED");

        if (aboutMePanel != null)
        {
            aboutMePanel.SetActive(false);
        }

        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
        }
    }

    public void ExitGame()
    {
        Debug.Log("EXIT GAME CLICKED");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void StartNewGame()
    {
        gameHasStarted = true;

        if (mainButtonText != null)
        {
            mainButtonText.text = "CONTINUE";
        }

        HideMenu();
    }

    private void ContinueGame()
    {
        if (!gameHasStarted)
        {
            return;
        }

        HideMenu();
    }

    private void ShowStartMenu()
    {
        gameHasStarted = false;
        OpenMenu();

        if (mainButtonText != null)
        {
            mainButtonText.text = "NEW GAME";
        }
    }

    private void OpenPauseMenu()
    {
        OpenMenu();

        if (mainButtonText != null)
        {
            mainButtonText.text = "CONTINUE";
        }
    }

    private void OpenMenu()
    {
        menuIsOpen = true;

        if (starterCanvas != null)
        {
            starterCanvas.SetActive(true);
        }

        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
        }

        if (aboutMePanel != null)
        {
            aboutMePanel.SetActive(false);
        }

        SetGameplayEnabled(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HideMenu()
    {
        menuIsOpen = false;

        if (aboutMePanel != null)
        {
            aboutMePanel.SetActive(false);
        }

        if (starterCanvas != null)
        {
            starterCanvas.SetActive(false);
        }

        SetGameplayEnabled(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void SetGameplayEnabled(bool enabledState)
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = enabledState;
        }

        if (cameraFollow != null)
        {
            cameraFollow.enabled = enabledState;
        }
    }
}