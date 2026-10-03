using UnityEngine;
using UnityEngine.SceneManagement;

public enum ProfessionType
{
    None,
    Engineer,
    Doctor
}

public enum StageType
{
    Bachelor,
    Master,
    Researcher,
    PhD
}

public class GameProgressManager : MonoBehaviour
{
    [Header("Current Progress")]
    [SerializeField]
    private ProfessionType selectedProfession =
        ProfessionType.None;

    [SerializeField]
    private bool professionSelected;

    [Header("Stage Doors")]
    public StageSlidingDoor bachelorDoor;
    public StageSlidingDoor masterDoor;
    public StageSlidingDoor researcherDoor;
    public StageSlidingDoor phdDoor;

    [Header("Stage Sphere Renderers")]
    public Renderer bachelorSphere;
    public Renderer masterSphere;
    public Renderer researcherSphere;
    public Renderer phdSphere;

    [Header("Final Room")]
    public FinalRoomController finalRoomController;

    [Header("Sphere Materials")]
    public Material lockedMaterial;
    public Material availableMaterial;
    public Material completedMaterial;

    public ProfessionType SelectedProfession
    {
        get
        {
            return selectedProfession;
        }
    }

    public bool ProfessionSelected
    {
        get
        {
            return professionSelected;
        }
    }

    void Start()
    {
        ResetProgress();
    }

    public void SelectProfession(
        ProfessionType profession)
    {
        if (professionSelected)
        {
            Debug.Log(
                "A profession has already been selected."
            );

            return;
        }

        if (profession == ProfessionType.None)
        {
            Debug.LogWarning(
                "A valid profession must be selected."
            );

            return;
        }

        selectedProfession = profession;
        professionSelected = true;

        UnlockBachelorStage();

        Debug.Log(
            "Profession selected: " +
            selectedProfession
        );
    }

    void UnlockBachelorStage()
    {
        SetSphereAvailable(bachelorSphere);

        if (bachelorDoor != null)
        {
            bachelorDoor.UnlockDoor();
        }

        Debug.Log("Bachelor stage unlocked.");
    }

    public void CompleteStage(StageType stage)
    {
        switch (stage)
        {
            case StageType.Bachelor:

                SetSphereCompleted(
                    bachelorSphere
                );

                SetSphereAvailable(
                    masterSphere
                );

                if (masterDoor != null)
                {
                    masterDoor.UnlockDoor();
                }

                Debug.Log(
                    "Bachelor completed. " +
                    "Master unlocked."
                );

                break;

            case StageType.Master:

                SetSphereCompleted(
                    masterSphere
                );

                SetSphereAvailable(
                    researcherSphere
                );

                if (researcherDoor != null)
                {
                    researcherDoor.UnlockDoor();
                }

                Debug.Log(
                    "Master completed. " +
                    "Researcher unlocked."
                );

                break;

            case StageType.Researcher:

                SetSphereCompleted(
                    researcherSphere
                );

                SetSphereAvailable(
                    phdSphere
                );

                if (phdDoor != null)
                {
                    phdDoor.UnlockDoor();
                }

                Debug.Log(
                    "Researcher completed. " +
                    "PhD unlocked."
                );

                break;

            case StageType.PhD:

                SetSphereCompleted(
                    phdSphere
                );

                if (finalRoomController != null)
                {
                    finalRoomController.UnlockFinalRoom();
                }
                else
                {
                    Debug.LogWarning(
                        "Final Room Controller " +
                        "is not assigned."
                    );
                }

                Debug.Log(
                    "PhD completed. " +
                    "Final room unlocked."
                );

                break;
        }
    }

    void SetSphereLocked(Renderer sphere)
    {
        if (sphere == null ||
            lockedMaterial == null)
        {
            return;
        }

        sphere.material =
            lockedMaterial;
    }

    void SetSphereAvailable(Renderer sphere)
    {
        if (sphere == null ||
            availableMaterial == null)
        {
            return;
        }

        sphere.material =
            availableMaterial;
    }

    void SetSphereCompleted(Renderer sphere)
    {
        if (sphere == null ||
            completedMaterial == null)
        {
            return;
        }

        sphere.material =
            completedMaterial;
    }

    public void ResetProgress()
    {
        selectedProfession =
            ProfessionType.None;

        professionSelected = false;

        SetSphereLocked(
            bachelorSphere
        );

        SetSphereLocked(
            masterSphere
        );

        SetSphereLocked(
            researcherSphere
        );

        SetSphereLocked(
            phdSphere
        );

        if (bachelorDoor != null)
        {
            bachelorDoor.LockDoor();
        }

        if (masterDoor != null)
        {
            masterDoor.LockDoor();
        }

        if (researcherDoor != null)
        {
            researcherDoor.LockDoor();
        }

        if (phdDoor != null)
        {
            phdDoor.LockDoor();
        }

        Debug.Log("Game progress reset.");
    }

    public void GameOver()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}