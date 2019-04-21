using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractableUI : MonoBehaviour
{
    public GameObject interactableUIPanel;
    public Image icon;
    public TextMeshProUGUI text;

    private void Awake()
    {
        Hide();
    }

    public void Init(Sprite sprite, string desc)
    {
        icon.sprite = sprite;
        text.text = desc;
    }

    public void Show()
    {
        interactableUIPanel.SetActive(true);
    }

    public void Hide()
    {
        interactableUIPanel.SetActive(false);
    }
}