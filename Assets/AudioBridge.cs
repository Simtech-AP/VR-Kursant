using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBridge : MonoBehaviour
{
    public void Play(string clipId)
    {
        AudioManager.Instance.Play(clipId);
    }
}
