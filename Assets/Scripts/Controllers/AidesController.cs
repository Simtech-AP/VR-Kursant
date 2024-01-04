using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Specifies groups of aids 
/// </summary>
public enum HintType
{
    Cabinet = 0,
    Casette = 1,
    Console = 2,
    Pendant = 3
}

/// <summary>
/// Controller managing every hint logic and visbility
/// </summary>
public class AidesController : Controller
{
    /// <summary>
    /// Reference for language controller 
    /// </summary>
    private LanguageController languageController = default;

    /// <summary>
    /// Container wih references to aides parent objects.
    /// Replace with Dictionary if there's no need to expose this in the inspector
    /// </summary>
    [SerializeField]
    private List<HintContainer> hintContainers = default;

    /// <summary>
    /// Get reference to language controller
    /// Set texts in desired language
    /// </summary>
    private void Start()
    {
        SetAllTexts();
    }

    private void SetAllTexts()
    {
        languageController = ControllersManager.Instance.GetController<LanguageController>();

        foreach (var container in hintContainers)
        {
            foreach (var hint in container.Hints)
            {
                try
                {
                    var title = languageController.GetByName(hint.Id).title;
                    var text = languageController.GetByName(hint.Id).text;
                    hint.SetTexts(title, text);
                }
                catch (NullReferenceException _)
                {
                    Debug.LogError("Texts for hint " + hint.Id + " not found.");
                }
            }
        }
    }



    /// <summary>
    /// Set aid state for specified type
    /// </summary>
    /// <param name="aidType">Hint type to set state to</param>    
    /// <param name="state">Should aid be enabled or disabled</param>
    public void SetStateForAidType(HintType aidType, bool state)
    {
        hintContainers.Find(x => x.HintType == aidType).SetHintState(state);
    }

    public void SetStateForAidType(HintType aidType, string id, bool state)
    {
        hintContainers.Find(x => x.HintType == aidType).SetHintState(id, state);
    }

    /// <summary>
    /// Enable all types of hints
    /// </summary>
    public void EnableAllAides()
    {
        hintContainers.ForEach(x => x.SetHintState(true));
    }

    /// <summary>
    /// Disable all types of hints
    /// </summary>
    public void DisableAllAides()
    {
        hintContainers.ForEach(x => x.SetHintState(false));
    }

    public void RefreshTexts()
    {
        SetAllTexts();
    }
}
