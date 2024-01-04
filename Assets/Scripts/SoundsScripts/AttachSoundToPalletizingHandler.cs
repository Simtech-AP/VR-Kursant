using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttachSoundToPalletizingHandler : MonoBehaviour
{

    [SerializeField]
    private PalletizingHandler PalletizingHandler = default;

    private UnityAction leftStartAction = default;
    private UnityAction rightStartAction = default;
    private UnityAction leftStopAction = default;
    private UnityAction rightStopAction = default;

    private void Awake()
    {
        leftStartAction = () => AudioManager.Instance.Play("podajnikLewy");
        rightStartAction = () => AudioManager.Instance.Play("podajnikPrawy");
        leftStopAction = () => AudioManager.Instance.Stop("podajnikLewy");
        rightStopAction = () => AudioManager.Instance.Stop("podajnikPrawy");
    }

    private void OnEnable()
    {
        AddAudioListenersOnConveyors();
    }

    public void AddAudioListenersOnConveyors()
    {
        RemoveAudioListenersFromConveyors();    //to be sure we are not attacing listeners multiply times

        PalletizingHandler.leftConveyor.OnStart.AddListener(leftStartAction);
        PalletizingHandler.leftConveyor.OnStop.AddListener(leftStopAction);

        PalletizingHandler.rightConveyor.OnStart.AddListener(rightStartAction);
        PalletizingHandler.rightConveyor.OnStop.AddListener(rightStopAction);
    }

    public void RemoveAudioListenersFromConveyors()
    {
        PalletizingHandler.leftConveyor.OnStart.RemoveListener(leftStartAction);
        PalletizingHandler.leftConveyor.OnStop.RemoveListener(leftStopAction);

        PalletizingHandler.rightConveyor.OnStart.RemoveListener(rightStartAction);
        PalletizingHandler.rightConveyor.OnStop.RemoveListener(rightStopAction);
    }

}