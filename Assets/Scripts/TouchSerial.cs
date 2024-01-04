using UnityEngine;

/// <summary>
/// State of touch screen touches
/// </summary>
[System.Serializable]
public struct TouchScreenState
{
    public int X;
    public int Y;
    public bool leftHand;
    public bool touching;
}

/// <summary>
/// Class for logic of touch screen mounted on previous version of pendant
/// </summary>
// TODO: Since we are not using this pendants anymore do we even need this?
public class TouchSerial : MonoBehaviour
{
    /// <summary>
    /// Current state of touch screen
    /// </summary>
    public TouchScreenState state = new TouchScreenState();
    /// <summary>
    /// Is a pressed action active?
    /// </summary>
    public bool pressed = false;
    /// <summary>
    /// Is there any fingers on the screen?
    /// </summary>
    public bool touched = false;
    /// <summary>
    /// Has a finger been taken off the screen?
    /// </summary>
    public bool up = false;
    /// <summary>
    /// X value of pressed point
    /// </summary>
    public float pressedX = 0;
    /// <summary>
    /// Y value of pressed point
    /// </summary>
    public float pressedY = 0;
    /// <summary>
    /// Timer for detecting double tap
    /// </summary>
    private float timer = 0f;
    /// <summary>
    /// Maximum delay between screen touches to count as a press
    /// </summary>
    [SerializeField]
    private float doubleClickMaxDelay = 0.4f;

    /// <summary>
    /// Method deconstructing data to parse it to visual representation
    /// </summary>
    /// <param name="state">Current state of touch screen</param>
    public void OnMessageArrived(TouchScreenState state)
    {
        ParseMessage(state);
    }

    /// <summary>
    /// Parses message to visual
    /// </summary>
    /// <param name="state">Current state of touch screen</param>
    private void ParseMessage(TouchScreenState state)
    {
        if (state.touching && !touched)
        {
            timer = 0f;
            touched = true;
            up = false;
            pressed = false;
        }
        this.state = state;
    }

    /// <summary>
    /// Updates visuals in real time
    /// Detects doubletap
    /// </summary>
    private void Update()
    {
        timer += Time.deltaTime;
        if (!(state.touching))
        {
            if (timer <= doubleClickMaxDelay) up = true;
            pressed = false;
        }
        else if (state.touching)
        {
            if (up)
            {
                pressed = true;
                pressedX = state.X;
                pressedY = state.Y;
            }
        }
        if (timer >= doubleClickMaxDelay)
        {
            touched = false;
            up = false;
        }
    }
}
