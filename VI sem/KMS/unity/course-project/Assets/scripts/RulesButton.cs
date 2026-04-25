using UnityEngine;
using TMPro;

public class RulesButton : MonoBehaviour
{
    public GameObject rulesText;
    private bool isTextVisible = false;

    public void ToggleRules()
    {
        isTextVisible = !isTextVisible;
        rulesText.SetActive(isTextVisible);
    }
}