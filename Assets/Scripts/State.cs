/// <summary>
/// Template class for custom state enums
/// </summary>
public abstract class State
{ }

/// <summary>
/// State of bumper cap
/// </summary>

class BumperCapState : State
{
    public const int OnDoor = 0, InHand = 1, OnHead = 2;
}

/// <summary>
/// State of padlock
/// </summary>
class PadLockState : State
{
    public const int InLockBox = 0, InHand = 1, OnDoor = 2;
}

/// <summary>
/// State of cell entrance
/// </summary>
class CellEntranceState : State
{
    public const int Closed = 0, Moving = 1, Stopped = 2, Repealed = 3;
}

/// <summary>
/// State of EStop button
/// </summary>
class EStopButtonState : State
{
    public const int Released = 0, Pressed = 1;
}

class PlayerLocation : State
{
    public const int Outside = 0, Inside = 1;
}