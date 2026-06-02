using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ItemButton : MonoBehaviour
{
    public ItemBox targetBox;
    public int itemIndex;

    void Start()
    {
        var interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
            interactable.selectEntered.AddListener(OnButtonPressed);
    }

    void OnButtonPressed(SelectEnterEventArgs args)
    {
        targetBox?.SelectItem(itemIndex);
    }
}