using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{
    public GameObject[] characters;
    private int currentIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchCharacter();
        }
    }

    void SwitchCharacter()
    {
        characters[currentIndex].SetActive(false);
        currentIndex = (currentIndex + 1) % characters.Length;
        characters[currentIndex].SetActive(true);
    }
}