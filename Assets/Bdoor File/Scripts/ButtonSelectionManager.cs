using UnityEngine;
using UnityEngine.UI;
using Abdulaziz_File.Scripts;

public class ButtonSelectionManager : MonoBehaviour
{
    private Selector selector;
    private Button currentlySelectedButton;

    private void Awake()
    {
        selector = GetComponent<Selector>();
        if (selector == null)
        {
            Debug.LogError("Selector script not found!");
        }
    }

    private void Start()
    {
        if (selector != null)
        {
            selector.OnItemSelected += HandleItemSelected;
        }
    }

    private void HandleItemSelected(Mechanic? mechanic)
    {
        if (!mechanic.HasValue) return;

        Button selectedButton = GetButtonFromSelector(mechanic.Value);

        if (selectedButton != null)
        {
            UpdateButtonState(selectedButton);
        }
    }

    private Button GetButtonFromSelector(Mechanic mechanic)
    {
        switch (mechanic)
        {
            case Mechanic.Mirror:
                return selector.MirrorButton;
            case Mechanic.Splitter:
                return selector.SplitterButton;
            case Mechanic.Splitter_RGB:
                return selector.SplitterRgbButton;
            case Mechanic.Delete:
                return selector.UndoButton;
            default:
                return null;
        }
    }

    private void UpdateButtonState(Button button)
    {
        if (currentlySelectedButton != null && currentlySelectedButton != button)
        {
            ResetButtonState(currentlySelectedButton);
        }

        SetButtonAsSelected(button);
    }

    private void SetButtonAsSelected(Button button)
    {
        currentlySelectedButton = button;
        var spriteState = button.spriteState;
        button.image.sprite = spriteState.selectedSprite;
    }

    private void ResetButtonState(Button button)
    {
        var spriteState = button.spriteState;

        button.image.sprite = spriteState.highlightedSprite;

        if (button.targetGraphic is Image targetImage)
        {
            targetImage.sprite = spriteState.highlightedSprite; 
            targetImage.type = Image.Type.Sliced; 
        }
    }
}