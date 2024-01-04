using DG.Tweening;
using UnityEngine;

/// <summary>
/// Controls logic of drawer object
/// </summary>
public class Drawer : MonoBehaviour
{
    /// <summary>
    /// Default value of Z axis when drawer is opened
    /// </summary>
    public float DefaultOpenedPositionZ = 0.355f;
    /// <summary>
    /// Default value of Z axis when drawer is closed
    /// </summary>
    public float DefaultClosedPositionZ = -0.035f;

    /// <summary>
    /// States of drawer
    /// </summary>
    public enum DrawerState
    {
        OPENED,
        CLOSED
    }

    /// <summary>
    /// Current drawer state
    /// </summary>
    public DrawerState State;

    /// <summary>
    /// Sets up 
    /// </summary>
    private void Awake()
    {
        State = DrawerState.CLOSED;
    }

    /// <summary>
    /// Allows to interact with drawer
    /// </summary>
    public void Interact()
    {
        if (State == DrawerState.OPENED)
        {
            Close();
        }
        else if (State == DrawerState.CLOSED)
        {
            Open();
        }
    }

    /// <summary>
    /// Opens drawer
    /// </summary>
    public void Open()
    {
        State = DrawerState.OPENED;
        transform.DOLocalMoveZ(DefaultOpenedPositionZ, 1f);
    }

    /// <summary>
    /// Closes drawer
    /// </summary>
    public void Close()
    {
        State = DrawerState.CLOSED;
        transform.DOLocalMoveZ(DefaultClosedPositionZ, 1f);
    }
}
