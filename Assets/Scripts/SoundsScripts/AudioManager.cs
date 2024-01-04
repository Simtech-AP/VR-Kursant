using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : Singleton<AudioManager>
{

    public Sound[] sounds = default;

    protected override void Awake()
    {
        base.Awake();

        foreach (Sound s in sounds)
        {

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            // s.source.playOnAwake = s.playOnAwake;
            if (s.playOnAwake && s.source.isPlaying == false)
            {
                s.source.Play();
            }
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Unable to find sound \"" + name + "\" to play");
        }
        else
        {
            s.source.Play();
        }
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Unable to find sound \"" + name + "\" to stop");
        }
        else
        {
            s.source.Stop();
        }
    }
}
