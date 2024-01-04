using UnityEngine;

/// <summary>
/// Clas helping going to next step using only pendant
/// </summary>
public class PendantNextStep : MonoBehaviour
{
    /// <summary>
    /// Counter for pressed deadman switches
    /// </summary>
    private int deadmanClickCount = 0;
    /// <summary>
    /// Flag for checking if we can change to next step after next deadman press
    /// </summary>
    private bool nextDeadmanFlag = false;
    /// <summary>
    /// Timer counting pauses between dedaman presses
    /// </summary>
    private float timer = 0f;

    /// <summary>
    /// Main logic for changing to next step
    /// Counts time between presses
    /// Sets up necessary flag
    /// </summary>
    private void Update()
    {
        if (deadmanClickCount > 0)
        {
            timer += Time.deltaTime;
            nextDeadmanFlag = true;
            if (timer > 0.4f)
            {
                nextDeadmanFlag = false;
                deadmanClickCount = 0;
                timer = 0f;
            }
        }
        else
        {
            nextDeadmanFlag = false;
        }
    }

    /// <summary>
    /// Goes to the next step on deadman press if possible
    /// </summary>
    public void CheckDeadmanNextStep()
    {
        deadmanClickCount++;
        if (nextDeadmanFlag && deadmanClickCount >= 2)
        {
            NextStepButton nextStepButton = FindObjectOfType<NextStepButton>();
            if(nextStepButton)
                nextStepButton.GetComponent<Interactable>().Interact(null);
            timer = 0f;
            deadmanClickCount = 0;
        }
    }
}
