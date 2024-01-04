using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PushDirection
{
    X = 0,
    Y = 1,
    Z = 2
}

public class VisualButtonReposition : MonoBehaviour
{

    [SerializeField] private float pushedOffset = 0f;
    [SerializeField] private float feedbackDuration = 0f;
    [SerializeField] private Transform buttonVisual = default;
    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = buttonVisual.transform.localPosition;
    }

    [EnumAction(typeof(PushDirection))]
    public async void OnClick(int direction)
    {
        Vector3 pushedInPosition = default;
        switch ((PushDirection)direction)
        {
            case PushDirection.X:
                {
                    pushedInPosition = new Vector3(originalPosition.x + pushedOffset, originalPosition.y, originalPosition.z);
                    break;
                }

            case PushDirection.Y:
                {
                    pushedInPosition = new Vector3(originalPosition.x, originalPosition.y + pushedOffset, originalPosition.z);
                    break;
                }

            case PushDirection.Z:
                {
                    pushedInPosition = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z + pushedOffset);
                    break;
                }
            default:
                {
                    Debug.LogError("Unknown direction: " + direction);
                    break;
                }
        }


        buttonVisual.localPosition = pushedInPosition;
        await System.Threading.Tasks.Task.Delay((int)(feedbackDuration * 1000));
        buttonVisual.localPosition = originalPosition;
    }
}
