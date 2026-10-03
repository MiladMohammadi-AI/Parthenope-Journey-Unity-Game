using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{
    [Header("Character Models")]
    public GameObject studentCharacter;
    public GameObject suitCharacter;
    public GameObject doctorCharacter;

    void Start()
    {
        ShowStudent();
    }

    public void ShowStudent()
    {
        studentCharacter.SetActive(true);
        suitCharacter.SetActive(false);
        doctorCharacter.SetActive(false);
    }

    public void ShowSuit()
    {
        studentCharacter.SetActive(false);
        suitCharacter.SetActive(true);
        doctorCharacter.SetActive(false);
    }

    public void ShowDoctor()
    {
        studentCharacter.SetActive(false);
        suitCharacter.SetActive(false);
        doctorCharacter.SetActive(true);
    }
}