using UnityEngine;

public enum CrateTouchObjectType
{
    FLOOR,
    HAND
}

/// <summary>
/// Class for detecting if crate has touched the floor
/// </summary>
public class CrateTouchObject : StepEnabler
{
    [Header("Crate Touch Object")]
    /// <summary>
    /// Crate object to check
    /// </summary>
    [SerializeField]
    private GameObject crate = default;
    [SerializeField] private float waitTime = 0f;
    [SerializeField] private CrateTouchObjectType objectType;
    /// <summary>
    /// Time counter
    /// </summary>
    private float timer;
    private CrateTouch crateTouch;

    protected override void initialize()
    {
        crateTouch = crate.GetComponent<CrateTouch>();
    }

    protected override void onUpdate()
    {
        CheckTouchFloorCondition();
    }

    private void CheckTouchFloorCondition()
    {
        bool isTouching = objectType == CrateTouchObjectType.FLOOR ? crateTouch.touchingFloor : crateTouch.touchingHand;

        if (isTouching)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                Enabled = true;
            }
        }
        else
        {
            Enabled = false;
            timer = 0f;
        }
    }
}
