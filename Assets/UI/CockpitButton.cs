using UnityEngine;
using TMPro;

public class CockpitButton : PhysicalButton
{
    [SerializeField]
    private Color activeTextColor = default;

    [SerializeField]
    private Color inactiveTextColor = default;

    [SerializeField]
    protected Material activeMaterial = default;

    [SerializeField]
    private Material inactiveMaterial = default;

    [SerializeField]
    private TextMeshProUGUI buttonTitle = default;

    [SerializeField]
    private MeshRenderer buttonRenderer = default;

    protected bool allowInteraction { private set; get; }


    public void EnableButton()
    {
        buttonTitle.color = activeTextColor;
        SetMaterial(activeMaterial);
        allowInteraction = true;
    }

    public void DisableButton()
    {
        buttonTitle.color = inactiveTextColor;
        SetMaterial(inactiveMaterial);
        allowInteraction = false;
    }

    protected void SetMaterial(Material mat, bool overrideActive = false)
    {
        if (overrideActive)
        {
            activeMaterial = mat;
        }
        buttonRenderer.material = mat;
    }

    public void Press()
    {
        if (allowInteraction)
        {
            OnPress();
            OnPressed.Invoke();
        }
    }

    protected virtual void OnPress() { }
}