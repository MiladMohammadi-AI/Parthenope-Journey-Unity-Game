using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class QuizQuestion
{
    [TextArea(2, 4)]
    public string question;

    public string answer1;
    public string answer2;
    public string answer3;
    public string answer4;

    [Range(1, 4)]
    public int correctAnswer = 1;
}

public class StageQuizRoom : MonoBehaviour
{
    [Header("Stage")]
    public StageType stageType;

    [Header("Game References")]
    public GameProgressManager progressManager;
    public PlayerMovement playerMovement;

    [Header("Cameras")]
    public Camera mainCamera;
    public Camera quizCamera;

    [Header("Whiteboard")]
    public TMP_Text whiteboardText;

    [Header("Engineer Questions")]
    public QuizQuestion[] engineerQuestions;

    [Header("Doctor Questions")]
    public QuizQuestion[] doctorQuestions;

    [Header("Quiz Settings")]
    public float timeLimit = 240f;
    public KeyCode startKey = KeyCode.E;

    [Header("Feedback Settings")]
    public float feedbackDuration = 1.5f;

    [Header("Result Settings")]
    public float successDisplayDuration = 3f;
    public float gameOverDelay = 4f;

    private QuizQuestion[] activeQuestions;

    private bool playerIsNear;
    private bool quizStarted;
    private bool quizFinished;
    private bool showingFeedback;
    private bool restartingGame;

    private int currentQuestion;
    private int correctAnswers;
    private int answeredQuestions;

    private float remainingTime;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (quizCamera != null)
        {
            quizCamera.enabled = false;
        }

        ShowWaitingMessage();
    }

    void Update()
    {
        if (restartingGame)
        {
            return;
        }

        // داخل محدوده هستیم و با E آزمون را شروع می‌کنیم.
        if (playerIsNear &&
            !quizStarted &&
            !quizFinished &&
            Input.GetKeyDown(startKey))
        {
            StartQuiz();
        }

        if (!quizStarted ||
            quizFinished ||
            showingFeedback)
        {
            return;
        }

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            FinishQuizByTime();
            return;
        }

        UpdateQuestionBoard();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SubmitAnswer(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SubmitAnswer(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SubmitAnswer(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SubmitAnswer(4);
        }
    }

    void StartQuiz()
    {
        if (quizStarted ||
            quizFinished ||
            restartingGame)
        {
            return;
        }

        if (progressManager == null)
        {
            Debug.LogError(
                name +
                ": Progress Manager is missing."
            );

            return;
        }

        if (!progressManager.ProfessionSelected)
        {
            if (whiteboardText != null)
            {
                whiteboardText.text =
                    "<color=#FFCC55><b>" +
                    "PROFESSION REQUIRED" +
                    "</b></color>\n\n" +

                    "Choose your profession\n" +
                    "at the reception first.";
            }

            return;
        }

        if (progressManager.SelectedProfession ==
            ProfessionType.Engineer)
        {
            activeQuestions =
                engineerQuestions;
        }
        else
        {
            activeQuestions =
                doctorQuestions;
        }

        if (activeQuestions == null ||
            activeQuestions.Length < 4)
        {
            Debug.LogError(
                name +
                ": At least 4 questions are required."
            );

            if (whiteboardText != null)
            {
                whiteboardText.text =
                    "<color=#FF5555><b>" +
                    "QUIZ ERROR" +
                    "</b></color>\n\n" +

                    "Four questions are required.";
            }

            return;
        }

        quizStarted = true;
        quizFinished = false;
        showingFeedback = false;

        currentQuestion = 0;
        correctAnswers = 0;
        answeredQuestions = 0;

        remainingTime = timeLimit;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        SetQuizCamera(true);

        UpdateQuestionBoard();

        Debug.Log(
            stageType +
            " quiz started."
        );
    }

    void SubmitAnswer(int selectedAnswer)
    {
        if (!quizStarted ||
            quizFinished ||
            showingFeedback ||
            restartingGame ||
            activeQuestions == null ||
            currentQuestion >= 4)
        {
            return;
        }

        QuizQuestion question =
            activeQuestions[currentQuestion];

        bool isCorrect =
            selectedAnswer ==
            question.correctAnswer;

        showingFeedback = true;

        if (isCorrect)
        {
            correctAnswers++;
        }

        answeredQuestions++;

        StartCoroutine(
            ShowAnswerFeedback(
                selectedAnswer,
                isCorrect,
                question
            )
        );
    }

    IEnumerator ShowAnswerFeedback(
        int selectedAnswer,
        bool isCorrect,
        QuizQuestion question)
    {
        if (whiteboardText != null)
        {
            string feedback;

            if (isCorrect)
            {
                feedback =
                    "<color=#55FF78>" +
                    "<size=140%><b>CORRECT!</b></size>" +
                    "</color>\n\n" +

                    "Your answer:\n" +
                    selectedAnswer +
                    " - " +
                    GetAnswerText(
                        question,
                        selectedAnswer
                    );
            }
            else
            {
                feedback =
                    "<color=#FF5555>" +
                    "<size=140%><b>WRONG!</b></size>" +
                    "</color>\n\n" +

                    "Your answer:\n" +
                    selectedAnswer +
                    " - " +
                    GetAnswerText(
                        question,
                        selectedAnswer
                    ) +
                    "\n\n" +

                    "<color=#55FF78>" +
                    "Correct answer:\n" +
                    question.correctAnswer +
                    " - " +
                    GetAnswerText(
                        question,
                        question.correctAnswer
                    ) +
                    "</color>";
            }

            whiteboardText.text =
                GetStageTitle() +
                "\n" +

                GetPassRuleDisplay() +
                "\n\n" +

                feedback +
                "\n\n" +

                "<size=70%>" +
                "Current Score: " +
                correctAnswers +
                " / " +
                answeredQuestions +
                "</size>";
        }

        yield return new WaitForSeconds(
            feedbackDuration
        );

        currentQuestion++;

        if (answeredQuestions >= 4)
        {
            CheckFinalResult();
        }
        else
        {
            showingFeedback = false;
            UpdateQuestionBoard();
        }
    }

    void CheckFinalResult()
    {
        int requiredAnswers =
            GetRequiredCorrectAnswers();

        bool passed =
            correctAnswers >= requiredAnswers;

        quizStarted = false;
        quizFinished = true;
        showingFeedback = false;

        if (passed)
        {
            CompleteStage();

            StartCoroutine(
                ReturnToPlayerAfterSuccess()
            );
        }
        else
        {
            ShowGameOverResult(
                "You did not reach the required score."
            );

            StartCoroutine(
                RestartAfterGameOver()
            );
        }
    }

    void CompleteStage()
    {
        if (progressManager != null)
        {
            progressManager.CompleteStage(
                stageType
            );
        }

        ShowPassedResult();

        Debug.Log(
            stageType +
            " completed with " +
            correctAnswers +
            " correct answers."
        );
    }

    IEnumerator ReturnToPlayerAfterSuccess()
    {
        yield return new WaitForSeconds(
            successDisplayDuration
        );

        SetQuizCamera(false);

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }

    void FinishQuizByTime()
    {
        quizStarted = false;
        quizFinished = true;
        showingFeedback = false;

        ShowGameOverResult(
            "Time is over."
        );

        StartCoroutine(
            RestartAfterGameOver()
        );

        Debug.Log(
            stageType +
            " failed because time expired."
        );
    }

    void ShowPassedResult()
    {
        if (whiteboardText == null)
        {
            return;
        }

        whiteboardText.text =
            GetStageTitle() +
            "\n\n" +

            "<color=#55FF78>" +
            "<size=140%>" +
            "<b>STAGE COMPLETED!</b>" +
            "</size>" +
            "</color>\n\n" +

            "Final Score: " +
            correctAnswers +
            " / 4\n\n" +

            "Required Score: " +
            GetRequiredCorrectAnswers() +
            " / 4\n\n" +

            "<color=#55FF78>" +
            "<b>PASSED</b>" +
            "</color>\n\n" +

            "<size=70%>" +
            "The next stage is now unlocked." +
            "</size>";
    }

    void ShowGameOverResult(string reason)
    {
        if (whiteboardText == null)
        {
            return;
        }

        whiteboardText.text =
            GetStageTitle() +
            "\n\n" +

            "<color=#FF3333>" +
            "<size=160%>" +
            "<b>GAME OVER</b>" +
            "</size>" +
            "</color>\n\n" +

            "<color=#FFCC55>" +
            reason +
            "</color>\n\n" +

            "Final Score: " +
            correctAnswers +
            " / 4\n\n" +

            "Required Score: " +
            GetRequiredCorrectAnswers() +
            " / 4\n\n" +

            "<size=65%>" +
            "Restarting the game..." +
            "</size>";
    }

    IEnumerator RestartAfterGameOver()
    {
        restartingGame = true;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        yield return new WaitForSeconds(
            gameOverDelay
        );

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    void UpdateQuestionBoard()
    {
        if (whiteboardText == null ||
            activeQuestions == null ||
            currentQuestion >= 4)
        {
            return;
        }

        QuizQuestion question =
            activeQuestions[currentQuestion];

        int minutes =
            Mathf.FloorToInt(
                remainingTime / 60f
            );

        int seconds =
            Mathf.FloorToInt(
                remainingTime % 60f
            );

        whiteboardText.text =
            GetStageTitle() +
            "\n" +

            GetPassRuleDisplay() +
            "\n" +

            "<size=65%>" +
            "Question " +
            (currentQuestion + 1) +
            " / 4" +

            "   |   Time " +
            minutes.ToString("00") +
            ":" +
            seconds.ToString("00") +

            "   |   Score " +
            correctAnswers +
            " / " +
            answeredQuestions +
            "</size>\n\n" +

            "<b>" +
            question.question +
            "</b>\n\n" +

            "<color=#A8D8FF>1.</color> " +
            question.answer1 +
            "\n" +

            "<color=#A8D8FF>2.</color> " +
            question.answer2 +
            "\n" +

            "<color=#A8D8FF>3.</color> " +
            question.answer3 +
            "\n" +

            "<color=#A8D8FF>4.</color> " +
            question.answer4 +
            "\n\n" +

            "<size=60%>" +
            "Press 1, 2, 3 or 4 to answer" +
            "</size>";
    }

    void SetQuizCamera(bool showQuizCamera)
    {
        if (showQuizCamera)
        {
            if (quizCamera == null)
            {
                Debug.LogError(
                    name +
                    ": Quiz Camera is not assigned."
                );

                return;
            }

            // ابتدا دوربین آزمون روشن می‌شود.
            quizCamera.gameObject.SetActive(true);
            quizCamera.enabled = true;

            // سپس دوربین اصلی خاموش می‌شود.
            if (mainCamera != null)
            {
                mainCamera.enabled = false;
            }
        }
        else
        {
            // ابتدا دوربین اصلی روشن می‌شود.
            if (mainCamera != null)
            {
                mainCamera.enabled = true;
            }

            // سپس دوربین آزمون خاموش می‌شود.
            if (quizCamera != null)
            {
                quizCamera.enabled = false;
            }
        }
    }

    string GetStageTitle()
    {
        return
            "<color=#FFD54A><b>" +
            stageType.ToString().ToUpper() +
            " EXAM</b></color>";
    }

    string GetPassRuleDisplay()
    {
        return
            "<size=65%>" +
            "<color=#55FF78>" +
            "PASS RULE: " +
            GetPassRuleText() +
            "</color>" +
            "</size>";
    }

    string GetPassRuleText()
    {
        switch (stageType)
        {
            case StageType.Bachelor:
                return
                    "Get at least 1 correct answer";

            case StageType.Master:
                return
                    "Get at least 2 correct answers";

            case StageType.Researcher:
                return
                    "Get at least 3 correct answers";

            case StageType.PhD:
                return
                    "Answer all 4 questions correctly";

            default:
                return "";
        }
    }

    int GetRequiredCorrectAnswers()
    {
        switch (stageType)
        {
            case StageType.Bachelor:
                return 1;

            case StageType.Master:
                return 2;

            case StageType.Researcher:
                return 3;

            case StageType.PhD:
                return 4;

            default:
                return 1;
        }
    }

    string GetAnswerText(
        QuizQuestion question,
        int answerNumber)
    {
        switch (answerNumber)
        {
            case 1:
                return question.answer1;

            case 2:
                return question.answer2;

            case 3:
                return question.answer3;

            case 4:
                return question.answer4;

            default:
                return "";
        }
    }

    void ShowWaitingMessage()
    {
        if (whiteboardText == null)
        {
            return;
        }

        whiteboardText.text =
            "<color=#FFD54A><b>" +
            stageType.ToString().ToUpper() +
            " CLASSROOM</b></color>\n\n" +

            GetPassRuleDisplay() +
            "\n\n" +

            "Enter the exam area\n" +
            "and press E to start.\n\n" +

            "<size=65%>" +
            "You have 4 minutes and 4 questions." +
            "</size>";
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
        if (restartingGame)
        {
            return;
        }

        GUIStyle style =
            new GUIStyle(GUI.skin.label);

        style.fontSize = 24;
        style.alignment =
            TextAnchor.MiddleCenter;

        style.normal.textColor =
            Color.white;

        if (playerIsNear &&
            !quizStarted &&
            !quizFinished)
        {
            GUI.Label(
                new Rect(
                    Screen.width / 2f - 320f,
                    Screen.height - 100f,
                    640f,
                    50f
                ),
                "Press E to start the exam",
                style
            );
        }

        if (quizStarted &&
            !quizFinished &&
            !showingFeedback)
        {
            GUI.Label(
                new Rect(
                    Screen.width / 2f - 320f,
                    Screen.height - 80f,
                    640f,
                    40f
                ),
                "Press 1, 2, 3 or 4 to answer",
                style
            );
        }
    }
}