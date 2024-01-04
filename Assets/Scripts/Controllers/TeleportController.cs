using System;
using System.Collections;
using UnityEngine;
using Valve.VR;

/// <summary>
/// Controller in charge of teleporting user
/// </summary>
public class TeleportController : Controller
{
    /// <summary>
    /// Default position of user
    /// </summary>
    private Vector3 initialPosition;
    /// <summary>
    /// Position inside work cell to teleport to
    /// </summary>
    [SerializeField]
    private Transform insidePosition = default;
    /// <summary>
    /// Position outside work cell to teleport to
    /// </summary>
    [SerializeField]
    private Transform outsidePosition = default;
    /// <summary>
    /// Time for screen to black-out
    /// </summary>
    public float fadeTime = 0.2f;

    public Action OnTeleportInvoked = delegate { };

    /// <summary>
    /// Sets default position to staring position
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        initialPosition = transform.position;
    }


    /// <summary>
    /// Teleports user to specified position
    /// </summary>
    /// <param name="newPosition">Position to teleport to</param>
    private IEnumerator TeleportPlayer(Vector3 newPosition)
    {
        FadeStart();
        yield return new WaitForSeconds(fadeTime);
        transform.position = newPosition;
        FadeEnd();
        yield return new WaitForSeconds(fadeTime);
    }

    /// <summary>
    /// Teleports user to specified position
    /// </summary>
    /// <param name="teleportType">Type of teleport to transfer player to</param>
    /// <returns></returns>
    private IEnumerator TeleportPlayer(TeleportType teleportType)
    {
        Debug.Log(teleportType.ToString());
        FadeStart();
        yield return new WaitForSeconds(fadeTime);
        switch (teleportType)
        {
            case TeleportType.INSIDE:
                TeleportInsideCell();
                break;
            case TeleportType.OUTSIDE:
                TeleportOutsideCell();
                break;
        }
        FadeEnd();
        yield return new WaitForSeconds(fadeTime);
    }

    /// <summary>
    /// Starts coroutine teleporting player
    /// </summary>
    /// <param name="teleportType">Type of teleport to transfer player to</param>
    public void TeleportDestination(TeleportType teleportType)
    {
        StartCoroutine(TeleportPlayer(teleportType));
    }

    /// <summary>
    /// Starts coroutine teleporting player
    /// </summary>
    /// <param name="dest">Position to teleport to</param>
    public void TeleportDestination(Vector3 dest)
    {
        StartCoroutine(TeleportPlayer(dest));
    }

    /// <summary>
    /// Fades screen to black
    /// </summary>
    private void FadeStart()
    {
        SteamVR_Fade.Start(Color.clear, 0);
        SteamVR_Fade.Start(Color.black, fadeTime);
    }

    /// <summary>
    /// Fades screen from black to transparent
    /// </summary>
    private void FadeEnd()
    {
        SteamVR_Fade.Start(Color.black, 0);
        SteamVR_Fade.Start(Color.clear, fadeTime);
    }

    /// <summary>
    /// Moves player inside work cell immediately
    /// </summary>
    private void TeleportInsideCell()
    {
        if (CellStateData.playerLocation == PlayerLocation.Outside)
        {
            CellStateData.playerLocation = PlayerLocation.Inside;
            var offset = transform.position - outsidePosition.position;
            transform.position = insidePosition.position + offset;
            OnTeleportInvoked();
        }
    }

    /// <summary>
    /// Moves player outside work cell immediately
    /// </summary>
    private void TeleportOutsideCell()
    {
        if (CellStateData.playerLocation == PlayerLocation.Inside)
        {
            CellStateData.playerLocation = PlayerLocation.Outside;
            var offset = transform.position - insidePosition.position;
            transform.position = outsidePosition.position + offset;
            OnTeleportInvoked();
        }
    }

    /// <summary>
    /// Teleports user to default position
    /// </summary>
    public void ResetPosition()
    {
        transform.position = initialPosition;
    }
}
