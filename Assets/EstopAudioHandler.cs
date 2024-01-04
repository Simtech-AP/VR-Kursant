using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstopAudioHandler : MonoBehaviour
{
    [SerializeField]
    private EStop associatedEstop = default;

    [SerializeField]
    private AudioSource soundOn = default;

    [SerializeField]
    private AudioSource soundOff = default;

    public void OnToggle()
    {
        var estopState = CellStateData.EStopStates[associatedEstop.EStopIndex] == (int)EStopState.PRESSED;

        soundOff.Stop();
        soundOn.Stop();

        if (estopState)
        {
            soundOn.Play();
        }
        else
        {
            soundOff.Play();
        }
    }
}
